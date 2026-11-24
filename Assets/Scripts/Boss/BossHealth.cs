using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("Detection Settings")]
    public Transform player;
    public float detectionRange = 15f;

    [Header("References")]
    public BossHealthUI bossUI;
    public PortalController portal;



    private void Start()
    {
        currentHealth = maxHealth;

        bossUI.HideHealth();
        bossUI.UpdateHealth(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (player == null || isDead)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance <= detectionRange)
            bossUI.ShowHealth();
        else
            bossUI.HideHealth();
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10f);
        }


    }

    // Receives the player's actual damage
    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        bossUI.UpdateHealth(currentHealth, maxHealth);

        Debug.Log("Boss HP: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        bossUI.HideHealth();

        Debug.Log("Boss Defeated!");

        if (portal != null)
        {
            portal.UnlockPortal();
        }

        Destroy(gameObject);
    }
}