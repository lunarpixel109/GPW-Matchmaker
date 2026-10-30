using UnityEngine;

public class PlayerPatience : MonoBehaviour
{
    public float maxPatience;
    public float currentPatience;
    public bool isWaiting = true;
    public PlayerLeftNotification notificationPrefab;
    public Canvas notificationCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPatience = maxPatience;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaiting)
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
            Vector3 worldPosition = transform.position + Vector3.up * 2f;

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            if (screenPosition.z > 0f)
            {
                PlayerLeftNotification notification = Instantiate(notificationPrefab, notificationCanvas.transform);

                RectTransform canvasRect = notificationCanvas.GetComponent<RectTransform>();

                RectTransform notificationRect = notification.GetComponent<RectTransform>();

                Camera uiCamera = notificationCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : notificationCanvas.worldCamera;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, uiCamera, out Vector2 localPosition);

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
