using UnityEngine;

public class EnemyDamageOnContact : MonoBehaviour, ISilenceable
{
    [Header("Damage Settings")]
    [SerializeField] private float _damageDistance = 2f;
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _attackRate = 1f;

    [Header("Effect Settings")]
    [SerializeField] private EnemyEffectType _effectType = EnemyEffectType.None;
    [SerializeField] private float _damageEffectMultiplier = 1.5f;
    [SerializeField] private float _slowDuration = 2.5f;
    [SerializeField] private float _slowStrength = 0.5f;
    [SerializeField] private float _weaknessPercentage = 50f;
    [SerializeField] private float _weaknessDuration = 3f;
    [SerializeField] private float _stunDuration = 0.75f;

    [Header("References")]
    [SerializeField] private EnemyFollow _enemyFollow;

    [Header("Status")]
    [SerializeField] private bool _isSilenced = false;

    private float _attackTimer;

    public bool IsSilenced
    {
        get => _isSilenced;
    }

    public int Damage
    {
        get => _damage;
        set => _damage = value;
    }

    public float DamageDistance
    {
        get => _damageDistance;
    }

    public EnemyEffectType EffectType
    {
        get => _effectType;
    }

    public void InitializeDamage(int damage)
    {
        _damage = damage;
        _attackTimer = _attackRate;
    }

    public void InitializeEffect(EnemyEffectType effectType)
    {
        _effectType = effectType;
    }

    public void TickDamage(float deltaTime, Vector3 playerPosition, PlayerHealthController playerHealth)
    {
        if (playerHealth == null || _isSilenced)
        {
            return;
        }

        if (_enemyFollow != null && _enemyFollow.IsBlind)
        {
            return;
        }

        float damageDistSqr = _damageDistance * _damageDistance;
        if ((transform.position - playerPosition).sqrMagnitude <= damageDistSqr)
        {
            _attackTimer += deltaTime;

            if (_attackTimer >= _attackRate)
            {
                int dealtDamage = _damage;
                if (_effectType == EnemyEffectType.Damage)
                {
                    dealtDamage = Mathf.RoundToInt(_damage * _damageEffectMultiplier);
                }

                playerHealth.TakeDamage(dealtDamage);
                ApplyStatusEffectToPlayer(playerHealth);
                _attackTimer = 0f;
            }
        }
    }

    private void ApplyStatusEffectToPlayer(PlayerHealthController playerHealth)
    {
        if (playerHealth == null)
        {
            return;
        }

        switch (_effectType)
        {
            case EnemyEffectType.Slowness:
                if (playerHealth.TryGetComponent<ISlowable>(out var slowable))
                {
                    slowable.ApplySlowEffect(_slowDuration, _slowStrength);
                }
                break;
            case EnemyEffectType.Weakness:
                if (playerHealth.TryGetComponent<IVulnerable>(out var vulnerable))
                {
                    vulnerable.ApplyVulnerability(_weaknessPercentage, _weaknessDuration);
                }
                break;
            case EnemyEffectType.Stun:
                if (playerHealth.TryGetComponent<IStuneable>(out var stuneable))
                {
                    stuneable.ApplyStunEffect(_stunDuration);
                }
                break;
        }
    }

    public void Silence(float duration)
    {
        _isSilenced = true;
        CancelInvoke(nameof(UnSilence));
        Invoke(nameof(UnSilence), duration);
    }

    public void UnSilence()
    {
        _isSilenced = false;
    }
}