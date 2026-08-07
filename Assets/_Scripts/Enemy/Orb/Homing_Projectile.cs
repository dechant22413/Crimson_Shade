using UnityEngine;
using System.Collections;

public class Homing_Projectile : MonoBehaviour
{
    #region Settings
    [Header("Projectile Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float homingFactor = 3f;
    [SerializeField] private float growDuration = 0.3f;
    #endregion

    private Transform playerTransform;
    private Rigidbody rb;
    private HomingProjectileAudio homingProjectileAudio;

    private bool launched;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
            playerTransform = player.transform;

        //Scale wird auf 0 gesetzt
        transform.localScale = Vector3.zero;

        homingProjectileAudio = GetComponent<HomingProjectileAudio>();
    }

    private void Start()
    {
        //Startet direkt nach Instanziierung die Selbstzerstörung mit Timer
        StartCoroutine(SelfDestructRoutine());
    }

    public void Launch()
    {
        StartCoroutine(GrowRoutine());
        homingProjectileAudio.PlayCharging();
    }

    private IEnumerator GrowRoutine()
    {
        float timer = 0f;
        while (timer < growDuration)
        {
            //Projektil Scale wächst
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timer / growDuration);
            yield return null;
        }
        transform.localScale = Vector3.one;

        //Projektil abschussbereit
        launched = true;
        homingProjectileAudio.PlayLocmotionSound();
    }

    private void FixedUpdate()
    {
        if (!launched) return;
        if (playerTransform == null) return;

        // Vollständige 3D-Richtung zum Spieler
        Vector3 directionToPlayer =
            (playerTransform.position - transform.position).normalized;

        // Aktuelle Richtung langsam in Richtung Spieler drehen
        Vector3 newDirection = Vector3.RotateTowards(
            transform.forward,
            directionToPlayer,
            homingFactor * Time.fixedDeltaTime,
            0f
        );

        // Geschwindigkeit setzen
        rb.linearVelocity = newDirection * speed;

        // Projektil in Flugrichtung drehen
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Projektil berücksichtigt nicht den Ranged Enemy selbst
        if (other.GetComponentInParent<EnemyRanged>() != null) return;

        //Projektil damaged bei Auftreffen den Spieler
        if (other.CompareTag("Player"))
            PlayerStatsAndUIPanel.Instance.DamagePlayer(damage);

        homingProjectileAudio.PLayHit();
        Destroy(gameObject);
    }

    private IEnumerator SelfDestructRoutine()
    {
        //Selbstzerstörung nach eingestellter Zeit
        yield return new WaitForSeconds(lifeTime);
        Debug.Log("Destroy");
        Destroy(gameObject);
    }
}