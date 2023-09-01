using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private Image glassImage;
    
    // Start is called before the first frame update
    void Start()
    {
        glassImage = GetComponent<Image>();
        glassImage.enabled = false;
    }

    void UpdateInventory(PlayerInventory playerInventory)
    {
        Debug.Log("XDDD");
        glassImage.enabled = true;
    }
}
