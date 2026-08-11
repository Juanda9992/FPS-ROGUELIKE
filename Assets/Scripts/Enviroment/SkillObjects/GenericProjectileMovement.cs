using UnityEngine;

public class GenericProjectileMovement : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private bool useVelocity;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Rigidbody rb;
    public void Initialize(Vector3 direction, int damage, bool useVelocity, float speed, float lifeTime)
    {
        this.direction = direction;
        this.damage = damage;
        this.useVelocity = useVelocity;
        this.speed = speed;
        if (useVelocity)
        {
            rb.velocity = direction * speed;
            rb.useGravity = false;
        }
        else
        {
            transform.forward = direction;
        }

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable damagable))
        {
            damagable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
