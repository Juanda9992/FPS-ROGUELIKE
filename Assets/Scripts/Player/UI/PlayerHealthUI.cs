using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealthController playerHealth;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Image shieldFillImage;

    void Awake()
    {
        healthFillImage.fillAmount = 1;
        shieldFillImage.fillAmount = 0;
    }
    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthUI;
        playerHealth.OnShieldChanged += UpdateShieldUI;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthUI;
        playerHealth.OnShieldChanged -= UpdateShieldUI;
    }

    private void UpdateHealthUI(int current, int max)
    {
        healthFillImage.fillAmount = (float)current / max;
    }
    private void UpdateShieldUI(int current, int max)
    {
        shieldFillImage.fillAmount = (float)current / max;
    }
}