using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBurnable : MonoBehaviour
{
    public bool isBurning;

    // 清除灼烧效果的方法，供 WaterArea 调用
    public void ClearBurn()
    {
        isBurning = false;
        // 这里可以加更多逻辑，比如停止伤害、移除特效等
        Debug.Log($"{gameObject.name} 灼烧效果已清除");
    }

    // 示例：开始灼烧的方法，供火子弹调用
    public void StartBurn()
    {
        isBurning = true;
    }
}