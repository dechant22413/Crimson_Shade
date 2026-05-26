using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index;
    private bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;
        CheckPointManager.Instance.ActivateCheckpoint(this, index);
        // Hier Checkpoint Animation/Effekt abspielen
    }
}
