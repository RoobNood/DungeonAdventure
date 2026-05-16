using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MovementByVelocity))]
[DisallowMultipleComponent]
public class IceSlowOverTime : MonoBehaviour
{
    private MovementByVelocity movementByVelocity;
    private Coroutine slowCoroutine;

    private void Awake()
    {
        movementByVelocity = GetComponent<MovementByVelocity>();
    }

    public void ApplySlow(float slowRatio, float duration)
    {
        if (movementByVelocity == null || slowRatio <= 0f || duration <= 0f)
            return;

        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        slowCoroutine = StartCoroutine(SlowRoutine(slowRatio, duration));
    }

    private IEnumerator SlowRoutine(float slowRatio, float duration)
    {
        movementByVelocity.SetSpeedMultiplier(slowRatio);

        yield return new WaitForSeconds(duration);

        if (movementByVelocity != null)
        {
            movementByVelocity.SetSpeedMultiplier(1f);
        }

        slowCoroutine = null;
    }
}
