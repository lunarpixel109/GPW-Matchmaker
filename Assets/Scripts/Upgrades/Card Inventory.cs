using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CardInventory : MonoBehaviour
{
    //inventory size
    List<GameObject> cards = new List<GameObject>();
    [SerializeField] int maxInventorySize;
    //Test inputs
    InputAction one, two, three, four, five;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCardUpgradeToInventory(GameObject card)
    {
        if (cards.Count < maxInventorySize)
        {
            cards.Append(card);
            CardSelector sc = card.GetComponent<CardSelector>();
            if (sc != null)
            {
                sc.cardIndex = cards.IndexOf(card);
            }
        }
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            CardSelector sc = cards[i].GetComponent<CardSelector>();
            sc.cardIndex = i;
        }
    }
}
