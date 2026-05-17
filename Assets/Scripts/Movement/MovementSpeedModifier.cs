using System.Collections.Generic;
using UnityEngine;

public static class MovementSpeedModifier
{
    private class SpeedState
    {
        public float baseMultiplier;
        public readonly Dictionary<object, float> modifiers = new Dictionary<object, float>();
    }

    private static readonly Dictionary<MovementByVelocity, SpeedState> velocityStates = new Dictionary<MovementByVelocity, SpeedState>();
    private static readonly Dictionary<MovementToPosition, SpeedState> positionStates = new Dictionary<MovementToPosition, SpeedState>();

    public static void Add(MovementByVelocity movement, object source, float multiplier)
    {
        if (movement == null || source == null)
            return;

        SpeedState state = GetOrCreateState(velocityStates, movement.GetSpeedMultiplier(), movement);
        state.modifiers[source] = multiplier;
        Apply(movement, state);
    }

    public static void Add(MovementToPosition movement, object source, float multiplier)
    {
        if (movement == null || source == null)
            return;

        SpeedState state = GetOrCreateState(positionStates, movement.GetSpeedMultiplier(), movement);
        state.modifiers[source] = multiplier;
        Apply(movement, state);
    }

    public static void Remove(MovementByVelocity movement, object source)
    {
        if (movement == null || source == null || !velocityStates.TryGetValue(movement, out SpeedState state))
            return;

        state.modifiers.Remove(source);
        if (state.modifiers.Count == 0)
        {
            movement.SetSpeedMultiplier(state.baseMultiplier);
            velocityStates.Remove(movement);
            return;
        }

        Apply(movement, state);
    }

    public static void Remove(MovementToPosition movement, object source)
    {
        if (movement == null || source == null || !positionStates.TryGetValue(movement, out SpeedState state))
            return;

        state.modifiers.Remove(source);
        if (state.modifiers.Count == 0)
        {
            movement.SetSpeedMultiplier(state.baseMultiplier);
            positionStates.Remove(movement);
            return;
        }

        Apply(movement, state);
    }

    private static SpeedState GetOrCreateState<TMovement>(Dictionary<TMovement, SpeedState> states, float baseMultiplier, TMovement movement)
    {
        if (!states.TryGetValue(movement, out SpeedState state))
        {
            state = new SpeedState { baseMultiplier = baseMultiplier };
            states.Add(movement, state);
        }

        return state;
    }

    private static void Apply(MovementByVelocity movement, SpeedState state)
    {
        movement.SetSpeedMultiplier(CalculateMultiplier(state));
    }

    private static void Apply(MovementToPosition movement, SpeedState state)
    {
        movement.SetSpeedMultiplier(CalculateMultiplier(state));
    }

    private static float CalculateMultiplier(SpeedState state)
    {
        float multiplier = state.baseMultiplier;

        foreach (float modifier in state.modifiers.Values)
        {
            multiplier *= modifier;
        }

        return multiplier;
    }
}
