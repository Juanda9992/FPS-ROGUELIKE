using UnityEngine;

public class EnemyHealthController : MonoBehaviour, IDamageable, IVulnerable
{
    [SerializeField] private int health = 100;
    [SerializeField] private DamageFeedback damageFeedback;
    [SerializeField] private OrbGenerator orbGenerator;
    [SerializeField] private float _vulnerabilityPercentage = 1f;

    public int Health
    {
        get => health;
        set => health = value;
    }

    public float Percentage
    {
        get => _vulnerabilityPercentage;
        set => _vulnerabilityPercentage = value;
    }

    public void ApplyVulnerability(float percentage, float duration)
    {
        _vulnerabilityPercentage = percentage;
        StartCoroutine(UndoVulnerabilityCoroutine(duration));
    }

    private System.Collections.IEnumerator UndoVulnerabilityCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        UndoVulnerability();
    }

    public void UndoVulnerability()
    {
        _vulnerabilityPercentage = 1f;
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.RoundToInt(damage * _vulnerabilityPercentage);
        health -= finalDamage;

        damageFeedback.PlayDamageFlash();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        orbGenerator.SpawnOrbs();

        Destroy(gameObject);
    }

    [ContextMenu("Take Damage")]
    private void TakeDamageContextMenu()
    {
        TakeDamage(10);
    }
}