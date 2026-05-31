using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShotgun : Weapon
{
    #region Settings
    [Header("References")]
    public RectTransform crosshairContainer;
    public Camera playerCam;
    public CinemachineImpulseSource fireImpulse;
    public Image ammoIndicatorLeft;
    public Image ammoIndicatorRight;

    [Header("Shotgun Stats")]
    [SerializeField] private int magazinCapacity = 2;
    [SerializeField] private int lifeDrain = 30;
    [SerializeField] private float attackRange = 20f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private int pelletCount = 8;
    [SerializeField] private float spreadAngle = 10f;

    [Header("Ammo UI")]
    [SerializeField] private Color ammoActiveColor = Color.white;
    [SerializeField] private Color ammoEmptyColor = new Color(1f, 1f, 1f, 0.2f);

    [Header("Crosshair Spread Settings")]
    [SerializeField] private float spreadAmount = 60f;
    [SerializeField] private float expandSpeed = 20f;
    [SerializeField] private float contractSpeed = 8f;
    #endregion

    #region Animator Hashes
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int ReloadTrigger = Animator.StringToHash("Reload");
    #endregion

    private int ammoCount;
    private float baseWidth;
    private Coroutine uiSpreadCoroutine;
    private ShotgunAudio shotgunAudio;

    private void Start()
    {
        shotgunAudio = GetComponent<ShotgunAudio>();

        ammoCount = magazinCapacity;
        baseWidth = crosshairContainer.sizeDelta.x;

        UpdateAmmoUI();
    }

    public override void OnAttackPressed()
    {
        if (isPlaying)
            return;

        if (!CanAttack())
        {
            shotgunAudio.PlayEmpty();
            return;
        }

        animator.SetTrigger(Shoot);
    }

    public override void OnReload()
    {
        if (isPlaying)
            return;

        animator.SetTrigger(ReloadTrigger);
    }

    private bool CanAttack()
    {
        return ammoCount > 0;
    }

    public void Attack()
    {
        ammoCount--;

        fireImpulse.GenerateImpulse();

        Spread();

        UpdateAmmoUI();

        armorHitThisAttack.Clear();

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = GetSpreadDirection();

            if (Physics.Raycast(
                    playerCam.transform.position,
                    direction,
                    out RaycastHit hit,
                    attackRange,
                    Combined))
            {
                ProcessHit(hit, attackDamage);
            }
        }
    }

    public void InitializeReload()
    {
        if (ammoCount == magazinCapacity)
        {
            animator.Play("Idle", 0, 0f);
            OnAnimationEnd();
            return;
        }

        int missingShells = magazinCapacity - ammoCount;

        if (PlayerStatsAndUIPanel.Instance.GetCurrentLifePoints()
            <= lifeDrain * missingShells)
        {
            animator.Play("Idle", 0, 0f);
            OnAnimationEnd();
            return;
        }
    }

    public void Reload()
    {
        int missingShells = magazinCapacity - ammoCount;

        PlayerStatsAndUIPanel.Instance.ChangeLifePoints(
            -lifeDrain * missingShells);

        ammoCount = magazinCapacity;

        UpdateAmmoUI();

        PopWobbleJuice leftPop =
            ammoIndicatorLeft.GetComponent<PopWobbleJuice>();

        if (leftPop != null)
        {
            leftPop.StartPop();

            ammoIndicatorRight
                .GetComponent<PopWobbleJuice>()
                .StartPop();
        }
    }

    private void Spread()
    {
        if (uiSpreadCoroutine != null)
            StopCoroutine(uiSpreadCoroutine);

        uiSpreadCoroutine = StartCoroutine(UISpreadRoutine());
    }

    private IEnumerator UISpreadRoutine()
    {
        float targetWidth = baseWidth + spreadAmount;

        while (Mathf.Abs(crosshairContainer.sizeDelta.x - targetWidth) > 0.5f)
        {
            crosshairContainer.sizeDelta =
                new Vector2(
                    Mathf.Lerp(
                        crosshairContainer.sizeDelta.x,
                        targetWidth,
                        Time.deltaTime * expandSpeed),
                    crosshairContainer.sizeDelta.y);

            yield return null;
        }

        while (Mathf.Abs(crosshairContainer.sizeDelta.x - baseWidth) > 0.1f)
        {
            crosshairContainer.sizeDelta =
                new Vector2(
                    Mathf.Lerp(
                        crosshairContainer.sizeDelta.x,
                        baseWidth,
                        Time.deltaTime * contractSpeed),
                    crosshairContainer.sizeDelta.y);

            yield return null;
        }

        crosshairContainer.sizeDelta =
            new Vector2(baseWidth, crosshairContainer.sizeDelta.y);
    }

    private void UpdateAmmoUI()
    {
        if (ammoIndicatorLeft != null)
            ammoIndicatorLeft.color =
                ammoCount >= 2
                    ? ammoActiveColor
                    : ammoEmptyColor;

        if (ammoIndicatorRight != null)
            ammoIndicatorRight.color =
                ammoCount >= 1
                    ? ammoActiveColor
                    : ammoEmptyColor;
    }

    private Vector3 GetSpreadDirection()
    {
        Vector3 forward = playerCam.transform.forward;

        Vector2 randomCircle =
            Random.insideUnitCircle *
            Mathf.Tan(spreadAngle * Mathf.Deg2Rad);

        Vector3 spread =
            playerCam.transform.right * randomCircle.x +
            playerCam.transform.up * randomCircle.y;

        return (forward + spread).normalized;
    }

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (playerCam == null) return;

        Vector3 origin = playerCam.transform.position;
        Vector3 forward = playerCam.transform.forward;
        Vector3 right = playerCam.transform.right;
        Vector3 up = playerCam.transform.up;

        float radius =
            Mathf.Tan(spreadAngle * Mathf.Deg2Rad) * attackRange;

        Gizmos.color = Color.red;

        int segments = 32;

        for (int i = 0; i < segments; i++)
        {
            float angle1 =
                (i / (float)segments) * Mathf.PI * 2f;

            float angle2 =
                ((i + 1) / (float)segments) * Mathf.PI * 2f;

            Vector3 p1 =
                origin +
                forward * attackRange +
                (right * Mathf.Cos(angle1)
                + up * Mathf.Sin(angle1)) * radius;

            Vector3 p2 =
                origin +
                forward * attackRange +
                (right * Mathf.Cos(angle2)
                + up * Mathf.Sin(angle2)) * radius;

            Gizmos.DrawLine(p1, p2);
        }

        Gizmos.DrawLine(origin, origin + forward * attackRange + right * radius);
        Gizmos.DrawLine(origin, origin + forward * attackRange - right * radius);
        Gizmos.DrawLine(origin, origin + forward * attackRange + up * radius);
        Gizmos.DrawLine(origin, origin + forward * attackRange - up * radius);
    }
    #endregion
}