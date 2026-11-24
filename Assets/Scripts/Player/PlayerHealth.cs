using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Health Bar")]
    public Slider healthBar;

    [Header("Game Over UI")]
    public GameOverUI gameOverUI;

    private bool isDead = false;
    private PlayerMovement playerMovement;
    private CharacterController controller;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();

        // Kung san ka nag spawn, dun ka babalik
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    //Healing
    public void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        UpdateHealthBar();

        Debug.Log("Player healed! HP: " + currentHealth);
    }


    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        UpdateHealthBar();

        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0f)
            Die();
    }


    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (controller != null)
            controller.enabled = false;

        if (gameOverUI != null)
        {
            gameOverUI.ShowDeathScreen();
        }
        else
        {
            Debug.LogError(
                "GameOverUI is not assigned on PlayerHealth!"
            );
        }

        Debug.Log("Player has died!");
    }

    public void Respawn()
    {

        isDead = false;

        if (controller != null)
            controller.enabled = false;

        transform.SetPositionAndRotation(
            spawnPosition, spawnRotation
        );

        currentHealth = maxHealth;
        UpdateHealthBar();

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (controller != null)
            controller.enabled = true;


        if (playerMovement != null)
        {
            playerMovement.ResetForRespawn();
        }

        Debug.Log("Player respawned!");


    }
}