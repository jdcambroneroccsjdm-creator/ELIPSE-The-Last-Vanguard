
using UnityEngine;
using System.Collections;

public class EnemyRespawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;

    [Header("Respawn Settings")]
    public float respawnDelay = 5f;
    public bool respawnEnemy = true;

    [Header("Quest Settings")]
    public QuestMaker questMaker;

    private bool isRespawning = false;

    private void Start()
    {
        if (questMaker == null)
        {
            questMaker = FindObjectOfType<QuestMaker>();
        }

        SpawnEnemy();
    }

    public void EnemyDefeated()
    {
        if (!respawnEnemy || isRespawning)
            return;

        StartCoroutine(RespawnTimer());
    }

    private IEnumerator RespawnTimer()
    {
        isRespawning = true;

        yield return new WaitForSeconds(respawnDelay);

        SpawnEnemy();

        isRespawning = false;
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is missing!");
            return;
        }

        GameObject newEnemy = Instantiate(
            enemyPrefab,
            transform.position,
            transform.rotation
        );

        // HEALTH CHECK
        HealthBruh health =
            newEnemy.GetComponent<HealthBruh>();

        if (health != null)
        {
            health.respawner = this;
            health.questMaker = questMaker;
        }
        else
        {
            Debug.LogError(
                "Enemy Prefab is missing HealthBruh!"
            );
        }

        // LOOTER CHECK
        Looter looter =
            newEnemy.GetComponent<Looter>();

        if (looter != null)
        {
            if (looter.lootPrefab == null)
            {
                Debug.LogError(
                    "Loot Prefab is missing on spawned Enemy: "
                    + newEnemy.name
                );
            }
            else
            {
                Debug.Log(
                    "Loot Prefab successfully assigned: "
                    + looter.lootPrefab.name
                );
            }
        }
        else
        {
            Debug.LogError(
                "Enemy Prefab is missing Looter component!"
            );
        }

        Debug.Log("Enemy Spawned: " + newEnemy.name);
    }
}
