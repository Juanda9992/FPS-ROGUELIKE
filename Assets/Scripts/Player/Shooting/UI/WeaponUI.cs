using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private PlayerWeaponManager weaponManager;
    [SerializeField] private Image reloadFillImage;
    [SerializeField] private TextMeshProUGUI ammoText;

    private Tween reloadTween;
    private void Awake()
    {
        reloadFillImage.fillAmount = 0;
    }

    private void OnEnable()
    {
        weaponManager.OnShoot += HandleShoot;
        weaponManager.OnReload += HandleReload;
        weaponManager.OnAmmoChanged += UpdateAmmo;
        weaponManager.OnWeaponChanged += HandleWeaponChanged;
    }

    private void OnDisable()
    {
        weaponManager.OnShoot -= HandleShoot;
        weaponManager.OnReload -= HandleReload;
        weaponManager.OnAmmoChanged -= UpdateAmmo;
        weaponManager.OnWeaponChanged -= HandleWeaponChanged;

        reloadTween?.Kill();
    }

    private void HandleShoot()
    {
    }

    private void HandleReload()
    {
        PlayerWeaponInstance currentInstance = weaponManager.GetCurrentWeaponInstance();
        float reloadStatVal = (weaponManager.reloadSpeedStat != null && weaponManager.reloadSpeedStat.Value != 0) ? weaponManager.reloadSpeedStat.Value : 1f;

        reloadFillImage.fillAmount = 1;
        reloadTween?.Kill();
        reloadTween = reloadFillImage.DOFillAmount(0, currentInstance.reloadTime / reloadStatVal).SetEase(Ease.Linear);
    }

    private void HandleWeaponChanged()
    {
        reloadTween?.Kill();
        reloadFillImage.fillAmount = 0;
    }

    private void UpdateAmmo(int ammo, int reserveAmmo)
    {
        ammoText.text = $"{ammo}/{reserveAmmo}";
        if (ammo <= 0)
        {
            ammoText.color = Color.red;
        }
        else
        {
            ammoText.color = Color.white;
        }
    }
}