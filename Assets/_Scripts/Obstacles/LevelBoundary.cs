using UnityEngine;

public class LevelBoundary : MonoBehaviour
{
    public enum BoundaryType
    {
        KillZone,
        SceneTransition,
        Blocker
    }

    [Header("General")]
    [SerializeField] private BoundaryType boundaryType;

    [Header("Kill Zone")]
    [SerializeField] private Transform defaultSpawnPoint;

    [Header("Scene Transition")]
    [SerializeField] private int sceneIndex;

    [Header("Blocker")]
    [SerializeField] private float pushForce = 10f;


    private Collider boundaryCollider;


    private void Awake()
    {
        boundaryCollider = GetComponent<Collider>();
        UpdateColliderMode();
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

                if (CheckPointManager.Instance.HasSave())
                {
                    CheckPointManager.Instance.RespawnAtLastCheckpoint();
                }
                else if (defaultSpawnPoint != null)
                {
                    other.transform.position = defaultSpawnPoint.position;
                }

                PlayerMovement.Instance.SetVerticalVelocity(0f);

                break;


            case BoundaryType.SceneTransition:

                SceneTransitionManager.Instance.LoadScene(sceneIndex);

                break;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        // Nur noch als Sicherheit für alte Trigger-Blocker
        if (boundaryType != BoundaryType.Blocker)
            return;

        if (!other.CompareTag("Player"))
            return;


        CharacterController controller = other.GetComponent<CharacterController>();

        if (controller == null)
            return;


        Vector3 closestPoint = GetComponent<Collider>().ClosestPoint(other.transform.position);

        Vector3 pushDirection = (other.transform.position - closestPoint).normalized;
        pushDirection.y = 0f;


        if (pushDirection.sqrMagnitude < 0.001f)
        {
            pushDirection = (other.transform.position - transform.position).normalized;
        }


        controller.Move(pushDirection * pushForce * Time.deltaTime);
    }



    private void OnDrawGizmos()
    {
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