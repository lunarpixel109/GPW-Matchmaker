using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NameList", menuName = "Scriptable Objects/NameList")]
public class NameList : ScriptableObject
{
    public List<string> names;
}
