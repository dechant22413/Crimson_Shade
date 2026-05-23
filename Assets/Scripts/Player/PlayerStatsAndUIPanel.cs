using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsAndUIPanel : MonoBehaviour
{
    public static PlayerStatsAndUIPanel Instance;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    [Header("References")]
    public Slider lifePointsSlider;
    public Slider lifePointsSliderDelayed;
    public Slider staminaSlider;
    public Image powerUpBar;
    public Image skeletonHand;

    [Header("LifePoints Settings")]
    public float maxLifePoints = 90;
    public float lifePointsSmooth = 5f;
    public float lifePointsDelayedSmooth = 3f;
    public float lifePointsDelay = 0.5f;
    [SerializeField] private float currentLifePoints;

    [Header("Stamina Settings")]
    public float maxStamina = 20;
    public float recoverRate = 3f;
    public float staminaRecoverDelay = 1f;
    public float staminaSmooth = 5f;
    [SerializeField] private float currentStamina;

    [Header("PowerUp Settings")]
    public float maxPowerUp = 5;
    public int powerUpHeal;
    public float powerUpSmooth = 5f;
    [SerializeField] private float currentPowerUp;

    private float lifePointsTarget;
    private float lifePointsDelayedTarget;
    private float lifePointsDelayTimer;
    private float staminaTarget;
    private float powerUpTarget;

    private float currentRecoverDelay;

    private void Start()
    {
        currentLifePoints = maxLifePoints;
        currentStamina = maxStamina;

        if (lifePointsSlider != null)
        {
            lifePointsSlider.minValue = 0;
            lifePointsSlider.maxValue = maxLifePoints;
            lifePointsSlider.value = maxLifePoints; 
        }

        if(lifePointsSliderDelayed != null)
        {
            lifePointsSliderDelayed.minValue = 0;
            lifePointsSliderDelayed.maxValue = maxLifePoints;
            lifePointsSliderDelayed.value = maxLifePoints;
        }

        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0;
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = maxStamina; 
        }

        if (powerUpBar != null)
            powerUpBar.fillAmount = 0;

        lifePointsTarget = maxLifePoints;
        lifePointsDelayedTarget = maxLifePoints;
        staminaTarget = maxStamina;
        powerUpTarget = 0;
    }

    private void Update()
    {
        RecoverStamina();

        if (lifePointsDelayTimer > 0f)
            lifePointsDelayTimer -= Time.deltaTime;
        else
            lifePointsDelayedTarget = lifePointsTarget;

        if (lifePointsSlider != null)
            lifePointsSlider.value = Mathf.Lerp(lifePointsSlider.value, lifePointsTarget, Time.deltaTime * lifePointsSmooth);

        if (lifePointsSliderDelayed != null)
            lifePointsSliderDelayed.value = Mathf.Lerp(lifePointsSliderDelayed.value, lifePointsDelayedTarget, Time.deltaTime * lifePointsDelayedSmooth);

        if (staminaSlider != null)
            staminaSlider.value = Mathf.Lerp(staminaSlider.value, staminaTarget, Time.deltaTime * staminaSmooth);

        if (powerUpBar != null)
        {
            powerUpBar.fillAmount = Mathf.Lerp(powerUpBar.fillAmount, powerUpTarget, Time.deltaTime * powerUpSmooth);
        }
    }

    public void ChangeLifePoints(int amount)
    {
        currentLifePoints = Mathf.Clamp(currentLifePoints + amount, 0, maxLifePoints);
        lifePointsTarget = currentLifePoints;
        lifePointsDelayTimer = lifePointsDelay;
    }

    public void RecoverStamina()
    {
        if (currentRecoverDelay > 0)
        {
            currentRecoverDelay -= Time.deltaTime;
            return;
        }

        currentStamina = Mathf.Clamp(currentStamina + recoverRate * Time.deltaTime, 0, maxStamina);
        staminaTarget = currentStamina;
    }

    public void UseStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina - amount, 0, maxStamina);
        staminaTarget = currentStamina;

        currentRecoverDelay = staminaRecoverDelay;
    }

    public void ChangePowerUp(int amount)
    {
        currentPowerUp = Mathf.Clamp(currentPowerUp + amount, 0, maxPowerUp);
        powerUpTarget = currentPowerUp / maxPowerUp;

        if(currentPowerUp == maxPowerUp)
        {
            skeletonHand.GetComponent<PopWobbleJuice>().continuousWobble = true;
        }
    }

    public void ActivatePowerUp()
    {
        currentPowerUp = 0;
        powerUpTarget = currentPowerUp / maxPowerUp;

        ChangeLifePoints(powerUpHeal);
        skeletonHand.GetComponent<PopWobbleJuice>().continuousWobble = false;
    }

    public float GetStamina() => currentStamina;
    public float GetLifePoints() => currentLifePoints;

    public float GetPowerUp() => currentPowerUp;
}