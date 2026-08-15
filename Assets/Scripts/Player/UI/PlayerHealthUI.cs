using DG.Tweening;
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
        healthFillImage.DOFillAmount((float)current / max, 0.1f).SetEase(Ease.Linear);
    }
    private void UpdateShieldUI(int current, int max)
    {
        if (current <= 0)
        {
            shieldFillImage.gameObject.SetActive(false);
            return;
        }
        shieldFillImage.gameObject.SetActive(true);
        shieldFillImage.DOFillAmount((float)current / max, 0.1f).SetEase(Ease.Linear);
    }
}