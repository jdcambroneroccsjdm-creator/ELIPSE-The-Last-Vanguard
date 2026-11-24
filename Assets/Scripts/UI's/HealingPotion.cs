
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealingPotion : MonoBehaviour
{
    [Header("Potion Settings")]
    public float healAmount = 30f;
    public float cooldownDuration = 30f;
    public KeyCode potionKey = KeyCode.H;

    [Header("References")]
    public PlayerHealth playerHealth;
    public ParticleSystem healingEffect;

    [Header("Potion UI")]
    public Image cooldownImage;
    public TMP_Text cooldownText;

    private float nextAvailableTime = 0f;
    private bool isOnCooldown = false;

    void Awake()
    {
        if (healingEffect != null)
        {
            healingEffect.Stop(true,
                ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        UpdatePotionUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(potionKey))
        {
            UsePotion();
        }

        UpdateCooldown();
    }

    void UsePotion()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("Player Health is not assigned!");
            return;
        }

        if (isOnCooldown)
        {
            Debug.Log("Potion is still on cooldown!");
            return;
        }

        if (playerHealth.currentHealth >= playerHealth.maxHealth)
        {
            Debug.Log("Health is already full!");
            return;
        }

        // Restore player health
        playerHealth.Heal(healAmount);

        // Play healing particles
        if (healingEffect != null)
        {
            healingEffect.Stop(true,
                ParticleSystemStopBehavior.StopEmittingAndClear);

            healingEffect.Play();
        }

        // Start cooldown
        isOnCooldown = true;
        nextAvailableTime = Time.time + cooldownDuration;

        UpdatePotionUI();

        Debug.Log("Potion used!");
    }

    void UpdateCooldown()
    {
        if (!isOnCooldown)
            return;

        float remainingTime = Mathf.Max(
            0f, nextAvailableTime - Time.time);

        if (cooldownImage != null)
        {
            cooldownImage.fillAmount =
                remainingTime / cooldownDuration;
        }

        if (cooldownText != null)
        {
            cooldownText.text = Mathf.CeilToInt(
                remainingTime).ToString();
        }

        if (remainingTime <= 0f)
        {
            isOnCooldown = false;
            UpdatePotionUI();
        }
    }

    void UpdatePotionUI()
    {
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = isOnCooldown ? 1f : 0f;
        }

        if (cooldownText != null)
        {
            cooldownText.text = isOnCooldown
                ? Mathf.CeilToInt(
                    nextAvailableTime - Time.time).ToString()
                : "";
        }
    }
}
