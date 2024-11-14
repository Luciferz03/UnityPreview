using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFadeAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("The button whose alpha will be animated.")]
    public Button targetButton;

    [Tooltip("Minimum alpha value.")]
    [Range(0f, 1f)]
    public float minAlpha = 0.5f;

    [Tooltip("Maximum alpha value.")]
    [Range(0f, 1f)]
    public float maxAlpha = 1f;

    [Tooltip("Duration of one full cycle (fade in and out).")]
    public float cycleDuration = 2f;

    [SerializeField] private bool isAnimating = true;

    private void Start()
    {
        if (targetButton != null)
        {
            StartCoroutine(AnimateAlpha());
        }
        else
        {
            Debug.LogError("Target button is not assigned.");
        }
    }

    private IEnumerator AnimateAlpha()
    {
        while (isAnimating)
        {
            yield return StartCoroutine(FadeAlpha(minAlpha, maxAlpha, cycleDuration / 2));

            yield return StartCoroutine(FadeAlpha(maxAlpha, minAlpha, cycleDuration / 2));

        }
    }

    private IEnumerator FadeAlpha(float fromAlpha, float toAlpha, float duration)
    {
        float elapsedTime = 0f;
        ColorBlock colorBlock = targetButton.colors;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore timeScale
            float newAlpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / duration);

            // Set the alpha for the normal color of the button
            Color normalColor = colorBlock.normalColor;
            normalColor.a = newAlpha;
            colorBlock.normalColor = normalColor;

            // Apply the updated ColorBlock to the button
            targetButton.colors = colorBlock;

            // Debug log to track the alpha value in the cycle

            yield return null;
        }

        // Ensure final alpha is set at the end of the fade
        Color finalColor = colorBlock.normalColor;
        finalColor.a = toAlpha;
        colorBlock.normalColor = finalColor;
        targetButton.colors = colorBlock;

    }

    // Optionally, you can add methods to start/stop the animation dynamically
    public void StartAnimation()
    {
        if (!isAnimating)
        {
            isAnimating = true;
            StartCoroutine(AnimateAlpha());
        }
    }

    public void StopAnimation()
    {
        isAnimating = false;
        StopAllCoroutines();
    }
}
