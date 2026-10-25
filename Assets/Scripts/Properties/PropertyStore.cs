using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


[CreateAssetMenu(fileName = "Property Store", menuName = "Game Data/Property Store")]
public class PropertyStore: ScriptableObject {
    
    public List<Property> properties;
    public List<PropertyValue> propertyValues;

    public PropertyValue GetPropertyValue(Property property)
    {
        int index = properties.FindIndex(x => x.name == property.name);

        return propertyValues[index];
    }

}