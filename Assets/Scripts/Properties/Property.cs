using System.Collections.Generic;
using UnityEngine;

public enum PropertyType {
    Number,
    Bool,
    String
}

[CreateAssetMenu(fileName = "Property", menuName = "Game Data/Property")]
public class Property : ScriptableObject {

    [field: SerializeField] public string propertyName { get; private set; }
    [field: SerializeField] public string propertyDescription { get; private set; }

    [field: SerializeField] public PropertyType propertyType { get; private set; }


    [field: SerializeField] public float minValue { get; private set; }
    [field: SerializeField] public float maxValue { get; private set; }
    [field: SerializeField] public List<string> validStrings { get; private set; }
}
