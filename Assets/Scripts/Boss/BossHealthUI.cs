using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("Health UI")]
    public Slider healthSlider;
    public GameObject healthUI;

    public void ShowHealth()
    {
        healthUI.SetActive(true);
    }

    public void HideHealth()
    {
        healthUI.SetActive(false);
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth / maxHealth;
    }
}