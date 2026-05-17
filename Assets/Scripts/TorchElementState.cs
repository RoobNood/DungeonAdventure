using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TorchElementState : MonoBehaviour
{
    [Header("State")]
    public bool isLit;

    [Header("Reaction")]
    public Transform reactionAnchor;

    [Header("Visuals")]
    [SerializeField] private GameObject flameEffectObject;
    [SerializeField] private Light2D torchLight;

    [Header("Audio")]
    [SerializeField] private AudioSource loopAudioSource;

    private void Awake()
    {
        CacheMissingReferences();
        ApplyState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Ammo ammo = other.GetComponent<Ammo>();
        if (ammo == null)
            return;

        switch (ammo.GetElementType())
        {
            case AmmoDetailsSO.ElementType.Fire:
                Ignite();
                ammo.ConsumeAfterElementReaction();
                break;

            case AmmoDetailsSO.ElementType.Water:
                if (Extinguish())
                {
                    SpawnSteamFromAmmo(ammo);
                }

                ammo.ConsumeAfterElementReaction();
                break;
        }
    }

    public void Ignite()
    {
        if (isLit)
        {
            return;
        }

        isLit = true;
        ApplyState();
    }

    public bool Extinguish()
    {
        if (!isLit)
        {
            return false;
        }

        isLit = false;
        ApplyState();
        return true;
    }

    private void SpawnSteamFromAmmo(Ammo ammo)
    {
        AmmoDetailsSO ammoDetails = ammo.GetAmmoDetails();
        if (ammoDetails == null || ammoDetails.steamAreaPrefab == null)
            return;

        GameObject steamArea = Instantiate(ammoDetails.steamAreaPrefab, GetReactionPosition(), Quaternion.identity);
        steamArea.transform.localScale = ammoDetails.elementAreaScale;
        Destroy(steamArea, ammoDetails.elementAreaLifetime);
    }

    private void CacheMissingReferences()
    {
        if (flameEffectObject == null)
        {
            Transform flameTransform = transform.Find("TorchHolder/Flame");
            if (flameTransform == null)
            {
                flameTransform = transform.Find("Flame");
            }

            if (flameTransform != null)
            {
                flameEffectObject = flameTransform.gameObject;
            }
        }

        if (torchLight == null)
        {
            torchLight = GetComponentInChildren<Light2D>(true);
        }

        if (loopAudioSource == null)
        {
            loopAudioSource = GetComponentInChildren<AudioSource>(true);
        }
    }

    private void ApplyState()
    {
        if (flameEffectObject != null)
        {
            flameEffectObject.SetActive(isLit);
        }

        if (torchLight != null)
        {
            torchLight.enabled = isLit;
        }

        if (loopAudioSource != null)
        {
            if (isLit)
            {
                if (!loopAudioSource.isPlaying)
                {
                    loopAudioSource.Play();
                }
            }
            else if (loopAudioSource.isPlaying)
            {
                loopAudioSource.Stop();
            }
        }
    }

    public Vector3 GetReactionPosition()
    {
        return reactionAnchor != null ? reactionAnchor.position : transform.position;
    }
}
