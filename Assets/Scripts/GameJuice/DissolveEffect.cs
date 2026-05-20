using System.Collections;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    [Header("References")]
    public Renderer[] renderers;

    [Header("Settings")]
    public float dissolveDuration = 2f;
    public bool destroyOnDissolve = true;

    private static readonly int dissolveAmountHash = Shader.PropertyToID("_DissolveAmount");

    private Coroutine dissolveCoroutine;
    private Coroutine resolveCoroutine;

    public void StartDissolve()
    {
        if (dissolveCoroutine != null) StopCoroutine(dissolveCoroutine);
        dissolveCoroutine = StartCoroutine(DissolveRoutine());
    }

    public void StartResolve()
    {
        if (resolveCoroutine != null) StopCoroutine(resolveCoroutine);
        dissolveCoroutine = StartCoroutine(ResolveRoutine());
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

        if (destroyOnDissolve) Destroy(gameObject);
    }

    private IEnumerator ResolveRoutine()
    {
        float timer = 0f;
        while (timer < dissolveDuration)
        {
            timer += Time.deltaTime;
            float amount = Mathf.Clamp01(1f - (timer / dissolveDuration));
            foreach (var renderer in renderers)
            {
                if (renderer == null || !renderer.gameObject.activeInHierarchy) continue;
                foreach (var mat in renderer.materials)
                    mat.SetFloat(dissolveAmountHash, amount);
            }
            yield return null;
        }
    }
}
