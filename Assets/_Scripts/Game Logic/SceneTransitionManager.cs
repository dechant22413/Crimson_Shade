using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject loadingCanvas;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Loading")]
    [SerializeField] private float minLoadDuration = 3f;

    private bool isLoading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (loadingCanvas != null)
            loadingCanvas.SetActive(false);

        // Transparent starten
        fadeCanvasGroup.alpha = 0f;
    }

    public void LoadScene(int buildIndex)
    {
        if (isLoading)
            return;
        StartCoroutine(LoadRoutine(buildIndex));
    }

    private IEnumerator LoadRoutine(int buildIndex)
    {
        isLoading = true;

        // 1. Loading Screen einblenden
        loadingCanvas.SetActive(true);

        // 2. Szene im Hintergrund laden
        AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);
        operation.allowSceneActivation = false;

        // 3. Warten bis Unity fertig geladen hat UND die minimale Ladezeit vergangen ist
        float elapsed = 0f;
        while (operation.progress < 0.9f || elapsed < minLoadDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4. FadeOut - Loading Canvas liegt darunter und bleibt sichtbar
        yield return StartCoroutine(Fade(0f, 1f));

        // 5. Screen ist jetzt komplett schwarz - Loading Canvas ausblenden
        loadingCanvas.SetActive(false);

        // 6. Neue Szene aktivieren
        operation.allowSceneActivation = true;
        while (!operation.isDone)
        {
            yield return null;
        }

        // 7. Einen Frame warten damit die neue Szene komplett initialisiert ist
        yield return null;

        // 8. FadeIn in die neue Szene
        yield return StartCoroutine(Fade(1f, 0f));

        isLoading = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        fadeCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        // Sicherstellen dass der Zielwert exakt erreicht wird
        fadeCanvasGroup.alpha = to;
    }
}