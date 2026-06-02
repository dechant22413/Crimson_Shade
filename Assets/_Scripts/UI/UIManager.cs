using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    #region Singleton Initialization
    //Singelton
    public static UIManager Instance;
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    #endregion

    #region Settings
    [Header("References")]
    [SerializeField] private GameObject hitIndicator;
    [SerializeField] private GameObject killIndicator;

    [SerializeField] private GameObject gameOverPanel;

    [Header("Indicator Settings")]
    [SerializeField] private float displayTime = 0.2f;
    #endregion

    private Coroutine currentIndicatorCoroutine;

    private void Start()
    {
        //Hided Cursor zu Beginn der Szene
        HideCursor();

        gameOverPanel.SetActive(false);
    }

    public void HideCursor()
    {
        //Hided Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCursor()
    {
        //Zeigt Cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ShowHitIndicator()
    {
        // Hit nicht anzeigen, wenn Kill aktiv ist
        if (killIndicator.activeSelf) return;

        //Zeigt Hit Indicator
        StartIndicator(hitIndicator);
    }

    public void ShowKillIndicator()
    {
        // laufende Anzeige abbrechen
        if (currentIndicatorCoroutine != null)
            StopCoroutine(currentIndicatorCoroutine);

        // Hit sofort ausblenden, da Kill Indicator Priorität haben soll
        hitIndicator.SetActive(false); 

        //Zeigt Kill Indicator
        currentIndicatorCoroutine = StartCoroutine(IndicatorCoroutine(killIndicator));
    }

    public void ActivateGameOverPanel(bool activate)
    {
        if (activate)
        {
            gameOverPanel.SetActive(true);
            ShowCursor();
        }
        else
        {
            gameOverPanel.SetActive(false);
            HideCursor();
        }
    }

    private void StartIndicator(GameObject indicator)
    {
        if (currentIndicatorCoroutine != null)
            StopCoroutine(currentIndicatorCoroutine);

        //Startet Indicator Coroutine
        currentIndicatorCoroutine = StartCoroutine(IndicatorCoroutine(indicator));
    }

    private IEnumerator IndicatorCoroutine(GameObject indicator)
    {
        //Aktiviert ensprechenden Indicator
        indicator.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        indicator.SetActive(false);
        currentIndicatorCoroutine = null;
    }
}