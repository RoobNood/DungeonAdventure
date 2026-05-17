using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class IceSlowOverTime : MonoBehaviour
{
    private MovementByVelocity movementByVelocity;
    private MovementToPosition movementToPosition;
    private Coroutine slowCoroutine;

    private void Awake()
    {
        movementByVelocity = GetComponent<MovementByVelocity>();
        movementToPosition = GetComponent<MovementToPosition>();
    }

    public void ApplySlow(float slowMultiplier, float duration)
    {
        if (slowMultiplier <= 0f || slowMultiplier > 1f || duration <= 0f)
            return;

        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        ApplyMovementModifier(slowMultiplier);
        slowCoroutine = StartCoroutine(SlowRoutine(duration));
    }

    private IEnumerator SlowRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        RemoveMovementModifier();

        slowCoroutine = null;
    }

    private void OnDisable()
    {
        RemoveMovementModifier();
    }

    private void ApplyMovementModifier(float multiplier)
    {
        if (movementByVelocity != null)
            MovementSpeedModifier.Add(movementByVelocity, this, multiplier);

        if (movementToPosition != null)
            MovementSpeedModifier.Add(movementToPosition, this, multiplier);
    }

    private void RemoveMovementModifier()
    {
        if (movementByVelocity != null)
            MovementSpeedModifier.Remove(movementByVelocity, this);

        if (movementToPosition != null)
            MovementSpeedModifier.Remove(movementToPosition, this);
    }
}
