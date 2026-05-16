using UnityEngine;

public class IceAmmo : MonoBehaviour
{
    [Header("Element Area Prefabs")]
    public GameObject waterAreaPrefab;
    public GameObject iceAreaPrefab;
    public float areaExistTime = 5f;
    public float slowRatio = 0.5f;
    public float slowDuration = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Monster"))
        {
            IceSlowOverTime iceSlow = collision.GetComponent<IceSlowOverTime>();
            if (iceSlow == null)
            {
                iceSlow = collision.gameObject.AddComponent<IceSlowOverTime>();
            }

            iceSlow.ApplySlow(slowRatio, slowDuration);
        }

        if (collision.CompareTag("FireBullet"))
        {
            SpawnArea(waterAreaPrefab);
        }

        if (collision.CompareTag("WaterBullet"))
        {
            SpawnArea(iceAreaPrefab);
        }
    }

    private void SpawnArea(GameObject prefab)
    {
        if (prefab == null) return;
        GameObject area = Instantiate(prefab, transform.position, Quaternion.identity);
        area.transform.localScale = new Vector3(3, 3, 1);
        Destroy(area, areaExistTime);
    }
}
