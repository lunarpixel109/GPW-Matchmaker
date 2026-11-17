using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Patience2D : MonoBehaviour
{
    public PlayerPatience patience;

    private Vector3 startPosition;

    public Image playerImage;

    private Color normalColour;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.localPosition;
        normalColour = playerImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (patience == null)
        {
            playerImage.color = normalColour;
            transform.localPosition = startPosition;
            return;
        }

        if (!patience.isWaiting)
        {
            enabled = false;
            return;
        }

        float patiencePercent = patience.currentPatience / patience.maxPatience;

        float shakeAmount = 0f;

        if (patiencePercent <= 0.1f)
        {
            shakeAmount = 5f;
        }
        else if (patiencePercent <= 0.3f)
        {
            shakeAmount = 3f;
        }
        else if (patiencePercent <= 0.6f)
        {
            shakeAmount = 1f;
        }

        if (shakeAmount > 0)
        {
            float x = Random.Range(-shakeAmount, shakeAmount);
            float y = Random.Range(-shakeAmount, shakeAmount);

            transform.localPosition = startPosition + new Vector3(x, y, 0);
        }
        else
        {
            transform.localPosition = startPosition;
        }

        if (patiencePercent <= 0.3f)
        {
            float flashSpeed = patiencePercent <= 0.1f ? 8f : 4f;

            float flash = (Mathf.Sin(Time.time * flashSpeed) + 1f) / 2f;

            playerImage.color = Color.Lerp(normalColour, Color.red, flash);
        }
        else
        {
            playerImage.color = normalColour;
        }
    }
}
