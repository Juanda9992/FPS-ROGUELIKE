using UnityEngine;

public class GenericProjectileMovement : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private bool explodeOnImpact;
    [Header("Area Damage")]
    [SerializeField] private bool areaDamage;
    [SerializeField] private float areaRadius;
    [SerializeField] private LayerMask areaMask;

    public void Initialize(Vector3 direction, int damage, bool useVelocity, float speed, float lifeTime)
    {
        this.damage = damage;
        if (useVelocity)
        {
            rb.velocity = direction * speed;
            rb.useGravity = false;
        }
        else
        {
            transform.forward = direction;
        }
        Invoke(nameof(OnBallEndLife), lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (explodeOnImpact)
        {
            AreaDamage();
            Destroy(gameObject);
            return;
        }
        if (collision.collider.TryGetComponent<IDamageable>(out IDamageable damagable))
        {
            if (areaDamage)
            {
                AreaDamage();
            }
            else
            {
                damagable.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    private void AreaDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, areaRadius, areaMask);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IDamageable>(out IDamageable damagable))
            {
                damagable.TakeDamage(damage);
            }
        }
    }

    private void OnBallEndLife()
    {
        if (areaDamage)
        {
            AreaDamage();
        }
        Destroy(gameObject);
    }
}
