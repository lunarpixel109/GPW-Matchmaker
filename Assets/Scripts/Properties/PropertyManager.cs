using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PropertyManager : MonoBehaviour {

    [SerializeField]
    public List<Property> properties;

    // ONLY USED FOR DEBUG
    public void Start() {
        //GeneratePlayerData();

        PropertyStore[] propertyStores = FindObjectsByType<PropertyStore>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var store in propertyStores) {
            store.PrintAllProperties();
        }
    }

    /// <summary>
    /// Used to Generate new properties for all currently spawned players
    /// </summary>
    public void GeneratePlayerData() {
        PropertyStore[] propertyStores = FindObjectsByType<PropertyStore>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (propertyStores == null || properties == null) {
            throw new Exception("no properties or property stores");
        }
        foreach (PropertyStore propertyStore in propertyStores) {
            foreach (Property property in properties) {
                propertyStore.AddProperty(property);
            }
        }
    }

}
