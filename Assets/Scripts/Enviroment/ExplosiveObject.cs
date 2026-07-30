using UnityEngine;

public class ExplosiveObject : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 50;
    public int Health { get; set; }

    [Header("Explosión")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int maxDamage = 100;
    [SerializeField] private LayerMask damageLayers;

    private bool exploded = false;

    private void Start()
    {
        Health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (exploded) return;

        Health -= damage;

        if (Health <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        exploded = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageLayers);

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();

            if (damageable != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);

                float normalizedDistance = distance / explosionRadius;
                float damageMultiplier = 1f - Mathf.Clamp01(normalizedDistance);

                int finalDamage = Mathf.RoundToInt(maxDamage * damageMultiplier);

                damageable.TakeDamage(finalDamage);
            }
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}