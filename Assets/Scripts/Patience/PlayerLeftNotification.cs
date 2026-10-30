using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerLeftNotification : MonoBehaviour
{
    public float displayDuration = 2f;

    private TMP_Text notificationText;

    void Awake()
    {
        notificationText = GetComponent<TMP_Text>();
        notificationText.enabled = false;
    }

    public void ShowNotification()
    {
        notificationText.enabled = true;

        StartCoroutine(HideNotification());
    }

    IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(displayDuration);

        Destroy(gameObject);
    }
}