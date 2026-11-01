using System.Globalization;
using TMPro;
using UnityEngine;

public class PlayerInformationPanel : MonoBehaviour
{

    [SerializeField] private GameObject playerInformationHeader;

    [SerializeField] private GameObject playerPropertyPanel;

    [SerializeField] private GameObject playerPropertyPrefab;

    [SerializeField] private RoomWindow roomWindow;

    private RoomItem previousPlayer;
    private RoomItem currentPlayer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roomWindow = FindObjectsByType<RoomWindow>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)[0];
    }

    // Update is called once per frame
    void Update() {

        currentPlayer = roomWindow.currentItem;

        if (previousPlayer != currentPlayer) {

            for (int i = 0; i < playerPropertyPanel.transform.childCount; ++i) {
                Destroy(playerPropertyPanel.transform.GetChild(i).gameObject);
            }

            previousPlayer = currentPlayer;

            playerInformationHeader.GetComponentInChildren<TextMeshProUGUI>().text = currentPlayer.name;

            for (int i = 0; i < currentPlayer.data.propertyStore.properties.Count; ++i) {
                var propertyObject = Instantiate(playerPropertyPrefab, playerPropertyPanel.transform);
                propertyObject.name = currentPlayer.data.propertyStore.properties[i].name;
                var header = GameObject
                    .Find($"Canvas/PlayerInformationPanel/PlayerProperties/{propertyObject.name}/Property Name")
                    .GetComponent<TextMeshProUGUI>();
                var value = GameObject
                    .Find($"Canvas/PlayerInformationPanel/PlayerProperties/{propertyObject.name}/Property Value")
                    .GetComponent<TextMeshProUGUI>();
                header.text = currentPlayer.data.propertyStore.properties[i].name;
                switch (currentPlayer.data.propertyStore.properties[i].propertyType) {
                    case PropertyType.Bool:
                        value.text = currentPlayer.data.propertyStore.propertyValues[i].BoolValue.ToString();
                        break;
                    case PropertyType.Number:
                        value.text = currentPlayer.data.propertyStore.propertyValues[i].NumberValue.ToString(CultureInfo.CurrentCulture);
                        break;
                    case PropertyType.String:
                        value.text = currentPlayer.data.propertyStore.properties[i].validStrings[(int)currentPlayer.data.propertyStore.propertyValues[i].NumberValue];
                        break;
                }
            }
        }
    }




}
