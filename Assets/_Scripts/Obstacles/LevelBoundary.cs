using UnityEngine;

public class LevelBoundary : MonoBehaviour
{
    public enum BoundaryType
    {
        KillZone,
        SceneTransition,
        DemoOver,
        Blocker
    }

    [Header("General")]
    [SerializeField] private BoundaryType boundaryType;

    [Header("Scene Transition")]
    [SerializeField] private int sceneIndex;

    [Header("Blocker")]
    [SerializeField] private float pushForce = 10f;

    [Header("Show Gizmos")]
    [SerializeField] private bool showBoundaryGizmos = true;

    private Collider boundaryCollider;
    private Transform defaultSpawnPoint;

    private void Awake()
    {
        boundaryCollider = GetComponent<Collider>();
        UpdateColliderMode();

        if (defaultSpawnPoint == null)
        {
            GameObject spawn = GameObject.FindGameObjectWithTag("Player");

            if (spawn != null)
                defaultSpawnPoint = spawn.transform;
        }
    }

    private void OnValidate()
    {
        if (boundaryCollider == null)
            boundaryCollider = GetComponent<Collider>();

        UpdateColliderMode();
    }

    private void UpdateColliderMode()
    {
        if (boundaryCollider == null)
            return;

        switch (boundaryType)
        {
            case BoundaryType.Blocker:
                boundaryCollider.isTrigger = false;
                break;

            case BoundaryType.KillZone:
            case BoundaryType.SceneTransition:
            case BoundaryType.DemoOver:
                boundaryCollider.isTrigger = true;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        switch (boundaryType)
        {
            case BoundaryType.KillZone:

                // Triggert den kompletten GameOver/Tod-Flow
                GameManager.Instance.PlayerLifePoints(0);

                break;

            case BoundaryType.SceneTransition:

                // Lädt die angegebene Szene
                SceneTransitionManager.Instance.LoadScene(sceneIndex);

                break;

            case BoundaryType.DemoOver:

                // Beendet die Demo
                GameManager.Instance.EndGame();

                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Nur für Blocker
        if (boundaryType != BoundaryType.Blocker)
            return;

        if (!other.CompareTag("Player"))
            return;

        CharacterController controller = other.GetComponent<CharacterController>();

        if (controller == null)
            return;

        Vector3 closestPoint =
            boundaryCollider.ClosestPoint(other.transform.position);

        Vector3 pushDirection =
            (other.transform.position - closestPoint).normalized;

        pushDirection.y = 0f;

        if (pushDirection.sqrMagnitude < 0.001f)
        {
            pushDirection =
                (other.transform.position - transform.position).normalized;

            pushDirection.y = 0f;
        }

        controller.Move(pushDirection * pushForce * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if (!showBoundaryGizmos)
            return;

        Collider col = GetComponent<Collider>();

        if (col == null)
            return;

        switch (boundaryType)
        {
            case BoundaryType.KillZone:
                Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
                break;

            case BoundaryType.SceneTransition:
                Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
                break;

            case BoundaryType.DemoOver:
                Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
                break;

            case BoundaryType.Blocker:
                Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
                break;
        }

        Gizmos.matrix = transform.localToWorldMatrix;

        if (col is BoxCollider box)
        {
            Gizmos.DrawCube(box.center, box.size);

            Gizmos.color = new Color(
                Gizmos.color.r,
                Gizmos.color.g,
                Gizmos.color.b,
                1f
            );

            Gizmos.DrawWireCube(box.center, box.size);

            Vector3 center = transform.TransformPoint(box.center);
            Gizmos.DrawRay(center, -transform.forward * 2f);
        }
        else if (col is SphereCollider sphere)
        {
            Gizmos.DrawSphere(sphere.center, sphere.radius);

            Gizmos.color = new Color(
                Gizmos.color.r,
                Gizmos.color.g,
                Gizmos.color.b,
                1f
            );

            Gizmos.DrawWireSphere(sphere.center, sphere.radius);

            Vector3 center = transform.TransformPoint(sphere.center);
            Gizmos.DrawRay(center, -transform.forward * 2f);
        }
    }
}