using System.Collections.Generic;
using UnityEngine;

public class IceArea : MonoBehaviour
{
    [Tooltip("Movement speed multiplier while inside ice. 1.5 = 150% speed")]
    public float speedMultiplier = 1.5f;

    private static readonly object speedModifierSource = new object();
    private static readonly Dictionary<MovementByVelocity, int> globalVelocityContactCounts = new Dictionary<MovementByVelocity, int>();
    private static readonly Dictionary<MovementToPosition, int> globalPositionContactCounts = new Dictionary<MovementToPosition, int>();

    private readonly Dictionary<MovementByVelocity, int> velocityContactCounts = new Dictionary<MovementByVelocity, int>();
    private readonly Dictionary<MovementToPosition, int> positionContactCounts = new Dictionary<MovementToPosition, int>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!CanAffect(other))
            return;

        ApplyMultiplier(other, true);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!CanAffect(other))
            return;

        ApplyMultiplier(other, false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!CanAffect(other))
            return;

        RestoreMultiplier(other);
    }

    private void OnDisable()
    {
        foreach (MovementByVelocity movementByVelocity in velocityContactCounts.Keys)
        {
            RemoveGlobalVelocityContact(movementByVelocity);
        }

        foreach (MovementToPosition movementToPosition in positionContactCounts.Keys)
        {
            RemoveGlobalPositionContact(movementToPosition);
        }

        velocityContactCounts.Clear();
        positionContactCounts.Clear();
    }

    private bool CanAffect(Collider2D other)
    {
        return other.CompareTag("Player") || other.CompareTag("Monster");
    }

    private void ApplyMultiplier(Collider2D other, bool incrementContactCount)
    {
        RegisterVelocityMovement(other.GetComponent<MovementByVelocity>(), incrementContactCount);
        RegisterPositionMovement(other.GetComponent<MovementToPosition>(), incrementContactCount);
    }

    private void RegisterVelocityMovement(MovementByVelocity movementByVelocity, bool incrementContactCount)
    {
        if (movementByVelocity == null)
            return;

        if (!velocityContactCounts.ContainsKey(movementByVelocity))
        {
            velocityContactCounts.Add(movementByVelocity, 0);
            AddGlobalVelocityContact(movementByVelocity);
        }

        if (incrementContactCount || velocityContactCounts[movementByVelocity] == 0)
        {
            velocityContactCounts[movementByVelocity]++;
        }
    }

    private void RegisterPositionMovement(MovementToPosition movementToPosition, bool incrementContactCount)
    {
        if (movementToPosition == null)
            return;

        if (!positionContactCounts.ContainsKey(movementToPosition))
        {
            positionContactCounts.Add(movementToPosition, 0);
            AddGlobalPositionContact(movementToPosition);
        }

        if (incrementContactCount || positionContactCounts[movementToPosition] == 0)
        {
            positionContactCounts[movementToPosition]++;
        }
    }

    private void RestoreMultiplier(Collider2D other)
    {
        MovementByVelocity movementByVelocity = other.GetComponent<MovementByVelocity>();
        if (movementByVelocity != null && velocityContactCounts.TryGetValue(movementByVelocity, out int velocityContactCount))
        {
            velocityContactCount--;
            if (velocityContactCount <= 0)
            {
                RemoveGlobalVelocityContact(movementByVelocity);
                velocityContactCounts.Remove(movementByVelocity);
            }
            else
            {
                velocityContactCounts[movementByVelocity] = velocityContactCount;
            }
        }

        MovementToPosition movementToPosition = other.GetComponent<MovementToPosition>();
        if (movementToPosition != null && positionContactCounts.TryGetValue(movementToPosition, out int positionContactCount))
        {
            positionContactCount--;
            if (positionContactCount <= 0)
            {
                RemoveGlobalPositionContact(movementToPosition);
                positionContactCounts.Remove(movementToPosition);
            }
            else
            {
                positionContactCounts[movementToPosition] = positionContactCount;
            }
        }
    }

    private void AddGlobalVelocityContact(MovementByVelocity movementByVelocity)
    {
        if (!globalVelocityContactCounts.TryGetValue(movementByVelocity, out int contactCount))
        {
            globalVelocityContactCounts.Add(movementByVelocity, 1);
            MovementSpeedModifier.Add(movementByVelocity, speedModifierSource, speedMultiplier);
            return;
        }

        globalVelocityContactCounts[movementByVelocity] = contactCount + 1;
    }

    private void AddGlobalPositionContact(MovementToPosition movementToPosition)
    {
        if (!globalPositionContactCounts.TryGetValue(movementToPosition, out int contactCount))
        {
            globalPositionContactCounts.Add(movementToPosition, 1);
            MovementSpeedModifier.Add(movementToPosition, speedModifierSource, speedMultiplier);
            return;
        }

        globalPositionContactCounts[movementToPosition] = contactCount + 1;
    }

    private static void RemoveGlobalVelocityContact(MovementByVelocity movementByVelocity)
    {
        if (!globalVelocityContactCounts.TryGetValue(movementByVelocity, out int contactCount))
            return;

        contactCount--;
        if (contactCount <= 0)
        {
            MovementSpeedModifier.Remove(movementByVelocity, speedModifierSource);
            globalVelocityContactCounts.Remove(movementByVelocity);
            return;
        }

        globalVelocityContactCounts[movementByVelocity] = contactCount;
    }

    private static void RemoveGlobalPositionContact(MovementToPosition movementToPosition)
    {
        if (!globalPositionContactCounts.TryGetValue(movementToPosition, out int contactCount))
            return;

        contactCount--;
        if (contactCount <= 0)
        {
            MovementSpeedModifier.Remove(movementToPosition, speedModifierSource);
            globalPositionContactCounts.Remove(movementToPosition);
            return;
        }

        globalPositionContactCounts[movementToPosition] = contactCount;
    }
}
