using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire_SavePoint : MonoBehaviour, IInteractable
{
    [SerializeField] private List<ParticleSystem> fireParticleSystems;
    [SerializeField] private List<ParticleSystem> interactableParticleSystems;

    [SerializeField] private float cooldown = 5f;

    private bool bonfireActivated;
    private BonfireAudio bonFireAudio;
    private Coroutine bonfireCooldown;

    private void Start()
    {
        bonFireAudio = GetComponent<BonfireAudio>();
    }

    public string GetInteractionLabel()
    {
        return bonfireActivated ? "PROGRESS SAVED" : "SAVE PROGRESS [E]";
    }

    public void Interact()
    {
        if (bonfireActivated || bonfireCooldown != null)
            return;

        SaveSpawnPoint();
    }

    private void SaveSpawnPoint()
    {
        GameManager.Instance.SetRespawnPoint(transform);

        bonfireActivated = true;

        Debug.Log("Respawn Point Saved");

        SetParticlesActive(interactableParticleSystems, false);
        SetParticlesActive(fireParticleSystems, true);

        bonfireCooldown = StartCoroutine(CoolDownCoroutine());

        bonFireAudio.PlayActivate();
        bonFireAudio.PlayLoopSound();
    }

    private IEnumerator CoolDownCoroutine()
    {
        yield return new WaitForSeconds(cooldown);

        bonfireActivated = false;
        bonfireCooldown = null;

        SetParticlesActive(interactableParticleSystems, true);
        SetParticlesActive(fireParticleSystems, false);
        bonFireAudio.StopLoopSound();
    }

    private void SetParticlesActive(List<ParticleSystem> particles, bool active)
    {
        foreach (ParticleSystem ps in particles)
        {
            if (ps == null)
                continue;

            if (active)
                ps.Play();
            else
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
