using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    public string levelSceneName = "Level1";

    [Header("UI References (Drag from Scene)")]
    public CanvasGroup canvasGroup;
    public RectTransform titleTransform;
    public TMP_Text titleText;
    public Color accentColor = new Color(0.00f, 0.90f, 1.00f);

    [Header("Animation Settings")]
    public float floatAmplitude = 6f;
    public float floatSpeed = 1.2f;
    public float pulseSpeed = 1.6f;
    public float fadeDuration = 0.8f;

    private Vector2 titleBasePos;
    private bool loading;

    void Start()
    {
        // Cache the starting position of the title
        if (titleTransform != null)
            titleBasePos = titleTransform.anchoredPosition;

        // Start with the screen invisible
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        // Fade in
        StartCoroutine(Fade(0f, 1f, fadeDuration));
    }

    void Update()
    {
        // Title float and pulse effect
        if (titleTransform != null && titleText != null)
        {
            titleTransform.anchoredPosition = titleBasePos + new Vector2(0f, Mathf.Sin(Time.unscaledTime * floatSpeed) * floatAmplitude);
            
            float pulse = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f * 0.25f;
            titleText.color = Color.Lerp(accentColor, Color.white, pulse);
        }

        // Keyboard shortcut: Enter starts the game
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            PlayGame();
    }

    // --- Public API for Buttons ---

    public void PlayGame()
    {
        if (loading) return;
        loading = true;
        StartCoroutine(FadeAndLoad());
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game Requested!");

        #if UNITY_EDITOR
            // Stops Play Mode in the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Closes the application in a built game (.exe / .app)
            Application.Quit();
        #endif
    }

    // --- Transitions ---

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            if (canvasGroup != null) canvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        if (canvasGroup != null) canvasGroup.alpha = to;
    }

    IEnumerator FadeAndLoad()
    {
        yield return Fade(canvasGroup != null ? canvasGroup.alpha : 1f, 0f, 0.4f);
        SceneManager.LoadScene(levelSceneName);
    }
}