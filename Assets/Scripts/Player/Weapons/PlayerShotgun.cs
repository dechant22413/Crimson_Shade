using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerShotgun : MonoBehaviour
{
    [Header("References")]
    public LayerMask enemyBodyHit;
    public LayerMask enemyHeadHit;
    public LayerMask environmentHit;
    public LayerMask enemyArmorHit;
    public RectTransform container;
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

    [Header("Ammo UI")]
    public Image ammoIndicatorLeft;
    public Image ammoIndicatorRight;
    public Color ammoActiveColor = Color.white;
    public Color ammoEmptyColor = new Color(1f, 1f, 1f, 0.2f);

    [Header("Crosshair Spread Settings")]
    public float spreadAmount = 60f;
    public float expandSpeed = 20f;
    public float contractSpeed = 8f;

    private int ammoCount;
    private float baseWidth;
    private Coroutine spreadCoroutine;
    private HashSet<EnemyBase> armorHitThisShot = new HashSet<EnemyBase>();

    private void Start()
    {
        ammoCount = magazinCapacity;
        baseWidth = container.sizeDelta.x;
    }

    public void Attack()
    {
        if (ammoCount == 0)
        {
            shotgunAnimations.Play("Idle", 0, 0f);
            PlayerAnimations.Instance.IsRightArmPlaying = false;
            return;
        }

        ammoCount--;
        fireImpulse.GenerateImpulse();
        Spread();
        UpdateAmmoUI();
        armorHitThisShot.Clear();

        LayerMask combined = enemyBodyHit | enemyHeadHit | environmentHit | enemyArmorHit;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = GetSpreadDirection();
            if (Physics.Raycast(playerCam.transform.position, direction, out RaycastHit hit, attackRange, combined))
            {
                int layer = hit.collider.gameObject.layer;
                if (((1 << layer) & enemyBodyHit) != 0)
                    EnemyBodyHit(hit.point, hit.collider);
                else if (((1 << layer) & enemyHeadHit) != 0)
                    EnemyHeadHit(hit.point, hit.collider);
                else if (((1 << layer) & enemyArmorHit) != 0)
                    EnemyArmorHit(hit.point, hit.collider);
                else if (((1 << layer) & environmentHit) != 0)
                    EnvironmentHit(hit.point);
            }
        }
    }

    private void EnemyBodyHit(Vector3 pos, Collider col)
    {
        EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
        if (enemy != null)
            enemy.TakeDamage(attackDamage);
    }

    private void EnemyHeadHit(Vector3 pos, Collider col)
    {
        EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
        if (enemy != null)
            enemy.TakeDamage(attackDamage * 2f);
    }

    private void EnemyArmorHit(Vector3 pos, Collider col)
    {
        EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
        if (enemy != null && !armorHitThisShot.Contains(enemy))
        {
            armorHitThisShot.Add(enemy);
            enemy.ArmorHit();
        }
    }

    private void EnvironmentHit(Vector3 pos)
    {
        Debug.Log("Environment Hit");
    }

    public void OnAnimationStart()
    {
        PlayerAnimations.Instance.IsRightArmPlaying = true;
    }

    public void OnAnimationEnd()
    {
        PlayerAnimations.Instance.IsRightArmPlaying = false;
    }

    public void InitializeReload()
    {
        if (ammoCount == magazinCapacity)
        {
            shotgunAnimations.Play("Idle", 0, 0f);
            OnAnimationEnd();
            return;
        }

        if (PlayerStats.Instance.GetLifePoints() <= lifeDrain * (magazinCapacity - ammoCount))
        {
            shotgunAnimations.Play("Idle", 0, 0f);
            OnAnimationEnd();
            return;
        }
    }

    public void Reload()
    {
        PlayerStats.Instance.ChangeLifePoints(lifeDrain * (-1) * (magazinCapacity - ammoCount));
        ammoCount = magazinCapacity;
        UpdateAmmoUI();

        if (ammoIndicatorLeft.GetComponent<PopWobbleJuice>() != null)
        {
            ammoIndicatorLeft.GetComponent<PopWobbleJuice>().StartPop();
            ammoIndicatorRight.GetComponent<PopWobbleJuice>().StartPop();
        }
    }

    public void Spread()
    {
        if (spreadCoroutine != null) StopCoroutine(spreadCoroutine);
        spreadCoroutine = StartCoroutine(SpreadRoutine());
    }

    private IEnumerator SpreadRoutine()
    {
        float targetWidth = baseWidth + spreadAmount;

        while (Mathf.Abs(container.sizeDelta.x - targetWidth) > 0.5f)
        {
            container.sizeDelta = new Vector2(Mathf.Lerp(container.sizeDelta.x, targetWidth, Time.deltaTime * expandSpeed), container.sizeDelta.y);
            yield return null;
        }

        while (Mathf.Abs(container.sizeDelta.x - baseWidth) > 0.1f)
        {
            container.sizeDelta = new Vector2(Mathf.Lerp(container.sizeDelta.x, baseWidth, Time.deltaTime * contractSpeed), container.sizeDelta.y);
            yield return null;
        }

        container.sizeDelta = new Vector2(baseWidth, container.sizeDelta.y);
    }

    private void UpdateAmmoUI()
    {
        if (ammoIndicatorLeft != null)
            ammoIndicatorLeft.color = ammoCount >= 2 ? ammoActiveColor : ammoEmptyColor;
        if (ammoIndicatorRight != null)
            ammoIndicatorRight.color = ammoCount >= 1 ? ammoActiveColor : ammoEmptyColor;
    }

    private Vector3 GetSpreadDirection()
    {
        Vector3 forward = playerCam.transform.forward;
        Vector2 randomCircle = Random.insideUnitCircle * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
        Vector3 spread = playerCam.transform.right * randomCircle.x
                       + playerCam.transform.up * randomCircle.y;
        return (forward + spread).normalized;
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