using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamArea : MonoBehaviour
{
    public float slowRate = 0.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Monster"))
        {
            MovementByVelocity movement = other.GetComponent<MovementByVelocity>();
            if (movement != null)
            {
                movement.SetSpeedMultiplier(slowRate);
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
                movement.SetSpeedMultiplier(1f);
            }
        }
    }
}