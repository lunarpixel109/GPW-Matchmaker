using UnityEngine;

public class PlayerPatience : MonoBehaviour
{
    public float maxPatience;
    public float currentPatience;
    public bool isWaiting = true;
    public PlayerLeftNotification playerLeftNotification;

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

        if (playerLeftNotification != null)
        {
            playerLeftNotification.ShowNotification();
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
