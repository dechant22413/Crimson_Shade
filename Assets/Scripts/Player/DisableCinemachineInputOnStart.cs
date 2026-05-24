using UnityEngine;
using Unity.Cinemachine;

public class DisableCinemachineInputOnStart : MonoBehaviour
{
    private CinemachineInputAxisController inputController;

    //Dieses Skript sorgt dafür, dass die Spieler Kamera bei Start des Spiels nicht plötzlich springt, sondern in der eingestellten Position bleibt
    //Dafür wir der Cinemachine Controller für einen kurzen Moment disabled
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