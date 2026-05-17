using UnityEngine;

public class WaterArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        ClearBurn(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        ClearBurn(other);
    }

    private void ClearBurn(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Monster"))
            return;

        BurnDamageOverTime burnDamageOverTime = other.GetComponent<BurnDamageOverTime>();
        if (burnDamageOverTime != null)
        {
            burnDamageOverTime.ClearBurn();
        }
    }
}