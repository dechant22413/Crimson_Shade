using UnityEngine;
using UnityEngine.UI;

public class BossBarManager : MonoBehaviour
{
    public static BossBarManager Instance;

    [Header("References")]
    [SerializeField] private GameObject bossBarPanel;
    [SerializeField] private Slider bossHealthSlider;

    [Header("Smooth")]
    [SerializeField] private float healthSmooth = 5f;

    private float maxHealth;
    private float healthTarget;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        HideBossBar();
    }

    private void Update()
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = Mathf.Lerp(
                bossHealthSlider.value,
                healthTarget,
                Time.deltaTime * healthSmooth
            );
        }
    }

    public void InitializeBoss(float bossMaxHealth)
    {
        maxHealth = bossMaxHealth;

        bossHealthSlider.minValue = 0f;
        bossHealthSlider.maxValue = maxHealth;
    }

    public void ShowBossBar(float currentHealth)
    {
        bossBarPanel.SetActive(true);

        healthTarget = currentHealth;

        // Beim Anzeigen direkt auf den aktuellen Wert setzen
        bossHealthSlider.value = currentHealth;
    }

    public void UpdateBossHealth(float currentHealth)
    {
        healthTarget = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void HideBossBar()
    {
        bossBarPanel.SetActive(false);
    }
}