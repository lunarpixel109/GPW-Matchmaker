using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerLeftNotification : MonoBehaviour
{
    public float displayDuration = 1.5f;

    [SerializeField] private TMP_Text notificationText;

    private Coroutine hideCoroutine;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ShowNotification(string playerName)
    {
        gameObject.SetActive(true);

        notificationText.text = playerName + " Left!";

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideNotification());
    }

    IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(displayDuration);

        gameObject.SetActive(false);
        hideCoroutine = null;
    }
}