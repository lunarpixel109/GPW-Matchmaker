using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using System.Linq;
using TMPro;
using UnityEngine.UI;
public class CardSpawner : MonoBehaviour
{
    [SerializeField] int spawnCount;
    InputAction spawnCards;
    [SerializeField] List<CardProperties> cards = new List<CardProperties>();
    [SerializeField] GameObject baseCardObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnCards = InputSystem.actions.FindAction("SpawnCards");
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnCards.WasPressedThisFrame())
        {
            SpawnCards();
            Debug.Log("Button pressed");
        }
    }

    void SpawnCards()
    {
        List<int> cardsToSpawn = new List<int>();

        if (spawnCount > cards.Count) //If the given number of cards to spawn is higher than the amount of cards that exist.
            spawnCount = cards.Count; //Set cards to be spawned to the max possible

        for (int i = 0; i < spawnCount; i++) //Loops for how many cards are wanting to be spawned
        {
            int cardToUse = Random.Range(0, cards.Count); //Get a random int co-orosponding to IDs of the cards
            bool unique = false;
            
            while (!unique) //Ensure's the card doesn't appear twice in the list of cards to spawn
            {
                unique = true; //If the card is unique, exit the loop...
                

                for (int j = 0; j < cardsToSpawn.Count; j++) //Runs for how many cards are already in the list
                {
                    if (cardsToSpawn[j] == cardToUse) //If the currently card already selected is the same as the one that might be added 
                    {
                        cardToUse = Random.Range(0, cards.Count); //If it is. try again and generate a new card ID.
                        unique = false;
                    }
                }
            }
            Debug.Log(cardToUse);
            cardsToSpawn.Add(cardToUse); //And add it to the list of cards to be spawned
        }
        for (int i = 0; i < cardsToSpawn.Count; i++) //For every card to be spawned
        {
            for (int j = 0; j < cards.Count; j++) //Check the database of card data
            {
                if (cards[j].cardID == cardsToSpawn[i]) //If card data ID matches that of the card to be spawned...
                {
                    GameObject spawnedCard = Instantiate(baseCardObject); //Create the base card object
                    InitializeCard(spawnedCard, j); //Give it the name, Description and icon of the card to be spawned.
                }
            }
        }
        
    }

    void InitializeCard(GameObject newCard, int ID)
    {
        newCard.transform.SetParent(GameObject.Find("Canvas").transform); //Set the parent to the canvas

        TextMeshPro name = newCard.transform.Find("Name").GetComponent<TextMeshPro>(); //Find the text component for the text of the prefab
        TextMeshPro description = newCard.transform.Find("Description").GetComponent<TextMeshPro>(); //find the description component for the text of the prefab
        Image icon = newCard.transform.Find("Icon").GetComponent<Image>(); //Find the icon component for image of the prefab

        name.text = cards[ID].cardName;
        description.text = cards[ID].cardDescription;
        icon.sprite = cards[ID].Icon;
    } 
}
