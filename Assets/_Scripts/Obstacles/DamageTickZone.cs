using UnityEngine;
using System.Collections;

public class DamageTickZone : MonoBehaviour
{
    #region Settings
    [Header("References")]
    public BoxCollider boxCollider;

    [Header("Damage Tick Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float tickRate;
    #endregion

    private Coroutine damageTickCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(damageTickCoroutine == null)
            {
                damageTickCoroutine = StartCoroutine(DamageTick());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(damageTickCoroutine != null)
            {
                StopCoroutine(damageTickCoroutine);
                damageTickCoroutine = null;
            }
        }
    }

    private IEnumerator DamageTick()
    {
        while(true)
        {
            PlayerStatsAndUIPanel.Instance.DamagePlayer(damage);
            yield return new WaitForSeconds(tickRate);
        }
    }
}
