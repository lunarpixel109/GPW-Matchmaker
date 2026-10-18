using System.Collections.Generic;
using UnityEngine;

public class PropertyManager : MonoBehaviour {

    [SerializeField] private List<Property> properties;

    public void GeneratePlayerData() {
        PropertyStore[] propertyStores = FindObjectsByType<PropertyStore>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (PropertyStore propertyStore in propertyStores) {
            foreach (Property property in properties) {
                propertyStore.AddProperty(property);
            }
        }
    }

}
