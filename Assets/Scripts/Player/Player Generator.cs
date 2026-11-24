using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Scriptables;

public class PlayerGenerator : MonoBehaviour
{

	private float timer = 0f;

	[SerializeField]
	private LevelData levelData;

	private List<GameObject> playerPrefabs;

	[SerializeField]
	private Sprite playerSpriteDefalt;

	[SerializeField]
	private GameObject Queue;
	[SerializeField]
	private GameObject[] Lobbies;

	[SerializeField] private List<int> indexes;

	GameObject newPlayer;
	ItemData newItemData;
	PropertyStore newPropertyStore;

    [SerializeField] private NameList prefix;
    [SerializeField] private NameList firstName;
	[SerializeField] private NameList secondName;
	[SerializeField] private NameList suffix;

	[SerializeField] private bool printDebug;

	[SerializeField] private GameManager gameManager;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

		gameManager = this.gameObject.GetComponent<GameManager>();

        levelData = gameManager.GetCurrentLevelData();

		playerPrefabs = levelData.playerPrefabs;

		timer = levelData.playerGenerationCooldown;

		indexes = new List<int>();
		foreach (Property property in levelData.properties)
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
			timer = levelData.playerGenerationCooldown;

			GeneratePlayer();
		}

		if (!gameManager.pause) { timer -= Time.deltaTime; }
	}

	private void GeneratePlayer()
	{
		SetupPlayer();

		foreach (Property property in levelData.properties)
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

						PrintPlayer(property,stringValue);

						newPropertyStore.AddProperty(property,stringValue);
                        break;
					}
				case PropertyType.Number:
					{
                        floatValue = property.bag.numberValues[property.index];
                        property.IncrementIndex();

                        PrintPlayer(property, floatValue);

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

                        PrintPlayer(property, boolValue);

                        newPropertyStore.AddProperty(property, boolValue);
                        break;
					}
			}
		}

		newPlayer.transform.parent = Queue.transform;

        string PlayerName = "";
        //for (int i = 0; i < 5; i++)
        //{
        //	PlayerName += (char)UnityEngine.Random.Range(65, 90);
        //}

        if (UnityEngine.Random.Range(0, 100) > 75)
        {
            PlayerName += prefix.names[UnityEngine.Random.Range(0, prefix.names.Count)];
        }

        PlayerName += firstName.names[UnityEngine.Random.Range(0, firstName.names.Count)];

        if (UnityEngine.Random.Range(0, 100) > 50)
        {
            PlayerName += secondName.names[UnityEngine.Random.Range(0, secondName.names.Count)];
        }

        if (UnityEngine.Random.Range(0, 100) > 75)
        {
            PlayerName += suffix.names[UnityEngine.Random.Range(0, suffix.names.Count)];
        }


        newPlayer.name = PlayerName;
        newItemData.name = PlayerName;
		newItemData.displayName = PlayerName;
	}

	private void PrintPlayer(Property property, string value)
	{
		if (printDebug == true)
		{
            print($"{property.name} was generated with value {value}");
        }
    }
	private void PrintPlayer(Property property, float value)
	{
        if (printDebug == true)
        {
            print($"{property.name} was generated with value {value}");
        }
    }
	private void PrintPlayer(Property property, bool value)
	{
        if (printDebug == true)
        {
            print($"{property.name} was generated with value {value}");
        }
    }

	private void SetupPlayer()
	{
        Vector3 randPos = Queue.transform.position;
        randPos += new Vector3(RandomInRanges(-3,-2,2,3), 0.5f, Random.Range(-5, -4.5f));

        int rand = Random.Range(0, playerPrefabs.Count);
        
        newPlayer = Instantiate(playerPrefabs[rand], randPos, Quaternion.identity);
        newItemData = ScriptableObject.CreateInstance<ItemData>();
        newPropertyStore = ScriptableObject.CreateInstance<PropertyStore>();

        newItemData.propertyStore = newPropertyStore;
        newPlayer.GetComponent<RoomItem>().data = newItemData;
        PlayerManager pm = newPlayer.GetComponent<PlayerManager>();
        pm.autoPlaceOnStart =  true;

		newItemData.prefab3D = playerPrefabs[rand];
		newItemData.icon = playerSpriteDefalt;

		//newItemData.colour = new Color(Random.Range(0,255), Random.Range(0, 255), Random.Range(0, 255));

    }

	private static float RandomInRanges(float minA, float maxA, float minB, float maxB)
	{
		float widthA = maxA - minA;
		float widthB = maxB - minB;

		float roll = Random.Range(0f, widthA + widthB);
		return roll < widthA
			? minA + roll
			: minB + (roll - widthA);
	}
}
