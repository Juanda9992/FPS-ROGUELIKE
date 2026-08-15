using UnityEngine;

public class EnemyDamageOnContact : MonoBehaviour, ISilenceable
{
    [Header("Damage Settings")]
    [SerializeField] private float _damageDistance = 2f;
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _attackRate = 1f;

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
    }

    public float DamageDistance
    {
        get => _damageDistance;
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
                playerHealth.TakeDamage(_damage);
                _attackTimer = 0f;
            }
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