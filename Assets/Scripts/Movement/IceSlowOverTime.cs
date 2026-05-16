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

    public void ApplySlow(float slowRatio, float duration)
    {
        if ((movementByVelocity == null && movementToPosition == null) || slowRatio <= 0f || duration <= 0f)
            return;

        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        slowCoroutine = StartCoroutine(SlowRoutine(slowRatio, duration));
    }

    private IEnumerator SlowRoutine(float slowRatio, float duration)
    {
        SetSpeedMultiplier(slowRatio);

        yield return new WaitForSeconds(duration);

        SetSpeedMultiplier(1f);

        slowCoroutine = null;
    }

    private void SetSpeedMultiplier(float multiplier)
    {
        if (movementByVelocity != null)
        {
            movementByVelocity.SetSpeedMultiplier(multiplier);
        }

        if (movementToPosition != null)
        {
            movementToPosition.SetSpeedMultiplier(multiplier);
        }
    }
}
