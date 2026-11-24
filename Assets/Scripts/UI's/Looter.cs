
using UnityEngine;

public class Looter : MonoBehaviour
{
    [Header("Loot Drop")]
    public GameObject lootPrefab;
    public int minAmount = 1;
    public int maxAmount = 3;
    public float dropHeight = 0.5f;

    [Header("Pickup Settings")]
    public float pickupRadius = 5f;
    public float pickupSpeed = 8f;
    public float collectDistance = 1f;

    private Transform player;
    private Inventory inventory;

    private bool isLoot;
    private bool attracted;
    private bool collected;

    private GameObject lootObject;

    // DROP LOOT
    public void DropLoot()
    {
        if (lootPrefab == null)
        {
            Debug.LogError(
                "Loot Prefab is missing on " + gameObject.name
            );
            return;
        }

        // Always spawn at the enemy's root position
        Vector3 spawnPosition =
            transform.root.position + Vector3.up * dropHeight;

        GameObject spawnedLoot = Instantiate(
            lootPrefab,
            spawnPosition,
            Quaternion.identity
        );

        // Find pickup script on prefab or children
        Looter pickupScript =
            spawnedLoot.GetComponentInChildren<Looter>(true);

        if (pickupScript == null)
        {
            Debug.LogError(
                "Spawned material has no Looter component!"
            );

            Destroy(spawnedLoot);
            return;
        }

        // Determine random amount
        int amount = Random.Range(
            Mathf.Min(minAmount, maxAmount),
            Mathf.Max(minAmount, maxAmount) + 1
        );

        // Initialize spawned pickup
        pickupScript.InitializePickup(
            spawnedLoot,
            amount
        );

        Debug.Log(
            "MATERIAL SUCCESSFULLY SPAWNED! Amount: " + amount
        );
    }

    // INITIALIZE PICKUP
    private void InitializePickup(GameObject spawnedLoot, int amount)
    {
        lootObject = spawnedLoot;

        isLoot = true;
        attracted = false;
        collected = false;

        minAmount = amount;
        maxAmount = amount;

        // Prevent spawned material from dropping more materials
        lootPrefab = null;

        // Prevent physics from fighting the attraction movement
        Rigidbody rb = lootObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        FindPlayer();
    }

    // FIND PLAYER AND INVENTORY
    public void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogWarning("Player with Player tag not found!");
            return;
        }

        player = playerObject.transform;

        inventory = playerObject.GetComponent<Inventory>();

        if (inventory == null)
        {
            inventory =
                playerObject.GetComponentInChildren<Inventory>(true);
        }

        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
        }

        if (inventory == null)
        {
            Debug.LogError("Inventory not found!");
            return;
        }

        Debug.Log("PLAYER FOUND: " + playerObject.name);
        Debug.Log("INVENTORY FOUND!");
    }

    // AUTOMATIC PICKUP
    private void Update()
    {
        if (!isLoot || collected)
            return;

        if (lootObject == null)
            lootObject = transform.root.gameObject;

        if (player == null || inventory == null)
        {
            FindPlayer();

            if (player == null || inventory == null)
                return;
        }

        float distance = Vector3.Distance(
            lootObject.transform.position,
            player.position
        );

        // START ATTRACTION
        if (distance <= pickupRadius)
        {
            if (!attracted)
                Debug.Log("MATERIAL ATTRACTED TO PLAYER!");

            attracted = true;
        }

        if (!attracted)
            return;

        // MOVE MATERIAL TOWARD PLAYER
        lootObject.transform.position = Vector3.MoveTowards(
            lootObject.transform.position,
            player.position,
            pickupSpeed * Time.deltaTime
        );

        float newDistance = Vector3.Distance(
            lootObject.transform.position,
            player.position
        );

        // COLLECT MATERIAL
        if (newDistance <= collectDistance)
        {
            CollectLoot();
        }
    }

    // COLLECT MATERIALS
    private void CollectLoot()
    {
        if (collected)
            return;

        if (inventory == null)
        {
            Debug.LogError("Cannot collect: Inventory is missing!");
            return;
        }

        collected = true;

        int amount = minAmount;

        inventory.AddMaterials(amount);

        Debug.Log("MATERIALS COLLECTED: " + amount);

        Debug.Log(
            "TOTAL MATERIALS: " +
            inventory.upgradeMaterials
        );

        Destroy(lootObject);
    }
}
