using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 测试第一步：不管碰到啥，都打印！
        Debug.Log("有东西进入水面了：" + other.name);
    }
}