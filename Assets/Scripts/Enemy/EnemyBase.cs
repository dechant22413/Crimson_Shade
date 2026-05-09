using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    public enum EnemyState { Inactive, Idle, Patrol, Chase, Attack }

    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;

    [Header("Stats")]
    public float health = 100f;
    public float sightRange = 10f;
    public float attackRange = 5f;

    [Header("Start State")]
    public EnemyState startState = EnemyState.Idle;
    [SerializeField] protected EnemyState currentState;

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

        navUpdateTimer -= Time.deltaTime;

        UpdateState();

        switch (currentState)
        {
            case EnemyState.Inactive: Inactive(); break;
            case EnemyState.Idle: Idle(); break;
            case EnemyState.Patrol: Patrol(); break;
            case EnemyState.Chase: Chase(); break;
            case EnemyState.Attack: Attack(); break;
        }
    }

    protected virtual void UpdateState()
    {
        bool inSight = Physics.CheckSphere(transform.position, sightRange, LayerMask.GetMask("Player"));
        bool inAttack = Physics.CheckSphere(transform.position, attackRange, LayerMask.GetMask("Player"));

        if (inAttack)
            SetState(EnemyState.Attack);
        else if (inSight)
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

    protected virtual void OnStateChanged(EnemyState newState) { }

    protected abstract void Inactive();
    protected abstract void Idle();
    protected abstract void Patrol();
    protected abstract void Chase();
    protected abstract void Attack();

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f) Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}