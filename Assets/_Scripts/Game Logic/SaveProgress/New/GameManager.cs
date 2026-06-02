using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Transform respawnPoint;
    private GameObject player;

    private int playerLifePoints;

    private bool gameOver;

    private Vector3 respawnPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        player = GameObject.FindWithTag("Player");
        respawnPosition = player.transform.position;
    }

    public void SetRespawnPoint(Transform respawn)
    {
        respawnPosition = respawn.position;
    }

    public void RespawnPlayer()
    {
        Debug.Log("RespawnPlayer");

        player.transform.position = respawnPoint.position;
    }

    public void PlayerLifePoints(int lifepoints)
    {
        playerLifePoints = lifepoints;

        if(playerLifePoints == 0)
        {
            gameOver = true;

            PauseGame();

            UIManager.Instance.ActivateGameOverPanel(true);
        }

    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Debug.Log("ResumeGame");
        Time.timeScale = 1f;
    }
}
