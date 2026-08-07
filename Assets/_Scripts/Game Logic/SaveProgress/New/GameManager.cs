using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

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
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += TogglePause;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= TogglePause;
            pauseAction.action.Disable();
        }
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

        if (player != null)
        {
            respawnPosition = player.transform.position;
        }
    }

    public void SetRespawnPoint(Transform respawn)
    {
        if (respawn == null)
            return;

        respawnPosition = respawn.position;
    }

    public void RespawnPlayer()
    {
        Time.timeScale = 1f;

        if (player == null)
            return;

        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        player.transform.position = respawnPosition;

        if (cc != null)
            cc.enabled = true;

        initialLife = PlayerStatsAndUIPanel.Instance.GetMaxLifePoints();

        PlayerStatsAndUIPanel.Instance.ChangeLifePoints(initialLife);

        gameOver = false;
        isPaused = false;

        HideCursor();
    }

    public void PlayerLifePoints(int lifepoints)
    {
        playerLifePoints = lifepoints;

        if (playerLifePoints == 0)
        {
            gameOver = true;
            Time.timeScale = 0f;

            UIManager.Instance.ActivateGameOverMenu(true);

            // Cursor für das Game-Over-Menü anzeigen
            ShowCursor();
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

        // Cursor für das Pause-Menü anzeigen
        ShowCursor();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        audioMixer.FindSnapshot(gameplaySnapshotName)
            .TransitionTo(snapshotTransitionTime);

        UIManager.Instance.ActivatePauseGameMenu(false);
        UIManager.Instance.ActivateGameOverMenu(false);

        // Cursor im Gameplay wieder verstecken
        HideCursor();

        // Einen Moment warten, damit der Input-Buffer geleert wird
        StartCoroutine(ReenableInputNextFrame());
    }

    public void EndGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // Player Input während EndGame deaktivieren
        PlayerActions.Instance.enabled = false;
        PlayerMovement.Instance.enabled = false;

        audioMixer.FindSnapshot(pausedSnapshotName)
            .TransitionTo(snapshotTransitionTime);

        UIManager.Instance.ActivateDemoFinishedMenu(true);

        // Cursor für das EndGame-Menü anzeigen
        ShowCursor();
    }

    private IEnumerator ReenableInputNextFrame()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        PlayerActions.Instance.enabled = true;
        PlayerMovement.Instance.enabled = true;
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

