using UnityEngine;
using TMPro;
public class WeaponUI : MonoBehaviour
{
    public FPSWeapon weapon;

    [SerializeField] private TextMeshProUGUI ammoText;
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
        Debug.Log("Disparo detectado (UI)");
    }

    void HandleReload()
    {
        Debug.Log("Recarga detectada (UI)");
    }

    void UpdateAmmo(int ammo)
    {
        ammoText.text = $"Ammo: {ammo}/{weapon.maxAmmo}";

        if(weapon.currentAmmo <= 0)
        {
            ammoText.color = Color.red;
        }
        else
        {
            ammoText.color = Color.white;
        }
    }
}