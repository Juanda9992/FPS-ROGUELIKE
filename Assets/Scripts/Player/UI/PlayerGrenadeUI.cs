using UnityEngine;
using UnityEngine.UI;

public class PlayerGrenadeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerGrenadeController grenadeController;
    [SerializeField] private Image cooldownFillImage;

    private void Awake()
    {
        cooldownFillImage.fillAmount = 1f;
    }
    private void OnEnable()
    {
        grenadeController.OnCooldownChanged += UpdateCooldownUI;
    }

    private void OnDisable()
    {
        grenadeController.OnCooldownChanged -= UpdateCooldownUI;
    }

    private void UpdateCooldownUI(float remainingTime, float totalCooldown)
    {
        if (totalCooldown > 0f)
        {
            float fillProgress = 1f - Mathf.Clamp01(remainingTime / totalCooldown);
            cooldownFillImage.fillAmount = fillProgress;
        }
        else
        {
            cooldownFillImage.fillAmount = 1f;
        }
    }
}
