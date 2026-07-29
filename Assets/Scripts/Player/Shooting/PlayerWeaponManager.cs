using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public Weapon[] weapons;

    private int currentIndex = 0;
    private Weapon currentWeapon;

    private void Start()
    {
        if (weapons.Length > 0)
        {
            EquipWeapon(0);
        }
    }

    private void Update()
    {
        HandleScrollInput();
    }

    private void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            NextWeapon();
        }
        else if (scroll < 0f)
        {
            PreviousWeapon();
        }
    }

    private void NextWeapon()
    {
        currentIndex++;
        if (currentIndex >= weapons.Length)
        {
            currentIndex = 0;
        }

        EquipWeapon(currentIndex);
    }

    private void PreviousWeapon()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = weapons.Length - 1;
        }

        EquipWeapon(currentIndex);
    }

    private void EquipWeapon(int index)
    {
        currentWeapon = weapons[index];

        Debug.Log("Arma equipada: " + currentWeapon.weaponName);
        Debug.Log("Daño: " + currentWeapon.damage);
        Debug.Log("Ammo: " + currentWeapon.ammo);
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
}