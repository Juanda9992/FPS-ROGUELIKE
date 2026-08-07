using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerGrenadeUI : MonoBehaviour
{
    [SerializeField] private PlayerGrenadeController grenadeController;
    [SerializeField] private Image cooldownFillImage;
    [SerializeField] private TextMeshProUGUI ammoText;
    private void Awake()
    {
        cooldownFillImage.fillAmount = 1f;
    }
    private void OnEnable()
    {
        grenadeController.OnGrenadeThrown += UpdateAmmoUI;
        grenadeController.OnCooldownChanged += UpdateCooldownUI;
    }

    private void OnDisable()
    {
        grenadeController.OnGrenadeThrown -= UpdateAmmoUI;
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
    private void UpdateAmmoUI(int ammo)
    {
        ammoText.text = ammo.ToString();

        ammoText.color = ammo == 0 ? Color.red : Color.white;
    }
}
