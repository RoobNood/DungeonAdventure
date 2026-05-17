using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    #region Tooltip
    [Tooltip("Populate with child TrailRenderer component")]
    #endregion Tooltip
    [SerializeField] private TrailRenderer trailRenderer;

    private float ammoRange = 0f;
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement;
    private bool isColliding = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!isAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        if (!overrideAmmoMovement)
        {
            Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;
            transform.position += distanceVector;
            ammoRange -= distanceVector.magnitude;

            if (ammoRange < 0f)
            {
                if (ammoDetails.isPlayerAmmo)
                {
                    StaticEventHandler.CallMultiplierEvent(false);
                }

                DisableAmmo();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isColliding) return;

        bool shouldDisableAmmo = HandleCollision(collision);

        if (shouldDisableAmmo)
        {
            AmmoHitEffect();
            DisableAmmo();
        }
    }

    private bool HandleCollision(Collider2D collision)
    {
        Ammo otherAmmo = collision.GetComponent<Ammo>();
        if (otherAmmo != null)
        {
            return HandleAmmoReaction(otherAmmo);
        }

        TorchElementState torchElementState = GetTorchElementState(collision);
        if (torchElementState != null)
        {
            isColliding = true;
            ApplyTorchElementReaction(torchElementState);
            UpdatePlayerMultiplier(false);
            return true;
        }

        Health health = collision.GetComponent<Health>();
        if (health != null)
        {
            isColliding = true;
            ApplyHealthHit(collision, health);
            UpdatePlayerMultiplier(health.enemy != null);
            return true;
        }

        UpdatePlayerMultiplier(false);
        return true;
    }


    private TorchElementState GetTorchElementState(Collider2D collision)
    {
        TorchElementState torchElementState = collision.GetComponent<TorchElementState>();
        if (torchElementState != null)
            return torchElementState;

        torchElementState = collision.GetComponentInParent<TorchElementState>();
        if (torchElementState != null)
            return torchElementState;

        return collision.GetComponentInChildren<TorchElementState>();
    }

    private void ApplyTorchElementReaction(TorchElementState torchElementState)
    {
        switch (GetElementType())
        {
            case AmmoDetailsSO.ElementType.Fire:
                torchElementState.Ignite();
                break;

            case AmmoDetailsSO.ElementType.Water:
                if (torchElementState.Extinguish())
                {
                    SpawnElementArea(ammoDetails.steamAreaPrefab, torchElementState.GetReactionPosition());
                }

                break;
        }
    }

    private void ApplyHealthHit(Collider2D collision, Health health)
    {
        bool didDamage = health.TakeDamage(ammoDetails.ammoDamage);
        if (!didDamage)
            return;

        switch (GetElementType())
        {
            case AmmoDetailsSO.ElementType.Fire:
                ApplyBurn(collision);
                break;

            case AmmoDetailsSO.ElementType.Ice:
                ApplyIceSlow(collision);
                break;

            case AmmoDetailsSO.ElementType.Water:
                ClearBurn(collision);
                break;
        }
    }

    private void ApplyBurn(Collider2D collision)
    {
        if (!ammoDetails.isFireDotEnabled)
            return;

        BurnDamageOverTime burnDamageOverTime = collision.GetComponent<BurnDamageOverTime>();
        if (burnDamageOverTime == null)
        {
            burnDamageOverTime = collision.gameObject.AddComponent<BurnDamageOverTime>();
        }

        burnDamageOverTime.ApplyBurn(ammoDetails.fireDotDamagePerTick, ammoDetails.fireDotTickInterval, ammoDetails.fireDotDuration);
    }

    private void ApplyIceSlow(Collider2D collision)
    {
        if (!ammoDetails.isIceSlowEnabled)
            return;

        IceSlowOverTime iceSlowOverTime = collision.GetComponent<IceSlowOverTime>();
        if (iceSlowOverTime == null)
        {
            iceSlowOverTime = collision.gameObject.AddComponent<IceSlowOverTime>();
        }

        iceSlowOverTime.ApplySlow(ammoDetails.iceSlowMultiplier, ammoDetails.iceSlowDuration);
    }

    private void ClearBurn(Collider2D collision)
    {
        BurnDamageOverTime burnDamageOverTime = collision.GetComponent<BurnDamageOverTime>();
        if (burnDamageOverTime != null)
        {
            burnDamageOverTime.ClearBurn();
        }
    }

    private bool HandleAmmoReaction(Ammo otherAmmo)
    {
        GameObject areaPrefab = GetReactionAreaPrefab(otherAmmo);
        if (areaPrefab == null)
        {
            return false;
        }

        isColliding = true;
        otherAmmo.isColliding = true;

        SpawnElementArea(areaPrefab, transform.position);

        UpdatePlayerMultiplier(false);
        otherAmmo.UpdatePlayerMultiplier(false);
        otherAmmo.AmmoHitEffect();
        otherAmmo.DisableAmmo();
        return true;
    }

    private GameObject GetReactionAreaPrefab(Ammo otherAmmo)
    {
        AmmoDetailsSO.ElementType firstElement = GetElementType();
        AmmoDetailsSO.ElementType secondElement = otherAmmo.GetElementType();
        AmmoDetailsSO otherAmmoDetails = otherAmmo.GetAmmoDetails();

        if (IsElementPair(firstElement, secondElement, AmmoDetailsSO.ElementType.Fire, AmmoDetailsSO.ElementType.Water))
            return GetFirstConfiguredArea(ammoDetails.steamAreaPrefab, otherAmmoDetails != null ? otherAmmoDetails.steamAreaPrefab : null);

        if (IsElementPair(firstElement, secondElement, AmmoDetailsSO.ElementType.Water, AmmoDetailsSO.ElementType.Ice))
            return GetFirstConfiguredArea(ammoDetails.iceAreaPrefab, otherAmmoDetails != null ? otherAmmoDetails.iceAreaPrefab : null);

        if (IsElementPair(firstElement, secondElement, AmmoDetailsSO.ElementType.Fire, AmmoDetailsSO.ElementType.Ice))
            return GetFirstConfiguredArea(ammoDetails.waterAreaPrefab, otherAmmoDetails != null ? otherAmmoDetails.waterAreaPrefab : null);

        return null;
    }

    private GameObject GetFirstConfiguredArea(GameObject primaryAreaPrefab, GameObject fallbackAreaPrefab)
    {
        return primaryAreaPrefab != null ? primaryAreaPrefab : fallbackAreaPrefab;
    }

    private bool IsElementPair(AmmoDetailsSO.ElementType firstElement, AmmoDetailsSO.ElementType secondElement, AmmoDetailsSO.ElementType expectedFirst, AmmoDetailsSO.ElementType expectedSecond)
    {
        return (firstElement == expectedFirst && secondElement == expectedSecond) || (firstElement == expectedSecond && secondElement == expectedFirst);
    }

    private void SpawnElementArea(GameObject areaPrefab, Vector3 position)
    {
        if (areaPrefab == null)
            return;

        GameObject area = Instantiate(areaPrefab, position, Quaternion.identity);
        area.transform.localScale = ammoDetails.elementAreaScale;
        Destroy(area, ammoDetails.elementAreaLifetime);
    }

    private void UpdatePlayerMultiplier(bool enemyHit)
    {
        if (ammoDetails != null && ammoDetails.isPlayerAmmo)
        {
            StaticEventHandler.CallMultiplierEvent(enemyHit);
        }
    }


    public void ConsumeAfterElementReaction()
    {
        if (isColliding)
            return;

        isColliding = true;
        UpdatePlayerMultiplier(false);
        AmmoHitEffect();
        DisableAmmo();
    }

    public AmmoDetailsSO.ElementType GetElementType()
    {
        return ammoDetails != null ? ammoDetails.elementType : AmmoDetailsSO.ElementType.None;
    }

    public AmmoDetailsSO GetAmmoDetails()
    {
        return ammoDetails;
    }

    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Ammo

        this.ammoDetails = ammoDetails;
        isColliding = false;
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);
        spriteRenderer.sprite = ammoDetails.ammoSprite;

        if (ammoDetails.ammoChargeTime > 0f)
        {
            ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            isAmmoMaterialSet = false;
        }
        else
        {
            ammoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        ammoRange = ammoDetails.ammoRange;
        this.ammoSpeed = ammoSpeed;
        this.overrideAmmoMovement = overrideAmmoMovement;
        gameObject.SetActive(true);

        #endregion Ammo

        #region Trail

        if (ammoDetails.isAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailTime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }

        #endregion Trail
    }

    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        fireDirectionAngle += spreadToggle * randomSpread;
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    private void AmmoHitEffect()
    {
        if (ammoDetails.ammoHitEffect != null && ammoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
        {
            AmmoHitEffect ammoHitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent(ammoDetails.ammoHitEffect.ammoHitEffectPrefab, transform.position, Quaternion.identity);
            ammoHitEffect.SetHitEffect(ammoDetails.ammoHitEffect);
            ammoHitEffect.gameObject.SetActive(true);
        }
    }

    public void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }
#endif
    #endregion Validation
}
