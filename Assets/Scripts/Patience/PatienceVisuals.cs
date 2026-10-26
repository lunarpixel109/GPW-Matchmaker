using UnityEngine;
using System.Collections;
using TMPro;

public class PatienceVisuals : MonoBehaviour
{
    public PlayerPatience patience;
    public Transform characterVisual;
    public Renderer characterRenderer;

    private Vector3 startPosition;
    public Material characterMaterial;
    private Color originalColour;
    public TMP_Text warningText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = characterVisual.localPosition;

        characterMaterial = characterRenderer.material;
        originalColour = characterMaterial.color;

        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!patience.isWaiting)
        {
            characterVisual.localPosition = startPosition;
            return;
        }

        float patiencePercent = patience.currentPatience / patience.maxPatience;


        if (warningText != null)
        {
            if (patiencePercent <= 0.2f)
            {
                // Critical warning: red
                warningText.gameObject.SetActive(true);

                warningText.color = Color.red;
                warningText.alpha = 1f;

                float pulse = 1f + Mathf.Sin(Time.time * 6f) * 0.1f;
                warningText.transform.localScale =
                    Vector3.one * 0.2f * pulse;
            }
            else if (patiencePercent <= 0.45f)
            {
                // Early warning: amber
                warningText.gameObject.SetActive(true);

                warningText.color = new Color(1f, 0.65f, 0f, 1f);
                warningText.alpha = 1f;

                warningText.transform.localScale = Vector3.one * 0.2f;
            }
            else
            {
                warningText.gameObject.SetActive(false);
            }
        }


        float shakeAmount = 0f;
        float shakeSpeed = 0f;

        if (patiencePercent <= 0.5f)
        {
            float frustration =
                Mathf.InverseLerp(0.5f, 0f, patiencePercent);

            shakeAmount =
                Mathf.Lerp(0.005f, 0.025f, frustration);

            shakeSpeed =
                Mathf.Lerp(12f, 30f, frustration);
        }

        if (shakeAmount > 0f)
        {
            float x =
                Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;

            float z =
                Mathf.Sin(Time.time * shakeSpeed * 1.3f)
                * shakeAmount * 0.5f;

            characterVisual.localPosition =
                startPosition + new Vector3(x, 0f, z);
        }
        else
        {
            characterVisual.localPosition = startPosition;
        }

        // Red flashing
        if (patiencePercent <= 0.3f)
        {
            float flashSpeed =
                Mathf.Lerp(4f, 9f, Mathf.InverseLerp(0.3f, 0f, patiencePercent));

            float flash =
                (Mathf.Sin(Time.time * flashSpeed) + 1f) / 2f;

            characterMaterial.color =
                Color.Lerp(originalColour, Color.red, flash);
        }
        else
        {
            characterMaterial.color = originalColour;
        }
    }

    public void PlayLeaveEffect()
    {
        StartCoroutine(LeaveAnimation());
    }


    IEnumerator LeaveAnimation()
    {
        float effectDuration = 0.4f;
        float impactDuration = 0.12f;
        float impactScale = 1.2f;

        Vector3 originalScale = characterVisual.localScale;

        characterVisual.localPosition = startPosition;
        characterMaterial.color = Color.red;

        float elapsedTime = 0f;

        // Quick enlargement
        while (elapsedTime < impactDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / impactDuration);

            // Smooth acceleration and deceleration
            t = Mathf.SmoothStep(0f, 1f, t);

            characterVisual.localScale =
                Vector3.LerpUnclamped(
                    originalScale,
                    originalScale * impactScale,
                    t
                );

            yield return null;
        }

        elapsedTime = 0f;
        float shrinkDuration = effectDuration - impactDuration;

        // Smooth shrinking and fading
        while (elapsedTime < shrinkDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / shrinkDuration);

            // Fast initial shrink, smooth finish
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);

            characterVisual.localScale =
                Vector3.LerpUnclamped(
                    originalScale * impactScale,
                    Vector3.zero,
                    smoothT
                );

            Color colour = characterMaterial.color;
            colour.a = 1f - t;
            characterMaterial.color = colour;

            yield return null;
        }

        Destroy(gameObject);
    }


}
