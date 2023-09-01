using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        PlayerInventory playerInventory = other.gameObject.GetComponent<PlayerInventory>();
        

        if (playerInventory != null)
        {
            playerInventory.GlassCollected();
            gameObject.SetActive(false);
        }
    }
}
