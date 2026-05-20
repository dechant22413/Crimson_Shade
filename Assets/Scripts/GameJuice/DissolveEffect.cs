using System.Collections;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    [Header("References")]
    public Renderer[] renderers;

    [Header("Settings")]
    public float dissolveDuration = 2f;

    private static readonly int dissolveAmountHash = Shader.PropertyToID("_DissolveAmount");

    private Coroutine dissolveCoroutine;

    public void StartDissolve()
    {
        if (dissolveCoroutine != null) StopCoroutine(dissolveCoroutine);
        dissolveCoroutine = StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        float timer = 0f;

        while (timer < dissolveDuration)
        {
            timer += Time.deltaTime;
            float amount = Mathf.Clamp01(timer / dissolveDuration);

            foreach (var renderer in renderers)
            {
                if (renderer == null || !renderer.gameObject.activeInHierarchy) continue;
                foreach (var mat in renderer.materials)
                    mat.SetFloat(dissolveAmountHash, amount);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
