using UnityEngine;

public class PatienceVisuals : MonoBehaviour
{
    public PlayerPatience patience;
    public Transform characterVisual;

    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = characterVisual.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Shake script running. Patience: " + patience.currentPatience);

        if (!patience.isWaiting)
        {
            characterVisual.localPosition = startPosition;
            return;
        }

        float patiencePercent = patience.currentPatience / patience.maxPatience;

        float shakeAmount = 0f;

        if (patiencePercent <= 0.1f)
        {
            shakeAmount = 0.5f;
        }
        else if (patiencePercent <= 0.3f)
        {
            shakeAmount = 0.3f;
        }
        else if (patiencePercent <= 0.6f)
        {
            shakeAmount = 0.15f;
        }

        if (shakeAmount > 0f)
        {
            float x = Random.Range(-shakeAmount, shakeAmount);
            float z = Random.Range(-shakeAmount, shakeAmount);

            characterVisual.localPosition = startPosition + new Vector3(x, 0f, z);
        }
        else
        {
            characterVisual.localPosition = startPosition;
        }
    }
}
