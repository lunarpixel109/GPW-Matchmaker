using UnityEngine;

public class PlayerPatience : MonoBehaviour
{
    public float maxPatience;
    public float currentPatience;
    public bool isWaiting = true;
    public PlayerLeftNotification notificationPanel;

    public GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPatience = maxPatience;

        gameManager = GameManager.Instance;

        notificationPanel = FindFirstObjectByType<PlayerLeftNotification>(FindObjectsInactive.Include);
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

        // Display the players gamertag in the panel
        if (notificationPanel != null)
        {
            notificationPanel.ShowNotification(gameObject.name);
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
