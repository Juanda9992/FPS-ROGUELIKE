using UnityEngine;

public class EnemyHealthController : MonoBehaviour, IDamageable
{
    [SerializeField] private int health = 100;
    [SerializeField] private DamageFeedback damageFeedback;

    public int Health
    {
        get => health;
        set => health = value;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        damageFeedback.PlayDamageFlash();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    [ContextMenu("Take Damage")]
    private void TakeDamageContextMenu()
    {
        TakeDamage(10);
    }
}