using UnityEngine;

public class PlayerPatience : MonoBehaviour
{
    public float maxPatience;
    public float currentPatience;
    public bool isWaiting = true;
    public PlayerLeftNotification notificationPrefab;
    public Canvas notificationCanvas;
    public Camera roomCamera;
    public RectTransform roomWindow;

    public GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPatience = maxPatience;

        gameManager = GameManager.Instance;

        roomCamera = GameObject.Find("3D-room-camera").GetComponent<Camera>();
        roomWindow = GameObject.Find("3D-window").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager == null)
        {
            print("sad");
        }

        if (isWaiting && !gameManager.pause)
        {
            currentPatience -= Time.deltaTime;
            if (currentPatience <= 0f)
            {
                currentPatience = 0f;
                LeaveMatchmaking();
            }
        }
    }

    void LeaveMatchmaking()
    {
        isWaiting = false;

        if (notificationPrefab != null && notificationCanvas != null)
        {
            // Find the players position in the waiting room
            Vector3 viewportPosition = roomCamera.WorldToViewportPoint(transform.position + Vector3.up * 1f);

            // Only show the notification if the player is visible
            if (viewportPosition.z > 0f &&
               viewportPosition.x >= 0f && viewportPosition.x <= 1f &&
               viewportPosition.y >= 0f && viewportPosition.y <= 1f)
            {
                // Convert the position into the waiting room window
                Vector2 windowPosition = new Vector2(
                    (viewportPosition.x - 0.5f) * roomWindow.rect.width,
                    (viewportPosition.y - 0.5f) * roomWindow.rect.height
                    );


                Vector3 worldPosition = roomWindow.TransformPoint(windowPosition);

                //Convert into the canvas coordinates
                Camera uiCamera = notificationCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : notificationCanvas.worldCamera;

                Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPosition);

                RectTransform canvasRect = notificationCanvas.GetComponent<RectTransform>();

                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, uiCamera, out Vector2 localPosition);

                // Create and position the notification
                PlayerLeftNotification notification = Instantiate(notificationPrefab, notificationCanvas.transform);

                RectTransform notificationRect = notification.GetComponent<RectTransform>();

                notificationRect.anchoredPosition = localPosition + new Vector2(0f, -35f);

                notification.ShowNotification();
            }
        }

        PatienceVisuals visuals = GetComponent<PatienceVisuals>();

        if (visuals != null)
        {
            visuals.PlayLeaveEffect();
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("Player has left matchmaking!");
    }
}
