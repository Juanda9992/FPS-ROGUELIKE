using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Stats")]
    public string weaponName;
    public float damage;
    public int ammo;
    public float reloadTime;
    public float fireRate;
}