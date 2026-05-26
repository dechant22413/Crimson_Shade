using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerShotgun : Weapon
{
    #region Settings
    [Header("References")]
    public RectTransform crosshairContainer;
    public Camera playerCam;
    public CinemachineImpulseSource fireImpulse;
    public Animator shotgunAnimations;
    public Image ammoIndicatorLeft;
    public Image ammoIndicatorRight;

    [Header("Shotgun Stats")]
    [SerializeField] private int magazinCapacity = 2;
    [SerializeField] private int lifeDrain = 30;
    [SerializeField] private float attackRange = 20f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private int pelletCount = 8;
    [SerializeField] private float spreadAngle = 10f;

    [Header("Audio Strings")]
    [SerializeField] private string gunEmpty;

    [Header("Ammo UI")]
    [SerializeField] private Color ammoActiveColor = Color.white;
    [SerializeField] private Color ammoEmptyColor = new Color(1f, 1f, 1f, 0.2f);

    [Header("Crosshair Spread Settings")]
    [SerializeField] private float spreadAmount = 60f;
    [SerializeField] private float expandSpeed = 20f;
    [SerializeField] private float contractSpeed = 8f;
    #endregion

    private int ammoCount;
    private float baseWidth;
    private Coroutine UIspreadCoroutine;

    private void Start()
    {
        ammoCount = magazinCapacity;
        baseWidth = crosshairContainer.sizeDelta.x;
    }

    public void Attack()
    {
        if (ammoCount == 0)
        {
            //Bei leerem Magazin wird Shoot Animation gecancelt
            shotgunAnimations.Play("Idle", 0, 0f);
            PlayerAnimations.Instance.IsRightArmPlaying = false;

            PlayAudio(gunEmpty);
            return;
        }
        
        ammoCount--;

        //Generiert Cinemachine ScreenShake
        fireImpulse.GenerateImpulse();

        //Skript Animation des Crosshair Containers
        Spread();
        UpdateAmmoUI();
        armorHitThisAttack.Clear();

        for (int i = 0; i < pelletCount; i++)
        {
            //erstellt für jede Shotgun Kugel einen Ray innerhalb des eingestellten Spread Cones
            Vector3 direction = GetSpreadDirection();
            if (Physics.Raycast(playerCam.transform.position, direction, out RaycastHit hit, attackRange, Combined))
                ProcessHit(hit, attackDamage);
        }
    }

    public void InitializeReload()
    {
        //Wird im ersten Frame der Reload Animation aufgerufen
        if (ammoCount == magazinCapacity)
        {
            //Canacelt Reload, wenn volles Magazin
            shotgunAnimations.Play("Idle", 0, 0f);
            OnAnimationEnd();
            return;
        }

        if (PlayerStatsAndUIPanel.Instance.GetCurrentLifePoints() <= lifeDrain * (magazinCapacity - ammoCount))
        {
            //Cancelt Reload, wenn zu wenige Leben
            shotgunAnimations.Play("Idle", 0, 0f);
            OnAnimationEnd();
            return;
        }
    }

    public void Reload()
    {
        //wird als Animation Event der Reload Animation aufgerufen
        //Abziehen des LifeDrains von den Player Leben
        PlayerStatsAndUIPanel.Instance.ChangeLifePoints(lifeDrain * (-1) * (magazinCapacity - ammoCount));
        //Resetten des ammoCounts
        ammoCount = magazinCapacity;
        //Resetten der AmmoUI
        UpdateAmmoUI();

        if (ammoIndicatorLeft.GetComponent<PopWobbleJuice>() != null)
        {
            //Pop der Ammo Indicator nach Reload
            ammoIndicatorLeft.GetComponent<PopWobbleJuice>().StartPop();
            ammoIndicatorRight.GetComponent<PopWobbleJuice>().StartPop();
        }
    }

    public void Spread()
    {
        //Startet die Spread Coroutine
        if (UIspreadCoroutine != null) StopCoroutine(UIspreadCoroutine);
        UIspreadCoroutine = StartCoroutine(UISpreadRoutine());
    }

    private IEnumerator UISpreadRoutine()
    {
        //Kurzes Spreizen des Crosshair Containers beim Schießen
        float targetWidth = baseWidth + spreadAmount;

        while (Mathf.Abs(crosshairContainer.sizeDelta.x - targetWidth) > 0.5f)
        {
            crosshairContainer.sizeDelta = new Vector2(Mathf.Lerp(crosshairContainer.sizeDelta.x, targetWidth, Time.deltaTime * expandSpeed), crosshairContainer.sizeDelta.y);
            yield return null;
        }

        while (Mathf.Abs(crosshairContainer.sizeDelta.x - baseWidth) > 0.1f)
        {
            crosshairContainer.sizeDelta = new Vector2(Mathf.Lerp(crosshairContainer.sizeDelta.x, baseWidth, Time.deltaTime * contractSpeed), crosshairContainer.sizeDelta.y);
            yield return null;
        }

        crosshairContainer.sizeDelta = new Vector2(baseWidth, crosshairContainer.sizeDelta.y);
    }

    private void UpdateAmmoUI()
    {
        //Resetten der AmmoUI Color
        if (ammoIndicatorLeft != null)
            ammoIndicatorLeft.color = ammoCount >= 2 ? ammoActiveColor : ammoEmptyColor;
        if (ammoIndicatorRight != null)
            ammoIndicatorRight.color = ammoCount >= 1 ? ammoActiveColor : ammoEmptyColor;
    }

    private Vector3 GetSpreadDirection()
    {
        //returned eine randomized Streurichtung für jeden abgeschossenen Ray der Shotgun
        Vector3 forward = playerCam.transform.forward;
        Vector2 randomCircle = Random.insideUnitCircle * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
        Vector3 spread = playerCam.transform.right * randomCircle.x
                       + playerCam.transform.up * randomCircle.y;
        return (forward + spread).normalized;
    }

    public void OnAnimationStart() => PlayerAnimations.Instance.IsRightArmPlaying = true;
    public void OnAnimationEnd() => PlayerAnimations.Instance.IsRightArmPlaying = false;
    private void PlayAudio(string audioString)
    {
        AudioManager.Instance.PlayAudio(audioString);
    }

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        //zeigt Gizmos für den StreuungsCone der Shotgun
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
    #endregion
}