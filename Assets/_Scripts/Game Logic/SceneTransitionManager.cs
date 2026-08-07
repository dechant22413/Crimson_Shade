
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

        // Fade transparent starten
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        // Cursor beim Start sichtbar machen
        ShowCursor();
    }

    private void OnEnable()
    {
        isLoading = false;

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadScene(int buildIndex)
    {
        // Alle laufenden Coroutinen stoppen
        StopAllCoroutines();

        isLoading = false;

        // Sicherstellen, dass das Spiel nicht pausiert ist
        Time.timeScale = 1f;

        StartCoroutine(LoadRoutine(buildIndex));
    }

    private IEnumerator LoadRoutine(int buildIndex)
    {
        isLoading = true;

        Debug.Log("LoadRoutine gestartet");

        // --------------------------------------------------
        // 1. Loading Screen anzeigen
        // --------------------------------------------------

        Debug.Log("loadingCanvas: " + loadingCanvas);
        Debug.Log("fadeCanvasGroup: " + fadeCanvasGroup);

        if (loadingCanvas != null)
            loadingCanvas.SetActive(true);

        Debug.Log("Loading Screen aktiv");

        // --------------------------------------------------
        // 2. Szene im Hintergrund laden
        // --------------------------------------------------

        AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);

        operation.allowSceneActivation = false;

        Debug.Log("AsyncOperation gestartet");

        // --------------------------------------------------
        // 3. Warten bis Unity fertig geladen hat
        //    und die minimale Ladezeit vergangen ist
        // --------------------------------------------------

        float elapsed = 0f;

        while (operation.progress < 0.9f || elapsed < minLoadDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Debug.Log("Laden abgeschlossen");

        // --------------------------------------------------
        // 4. Fade auf schwarz
        // --------------------------------------------------

        Debug.Log("FadeOut startet");

        yield return StartCoroutine(Fade(0f, 1f));

        Debug.Log("FadeOut fertig");

        // --------------------------------------------------
        // 5. Loading Canvas ausblenden
        // --------------------------------------------------

        if (loadingCanvas != null)
            loadingCanvas.SetActive(false);

        // --------------------------------------------------
        // 6. Neue Szene aktivieren
        // --------------------------------------------------

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }

        Debug.Log("Szene geladen");

        // --------------------------------------------------
        // 7. Cursor zurücksetzen
        // --------------------------------------------------

        ShowCursor();

        // Einen Frame warten, damit die neue Szene
        // vollständig initialisiert werden kann
        yield return null;

        // --------------------------------------------------
        // 8. Fade von schwarz auf transparent
        // --------------------------------------------------

        Debug.Log("FadeIn startet");

        yield return StartCoroutine(Fade(1f, 0f));

        Debug.Log("FadeIn fertig");

        isLoading = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeCanvasGroup == null)
            yield break;

        float elapsed = 0f;

        fadeCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            fadeCanvasGroup.alpha = Mathf.Lerp(
                from,
                to,
                elapsed / fadeDuration
            );

            yield return null;
        }

        // Sicherstellen, dass der Zielwert exakt erreicht wird
        fadeCanvasGroup.alpha = to;
    }
}

