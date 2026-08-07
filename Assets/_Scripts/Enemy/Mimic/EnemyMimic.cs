using UnityEngine;
using System.Collections;

public class EnemyMimic : EnemyMelee, IInteractable
{
    public enum MimicState { Dormant, Defence }

    [Header("Mimic Settings")]
    public float defenceRadius = 6f;
    public Vector2 defenceDelay = new Vector2(2f, 4f);
    public float defenceExitTime = 5f;
    public float returnToInactiveTime = 10f;

    [Header("Dormant Transform")]
    private Vector3 dormantPosition;
    private Quaternion dormantRotation;

    [Header("Item Drop")]
    [SerializeField] private GameObject item;

    private MimicState mimicState = MimicState.Dormant;
    private bool isDormant = true;
    private bool isInDefence = false;
    private Coroutine defenceCoroutine;
    private Coroutine returnToInactiveCoroutine;
    private MimicAudio mimicAudio;

    private static readonly int dormantHash = Animator.StringToHash("Dormant");
    private static readonly int defenceHash = Animator.StringToHash("Defence");

    #region Animator Variables (Mimic specific)
    #endregion

    protected override void Start()
    {
        base.Start();
        dormantPosition = transform.position;
        dormantRotation = transform.rotation;
        animator.SetBool(dormantHash, true);
        mimicAudio = GetComponent<MimicAudio>();
    }

    protected override void Update()
    {
        if (isDormant) return;
        base.Update();

        // Defence Zone Check
        if (!isInDefence && currentState != EnemyState.Dead && currentState != EnemyState.Stunned)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= defenceRadius && currentState != EnemyState.Attack)
            {
                if (defenceCoroutine == null)
                    defenceCoroutine = StartCoroutine(DefenceDelayRoutine());
            }
            else
            {
                if (defenceCoroutine != null)
                {
                    StopCoroutine(defenceCoroutine);
                    defenceCoroutine = null;
                }
            }
        }
    }

    // IInteractable
    public string GetInteractionLabel() => isDormant ? "OPEN [E]" : "";

    public void Interact()
    {
        if (!isDormant) return;
        Activate();
    }

    private void Activate()
    {
        isDormant = false;

        animator.SetBool(dormantHash, false);
        SetState(EnemyState.Idle);

        BossBarManager.Instance.InitializeBoss(health);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        BossBarManager.Instance.UpdateBossHealth(health);

        if (health > 0f)
        {
            mimicAudio.PlayTakeDamage();
        }
    }

    private IEnumerator DefenceDelayRoutine()
    {
        float delay = Random.Range(defenceDelay.x, defenceDelay.y);
        yield return new WaitForSeconds(delay);

        if (Vector3.Distance(transform.position, player.position) <= defenceRadius)
            EnterDefence();

        defenceCoroutine = null;
    }

    private void EnterDefence()
    {
        isInDefence = true;
        animator.SetBool(defenceHash, true);
        StartCoroutine(DefenceRoutine());
    }

    private IEnumerator DefenceRoutine()
    {
        float timer = 0f;

        while (isInDefence)
        {
            // Drehen in Spielerrichtung
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

            // Exit wenn in Attack Range oder Zeit abgelaufen
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= attackRange || timer >= defenceExitTime)
            {
                ExitDefence();
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void ExitDefence()
    {
        isInDefence = false;
        animator.SetBool(defenceHash, false);
    }

    protected override void OnStateChanged(EnemyState newState)
    {
        base.OnStateChanged(newState);

        // Bossbar Visibility
        if (newState == EnemyState.Chase ||
            newState == EnemyState.Attack)
        {
            BossBarManager.Instance.ShowBossBar(health);
        }
        else if (newState == EnemyState.Patrol ||
                 newState == EnemyState.Idle ||
                 newState == EnemyState.Inactive ||
                 newState == EnemyState.Dead)
        {
            BossBarManager.Instance.HideBossBar();
        }

        // Bei Attack Defence aufheben
        if (newState == EnemyState.Attack && isInDefence)
            ExitDefence();

        // Bei Patrol ReturnToInactive starten
        if (newState == EnemyState.Patrol)
        {
            if (returnToInactiveCoroutine != null)
                StopCoroutine(returnToInactiveCoroutine);

            returnToInactiveCoroutine = StartCoroutine(ReturnToInactiveRoutine());
        }
        else
        {
            if (returnToInactiveCoroutine != null)
            {
                StopCoroutine(returnToInactiveCoroutine);
                returnToInactiveCoroutine = null;
            }
        }
    }

    private IEnumerator ReturnToInactiveRoutine()
    {
        yield return new WaitForSeconds(returnToInactiveTime);

        // Zu dormant Position navigieren
        agent.SetDestination(dormantPosition);

        // Warten bis angekommen
        while (Vector3.Distance(transform.position, dormantPosition) > 0.3f)
            yield return null;

        // Rotation anpassen
        float rotTimer = 0f;
        while (rotTimer < 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, dormantRotation, rotTimer);
            rotTimer += Time.deltaTime * 2f;
            yield return null;
        }

        transform.rotation = dormantRotation;
        SetState(EnemyState.Inactive);
        returnToInactiveCoroutine = null;
    }

    public override void ArmorHit(bool stun)
    {

    }

    protected override void Die()
    {
        base.Die();
        DissolveEffect dissolve = GetComponent<DissolveEffect>();
        if (dissolve != null)
            dissolve.StartDissolve();

        mimicAudio.PlayDeath();
        Debug.Log("Item Drop");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, defenceRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, guaranteedDetectRange);

        Vector3 leftBound =
            Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0)
            * transform.forward * sightRange;

        Vector3 rightBound =
            Quaternion.Euler(0, fieldOfViewAngle * 0.5f, 0)
            * transform.forward * sightRange;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBound);
        Gizmos.DrawLine(transform.position, transform.position + rightBound);
    }
}