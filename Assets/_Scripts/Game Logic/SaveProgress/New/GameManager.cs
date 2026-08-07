using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Actions")]
    public InputActionReference pauseAction;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string gameplaySnapshotName = "Gameplay";
    [SerializeField] private string pausedSnapshotName = "Paused";
    [SerializeField] private float snapshotTransitionTime = 0.1f;

    public static GameManager Instance;

    private Transform respawnPoint;
    private GameObject player;
    private int initialLife;
    private int playerLifePoints;
    private bool gameOver;
    private bool isPaused;
    private Vector3 respawnPosition;

    private void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.performed += TogglePause;
    }

    private void OnDisable()
    {
        pauseAction.action.Disable();
        pauseAction.action.performed -= TogglePause;
    }

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
        Time.timeScale = 1f;
        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.transform.position = respawnPosition;
        cc.enabled = true;
        initialLife = PlayerStatsAndUIPanel.Instance.GetMaxLifePoints();
        PlayerStatsAndUIPanel.Instance.ChangeLifePoints(initialLife);
    }

    public void PlayerLifePoints(int lifepoints)
    {
        playerLifePoints = lifepoints;
        if (playerLifePoints == 0)
        {
            gameOver = true;
            Time.timeScale = 0f;
            UIManager.Instance.ActivateGameOverMenu(true);
        }
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        // Kein Pausieren wenn GameOver aktiv ist
        if (gameOver)
            return;

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // Player Input während Pause deaktivieren
        PlayerActions.Instance.enabled = false;
        PlayerMovement.Instance.enabled = false;

        audioMixer.FindSnapshot(pausedSnapshotName)
                  .TransitionTo(snapshotTransitionTime);

        UIManager.Instance.ActivatePauseGameMenu(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        audioMixer.FindSnapshot(gameplaySnapshotName)
                  .TransitionTo(snapshotTransitionTime);

        UIManager.Instance.ActivatePauseGameMenu(false);
        UIManager.Instance.ActivateGameOverMenu(false);

        // Einen Frame warten damit der Input-Buffer geleert wird
        StartCoroutine(ReenableInputNextFrame());
    }

    public void EndGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // Player Input während Pause deaktivieren
        PlayerActions.Instance.enabled = false;
        PlayerMovement.Instance.enabled = false;

        audioMixer.FindSnapshot(pausedSnapshotName)
                  .TransitionTo(snapshotTransitionTime);

        UIManager.Instance.ActivateDemoFinishedMenu(true);
    }

    private IEnumerator ReenableInputNextFrame()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        PlayerActions.Instance.enabled = true;
        PlayerMovement.Instance.enabled = true;
    }
}