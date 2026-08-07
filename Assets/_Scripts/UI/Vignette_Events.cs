using UnityEngine;

public class Vignette_Events : MonoBehaviour
{

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        animator.SetTrigger("Show");
    }

    private void DisableVignette()
    {
        gameObject.SetActive(false);
    }
}
