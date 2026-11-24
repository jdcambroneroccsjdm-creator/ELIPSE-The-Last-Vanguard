
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;

    [Header("Regeneration")]
    public float staminaRegenRate = 20f;
    public float staminaRegenDelay = 1f;

    private float staminaRegenTimer;
    private bool regenerationPaused;

    private void Awake()
    {
        currentStamina = Mathf.Clamp(
            currentStamina,
            0f,
            maxStamina
        );
    }

    private void Update()
    {
        UpdateStamina();
    }

    private void UpdateStamina()
    {
        if (staminaRegenTimer > 0f)
        {
            staminaRegenTimer -= Time.deltaTime;
            return;
        }

        if (!regenerationPaused &&
            currentStamina < maxStamina)
        {
            currentStamina +=
                staminaRegenRate * Time.deltaTime;

            currentStamina = Mathf.Min(
                currentStamina,
                maxStamina
            );
        }
    }

    public bool TryUseStamina(float amount)
    {
        if (currentStamina < amount)
            return false;

        currentStamina -= amount;

        currentStamina = Mathf.Max(
            0f,
            currentStamina
        );

        staminaRegenTimer = staminaRegenDelay;

        return true;
    }

    public void RestoreFullStamina()
    {
        currentStamina = maxStamina;
        staminaRegenTimer = 0f;
    }

    public void SetRegenerationPaused(bool paused)
    {
        regenerationPaused = paused;
    }
}
