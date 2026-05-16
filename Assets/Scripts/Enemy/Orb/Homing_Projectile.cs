using UnityEngine;
using System.Collections;

public class Homing_Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float homingFactor = 3f;

    private Transform playerTransform;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    private void Start()
    {
        StartCoroutine(SelfDestructRoutine());
    }

    private void FixedUpdate()
    {
        if (transform.localScale != Vector3.one) return;

        if (playerTransform == null) return;

        // Richtung zum Spieler berechnen
        Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;

        // Aktuelle Richtung sanft zum Spieler hinlenken
        Vector3 newDir = Vector3.Lerp(transform.forward, dirToPlayer, homingFactor * Time.fixedDeltaTime);

        rb.linearVelocity = newDir * speed;
        transform.forward = newDir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PlayerStats.Instance.ChangeLifePoints(damage * (-1));

        Destroy(gameObject);
    }

    private IEnumerator SelfDestructRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}