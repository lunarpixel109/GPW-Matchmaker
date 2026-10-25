using System;
using Scriptables;
using Unity.VisualScripting;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    public ItemData data;

    void Start() => ApplyColour();

    public void ApplyColour() {
        if (data == null) {
            return;
        }
        
        var rend = GetComponent<Renderer>();

        if (rend != null) {
            rend.material.color = data.colour;
        }
    }
}

