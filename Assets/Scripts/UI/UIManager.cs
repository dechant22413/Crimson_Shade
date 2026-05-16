using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    [SerializeField] private GameObject hitIndicator;
    [SerializeField] private GameObject killIndicator;
    [SerializeField] private float displayTime = 0.2f;

    private Coroutine currentIndicatorCoroutine;

    public void ShowHitIndicator()
    {
        // Hit nicht anzeigen, wenn Kill aktiv ist
        if (killIndicator.activeSelf) return;

        StartIndicator(hitIndicator);
    }

    public void ShowKillIndicator()
    {
        // laufende Anzeige abbrechen
        if (currentIndicatorCoroutine != null)
            StopCoroutine(currentIndicatorCoroutine);

        hitIndicator.SetActive(false); // Hit sofort ausblenden

        currentIndicatorCoroutine =
            StartCoroutine(IndicatorCoroutine(killIndicator));
    }

    private void StartIndicator(GameObject indicator)
    {
        if (currentIndicatorCoroutine != null)
            StopCoroutine(currentIndicatorCoroutine);

        currentIndicatorCoroutine =
            StartCoroutine(IndicatorCoroutine(indicator));
    }

    private IEnumerator IndicatorCoroutine(GameObject indicator)
    {
        indicator.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        indicator.SetActive(false);
        currentIndicatorCoroutine = null;
    }
}