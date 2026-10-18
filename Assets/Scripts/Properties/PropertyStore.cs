using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PropertyStore : MonoBehaviour {

    private Dictionary<Property, PropertyValue> properties;

    public void UpdateProperty(Property property, PropertyValue value) {
        properties[property] = value;
    }

    public PropertyValue GetPropertyValue(Property property) {
        return properties[property];
    }

    public List<(Property, PropertyValue)> GetPropertiesOfType(PropertyType type) {
        List<(Property, PropertyValue)> retVal = new List<(Property, PropertyValue)>();

        foreach (KeyValuePair<Property, PropertyValue> kvp in properties) {
            if (kvp.Key.propertyType == type) {
                retVal.Add((kvp.Key, kvp.Value));
            }
        }

        return retVal;
    }

    public List<Property> GetProperties() {
        List<Property> retVal = new List<Property>();
        foreach (KeyValuePair<Property, PropertyValue> kvp in properties) {
            retVal.Add(kvp.Key);
        }
        return retVal;
    }

    public void AddProperty(Property property) {
        properties.Add(property, new PropertyValue());
        switch (property.propertyType) {
            case PropertyType.Number:
                float value = Random.Range(property.minValue, property.maxValue);
                properties[property] = PropertyValue.Create(value);
                break;
            case PropertyType.String:
                int stringVal = Random.Range(0, property.validStrings.Count);
                properties[property] = PropertyValue.Create(property, property.validStrings[stringVal]);
                break;
            case PropertyType.Bool:
                int random = Random.Range(0, 1);
                properties[property] = PropertyValue.Create(random == 1);
                break;
        }
    }

}
