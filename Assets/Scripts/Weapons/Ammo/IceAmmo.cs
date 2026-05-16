using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceAmmo : MonoBehaviour
{
    [Header("冰子弹元素配置")]
    public GameObject waterAreaPrefab;
    public GameObject iceAreaPrefab;
    public float areaExistTime = 5f;
    public float slowRatio = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 暂时注释减速逻辑，避免报错
        /*
        if (collision.CompareTag("Player") || collision.CompareTag("Monster"))
        {
            MovementByVelocity move = collision.GetComponent<MovementByVelocity>();
            if (move != null)
            {
                move.SetSpeedMultiplier(slowRatio);
            }
        }
        */

        // 冰 + 火 = 生成水面
        if (collision.CompareTag("FireBullet"))
        {
            SpawnArea(waterAreaPrefab);
        }

        // 冰 + 水 = 生成冰面
        if (collision.CompareTag("WaterBullet"))
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