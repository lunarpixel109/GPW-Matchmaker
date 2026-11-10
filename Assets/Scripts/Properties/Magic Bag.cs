using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MagicBag", menuName = "Scriptable Objects/MagicBag")]
public class MagicBag : ScriptableObject
{
    [field: SerializeField] public List<float> numberValues;
    [field: SerializeField] public List<string> stringValues;
}
