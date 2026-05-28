using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsAndUIPanel : MonoBehaviour
{
    #region Singleton Initialization
    //Singelton
    public static PlayerStatsAndUIPanel Instance;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    #endregion

    #region Settings
    [Header("References")]
    public Slider lifePointsSlider;
    public Slider lifePointsSliderDelayed;
    public Slider staminaSlider;
    public Image powerUpBar;
    public Image skeletonHand;

    [Header("LifePoints Settings")]
    [SerializeField] private int maxLifePoints = 90;
    [SerializeField] private float lifePointsSmooth = 5f;
    [SerializeField] private float lifePointsDelayedSmooth = 3f;
    [SerializeField] private float lifePointsDelay = 0.5f;
    [SerializeField] private int currentLifePoints;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 20;
    [SerializeField] private float recoverRate = 3f;
    [SerializeField] private float staminaRecoverDelay = 1f;
    [SerializeField] private float staminaSmooth = 5f;
    [SerializeField] private float currentStamina;

    [Header("PowerUp Settings")]
    [SerializeField] private float maxPowerUp = 5;
    [SerializeField] private int powerUpHeal;
    [SerializeField] private float powerUpSmooth = 5f;
    [SerializeField] private float currentPowerUp;

    [Header("Audio Strings and Source")]
    [SerializeField] private AudioSource playerAudioSource;

    [SerializeField] private string recoverStamina;
    [SerializeField] private string maxPowerUpString;
    #endregion

    private float lifePointsTarget;
    private float lifePointsDelayedTarget;
    private float lifePointsDelayTimer;
    private float staminaTarget;
    private float powerUpTarget;
    private float currentRecoverDelay;
    private bool powerUpFullPlayed = false;

    private void Start()
    {
        //Initialisierung aller Werte
        currentLifePoints = maxLifePoints;
        currentStamina = maxStamina;
        lifePointsTarget = maxLifePoints;
        lifePointsDelayedTarget = maxLifePoints;
        staminaTarget = maxStamina;
        powerUpTarget = 0;

        //Einstellung der Slider und Bars
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
    }

    private void Update()
    {
        //Stamina des Dash wird dauerhaft wieder aufgeladen
        if(currentStamina != maxStamina) RecoverStamina();

        if (lifePointsDelayTimer > 0f)
            lifePointsDelayTimer -= Time.deltaTime;
        else
            lifePointsDelayedTarget = lifePointsTarget;

        //Alle Slider werden bei Werteveränderungen mit einem Lerp angepasst
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
        //LifePoints werden um den gewünschten Wert verändert
        currentLifePoints = Mathf.Clamp(currentLifePoints + amount, 0, maxLifePoints);
        lifePointsTarget = currentLifePoints;
        lifePointsDelayTimer = lifePointsDelay;
    }

    public void RecoverStamina()
    {
        //Kleines Delay vor dem Wiederaufladen des Stamina Sliders
        if (currentRecoverDelay > 0)
        {
            currentRecoverDelay -= Time.deltaTime;
            return;
        }

        float previousStamina = currentStamina;

        //Wiederaufladen des Stamina Sliders
        currentStamina = Mathf.Clamp(currentStamina + recoverRate * Time.deltaTime, 0, maxStamina);
        staminaTarget = currentStamina;

        if (previousStamina < maxStamina / 2 && currentStamina >= maxStamina / 2)
        {
            PlayAudio(recoverStamina);
        }

        if (previousStamina < maxStamina && currentStamina >= maxStamina)
        {
            PlayAudio(recoverStamina);
        }
    }

    public void UseStamina(float amount)
    {
        //currentStamina wird um gewünschten Wert verändert
        currentStamina = Mathf.Clamp(currentStamina - amount, 0, maxStamina);
        staminaTarget = currentStamina;

        //Recover Delay
        currentRecoverDelay = staminaRecoverDelay;
    }

    public void ChangePowerUp(int amount)
    {
        
        //currentPowerUp wird um gewünschten Wert verändert
        if(currentPowerUp != maxPowerUp)
        {
            currentPowerUp = Mathf.Clamp(currentPowerUp + amount, 0, maxPowerUp);
            powerUpFullPlayed = false;
        }

        powerUpTarget = currentPowerUp / maxPowerUp;

        if (currentPowerUp == maxPowerUp)
        {
            //PowerUp Bar Animation bei maxPowerUp
            skeletonHand.GetComponent<PopWobbleJuice>().continuousWobble = true;

            if (!powerUpFullPlayed)
            {
                PlayAudio(maxPowerUpString);
                powerUpFullPlayed = true;
            }
        }
    }

    public void ActivatePowerUp()
    {
        //currentPowerUp wird auf 0 gesetzt und PowerUp wird aktiviert
        currentPowerUp = 0;
        powerUpTarget = currentPowerUp / maxPowerUp;

        ChangeLifePoints(powerUpHeal);

        //Stoppen der PowerUp Bar Animation
        skeletonHand.GetComponent<PopWobbleJuice>().continuousWobble = false;
    }

    private void PlayAudio(string audioString)
    {
        AudioManager.Instance.PlayAudio(audioString, playerAudioSource);
    }

    #region Weitere Zugriffsfunktionien für andere Skripte
    public float GetStamina() => currentStamina;
    public float GetMaxLifePoints() => maxLifePoints;
    public float GetPowerUp() => currentPowerUp;
    public float GetMaxPowerUp() => maxPowerUp;
    public int GetCurrentLifePoints() => currentLifePoints;
    #endregion
}