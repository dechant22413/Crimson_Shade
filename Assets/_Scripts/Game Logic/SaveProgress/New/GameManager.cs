using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

    private Transform respawnPoint;
    private GameObject player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        player = GameObject.FindWithTag("Player");
    }

    public void SetRespawnPoint(Transform respawn)
    {
        respawnPoint = respawn;
    }

    public void RespawnPlayer()
    {
        player.transform.localPosition = respawnPoint.position;
    }
    #endregion
}
