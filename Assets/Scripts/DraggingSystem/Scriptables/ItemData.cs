using System;
using UnityEngine;

namespace Scriptables {
    [CreateAssetMenu(menuName = "ScriptableObjects/ItemData")]
    public class ItemData : ScriptableObject
    {
        public GameObject prefab3D;
        public Sprite icon;
        public Color colour = Color.white;
        public string displayName = "Player";
        
        public DragFeel feel;

        [Space]
        public PropertyStore propertyStore;
    }
}
