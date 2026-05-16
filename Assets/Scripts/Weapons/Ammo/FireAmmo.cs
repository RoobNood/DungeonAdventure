using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAmmo : MonoBehaviour
{
    [Header("火子弹元素配置")]
    public GameObject steamAreaPrefab;
    public GameObject waterAreaPrefab;
    public float areaExistTime = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 击中玩家/怪物 施加灼烧
        if (collision.CompareTag("Player") || collision.CompareTag("Monster"))
        {
            FireBurnable burn = collision.GetComponent<FireBurnable>();
            if (burn != null)
            {
                burn.StartBurn();
            }
        }

        // 2. 火 + 水子弹 = 生成蒸汽
        if (collision.CompareTag("WaterBullet"))
        {
            SpawnArea(steamAreaPrefab);
        }

        // 3. 火 + 冰子弹 = 生成水面
        if (collision.CompareTag("IceBullet"))
        {
            SpawnArea(waterAreaPrefab);
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