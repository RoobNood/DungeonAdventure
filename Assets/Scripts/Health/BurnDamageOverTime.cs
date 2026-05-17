using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
[DisallowMultipleComponent]
public class BurnDamageOverTime : MonoBehaviour
{
    private Health health;
    private Coroutine burnCoroutine;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void ApplyBurn(int damagePerTick, float tickInterval, float duration)
    {
        if (damagePerTick <= 0 || tickInterval <= 0f || duration <= 0f)
            return;

        if (burnCoroutine != null)
            StopCoroutine(burnCoroutine);

        burnCoroutine = StartCoroutine(BurnRoutine(damagePerTick, tickInterval, duration));
    }

    public void ClearBurn()
    {
        if (burnCoroutine != null)
            StopCoroutine(burnCoroutine);

        burnCoroutine = null;

        // Optional: clear burn VFX here if needed.
    }

    private IEnumerator BurnRoutine(int damagePerTick, float tickInterval, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(tickInterval);

            if (health != null && health.isDamageable)
            {
                health.TakeDamage(damagePerTick);

                if (health.enemy != null && !health.enemy.enemyDetails.isImmuneAfterHit)
                {
                    health.FlashDamageIndicator();
                }
            }

            elapsedTime += tickInterval;
        }

        burnCoroutine = null;
    }
}
