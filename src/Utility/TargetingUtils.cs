using System;
using System.Collections.Generic;
using UnityEngine;

namespace RunicVoice.Utility;

internal static class TargetingUtils
{
    private static int _lineOfSightMask = -1;

    internal static List<Character> GetCharactersInForwardCone(
        Player caster,
        float range,
        float coneAngle,
        bool affectsPlayers,
        bool requireLineOfSight
    )
    {
        // The result list contains only targets that pass ownership, faction, range, cone, and sight checks.
        List<Character> targets = new();

        if (caster == null)
            return targets;

        Vector3 origin = caster.GetCenterPoint();
        Vector3 forward = Vector3.ProjectOnPlane(caster.transform.forward, Vector3.up).normalized;
        float halfConeAngle = coneAngle * 0.5f;

        // Character lookup is performed only during a cast, not during normal frame updates.
        foreach (Character target in Character.GetAllCharacters())
        {
            if (!IsCandidateTarget(caster, target, affectsPlayers))
                continue;

            Vector3 targetPoint = target.GetCenterPoint();
            Vector3 offset = targetPoint - origin;
            if (offset.magnitude > range)
                continue;

            Vector3 horizontalDirection = Vector3.ProjectOnPlane(offset, Vector3.up);
            if (horizontalDirection.sqrMagnitude <= 0.001f)
                continue;

            float angle = Vector3.Angle(forward, horizontalDirection.normalized);
            if (angle > halfConeAngle)
                continue;

            if (requireLineOfSight && !HasLineOfSight(caster, target, origin, targetPoint))
                continue;

            targets.Add(target);
        }

        return targets;
    }

    private static bool IsCandidateTarget(Player caster, Character target, bool affectsPlayers)
    {
        // Null, dead, and caster checks keep the ability from hitting invalid or self targets.
        if (target == null || target.IsDead() || ReferenceEquals(target, caster))
            return false;

        // Player targets are opt-in for friendly-fire style behavior.
        if (target.IsPlayer())
            return affectsPlayers;

        // Non-player targets must be enemies according to Valheim's faction logic.
        return BaseAI.IsEnemy(caster, target);
    }

    private static bool HasLineOfSight(Player caster, Character target, Vector3 origin, Vector3 targetPoint)
    {
        // RaycastAll lets the code ignore caster and target colliders while still detecting walls and terrain.
        Vector3 offset = targetPoint - origin;
        float distance = offset.magnitude;
        if (distance <= 0.001f)
            return true;

        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            offset / distance,
            distance,
            GetLineOfSightMask(),
            QueryTriggerInteraction.Ignore
        );

        Array.Sort(hits, static (left, right) => left.distance.CompareTo(right.distance));

        foreach (RaycastHit hit in hits)
        {
            Character? hitCharacter = hit.collider.GetComponentInParent<Character>();
            if (hitCharacter == caster || hitCharacter == target)
                continue;

            return false;
        }

        return true;
    }

    private static int GetLineOfSightMask()
    {
        // Valheim layer names are cached after first use to keep repeated casts cheap.
        if (_lineOfSightMask != -1)
            return _lineOfSightMask;

        int mask = LayerMask.GetMask(
            "Default",
            "static_solid",
            "terrain",
            "piece",
            "piece_nonsolid"
        );

        if (mask == 0)
        {
            mask = Physics.DefaultRaycastLayers;
            RunicVoicePlugin.Log.LogWarning("Could not resolve Valheim line-of-sight layers. Falling back to Physics.DefaultRaycastLayers.");
        }

        _lineOfSightMask = mask;
        return _lineOfSightMask;
    }
}
