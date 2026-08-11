using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillProjectileSO", menuName = "Skills/SkillProjectileSO")]
public class SkillProjectileSO : ActiveSkillSO
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float lifeTime = 5f;
    public float damage = 10f;

    [SerializeField] private Vector3 direction;
    [SerializeField] private bool usePlayerForward;
    [SerializeField] private Vector3 offset;
    public override void Activate(GameObject owner, SkillInstance instance = null)
    {
        Vector3 spawnPosition = owner.transform.position + owner.transform.TransformDirection(offset);
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, owner.transform.rotation);
        Vector3 spawnDirection = usePlayerForward ? Camera.main.transform.forward : direction;
        projectile.GetComponent<GenericProjectileMovement>().Initialize(spawnDirection, (int)instance?.GetEffectiveDamage(), true, projectileSpeed, lifeTime);
    }
}
