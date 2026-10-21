using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.EditorGUI;
using static UnityEngine.Audio.GeneratorInstance;
using Random = UnityEngine.Random;

public class PlayerGenerator : MonoBehaviour
{
    private float timer = 0f;
	[SerializeField]
	private int timerMax;

	[SerializeField]
	private GameObject playerPrefab;

	[SerializeField]
	private GameObject Queue;
	[SerializeField]
	private GameObject[] Lobbies;



	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		timer = timerMax;
		//properties = ;
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
            newPlayer.GetComponent<PropertyStore>().AddProperty(property);
        }

		newPlayer.transform.parent = Queue.transform;

		newPlayer.GetComponent<PropertyStore>().PrintAllProperties();
    }
}
