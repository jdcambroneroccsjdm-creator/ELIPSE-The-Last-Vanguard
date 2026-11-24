
using UnityEngine;

public class HealthBruh : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Loot Drop")]
    public Looter looter;

    [Header("Quest System")]
    public QuestMaker questMaker;

    [Header("Respawn System")]
    public EnemyRespawner respawner;

    private bool isDead = false;
    private EnemyHitFlash hitFlash;

    private void Awake()
    {
        currentHealth = maxHealth;
        hitFlash = GetComponent<EnemyHitFlash>();

        FindLooter();
    }

    private void Start()
    {
        if (questMaker == null)
            questMaker = FindObjectOfType<QuestMaker>();
    }

    private void FindLooter()
    {
        if (looter != null)
            return;

        looter = GetComponent<Looter>();

        if (looter == null)
            looter = GetComponentInChildren<Looter>(true);

        if (looter == null)
            looter = GetComponentInParent<Looter>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0f)
            return;

        if (hitFlash != null)
            hitFlash.Flash();

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        Debug.Log(gameObject.name + " HP: " + currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(gameObject.name + " has been defeated!");

        if (!CompareTag("Enemy"))
        {
            Debug.LogWarning(
                "Enemy tag is missing on " + gameObject.name
            );

            return;
        }

        // QUEST
        if (questMaker != null)
            questMaker.AddKill();
        else
            Debug.LogWarning("QuestMaker not found!");

        // LOOT
        FindLooter();

        if (looter != null)
        {
            looter.DropLoot();
        }
        else
        {
            Debug.LogError(
                "No Looter component found on enemy: " + gameObject.name
            );
        }

        // RESPAWN
        if (respawner != null)
            respawner.EnemyDefeated();
        else
            Debug.LogWarning("EnemyRespawner not assigned!");

        // DESTROY ENEMY
        Destroy(gameObject);
    }
}
