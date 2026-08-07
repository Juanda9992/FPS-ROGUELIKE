using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrenadeBase : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] protected float damage = 50;
    [SerializeField] protected float radius = 5f;
    [SerializeField] protected float fuseTime = 3f;
    [SerializeField] protected float explosionForce = 500f;
    [SerializeField] protected bool explodeOnImpact = false;

    [Header("Detection & Effects")]
    [SerializeField] protected LayerMask damageLayers = ~0;
    [SerializeField] protected GameObject explosionVFX;

    [SerializeField] protected Rigidbody rb;
    protected bool hasExploded = false;

    // Public Getters for Stats
    public float Damage => damage;
    public float Radius => radius;
    public float FuseTime => fuseTime;
    public float ExplosionForce => explosionForce;
    public bool HasExploded => hasExploded;

    protected virtual void Start()
    {
        if (!explodeOnImpact && fuseTime > 0)
        {
            StartCoroutine(FuseRoutine());
        }
    }

    /// <summary>
    /// Parametrizes grenade stats dynamically upon instantiation or throw by multiplying base stats with provided multipliers.
    /// </summary>
    public virtual void Initialize(float customDamageMultiplier = 1f, float customRadiusMultiplier = 1f)
    {
        damage *= customDamageMultiplier;
        radius *= customRadiusMultiplier;
    }

    protected virtual IEnumerator FuseRoutine()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;

        if (explodeOnImpact)
        {
            Explode();
        }
    }

    /// <summary>
    /// Executes the explosion logic: deals area damage to IDamageable targets, applies physical impulse, spawns effects.
    /// </summary>
    public virtual void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Spawn Visual & Audio Effects
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        // Detect targets in explosion radius
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, damageLayers);
        foreach (Collider hit in hits)
        {
            // Deal damage to IDamageable targets with linear falloff
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float normalizedDistance = Mathf.Clamp01(distance / radius);
                float damageMultiplier = 1f - normalizedDistance;
                int finalDamage = Mathf.RoundToInt(damage * damageMultiplier);

                damageable.TakeDamage(Mathf.Max(1, finalDamage));
            }

            // Apply physical explosion force if target has Rigidbody
            if (hit.TryGetComponent<Rigidbody>(out var targetRb) && targetRb != rb)
            {
                targetRb.AddExplosionForce(explosionForce, transform.position, radius, 1f, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
