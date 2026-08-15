using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Stats")]
    public string weaponName;
    public float damage;
    public int chargerAmmo;
    public int maxAmmo;
    public float reloadTime;
    public float fireRate;

    [Header("Recoil Kick & Shake")]
    public float recoilPitchKick = 1.8f;
    public float recoilYawKick = 0.9f;
    public float recoilRollKick = 0.4f;
    public float recoilSnappiness = 22f;
    public float recoilReturnSpeed = 10f;
    public float cameraShakeStrength = 0.15f;

    [Header("Spread & Accuracy Bloom")]
    public float baseSpread = 0.4f;
    public float maxSpread = 3.5f;
    public float spreadIncreasePerShot = 0.5f;
    public float spreadRecoverySpeed = 5.0f;
}