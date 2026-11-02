using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CardSelector : MonoBehaviour
{
    Camera cam;
    InputAction click;
    bool mouseOver;
    //card corners
    Vector3 bottomLeftCorner;
    Vector3 topRightCorner;
    //Scale at the start when spawned
    Vector3 baseScale;
    RectTransform rt;
    //Card Inventory system
    CardInventory cardInventory;
    public int cardIndex;
 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>(); //Finds the camera component
        click = InputSystem.actions.FindAction("click"); //binds the click action to a vartiable
        baseScale = transform.localScale; //Sets base scale to the scale set in the editor
        cardInventory = GameObject.FindGameObjectWithTag("UpgradeInventory").GetComponent<CardInventory>();
        rt = GetComponent<RectTransform>();
        GetCorners();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0); //Gets the mouse position on screen

        //Checks to see if the mouse position is withtin the bounds set by the bottom left and top right corner.
        if (mousePos.x > bottomLeftCorner.x && mousePos.y > bottomLeftCorner.y && mousePos.x < topRightCorner.x && mousePos.y < topRightCorner.y)
        {
            MouseOverCard();
        }
        //If not and it hasn't returned to normal it will run this script.
        else if(transform.localScale != baseScale)
        {
            MouseOffCard();
        }

        //If the mouse is over the card and the player clicks.
        if (mouseOver == true && click.WasPressedThisFrame())
        {
            cardInventory.AddCardUpgradeToInventory(this.transform.gameObject);
        }
    }

    void MouseOverCard()
    {
        mouseOver = true;
        //increase the card size.
        rt.localScale = new Vector3(baseScale.x * 1.2f, baseScale.y * 1.2f, baseScale.z * 1.2f);

        //Finds the bottom left and top right corners of the card in world space for use in detecting if the mouse is over the card.
        GetCorners();
    }

    void MouseOffCard()
    {
        mouseOver = false;
        //reset the card size.
        rt.localScale = new Vector3(baseScale.x, baseScale.y, baseScale.z);

        GetCorners();
    }

    void GetCorners()
    {
        Vector3[] v = new Vector3[4];
        rt.GetWorldCorners(v);

        //Finds the bottom left and top right corners of the card in world space for use in detecting if the mouse is over the card.    
        bottomLeftCorner = v[0];
        topRightCorner = v[2];
    }
}
