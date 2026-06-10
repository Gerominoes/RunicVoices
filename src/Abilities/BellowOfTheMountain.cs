using System.Collections.Generic;
using RunicVoice.Config;
using RunicVoice.Utility;
using UnityEngine;

namespace RunicVoice.Abilities;

internal sealed class BellowOfTheMountain : IRunicVoiceAbility
{
    private const string DisabledMessage = "This Runic Voice is disabled.";
    private const string CooldownMessage = "Your voice is recovering.";
    private const string NoStaminaMessage = "Not enough stamina.";
    private const string SuccessMessage = "Bellow of the Mountain!";

    private float _nextCastTime;

    public string Id => "bellow_of_the_mountain";
    public string DisplayName => "Bellow of the Mountain";

    public bool CanCast(Player player, out string reason)
    {
        // Enabled checks cover both global mod state and this individual ability.
        if (!RunicVoiceConfig.EnableMod.Value || !RunicVoiceConfig.EnableBellowOfTheMountain.Value)
        {
            reason = DisabledMessage;
            return false;
        }

        // Player state checks avoid spending stamina during death or invalid local state.
        if (player == null || player.IsDead())
        {
            reason = string.Empty;
            return false;
        }

        // Cooldown checks use Unity time because this release is local-player focused.
        if (Time.time < _nextCastTime)
        {
            reason = CooldownMessage;
            return false;
        }

        // Stamina checks happen before target scanning to keep failed casts inexpensive.
        float staminaCost = Mathf.Max(0f, RunicVoiceConfig.BellowStaminaCost.Value);
        if (!player.HaveStamina(staminaCost))
        {
            reason = NoStaminaMessage;
            return false;
        }

        reason = string.Empty;
        return true;
    }

    public void Cast(Player player)
    {
        // TODO: Future multiplayer implementation should send a client shout request to the server.
        // TODO: The server should validate stamina, cooldown, unlock state, and player position.
        // TODO: The server should apply damage and status effects, then broadcast VFX and SFX.

        // Cost and cooldown are committed once validation has passed.
        float staminaCost = Mathf.Max(0f, RunicVoiceConfig.BellowStaminaCost.Value);
        float cooldown = Mathf.Max(0f, RunicVoiceConfig.BellowCooldown.Value);
        player.UseStamina(staminaCost);
        _nextCastTime = Time.time + cooldown;

        // Targeting happens only on successful casts to avoid per-frame enemy scans.
        List<Character> targets = TargetingUtils.GetCharactersInForwardCone(
            player,
            Mathf.Max(0f, RunicVoiceConfig.BellowRange.Value),
            Mathf.Clamp(RunicVoiceConfig.BellowConeAngle.Value, 1f, 180f),
            RunicVoiceConfig.BellowAffectsPlayers.Value,
            true
        );

        // Damage application uses Valheim-native HitData so resistances and combat reactions can participate.
        foreach (Character target in targets)
            ApplyHit(player, target);

        MessageUtils.ShowCenterMessage(SuccessMessage);
        RunicVoicePlugin.DebugLog($"Number of targets hit: {targets.Count}");

        // TODO: Add an original Valheim-native placeholder VFX when a safe prefab is selected.
    }

    private static void ApplyHit(Player player, Character target)
    {
        // Hit direction points away from the caster so push force feels like a forward bellow.
        Vector3 targetPoint = target.GetCenterPoint();
        Vector3 direction = targetPoint - player.GetCenterPoint();
        if (direction.sqrMagnitude <= 0.001f)
            direction = player.transform.forward;

        direction.Normalize();

        // HitData carries the configured blunt damage and knockback into Valheim's damage pipeline.
        HitData hitData = new()
        {
            m_point = targetPoint,
            m_dir = direction,
            m_pushForce = Mathf.Max(0f, RunicVoiceConfig.BellowKnockbackForce.Value),
            m_skill = Skills.SkillType.Clubs,
            m_hitType = target.IsPlayer() ? HitData.HitType.PlayerHit : HitData.HitType.EnemyHit
        };

        hitData.m_damage.m_blunt = Mathf.Max(0f, RunicVoiceConfig.BellowBluntDamage.Value);
        hitData.SetAttacker(player);

        target.Damage(hitData);
    }
}
