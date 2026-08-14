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
                    if (!currentSpawnParams.affectPlayer && entity.CompareTag("Player"))
                    {
                        continue;
                    }

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
                    if (!currentSpawnParams.affectPlayer && entity.CompareTag("Player"))
                    {
                        continue;
                    }
                    entity.TryGetComponent<PlayerHealthController>(out PlayerHealthController playerHealthController);
                    if (playerHealthController != null)
                    {
                        playerHealthController.OnHealthRestored(Mathf.RoundToInt(currentSpawnParams.healAmount));
                    }
                }
                break;
            case AoEEffectType.Slow:
                foreach (Collider entity in entitiesInRange)
                {
                    if (!currentSpawnParams.affectPlayer && entity.CompareTag("Player"))
                    {
                        continue;
                    }
                    entity.TryGetComponent<ISlowable>(out ISlowable slowable);
                    if (slowable != null)
                    {
                        slowable.ApplySlowEffect(currentSpawnParams.slowDuration, currentSpawnParams.slowAmount);
                    }
                }
                break;

            case AoEEffectType.Stun:
                foreach (Collider entity in entitiesInRange)
                {
                    if (!currentSpawnParams.affectPlayer && entity.CompareTag("Player"))
                    {
                        continue;
                    }
                    entity.TryGetComponent<IStuneable>(out IStuneable stuneable);
                    if (stuneable != null)
                    {
                        stuneable.ApplyStunEffect(currentSpawnParams.stunDuration);
                    }
                }
                break;

            case AoEEffectType.Push:
                foreach (Collider entity in entitiesInRange)
                {
                    if (!currentSpawnParams.affectPlayer && entity.CompareTag("Player"))
                    {
                        continue;
                    }
                    entity.TryGetComponent<IPusheable>(out IPusheable pusheable);
                    if (pusheable != null)
                    {
                        pusheable.Push(transform.position, currentSpawnParams.pushForce);
                    }
                }
                break;

            case AoEEffectType.Pull:
                foreach (Collider entity in entitiesInRange)
                {
                    if (!currentSpawnParams.affectPlayer && entity.CompareTag("Player"))
                    {
                        continue;
                    }
                    entity.TryGetComponent<IPusheable>(out IPusheable pusheable);
                    if (pusheable != null)
                    {
                        pusheable.Push(transform.position, currentSpawnParams.pushForce, true);
                    }
                }
                break;

            case AoEEffectType.Blind:
                foreach (Collider entity in entitiesInRange)
                {
                    if (!currentSpawnParams.affectPlayer && entity.CompareTag("Player"))
                    {
                        continue;
                    }
                    entity.TryGetComponent<IBlindable>(out IBlindable blindable);
                    if (blindable != null)
                    {
                        blindable.Blind(currentSpawnParams.blindDuration);
                    }
                }
                break;
            case AoEEffectType.Silence:
                foreach (Collider entity in entitiesInRange)
                {
                    if (!currentSpawnParams.affectPlayer && entity.CompareTag("Player"))
                    {
                        continue;
                    }
                    entity.TryGetComponent<ISilenceable>(out ISilenceable silenceable);
                    if (silenceable != null)
                    {
                        silenceable.Silence(currentSpawnParams.silenceDuration);
                    }
                }
                break;
        }
    }

    private System.Collections.IEnumerator ApplyEffectCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
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
        Collider[] entities = Physics.OverlapSphere(transform.position, currentSpawnParams.radius);
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
            case AoEEffectType.Silence:
                return Color.gray;
            case AoEEffectType.Blind:
                return Color.black;
            default:
                return Color.white;
        }
    }
}

public enum AoEEffectType
{
    None = -1,
    Damage = 0,
    Heal = 1,
    Slow = 2,
    Stun = 3,
    Push = 4,
    Pull = 5,
    Silence = 6,
    Blind = 7
}
