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

	[field: SerializeField] public MagicBag bag { get; private set; }
	[field: SerializeField] public int index { get; private set; }
	

	public void IncrementIndex()
	{
		int indexMax = 0;

		if(propertyType == PropertyType.Number)
		{
			indexMax = bag.numberValues.Count;
			
		}
		else if(propertyType == PropertyType.String)
		{
			indexMax = bag.stringValues.Count;
		}
		

		index++;
		if (index >= indexMax)
		{
			index = 0;
		}
			
	}

}
