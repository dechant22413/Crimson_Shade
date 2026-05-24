using UnityEngine;
using System.Collections;

public class PopWobbleJuice : MonoBehaviour
{
    #region Settings
    [Header("UI Behaviour")]
    [SerializeField] private bool popOnEnable = false;
    [SerializeField] private bool popOnStart = false;
    public bool continuousWobble = false;

    [Header("Pop Settings")]
    [SerializeField] private float popScale = 1.1f;
    [SerializeField] private float popDuration = 0.1f;

    [Header("Wobble Settings")]
    [SerializeField] private float wobbleSpeed = 8f;
    [SerializeField] private float wobbleScale = 1.05f;
    #endregion

    private Vector3 originalScale;
    private bool wasWobbling;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        //Startet bei Start einen Pop, wenn gewünscht
        if (popOnStart)
        {
            StartPop();
        }
    }

    private void OnEnable()
    {
        //Startet bei Enable einen Pop, wenn gewünscht
        if (!popOnEnable)
            return;

        originalScale = transform.localScale;
        StartPop();
    }

    private void Update()
    {
        if (continuousWobble)
        {
            wasWobbling = true;
            float range = wobbleScale - 1f;
            float offset = Mathf.Sin(Time.time * wobbleSpeed) * range;
            transform.localScale = originalScale * (1f + offset);
        }
        else if (wasWobbling)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * wobbleSpeed);
            if (Vector3.Distance(transform.localScale, originalScale) < 0.001f)
            {
                transform.localScale = originalScale;
                wasWobbling = false;
            }
        }
    }

    //Startet einen Pop
    public void StartPop()
    {
        StartCoroutine(Pop());
    }

    private IEnumerator Pop()
    {
        Vector3 targetScale = originalScale * popScale;
        float timer = 0f;

        // hochpoppen
        while (timer < popDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;

        // wieder zurück
        timer = 0f;
        while (timer < popDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / popDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}
