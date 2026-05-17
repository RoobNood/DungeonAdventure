using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    #region Header References
    [Space(10)]
    [Header("References")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the HealthBar component on the HealthBar gameobject")]
    #endregion
    [SerializeField] private HealthBar healthBar;
    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;
    private Player player;
    private Coroutine immunityCoroutine;
    private Coroutine damageFlashCoroutine;
    private bool isImmuneAfterHit = false;
    private float immunityTime = 0f;
    private SpriteRenderer spriteRenderer = null;
    private SpriteRenderer[] damageFlashSpriteRenderers = null;
    private Color[] damageFlashOriginalColors = null;
    private const float spriteFlashInterval = 0.2f;
    private WaitForSeconds WaitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);

    [HideInInspector] public bool isDamageable = true;
    [HideInInspector] public Enemy enemy;

    private void Awake()
    {
        //Load compnents
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        // Trigger a health event for UI update
        CallHealthEvent(0);

        // Attempt to load enemy / player components
        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();


        // Get player / enemy hit immunity details
        if (player != null)
        {
            spriteRenderer = player.spriteRenderer;
            damageFlashSpriteRenderers = new SpriteRenderer[] { spriteRenderer };

            if (player.playerDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = player.playerDetails.hitImmunityTime;
            }
        }
        else if (enemy != null)
        {
            damageFlashSpriteRenderers = enemy.spriteRendererArray;
            if (enemy.spriteRendererArray != null && enemy.spriteRendererArray.Length > 0)
            {
                spriteRenderer = enemy.spriteRendererArray[0];
            }

            if (enemy.enemyDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = enemy.enemyDetails.hitImmunityTime;
            }
        }

        // Enable the health bar if required
        if (enemy != null && enemy.enemyDetails.isHealthBarDisplayed == true && healthBar != null)
        {
            healthBar.EnableHealthBar();
        }
        else if (healthBar != null)
        {
            healthBar.DisableHealthBar();
        }
    }

    /// <summary>
    /// Public method called when damage is taken
    /// </summary>
    public bool TakeDamage(int damageAmount)
    {
        if (CanTakeDamage())
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);

            PostHitImmunity();

            // Set health bar as the percentage of health remaining
            if (healthBar != null)
            {
                healthBar.SetHealthBarValue((float)currentHealth / (float)startingHealth);
            }

            return true;
        }

        return false;
    }

    public bool CanTakeDamage()
    {
        Player targetPlayer = player != null ? player : GetComponent<Player>();
        bool isRolling = targetPlayer != null && targetPlayer.playerControl.isPlayerRolling;

        return isDamageable && !isRolling;
    }

    /// <summary>
    /// Indicate a hit and give some post hit immunity
    /// </summary>
    private void PostHitImmunity()
    {
        // Check if gameobject is active - if not return
        if (gameObject.activeSelf == false)
            return;

        // If there is post hit immunity then
        if (isImmuneAfterHit)
        {
            if (immunityCoroutine != null)
                StopCoroutine(immunityCoroutine);

            // flash red and give period of immunity
            immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(immunityTime, spriteRenderer));
        }

    }

    public void FlashDamageIndicator()
    {
        if (gameObject.activeSelf == false || damageFlashSpriteRenderers == null || damageFlashSpriteRenderers.Length == 0)
            return;

        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
            RestoreDamageFlashSpriteColors();
        }

        damageFlashOriginalColors = CaptureDamageFlashSpriteColors();
        damageFlashCoroutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        SetDamageFlashSpriteColor(Color.red);

        yield return WaitForSecondsSpriteFlashInterval;

        RestoreDamageFlashSpriteColors();

        damageFlashOriginalColors = null;
        damageFlashCoroutine = null;
    }

    /// <summary>
    /// Coroutine to indicate a hit and give some post hit immunity
    /// </summary>
    private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
    {
        int iterations = Mathf.RoundToInt(immunityTime / spriteFlashInterval / 2f);
        Color[] originalSpriteColors = CaptureDamageFlashSpriteColors();

        isDamageable = false;

        while (iterations > 0)
        {
            SetDamageFlashSpriteColor(Color.red);

            yield return WaitForSecondsSpriteFlashInterval;

            RestoreDamageFlashSpriteColors(originalSpriteColors);

            yield return WaitForSecondsSpriteFlashInterval;

            iterations--;

            yield return null;

        }

        RestoreDamageFlashSpriteColors(originalSpriteColors);
        isDamageable = true;

    }

    private void SetDamageFlashSpriteColor(Color color)
    {
        if (damageFlashSpriteRenderers == null)
            return;

        foreach (SpriteRenderer damageFlashSpriteRenderer in damageFlashSpriteRenderers)
        {
            if (damageFlashSpriteRenderer != null)
            {
                damageFlashSpriteRenderer.color = color;
            }
        }
    }

    private Color[] CaptureDamageFlashSpriteColors()
    {
        if (damageFlashSpriteRenderers == null)
            return null;

        Color[] spriteColors = new Color[damageFlashSpriteRenderers.Length];

        for (int i = 0; i < damageFlashSpriteRenderers.Length; i++)
        {
            spriteColors[i] = damageFlashSpriteRenderers[i] != null ? damageFlashSpriteRenderers[i].color : Color.white;
        }

        return spriteColors;
    }

    private void RestoreDamageFlashSpriteColors()
    {
        RestoreDamageFlashSpriteColors(damageFlashOriginalColors);
    }

    private void RestoreDamageFlashSpriteColors(Color[] spriteColors)
    {
        if (damageFlashSpriteRenderers == null || spriteColors == null)
            return;

        int spriteCount = Mathf.Min(damageFlashSpriteRenderers.Length, spriteColors.Length);

        for (int i = 0; i < spriteCount; i++)
        {
            if (damageFlashSpriteRenderers[i] != null)
            {
                damageFlashSpriteRenderers[i].color = spriteColors[i];
            }
        }
    }

    private void CallHealthEvent(int damageAmount)
    {
        // Trigger health event
        healthEvent.CallHealthChangedEvent(((float)currentHealth / (float)startingHealth), currentHealth, damageAmount);
    }


    /// <summary>
    /// Set starting health 
    /// </summary>
    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    /// <summary>
    /// Get the starting health
    /// </summary>
    public int GetStartingHealth()
    {
        return startingHealth;
    }

    /// <summary>
    /// Increase health by specified percent
    /// </summary>
    public void AddHealth(int healthPercent)
    {
        int healthIncrease = Mathf.RoundToInt((startingHealth * healthPercent) / 100f);

        int totalHealth = currentHealth + healthIncrease;

        if (totalHealth > startingHealth)
        {
            currentHealth = startingHealth;
        }
        else
        {
            currentHealth = totalHealth;
        }

        CallHealthEvent(0);
    }

}
