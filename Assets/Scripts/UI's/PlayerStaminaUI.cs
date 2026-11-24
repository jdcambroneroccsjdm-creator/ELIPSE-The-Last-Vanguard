
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaUI : MonoBehaviour
{
    [Header("References")]
    public PlayerStamina playerStamina;
    public Slider staminaSlider;

    [System.Obsolete]
    private void Start()
    {
        if (playerStamina == null)  
            playerStamina = FindObjectOfType<PlayerStamina>();

        if (staminaSlider == null)
            staminaSlider = GetComponent<Slider>();

        if (playerStamina != null && staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = playerStamina.maxStamina;

            UpdateStaminaBar();
        }
    }

    private void Update()
    {
        UpdateStaminaBar();
    }

    private void UpdateStaminaBar()
    {
        if (playerStamina == null || staminaSlider == null)
            return;

        staminaSlider.value = playerStamina.currentStamina;
    }
}
