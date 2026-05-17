using UnityEngine;

public class TorchElementState : MonoBehaviour
{
    [Header("State")]
    public bool isLit;

    [Header("Reaction")]
    public Transform reactionAnchor;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem flameEffect;
    [SerializeField] private Light torchLight;

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
        ApplyState();
    }

    private void ApplyState()
    {
        if (flameEffect != null)
        {
            if (isLit)
            {
                if (!flameEffect.isPlaying)
                {
                    flameEffect.Play();
                }
            }
            else if (flameEffect.isPlaying)
            {
                flameEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
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
