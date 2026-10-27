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


    public void AddProperty(Property property, int value) {
        properties.Add(property);
        propertyValues.Add(PropertyValue.Create(value));
    }

    public void AddProperty(Property property, string value) {
        properties.Add(property);
        propertyValues.Add(PropertyValue.Create(property, value));
    }

    public void AddProperty(Property property, bool value) {
        properties.Add(property);
        propertyValues.Add(PropertyValue.Create(value));
    }
}