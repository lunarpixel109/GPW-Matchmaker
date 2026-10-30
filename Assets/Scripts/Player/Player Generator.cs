using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.EditorGUI;
using static UnityEngine.Audio.GeneratorInstance;
using Random = UnityEngine.Random;
using Scriptables;

public class PlayerGenerator : MonoBehaviour
{
	private float timer = 0f;
	[SerializeField]
	private float timerMax;

	[SerializeField]
	private GameObject playerPrefab;

	[SerializeField]
	private Sprite playerSpriteDefalt;

	[SerializeField]
	private GameObject Queue;
	[SerializeField]
	private GameObject[] Lobbies;

	[SerializeField] private List<Property> properties;
	[SerializeField] private List<int> indexes;

	GameObject newPlayer;
	ItemData newItemData;
	PropertyStore newPropertyStore;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		timer = timerMax;

		indexes = new List<int>();
		foreach (Property property in properties)
		{
			if (property.propertyType == PropertyType.Number)
			{
				indexes.Add(Random.Range(0, property.bag.numberValues.Count));
			}
			else if (property.propertyType == PropertyType.String)
			{
				indexes.Add(Random.Range(0, property.bag.stringValues.Count));
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (timer <= 0f)
		{
			timer = timerMax;

			GeneratePlayer();
		}

		timer -= Time.deltaTime;
	}

	private void GeneratePlayer()
	{
		SetupPlayer();

		foreach (Property property in properties)
		{
			float floatValue = 0;
			string stringValue = "A";
			bool boolValue = true;

			switch (property.propertyType)
			{
				case PropertyType.String:
					{
                        stringValue = property.bag.stringValues[property.index];
                        property.IncrementIndex();

                        print($"{property.name} was generated with value {stringValue}");

						newPropertyStore.AddProperty(property,stringValue);
                        break;
					}
				case PropertyType.Number:
					{
                        floatValue = property.bag.numberValues[property.index];
                        property.IncrementIndex();

						print($"{property.name} was generated with value {floatValue}");

                        newPropertyStore.AddProperty(property, floatValue);
                        break;
					}
				case PropertyType.Bool:
					{
						int randint = Random.Range(-10,10);

						if ( randint < 0 ) 
							{ boolValue = true; }
						else
							{ boolValue = false; }
						

                        print($"{property.name} was generated with value {boolValue}");

                        newPropertyStore.AddProperty(property, boolValue);
                        break;
					}
			}
		}

		newPlayer.transform.parent = Queue.transform;

		string PlayerName = "";
		for (int i = 0; i < 5; i++)
		{
			PlayerName += (char)UnityEngine.Random.Range(65, 90);
		}


		newPlayer.name = PlayerName;
		newItemData.name = PlayerName;
	}

	private void SetupPlayer()
	{
        Vector3 randPos = Queue.transform.position;
        randPos += new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
		

        newPlayer = Instantiate(playerPrefab, randPos, Quaternion.identity);
        newItemData = ScriptableObject.CreateInstance<ItemData>();
        newPropertyStore = ScriptableObject.CreateInstance<PropertyStore>();

        newItemData.propertyStore = newPropertyStore;
        newPlayer.GetComponent<RoomItem>().data = newItemData;

		newItemData.prefab3D = playerPrefab;
		newItemData.icon = playerSpriteDefalt;

		//newItemData.colour = new Color(Random.Range(0,255), Random.Range(0, 255), Random.Range(0, 255));

    }
}
