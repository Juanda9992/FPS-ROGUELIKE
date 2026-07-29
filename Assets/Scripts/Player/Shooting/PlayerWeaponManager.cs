using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public Weapon[] weapons;
    [SerializeField] private PlayerWeaponInstance[] weaponInstances;

    [SerializeField] private GameObject[] weaponPrefabs;
    [SerializeField] private FPSWeapon fpsWeapon;
    private int currentIndex = 0;
    private PlayerWeaponInstance currentWeapon;

    private void Start()
    {
        weaponInstances = new PlayerWeaponInstance[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            weaponInstances[i] = new PlayerWeaponInstance(weapons[i]);
        }

        if (weaponInstances.Length > 0)
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
        currentWeapon = weaponInstances[index];


        fpsWeapon.SetWeapon(currentWeapon);

        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            weaponPrefabs[i].SetActive(i == index);
        }
        Debug.Log($"Equipped weapon: {currentWeapon.weaponData.weaponName}");
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon.weaponData;
    }
}