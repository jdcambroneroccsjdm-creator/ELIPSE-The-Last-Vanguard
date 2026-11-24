using UnityEngine;
using TMPro;
using System.Collections;

public class Inventory : MonoBehaviour
{
    [Header("Upgrade Materials")]
    public int upgradeMaterials = 0;

    [Header("Inventory UI")]
    public TMP_Text materialCountText;
    public TMP_Text notificationText;

    [Header("Weapon References")]
    public PlayerAttack playerAttack;

    [Header("Weapon Upgrade Settings")]
    public int upgradeLevel = 1;
    public int maxUpgradeLevel = 10;

    public int baseUpgradeCost = 5;
    public int costIncrease = 5;

    public float normalDamageIncrease = 5f;
    public float heavyDamageIncrease = 10f;

    [Header("Weapon Upgrade UI")]
    public GameObject upgradePanel;
    public TMP_Text levelText;
    public TMP_Text damageText;
    public TMP_Text costText;
    public TMP_Text upgradeMessageText;

    private Coroutine notificationRoutine;

    void Start()
    {
        UpdateUI();
        UpdateUpgradeUI();

        // Hide upgrade panel when game starts
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("UpgradePanel is not assigned in Inventory!");
        }

        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }

    // OPEN UPGRADE PANEL
    public void OpenUpgradePanel()
    {
        Debug.Log("Anvil button clicked!");

        if (upgradePanel == null)
        {
            Debug.LogError("ERROR: UpgradePanel is NOT assigned in Inventory!");
            return;
        }

        upgradePanel.SetActive(true);

        UpdateUpgradeUI();

        if (upgradeMessageText != null)
            upgradeMessageText.text = "";

        Debug.Log("Upgrade Panel is now ACTIVE!");
    }

    // CLOSE UPGRADE PANEL
    public void CloseUpgradePanel()
    {
        if (upgradePanel == null)
        {
            Debug.LogError("UpgradePanel is not assigned!");
            return;
        }

        upgradePanel.SetActive(false);

        Debug.Log("Upgrade Panel Closed!");
    }

    // TOGGLE UPGRADE PANEL
    public void ToggleUpgradePanel()
    {
        if (upgradePanel == null)
        {
            Debug.LogError("UpgradePanel is not assigned!");
            return;
        }

        bool isOpen = !upgradePanel.activeSelf;

        upgradePanel.SetActive(isOpen);

        if (isOpen)
        {
            UpdateUpgradeUI();
        }

        Debug.Log("Upgrade Panel Active: " + isOpen);
    }

    // ADD MATERIALS
    public void AddMaterials(int amount)
    {
        if (amount <= 0) return;

        upgradeMaterials += amount;

        UpdateUI();

        ShowNotification("+" + amount + " Upgrade Materials");
    }

    // SPEND MATERIALS
    public bool SpendMaterials(int amount)
    {
        if (amount <= 0 || upgradeMaterials < amount)
            return false;

        upgradeMaterials -= amount;

        UpdateUI();

        return true;
    }

    // UPDATE INVENTORY UI
    void UpdateUI()
    {
        if (materialCountText != null)
        {
            materialCountText.text = upgradeMaterials.ToString();
        }
    }

    // NOTIFICATION
    void ShowNotification(string message)
    {
        if (notificationText == null) return;

        if (notificationRoutine != null)
            StopCoroutine(notificationRoutine);

        notificationRoutine =
            StartCoroutine(NotificationTimer(message));
    }

    IEnumerator NotificationTimer(string message)
    {
        notificationText.gameObject.SetActive(true);

        notificationText.text = message;

        yield return new WaitForSeconds(2f);

        notificationText.gameObject.SetActive(false);

        notificationRoutine = null;
    }

    // GET UPGRADE COST
    int GetUpgradeCost()
    {
        return baseUpgradeCost +
               (upgradeLevel - 1) * costIncrease;
    }

    // UPGRADE WEAPON
    public void UpgradeWeapon()
    {
        Debug.Log("UPGRADE BUTTON CLICKED!");

        if (upgradeLevel >= maxUpgradeLevel)
        {
            ShowUpgradeMessage("Weapon is already MAX level!");
            return;
        }

        if (playerAttack == null)
        {
            Debug.LogError("PlayerAttack is not assigned!");
            ShowUpgradeMessage("Weapon reference is missing!");
            return;
        }

        int upgradeCost = GetUpgradeCost();

        if (!SpendMaterials(upgradeCost))
        {
            ShowUpgradeMessage("Not enough upgrade materials!");
            Debug.Log("Not enough upgrade materials!");
            return;
        }

        // Increase weapon damage
        playerAttack.normalDamage += normalDamageIncrease;
        playerAttack.heavyDamage += heavyDamageIncrease;

        // Increase weapon level
        upgradeLevel++;

        ShowUpgradeMessage("Weapon successfully upgraded!");

        UpdateUpgradeUI();

        Debug.Log("Weapon upgraded to Level " + upgradeLevel);
        Debug.Log("Normal Damage: " + playerAttack.normalDamage);
        Debug.Log("Heavy Damage: " + playerAttack.heavyDamage);
    }

    // UPDATE WEAPON UPGRADE UI
    void UpdateUpgradeUI()
    {
        if (playerAttack == null)
            return;

        if (levelText != null)
        {
            levelText.text = "Weapon Level: " + upgradeLevel;
        }

        if (damageText != null)
        {
            damageText.text =
                "Normal Damage: " + playerAttack.normalDamage +
                "\nHeavy Damage: " + playerAttack.heavyDamage;
        }

        if (costText != null)
        {
            costText.text = upgradeLevel >= maxUpgradeLevel
                ? "MAX LEVEL"
                : "Upgrade Cost: " + GetUpgradeCost();
        }
    }

    // UPGRADE MESSAGE
    void ShowUpgradeMessage(string message)
    {
        if (upgradeMessageText != null)
        {
            upgradeMessageText.text = message;
        }
    }
}