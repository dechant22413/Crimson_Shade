using System.Collections;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    #region Settings
    [Header("References")]
    public Renderer[] renderers;

    [Header("Dissolve Settings")]
    public float dissolveDuration = 2f;
    public bool destroyOnDissolve = true;
    #endregion 

    private static readonly int dissolveAmountHash = Shader.PropertyToID("_DissolveAmount");

    private Coroutine dissolveCoroutine;
    private Coroutine resolveCoroutine;

    public void StartDissolve()
    {
        //Startet den Dissolve Effect
        if (dissolveCoroutine != null) StopCoroutine(dissolveCoroutine);
        dissolveCoroutine = StartCoroutine(DissolveRoutine());
    }

    public void StartResolve()
    {
        //Revertet den Dissolve Effect
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
                //Dissolve Effect wird auf alle angegebenen Renderer ausgeführt
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
                //Resolve Effect wird auf alle angegebenen Renderer ausgeführt
                if (renderer == null || !renderer.gameObject.activeInHierarchy) continue;
                foreach (var mat in renderer.materials)
                    mat.SetFloat(dissolveAmountHash, amount);
            }
            yield return null;
        }
    }
}
