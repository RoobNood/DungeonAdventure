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

        slowCoroutine = StartCoroutine(SlowRoutine(slowMultiplier, duration));
    }

    private IEnumerator SlowRoutine(float slowMultiplier, float duration)
    {
        SetMovementMultiplier(slowMultiplier);

        yield return new WaitForSeconds(duration);

        SetMovementMultiplier(1f);

        slowCoroutine = null;
    }

    private void SetMovementMultiplier(float multiplier)
    {
        if (movementByVelocity != null)
            movementByVelocity.SetSpeedMultiplier(multiplier);

        if (movementToPosition != null)
            movementToPosition.SetSpeedMultiplier(multiplier);
    }
}
