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
    }

    private void OnDisable()
    {
        weaponManager.OnShoot -= HandleShoot;
        weaponManager.OnReload -= HandleReload;
        weaponManager.OnAmmoChanged -= UpdateAmmo;

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

    private void UpdateAmmo(int ammo)
    {
        PlayerWeaponInstance currentInstance = weaponManager.GetCurrentWeaponInstance();
        ammoText.text = $"{ammo}/{currentInstance.weaponData.ammo}";
        if (currentInstance.currentAmmo <= 0)
        {
            ammoText.color = Color.red;
        }
        else
        {
            ammoText.color = Color.white;
        }
        reloadTween?.Kill();
        reloadFillImage.fillAmount = 0;
    }
}