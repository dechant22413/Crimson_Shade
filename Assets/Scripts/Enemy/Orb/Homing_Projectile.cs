using UnityEngine;
using System.Collections;

public class Homing_Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float homingFactor = 3f;
    [SerializeField] private float growDuration = 0.3f;

    private Transform playerTransform;
    private Rigidbody rb;
    private bool launched;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        transform.localScale = Vector3.zero;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    private void Start()
    {
        StartCoroutine(SelfDestructRoutine());
    }

    public void Launch()
    {
        StartCoroutine(GrowRoutine());
    }

    private IEnumerator GrowRoutine()
    {
        float timer = 0f;
        while (timer < growDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timer / growDuration);
            yield return null;
        }
        transform.localScale = Vector3.one;
        launched = true;
    }

    private void FixedUpdate()
    {
        if (!launched) return;
        if (playerTransform == null) return;

        Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 newDir = Vector3.Lerp(transform.forward, dirToPlayer, homingFactor * Time.fixedDeltaTime);
        rb.linearVelocity = newDir * speed;
        transform.forward = newDir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<EnemyRanged>() != null) return;

        if (other.CompareTag("Player"))
            PlayerStats.Instance.ChangeLifePoints(damage * (-1));

        Destroy(gameObject);
    }

    private IEnumerator SelfDestructRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        Debug.Log("Destroy");
        Destroy(gameObject);
    }
}