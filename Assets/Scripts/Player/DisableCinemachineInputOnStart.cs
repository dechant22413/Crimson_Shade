using UnityEngine;
using Unity.Cinemachine;

public class DisableCinemachineInputOnStart : MonoBehaviour
{
    private CinemachineInputAxisController inputController;

    void Start()
    {
        inputController = GetComponent<CinemachineInputAxisController>();
        inputController.enabled = false;
    }

    void Update()
    {
        if (!inputController.enabled)
        {
            inputController.enabled = true;
            Destroy(this);
        }
    }
}