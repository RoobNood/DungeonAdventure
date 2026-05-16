using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAmmo : MonoBehaviour
{
    [Header("水子弹元素配置")]
    public GameObject steamAreaPrefab;
    public GameObject iceAreaPrefab;
    public float areaExistTime = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 水 + 火 = 蒸汽
        if (collision.CompareTag("FireBullet"))
        {
            SpawnArea(steamAreaPrefab);
        }

        // 水 + 冰 = 冰面
        if (collision.CompareTag("IceBullet"))
        {
            SpawnArea(iceAreaPrefab);
        }
    }

    void SpawnArea(GameObject prefab)
    {
        if (prefab == null) return;
        GameObject area = Instantiate(prefab, transform.position, Quaternion.identity);
        area.transform.localScale = new Vector3(3, 3, 1);
        Destroy(area, areaExistTime);
    }
}