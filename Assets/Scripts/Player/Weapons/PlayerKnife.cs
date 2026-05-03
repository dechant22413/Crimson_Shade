using UnityEngine;

public class PlayerKnife : MonoBehaviour
{
    [Header("References")]
    public LayerMask attackLayer;
    public Camera playerCam;

    [Header("Knife Stats")]
    public float attackRange = 3f;
    public float attackDamage;

    public void Attack()
    {
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, attackRange, attackLayer))
        {
            HitTarget(hit.point);
        }
        ;
    }

    private void HitTarget(Vector3 pos)
    {
        Debug.Log("Hit!");
    }

    public void OnAnimationStart()
    {
        PlayerAnimations.Instance.IsLeftArmPlaying = true;
    }

    public void OnAnimationEnd()
    {
        PlayerAnimations.Instance.IsLeftArmPlaying = false;
    }
}
