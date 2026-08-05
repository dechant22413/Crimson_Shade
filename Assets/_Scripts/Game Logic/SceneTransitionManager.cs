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

    private void OnEnable()
    {
        // Sicherstellen dass isLoading nie hängen bleibt nach Szenenwechsel
        isLoading = false;
        fadeCanvasGroup.alpha = 0f;
    }

    public void LoadScene(int buildIndex)
    {
        // Alle laufenden Coroutinen stoppen falls noch eine hängt
        StopAllCoroutines();
        isLoading = false;

        Time.timeScale = 1f;
        StartCoroutine(LoadRoutine(buildIndex));
    }

    private IEnumerator LoadRoutine(int buildIndex)
    {
        isLoading = true;
        Debug.Log("LoadRoutine gestartet");

        // 1. Loading Screen einblenden
        Debug.Log("loadingCanvas: " + loadingCanvas);
        Debug.Log("fadeCanvasGroup: " + fadeCanvasGroup);
        loadingCanvas.SetActive(true);
        Debug.Log("Loading Screen aktiv");

        // 2. Szene im Hintergrund laden
        AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);
        operation.allowSceneActivation = false;
        Debug.Log("AsyncOperation gestartet");

        // 3. Warten bis Unity fertig geladen hat UND die minimale Ladezeit vergangen ist
        float elapsed = 0f;
        while (operation.progress < 0.9f || elapsed < minLoadDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Laden abgeschlossen");

        // 4. FadeOut
        Debug.Log("FadeOut startet");
        yield return StartCoroutine(Fade(0f, 1f));
        Debug.Log("FadeOut fertig");

        // 5. Loading Canvas ausblenden
        loadingCanvas.SetActive(false);

        // 6. Neue Szene aktivieren
        operation.allowSceneActivation = true;
        while (!operation.isDone)
        {
            yield return null;
        }
        Debug.Log("Szene geladen");

        yield return null;

        // 7. FadeIn
        Debug.Log("FadeIn startet");
        yield return StartCoroutine(Fade(1f, 0f));
        Debug.Log("FadeIn fertig");

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