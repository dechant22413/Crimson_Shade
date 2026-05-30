using UnityEngine;
using System.Collections;

public class SyncParticles : MonoBehaviour
{
    public Transform particleParent;
    public int loopCount = 5;

    private ParticleSystem[] particleSystems;

    private IEnumerator Start()
    {
        if (particleParent == null)
        {
            Debug.LogError("No parent assigned!");
            yield break;
        }

        // Get all child particle systems
        particleSystems = particleParent.GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < loopCount; i++)
        {
            // Reset and play all systems together
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Simulate(0, true, true);
                ps.Play(true);
            }

            // Wait until ALL particle systems are done
            bool stillPlaying = true;

            while (stillPlaying)
            {
                stillPlaying = false;

                foreach (ParticleSystem ps in particleSystems)
                {
                    if (ps.IsAlive(true))
                    {
                        stillPlaying = true;
                        break;
                    }
                }

                yield return null;
            }
        }

        Debug.Log("Finished all loops.");
    }
}