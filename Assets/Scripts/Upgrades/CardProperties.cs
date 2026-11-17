using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "CardProperties", menuName = "Scriptable Objects/CardProperties")]
public class CardProperties : ScriptableObject
{
    [field: SerializeField] public int cardID {  get; private set; }
    [field: SerializeField] public string cardName { get; private set; }
    [field: SerializeField] public string cardDescription { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
}
