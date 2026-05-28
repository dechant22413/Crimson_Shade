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
    public float attackDamage = 20f;
    public float firstAttackDelay = 0.8f;

    [Header("Detection")]
    public float sightRange = 10f;
    public float fieldOfViewAngle = 90f;
    public float guaranteedDetectRange = 2f;
    public float attackRange = 5f;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Patrol")]
    public float walkPointRange = 10f;
    public float waitAtPointDuration = 1.5f;
    public float chaseDelay = 0.5f;
    public LayerMask groundLayer;

    [Header("Start State")]
    public EnemyState startState = EnemyState.Idle;
    [SerializeField] protected EnemyState currentState;

    protected bool isStunnedFlag;
    public bool alreadyAttacked;
    protected bool firstAttackDone;
    protected bool chaseDelayActive;

    private EnemyState stateBeforeStun;
    private float navUpdateTimer;
    private const float NavUpdateInterval = 0.15f;

    private Vector3 walkPointA;
    private Vector3 walkPointB;
    private Vector3 currentWalkPoint;
    private bool walkPointsSet;
    private bool goingToB;
    private float waitTimer;
    private bool isWaiting;
    private bool firstChase = true;

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
        currentState = startState;
        OnStateChanged(startState);
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
            case EnemyState.Patrol: PatrolUpdate(); break;
            case EnemyState.Chase: ChaseUpdate(); break;
            case EnemyState.Attack: Attack(); break;
            case EnemyState.Stunned: Stunned(); break;
            case EnemyState.Dead: Dead(); break;
        }
    }

    protected virtual void UpdateState()
    {
        if (currentState == EnemyState.Dead ||
            currentState == EnemyState.Stunned)
            return;

        float distToPlayer =
            Vector3.Distance(transform.position, player.position);

        bool inAttack =
            distToPlayer <= attackRange;

        bool inGuaranteedRange =
            distToPlayer <= guaranteedDetectRange;

        Vector3 dir =
            (player.position - transform.position).normalized;

        float angle =
            Vector3.Angle(transform.forward, dir);

        bool inSight =
            distToPlayer <= sightRange &&
            angle <= fieldOfViewAngle * 0.5f;


        // Inactive ? Idle
        if (currentState == EnemyState.Inactive)
        {
            if (inGuaranteedRange)
                SetState(EnemyState.Idle);

            return;
        }


        // Spieler auﬂer Reichweite  Attack abbrechen
        if (!inAttack &&
            currentState == EnemyState.Attack)
        {
            CancelInvoke(nameof(EnableFirstAttack));

            firstAttackDone = false;
            alreadyAttacked = false;

            OnPlayerOutOfAttackRange();

            SetState(
                inSight || inGuaranteedRange
                ? EnemyState.Chase
                : EnemyState.Patrol
            );

            return;
        }


        // Attack
        if (inAttack)
        {
            SetState(EnemyState.Attack);
            return;
        }


        // Chase
        if (inSight || inGuaranteedRange)
        {
            firstChase =
                currentState != EnemyState.Chase;

            SetState(EnemyState.Chase);
            return;
        }


        // Patrol
        if (currentState ==
                EnemyState.Attack ||
            currentState ==
                EnemyState.Chase)
        {
            firstChase = true;

            SetState(
                EnemyState.Patrol
            );
        }

        if (alreadyAttacked)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > attackRange && !IsAnimationPlaying())
            {
                alreadyAttacked = false;
                OnPlayerOutOfAttackRange();
            }
            return;
        }
    }

    protected virtual bool IsAnimationPlaying() => false;

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
                agent.speed = chaseSpeed;

                if (agent.isOnNavMesh)
                    agent.SetDestination(transform.position);

                if (firstChase)
                    StartCoroutine(ChaseDelayRoutine());

                break;

            case EnemyState.Attack:
                agent.speed = chaseSpeed;
                break;

            case EnemyState.Dead:
                agent.speed = 0f;
                break;
        }


        switch (newState)
        {
            case EnemyState.Attack:

                if (!firstAttackDone)
                {
                    firstAttackDone = true;

                    if (firstAttackDelay > 0)
                    {
                        alreadyAttacked = true;
                        Invoke(nameof(EnableFirstAttack), firstAttackDelay);
                    }
                }

                break;


            case EnemyState.Chase:

                if (agent.isOnNavMesh)
                    agent.SetDestination(transform.position);

                StartCoroutine(ChaseDelayRoutine());

                break;


            case EnemyState.Stunned:

                CancelInvoke(nameof(EnableFirstAttack));

                alreadyAttacked = true;
                firstAttackDone = false;

                break;
        }
    }

    // Wird aufgerufen wenn Spieler auﬂer Attack Range geht w‰hrend alreadyAttacked true ist
    protected virtual void OnPlayerOutOfAttackRange() { }

    public virtual void OnAttackHit() { }
    public virtual void ResetAttack() { }

    private void PatrolUpdate()
    {
        if (!walkPointsSet) SearchWalkPoints();
        if (!walkPointsSet) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f) isWaiting = false;
            return;
        }

        if (CanUpdateNav())
        {
            currentWalkPoint = goingToB ? walkPointB : walkPointA;
            agent.SetDestination(currentWalkPoint);

            if ((transform.position - currentWalkPoint).magnitude < 1f)
            {
                goingToB = !goingToB;
                isWaiting = true;
                waitTimer = waitAtPointDuration;
                if (!goingToB) walkPointsSet = false;
            }
        }
    }

    private void ChaseUpdate()
    {
        if (chaseDelayActive) return;
        if (CanUpdateNav()) agent.SetDestination(player.position);
    }

    private void SearchWalkPoints()
    {
        Vector3 pointA = GetValidWalkPoint();
        Vector3 pointB = GetValidWalkPoint();

        if (pointA != Vector3.zero && pointB != Vector3.zero)
        {
            walkPointA = pointA;
            walkPointB = pointB;
            walkPointsSet = true;
        }
        else
        {
            Debug.LogWarning("Keine g¸ltigen Walkpoints gefunden ñ groundLayer korrekt gesetzt?");
        }
    }

    private Vector3 GetValidWalkPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            float randomX = Random.Range(-walkPointRange, walkPointRange);
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            Vector3 candidate = new Vector3(transform.position.x + randomX, transform.position.y + 10f, transform.position.z + randomZ);

            if (Physics.Raycast(candidate, Vector3.down, out RaycastHit rayHit, 20f, groundLayer))
                if (NavMesh.SamplePosition(rayHit.point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                    return navHit.position;
        }
        return Vector3.zero;
    }

    private System.Collections.IEnumerator ChaseDelayRoutine()
    {
        if (!firstChase) yield break;

        chaseDelayActive = true;
        float timer = chaseDelay;
        while (timer > 0f)
        {
            if (currentState != EnemyState.Chase)
            {
                chaseDelayActive = false;
                yield break;
            }
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
            timer -= Time.deltaTime;
            yield return null;
        }
        chaseDelayActive = false;
    }

    protected void EnableFirstAttack()
    {
        float dist = Vector3.Distance(
            transform.position,
            player.position
        );

        if (dist <= attackRange &&
            currentState == EnemyState.Attack)
        {
            alreadyAttacked = false;
        }
        else
        {
            firstAttackDone = false;
            alreadyAttacked = false;
        }
    }

    protected abstract void Inactive();
    protected abstract void Idle();
    protected abstract void Patrol();
    protected abstract void Chase();
    protected abstract void Attack();
    protected abstract void Stunned();
    protected abstract void Dead();

    public virtual void SpawnProjectile() { }

    public virtual void TakeDamage(float damage)
    {
        if (currentState == EnemyState.Dead) return;
        health -= damage;
        if (health <= 0f) Die();

        if (UIManager.Instance != null)
            UIManager.Instance.ShowHitIndicator();
    }

    public virtual void ArmorHit(bool stun) { }

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

        alreadyAttacked = false;

        SetState(stateBeforeStun);
    }

    protected virtual void Die()
    {
        SetState(EnemyState.Dead);
        if (agent.isOnNavMesh)
            agent.SetDestination(transform.position);
        agent.enabled = false;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowKillIndicator();
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