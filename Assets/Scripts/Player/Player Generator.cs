using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.EditorGUI;
using static UnityEngine.Audio.GeneratorInstance;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class PlayerGenerator : MonoBehaviour
{
    private float timer = 0f;
	[SerializeField]
	private float timerMax;

	[SerializeField]
	private GameObject playerPrefab;

	[SerializeField]
	private GameObject Queue;
	[SerializeField]
	private GameObject[] Lobbies;

	[SerializeField] private List<int> indexes;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		timer = timerMax;

		indexes = new List<int>();
        foreach (Property property in this.gameObject.GetComponent<PropertyManager>().properties)
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
		Vector3 randPos = new Vector3(Random.Range(-10,10),Random.Range(-10,10),Random.Range(-10,10));

		GameObject newPlayer = Instantiate(playerPrefab,randPos,Quaternion.identity);

        foreach (Property property in this.gameObject.GetComponent<PropertyManager>().properties)
        {
            //newPlayer.GetComponent<PropertyStore>().AddProperty(property);
        }

		newPlayer.transform.parent = Queue.transform;

        string PlayerName = "";
        for (int i = 0; i < 5; i++)
        {
        	PlayerName += (char)UnityEngine.Random.Range(65, 90);
        }


        newPlayer.name = PlayerName;

		//newPlayer.GetComponent<PropertyStore>().PrintAllProperties();
    }
}
