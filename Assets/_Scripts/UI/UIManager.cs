using System.Collections;
using UnityEngine;

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
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject DemoFinishedMenu;


    [Header("Indicator Settings")]
    [SerializeField] private float displayTime = 0.2f;
    #endregion

    private Coroutine currentIndicatorCoroutine;



    private void Start()
    {
        //Hided Cursor zu Beginn der Szene
        HideCursor();

        gameOverMenu.SetActive(false);
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

    public void ActivateGameOverMenu(bool activate)
    {
        if (activate)
        {
            gameOverMenu.SetActive(true);
            ShowCursor();
        }
        else
        {
            gameOverMenu.SetActive(false);
            HideCursor();
        }
    }

    public void ActivatePauseGameMenu(bool activate)
    {
        if (activate)
        {
            pauseMenu.SetActive(true);
            ShowCursor();
        }
        else
        {
            pauseMenu.SetActive(false);
            HideCursor();
        }
    }

    public void ActivateDemoFinishedMenu(bool activate)
    {
        if (activate)
        {
            DemoFinishedMenu.SetActive(true);
            ShowCursor();
        }
        else
        {
            DemoFinishedMenu.SetActive(false);
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