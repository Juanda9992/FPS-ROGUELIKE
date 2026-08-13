using UnityEngine;

public class GenericProjectileMovement : MonoBehaviour
{
    private ProjectileConfig config;
    [SerializeField] private Rigidbody rb;

    public void Initialize(ProjectileConfig config, Vector3 direction)
    {
        this.config = config;
        transform.localScale = config.scale;
        if (config.useVelocity)
        {
            rb.velocity = direction * config.speed;
            rb.useGravity = false;
        }
        else
        {
            transform.forward = direction;
        }
        Invoke(nameof(OnBallEndLife), config.lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (config.explodeOnImpact)
        {
            AreaDamage();
            Destroy(gameObject);
            return;
        }
        if (collision.collider.TryGetComponent<IDamageable>(out IDamageable damagable))
        {
            if (config.explodeOnImpact)
            {
                AreaDamage();
            }
            else
            {
                damagable.TakeDamage(config.damage);
            }
            Destroy(gameObject);
        }
    }

    private void AreaDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, config.radius, config.damageMask);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IDamageable>(out IDamageable damagable))
            {
                damagable.TakeDamage(config.damage);
            }
        }
    }

    private void OnBallEndLife()
    {
        if (config.explodeOnImpact)
        {
            AreaDamage();
        }
        Destroy(gameObject);
    }
}

[System.Serializable]
public class ProjectileConfig
{
    public float speed;
    public float lifeTime;
    public int damage;
    public bool useVelocity;
    public Vector3 scale;
    public float radius;
    public bool explodeOnImpact;
    public LayerMask damageMask;
}
