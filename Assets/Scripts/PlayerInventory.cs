using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public bool ifGlassCollected { get; private set; }
    public Image glassImage;
    
    void Start()
    {
        glassImage= GameObject.Find("GlassImage").GetComponent<Image>();
        glassImage.enabled = false;
    }
    public void GlassCollected()
    {
        ifGlassCollected = true;
        glassImage.enabled = true;
    }
}
