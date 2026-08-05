using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealthController playerHealth;
    [SerializeField] private Image healthFillImage;

    void Awake()
    {
        healthFillImage.fillAmount = 1;
    }
    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI(int current, int max)
    {
        healthFillImage.fillAmount = (float)current / max;
    }
}