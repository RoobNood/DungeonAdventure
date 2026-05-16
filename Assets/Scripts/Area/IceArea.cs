using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceArea : MonoBehaviour
{
    // 加速倍数 1.4 就是变快，你可以自己调 1.3 ~ 1.6
    public float speedUpRate = 1.4f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Monster"))
        {
            MovementByVelocity movement = other.GetComponent<MovementByVelocity>();
            if (movement != null)
            {
                // 进入冰面 加速
                movement.SetSpeedMultiplier(speedUpRate);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Monster"))
        {
            MovementByVelocity movement = other.GetComponent<MovementByVelocity>();
            if (movement != null)
            {
                // 离开冰面 恢复正常速度
                movement.SetSpeedMultiplier(1f);
            }
        }
    }
}