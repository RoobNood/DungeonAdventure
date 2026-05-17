using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchElementState : MonoBehaviour
{
    [Header("State")]
    public bool isLit;

    [Header("Reaction")]
    public Transform reactionAnchor;
    [SerializeField] private GameObject steamPrefab;
    [SerializeField] private float steamLifetime = 2f;

    [Header("Visuals")]
    [SerializeField] private GameObject flameEffectObject;
    [SerializeField] private Light2D torchLight2D;

    [Header("Audio")]
    [SerializeField] private AudioSource loopAudioSource;

    private void Awake()
    {
        ApplyState();
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

    public void Extinguish()
    {
        if (!isLit)
        {
            return;
        }

        isLit = false;
        SpawnSteamEffect();
        ApplyState();
    }

    private void ApplyState()
    {
        if (flameEffectObject != null)
        {
            flameEffectObject.SetActive(isLit);
        }

        if (torchLight2D != null)
        {
            torchLight2D.enabled = isLit;
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

    private void SpawnSteamEffect()
    {
        if (steamPrefab == null)
        {
            return;
        }

        GameObject steamInstance = Instantiate(steamPrefab, GetReactionPosition(), Quaternion.identity);

        if (steamLifetime > 0f)
        {
            Destroy(steamInstance, steamLifetime);
        }
    }

    public Vector3 GetReactionPosition()
    {
        return reactionAnchor != null ? reactionAnchor.position : transform.position;
    }
}
