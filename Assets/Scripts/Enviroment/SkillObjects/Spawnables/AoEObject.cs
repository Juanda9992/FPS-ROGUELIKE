using UnityEngine;
using System.Collections.Generic;
public class AoEObject : MonoBehaviour, ISpawneable
{
    [SerializeField] private AoEEffectType effectType;
    [SerializeField] private Renderer renderer;

    [SerializeField] private List<Collider> entitiesInRange = new List<Collider>();
    private SpawnParams currentSpawnParams;
    public void Initialize(SpawnParams spawnParams)
    {
        effectType = spawnParams.effectType;
        currentSpawnParams = spawnParams;
        Color color = GetEffectColor();
        color.a = 0.5f;
        renderer.material.color = color;
        transform.localScale = spawnParams.objectScale;
        StartCoroutine(ApplyEffectCoroutine());
        DestroySelf(spawnParams.duration);
    }

    private void ApplyEffect()
    {
        switch (effectType)
        {
            case AoEEffectType.Damage:
                foreach (Collider entity in entitiesInRange)
                {
                    entity.TryGetComponent<IDamageable>(out IDamageable damageable);
                    if (damageable != null)
                    {
                        damageable.TakeDamage(Mathf.RoundToInt(currentSpawnParams.damage));
                    }
                }
                break;
            case AoEEffectType.Heal:
                foreach (Collider entity in entitiesInRange)
                {
                    entity.TryGetComponent<PlayerHealthController>(out PlayerHealthController playerHealthController);
                    Debug.Log("playerHealthController: " + playerHealthController);
                    if (playerHealthController != null)
                    {
                        playerHealthController.OnHealthRestored(Mathf.RoundToInt(currentSpawnParams.healAmount));
                    }
                }
                break;
                /*
                case AoEEffectType.Slow:
                    foreach (Collider entity in enemiesInRange)
                    {
                        enemy.GetComponent<EnemyAI>().Slow(currentSpawnParams.slowDuration);
                    }
                    break;
                case AoEEffectType.Stun:
                    foreach (Collider enemy in enemiesInRange)
                    {
                        enemy.GetComponent<EnemyAI>().Stun(currentSpawnParams.stunDuration);
                    }
                    break;
                case AoEEffectType.Push:
                    foreach (Collider enemy in enemiesInRange)
                    {
                        enemy.GetComponent<EnemyAI>().Push(currentSpawnParams.pushForce);
                    }
                    break;
                case AoEEffectType.Pull:
                    foreach (Collider enemy in enemiesInRange)
                    {
                        enemy.GetComponent<EnemyAI>().Pull(currentSpawnParams.pullForce);
                    }
                    break;
                case AoEEffectType.Taunt:
                    foreach (Collider enemy in enemiesInRange)
                    {
                        enemy.GetComponent<EnemyAI>().Taunt(currentSpawnParams.tauntDuration);
                    }
                    break;
                case AoEEffectType.Silence:
                    foreach (Collider enemy in enemiesInRange)
                    {
                        enemy.GetComponent<EnemyAI>().Silence(currentSpawnParams.silenceDuration);
                    }
                    break;
                case AoEEffectType.Root:
                    foreach (Collider enemy in enemiesInRange)
                    {
                        enemy.GetComponent<EnemyAI>().Root(currentSpawnParams.rootDuration);
                    }
                    break;
                case AoEEffectType.Blind:
                    foreach (Collider enemy in enemiesInRange)
                    {
                        enemy.GetComponent<EnemyAI>().Blind(currentSpawnParams.blindDuration);
                    }
                    break;
                    */
        }
    }

    private System.Collections.IEnumerator ApplyEffectCoroutine()
    {
        while (true)
        {
            CheckEntities();
            yield return null;
            ApplyEffect();
            yield return new WaitForSeconds(currentSpawnParams.effectRate);
        }
    }

    private void CheckEntities()
    {
        Collider[] entities = Physics.OverlapSphere(transform.position, transform.localScale.x);
        entitiesInRange.Clear();
        foreach (Collider entity in entities)
        {
            entitiesInRange.Add(entity);
        }
    }

    private void DestroySelf(float duration)
    {
        if (duration <= 0)
        {
            return;
        }
        Destroy(gameObject, duration);
    }

    private Color GetEffectColor()
    {
        switch (effectType)
        {
            case AoEEffectType.Damage:
                return Color.red;
            case AoEEffectType.Heal:
                return Color.green;
            case AoEEffectType.Slow:
                return Color.blue;
            case AoEEffectType.Stun:
                return Color.yellow;
            case AoEEffectType.Push:
                return Color.cyan;
            case AoEEffectType.Pull:
                return Color.magenta;
            case AoEEffectType.Taunt:
                return Color.white;
            case AoEEffectType.Silence:
                return Color.gray;
            case AoEEffectType.Root:
                return Color.black;
            case AoEEffectType.Blind:
                return Color.black;
            default:
                return Color.white;
        }
    }
}

public enum AoEEffectType
{
    Damage,
    Heal,
    Slow,
    Stun,
    Push,
    Pull,
    Taunt,
    Silence,
    Root,
    Blind
}
