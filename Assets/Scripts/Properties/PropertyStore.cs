using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PropertyStore : MonoBehaviour {

	[Serialize]
	private Dictionary<Property, PropertyValue> properties ;

	void Awake() {
		properties = new Dictionary<Property, PropertyValue>();
	}

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
				//Debug.Log($"{property.name}: {property.minValue} - {property.maxValue}");
				float value = Random.Range(property.minValue, property.maxValue);
				//Debug.Log($"{property.name}: {value}");
				//float value = 0f;

				value = property.bag.numberValues[property.index];
				property.IncrementIndex();
				
				properties[property] = PropertyValue.Create(value);
				break;
			case PropertyType.String:
				int strValue;
				int.TryParse(property.bag.stringValues[property.index], out strValue);
				property.IncrementIndex();
	
				properties[property] = PropertyValue.Create(property, property.validStrings[strValue]);
				break;
			case PropertyType.Bool:
				int random = Random.Range(0, 1);
				properties[property] = PropertyValue.Create(random == 1);
				break;
		}
	}

	public void PrintAllProperties() {
		string str = $"Properties on object {gameObject.name}:\n";
		foreach (var property in properties) {
			str += $"{property.Key.name} - ";
			switch (property.Key.propertyType) {
				case PropertyType.Number:
					str += $"{property.Value.NumberValue}\n";
					break;
				case PropertyType.Bool:
					str += $"{property.Value.BoolValue}\n";
					break;
				case PropertyType.String:
					str += $"{property.Key.validStrings[(int)property.Value.NumberValue]}";
					break;
			}
		}
		Debug.Log(str);
	}

}
