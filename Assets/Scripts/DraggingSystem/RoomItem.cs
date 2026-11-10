using System;
using Scriptables;
using Unity.VisualScripting;
using UnityEngine;

public class RoomItem : MonoBehaviour {
    public event Action Settled;
    public event Action PickedUp;
    
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
    
    public void CallSettled() => Settled?.Invoke();
    public void CallPickedUp() => PickedUp?.Invoke();
}

