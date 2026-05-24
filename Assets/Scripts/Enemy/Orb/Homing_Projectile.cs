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

    private bool launched;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
            playerTransform = player.transform;

        //Scale wird auf 0 gesetzt
        transform.localScale = Vector3.zero;


    }

    private void Start()
    {
        //Startet direkt nach Instanziierung die Selbstzerstörung mit Timer
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
            //Projektil Scale wächst
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timer / growDuration);
            yield return null;
        }
        transform.localScale = Vector3.one;

        //Projektil abschussbereit
        launched = true;
    }

    private void FixedUpdate()
    {
        if (!launched) return;
        if (playerTransform == null) return;

        //Projektil bewegt sich nach vorne mit einem leichten Homing Effect zum Spieler hin
        Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 newDir = Vector3.Lerp(transform.forward, dirToPlayer, homingFactor * Time.fixedDeltaTime);
        rb.linearVelocity = newDir * speed;
        transform.forward = newDir;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Projektil berücksichtigt nicht den Ranged Enemy selbst
        if (other.GetComponentInParent<EnemyRanged>() != null) return;

        //Projektil damaged bei Auftreffen den Spieler
        if (other.CompareTag("Player"))
            PlayerStatsAndUIPanel.Instance.ChangeLifePoints(damage * (-1));

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