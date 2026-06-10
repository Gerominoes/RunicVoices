using BepInEx.Configuration;
using UnityEngine;

namespace RunicVoice.Config;

internal static class RunicVoiceConfig
{
    internal static ConfigEntry<bool> EnableMod = null!;
    internal static ConfigEntry<bool> EnableDebugLogs = null!;

    internal static ConfigEntry<KeyCode> ShoutKey = null!;

    internal static ConfigEntry<bool> EnableBellowOfTheMountain = null!;
    internal static ConfigEntry<float> BellowStaminaCost = null!;
    internal static ConfigEntry<float> BellowCooldown = null!;
    internal static ConfigEntry<float> BellowRange = null!;
    internal static ConfigEntry<float> BellowConeAngle = null!;
    internal static ConfigEntry<float> BellowBluntDamage = null!;
    internal static ConfigEntry<float> BellowKnockbackForce = null!;
    internal static ConfigEntry<bool> BellowAffectsPlayers = null!;

    internal static void Bind(ConfigFile config)
    {
        // General settings control plugin activation and diagnostic output.
        EnableMod = config.Bind(
            "General",
            "EnableMod",
            true,
            "Enable Runic Voice."
        );

        EnableDebugLogs = config.Bind(
            "General",
            "EnableDebugLogs",
            false,
            "Enable extra logs for shout attempts, blocked casts, ability registration, and target counts."
        );

        // Input settings control the single shout key used by the first release.
        ShoutKey = config.Bind(
            "Input",
            "ShoutKey",
            KeyCode.Z,
            "Press this key to cast the active Runic Voice."
        );

        // Bellow of the Mountain settings define cost, timing, area, and combat impact.
        EnableBellowOfTheMountain = config.Bind(
            "Bellow of the Mountain",
            "EnableBellowOfTheMountain",
            true,
            "Enable Bellow of the Mountain."
        );

        BellowStaminaCost = config.Bind(
            "Bellow of the Mountain",
            "BellowStaminaCost",
            35f,
            new ConfigDescription("Stamina required to cast Bellow of the Mountain.", new AcceptableValueRange<float>(0f, 250f))
        );

        BellowCooldown = config.Bind(
            "Bellow of the Mountain",
            "BellowCooldown",
            12f,
            new ConfigDescription("Seconds before Bellow of the Mountain can be cast again.", new AcceptableValueRange<float>(0f, 300f))
        );

        BellowRange = config.Bind(
            "Bellow of the Mountain",
            "BellowRange",
            10f,
            new ConfigDescription("Maximum range of the forward cone.", new AcceptableValueRange<float>(1f, 100f))
        );

        BellowConeAngle = config.Bind(
            "Bellow of the Mountain",
            "BellowConeAngle",
            45f,
            new ConfigDescription("Total width of the forward cone in degrees.", new AcceptableValueRange<float>(1f, 180f))
        );

        BellowBluntDamage = config.Bind(
            "Bellow of the Mountain",
            "BellowBluntDamage",
            15f,
            new ConfigDescription("Blunt damage dealt to each valid target.", new AcceptableValueRange<float>(0f, 500f))
        );

        BellowKnockbackForce = config.Bind(
            "Bellow of the Mountain",
            "BellowKnockbackForce",
            35f,
            new ConfigDescription("Push force applied to each valid target.", new AcceptableValueRange<float>(0f, 500f))
        );

        BellowAffectsPlayers = config.Bind(
            "Bellow of the Mountain",
            "BellowAffectsPlayers",
            false,
            "Allow Bellow of the Mountain to affect other players."
        );
    }
}
