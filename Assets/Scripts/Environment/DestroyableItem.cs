using System.Collections;
using UnityEngine;

// Don't add require directives since some legacy prefabs only add these components where needed.
[DisallowMultipleComponent]
public class DestroyableItem : MonoBehaviour
{
    #region Header HEALTH
    [Header("HEALTH")]
    #endregion Header HEALTH
    #region Tooltip
    [Tooltip("What the starting health for this destroyable item should be")]
    #endregion Tooltip
    [SerializeField] private int startingHealthAmount = 1;
    #region SOUND EFFECT
    [Header("SOUND EFFECT")]
    #endregion SOUND EFFECT
    #region Tooltip
    [Tooltip("The sound effect when this item is destroyed")]
    #endregion Tooltip
    [SerializeField] private SoundEffectSO destroySoundEffect;
    private Animator animator;
    private BoxCollider2D boxCollider2D;
    private HealthEvent healthEvent;
    private Health health;
    private ReceiveContactDamage receiveContactDamage;
    private bool isDestroyed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        health.SetStartingHealth(startingHealthAmount);
        receiveContactDamage = GetComponent<ReceiveContactDamage>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }


    private void OnDisable()
    {
        if (healthEvent != null)
        {
            healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
        }
    }

    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (!isDestroyed && healthEventArgs.healthAmount <= 0f)
        {
            isDestroyed = true;
            StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        // Destroy the trigger collider
        if (boxCollider2D != null)
        {
            Destroy(boxCollider2D);
        }

        if (health != null)
        {
            health.isDamageable = false;
        }

        BurnDamageOverTime burnDamageOverTime = GetComponent<BurnDamageOverTime>();
        if (burnDamageOverTime != null)
        {
            burnDamageOverTime.ClearBurn();
            Destroy(burnDamageOverTime);
        }

        // Play sound effect
        if (destroySoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(destroySoundEffect);
        }

        // Trigger the destroy animation
        if (animator != null)
        {
            animator.SetBool(Settings.destroy, true);
        }


        // Let the animation play through
        while (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.stateDestroyed))
        {
            yield return null;
        }

        // Keep Health and HealthEvent because Health requires HealthEvent, and temporary
        // effects like BurnDamageOverTime may also require Health during cleanup.
        if (animator != null)
        {
            Destroy(animator);
        }

        if (receiveContactDamage != null)
        {
            Destroy(receiveContactDamage);
        }

        Destroy(this);

    }
}
