using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    public enum EnemyState { Inactive, Idle, Patrol, Chase, Attack, Stunned, Dead }

    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;

    [Header("Stats")]
    public float health = 100f;

    [Header("Detection")]
    public float sightRange = 10f;
    public float fieldOfViewAngle = 90f;
    public float guaranteedDetectRange = 2f;
    public float attackRange = 5f;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Start State")]
    public EnemyState startState = EnemyState.Idle;
    [SerializeField] protected EnemyState currentState;

    protected bool isStunnedFlag;
    private EnemyState stateBeforeStun;
    private float navUpdateTimer;
    private const float NavUpdateInterval = 0.15f;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning($"{gameObject.name}: Kein Player mit Tag 'Player' gefunden.");
    }

    protected virtual void Start()
    {
        SetState(startState);
    }

    protected virtual void Update()
    {
        if (player == null) return;
        if (!agent.isOnNavMesh) return;

        navUpdateTimer -= Time.deltaTime;
        UpdateState();

        switch (currentState)
        {
            case EnemyState.Inactive: Inactive(); break;
            case EnemyState.Idle: Idle(); break;
            case EnemyState.Patrol: Patrol(); break;
            case EnemyState.Chase: Chase(); break;
            case EnemyState.Attack: Attack(); break;
            case EnemyState.Stunned: Stunned(); break;
            case EnemyState.Dead: Dead(); break;
        }
    }

    protected virtual void UpdateState()
    {
        if (currentState == EnemyState.Dead || currentState == EnemyState.Stunned) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        bool inAttack = distToPlayer <= attackRange;
        bool inGuaranteedRange = distToPlayer <= guaranteedDetectRange;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        bool inFieldOfView = angle <= fieldOfViewAngle * 0.5f;
        bool inSight = distToPlayer <= sightRange && inFieldOfView;

        if (inAttack)
            SetState(EnemyState.Attack);
        else if (inSight || inGuaranteedRange)
            SetState(EnemyState.Chase);
        else if (currentState == EnemyState.Attack || currentState == EnemyState.Chase)
            SetState(EnemyState.Patrol);
    }

    protected void SetState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        OnStateChanged(newState);
    }

    protected bool CanUpdateNav()
    {
        if (navUpdateTimer <= 0f)
        {
            navUpdateTimer = NavUpdateInterval;
            return true;
        }
        return false;
    }

    protected virtual void OnStateChanged(EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Patrol:
            case EnemyState.Idle:
            case EnemyState.Inactive:
            case EnemyState.Stunned:
                agent.speed = patrolSpeed;
                break;
            case EnemyState.Chase:
            case EnemyState.Attack:
                agent.speed = chaseSpeed;
                break;
            case EnemyState.Dead:
                agent.speed = 0f;
                break;
        }
    }

    protected abstract void Inactive();
    protected abstract void Idle();
    protected abstract void Patrol();
    protected abstract void Chase();
    protected abstract void Attack();
    protected abstract void Stunned();
    protected abstract void Dead();

    public virtual void TakeDamage(float damage)
    {
        if (currentState == EnemyState.Dead) return;
        health -= damage;
        if (health <= 0f) Die();
    }

    public virtual void ArmorHit() { }

    public virtual void Stun(float duration)
    {
        if (currentState == EnemyState.Dead) return;
        if (isStunnedFlag) return;

        isStunnedFlag = true;
        stateBeforeStun = currentState;
        SetState(EnemyState.Stunned);
        CancelInvoke(nameof(RecoverFromStun));
        Invoke(nameof(RecoverFromStun), duration);
    }

    private void RecoverFromStun()
    {
        isStunnedFlag = false;
        SetState(stateBeforeStun);
    }

    protected virtual void Die()
    {
        SetState(EnemyState.Dead);
        agent.SetDestination(transform.position);
        agent.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, guaranteedDetectRange);

        Vector3 leftBound = Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0) * transform.forward * sightRange;
        Vector3 rightBound = Quaternion.Euler(0, fieldOfViewAngle * 0.5f, 0) * transform.forward * sightRange;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBound);
        Gizmos.DrawLine(transform.position, transform.position + rightBound);
    }
}