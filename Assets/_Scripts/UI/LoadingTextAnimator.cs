using TMPro;
using UnityEngine;
using System.Collections;

public class LoadingTextAnimator : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;
    [SerializeField] private string baseText = "LOADING";
    [SerializeField] private int maxDots = 3;
    [SerializeField] private float interval = 0.4f;

    private void OnEnable()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        int dots = 1;

        while (true)
        {
            textField.text = baseText + new string('.', dots);

            dots++;

            if (dots > maxDots)
                dots = 1;

            yield return new WaitForSeconds(interval);
        }
    }
}