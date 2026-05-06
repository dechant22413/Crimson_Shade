using System.Collections;
using Unity.Cinemachine;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerShotgun : MonoBehaviour
{
    [Header("References")]
    public LayerMask attackLayer;
    public Camera playerCam;
    public CinemachineImpulseSource fireImpulse;
    public Animator shotgunAnimations;

    [Header("Shotgun Stats")]
    public int magazinCapacity = 2;
    public int lifeDrain = 30;
    public float attackRange = 20f;
    public float attackDamage = 10f;
    public int pelletCount = 8;
    public float spreadAngle = 10f;

    private int ammoCount;

    private void Start()
    {
        ammoCount = magazinCapacity;
    }

    public void Attack()
    {
        if (ammoCount == 0)
        {
            shotgunAnimations.Play("Idle", 0, 0f);
            PlayerAnimations.Instance.IsRightArmPlaying = false;
            Debug.Log("Magazin Empty");
            return;
        }

        ammoCount--;
        fireImpulse.GenerateImpulse();

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = GetSpreadDirection();
            if (Physics.Raycast(playerCam.transform.position, direction, out RaycastHit hit, attackRange, attackLayer))
            {
                HitTarget(hit.point, hit.collider);
            }
        }
    }

    public void InitializeReload()
    {
        if (ammoCount == magazinCapacity)
        {
            shotgunAnimations.Play("Idle", 0, 0f);
            PlayerAnimations.Instance.IsRightArmPlaying = false;
            Debug.Log("Magazin already full");
            return;
        }

        if (PlayerStats.Instance.GetLifePoints() <= lifeDrain)
        {
            shotgunAnimations.Play("Idle", 0, 0f);
            PlayerAnimations.Instance.IsRightArmPlaying = false;
            Debug.Log("Not Enough LifePoints to reload");
            return;
        }
    }

    public void Reload()
    {
        PlayerStats.Instance.ChangeLifePoints(lifeDrain * (-1) * (magazinCapacity - ammoCount));

        ammoCount = magazinCapacity;
    }

    private void HitTarget(Vector3 pos, Collider col)
    {
        Debug.Log($"Pellet hit {col.name} for {attackDamage} damage");
    }

    private Vector3 GetSpreadDirection()
    {
        Vector3 forward = playerCam.transform.forward;
        Vector2 randomCircle = Random.insideUnitCircle * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
        Vector3 spread = playerCam.transform.right * randomCircle.x
                       + playerCam.transform.up * randomCircle.y;
        return (forward + spread).normalized;
    }



    public void OnAnimationStart()
    {
        PlayerAnimations.Instance.IsRightArmPlaying = true;
    }

    public void OnAnimationEnd()
    {
        PlayerAnimations.Instance.IsRightArmPlaying = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCam == null) return;

        Vector3 origin = playerCam.transform.position;
        Vector3 forward = playerCam.transform.forward;
        Vector3 right = playerCam.transform.right;
        Vector3 up = playerCam.transform.up;

        float radius = Mathf.Tan(spreadAngle * Mathf.Deg2Rad) * attackRange;

        Gizmos.color = Color.red;
        int segments = 32;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = (i / (float)segments) * Mathf.PI * 2f;
            float angle2 = ((i + 1) / (float)segments) * Mathf.PI * 2f;

            Vector3 p1 = origin + forward * attackRange + (right * Mathf.Cos(angle1) + up * Mathf.Sin(angle1)) * radius;
            Vector3 p2 = origin + forward * attackRange + (right * Mathf.Cos(angle2) + up * Mathf.Sin(angle2)) * radius;

            Gizmos.DrawLine(p1, p2);
        }

        Gizmos.DrawLine(origin, origin + forward * attackRange + right * radius);
        Gizmos.DrawLine(origin, origin + forward * attackRange - right * radius);
        Gizmos.DrawLine(origin, origin + forward * attackRange + up * radius);
        Gizmos.DrawLine(origin, origin + forward * attackRange - up * radius);
    }
}
