using UnityEngine;
using System.Collections;

public class PopWobbleJuice : MonoBehaviour
{
    public bool continuousWobble;
    [SerializeField] private float popScale = 1.1f;
    [SerializeField] private float popDuration = 0.1f;

    [SerializeField] private bool popOnStart;
    [SerializeField] private float wobbleSpeed = 8f;
    [SerializeField] private float wobbleScale = 1.05f;

    [Header("UI Behaviour")]
    [SerializeField] private bool popOnEnable = false;

    private Vector3 originalScale;
    private bool wasWobbling;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        if (popOnStart)
        {
            StartPop();
        }
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

    //Startet einen Pop
    public void StartPop()
    {
        StartCoroutine(Pop());
    }

    private void OnEnable()
    {
        if (!popOnEnable)
            return;

        originalScale = transform.localScale;
        StartPop();
    }
}
