using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
public class WeaponUI : MonoBehaviour
{
    [SerializeField] FPSWeapon weapon;
    [SerializeField] private Image reloadFillImage;
    [SerializeField] private TextMeshProUGUI ammoText;

    private void Awake()
    {
        reloadFillImage.fillAmount = 0;
    }

    void OnEnable()
    {
        weapon.OnShoot += HandleShoot;
        weapon.OnReload += HandleReload;
        weapon.OnAmmoChanged += UpdateAmmo;
    }

    void OnDisable()
    {
        weapon.OnShoot -= HandleShoot;
        weapon.OnReload -= HandleReload;
        weapon.OnAmmoChanged -= UpdateAmmo;
    }

    void HandleShoot()
    {
    }

    void HandleReload()
    {
        reloadFillImage.fillAmount = 1;
        reloadFillImage.DOFillAmount(0, weapon.GetCurrentWeapon().reloadTime / weapon.reloadSpeedStat.Value).SetEase(Ease.Linear);
    }

    void UpdateAmmo(int ammo)
    {
        ammoText.text = $"{ammo}/{weapon.GetCurrentWeapon().weaponData.ammo}";

        if(weapon.GetCurrentWeapon().currentAmmo <= 0)
        {
            ammoText.color = Color.red;
        }
        else
        {
            ammoText.color = Color.white;
        }
    }
}