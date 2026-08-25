using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private EnemyHealthController _enemyHealthController;
    [SerializeField] private EnemyFollow _enemyFollow;
    [SerializeField] private EnemyDamageOnContact _enemyDamageOnContact;

    public EnemyHealthController EnemyHealthController
    {
        get => _enemyHealthController;
    }

    public EnemyFollow EnemyFollow
    {
        get => _enemyFollow;
    }

    public EnemyDamageOnContact EnemyDamageOnContact
    {
        get => _enemyDamageOnContact;
    }

    public void InitializeStats(int health, float speed, int damage)
    {
        if (_enemyHealthController != null)
        {
            _enemyHealthController.InitializeHealth(health);
        }

        if (_enemyFollow != null)
        {
            _enemyFollow.InitializeSpeed(speed);
        }

        if (_enemyDamageOnContact != null)
        {
            _enemyDamageOnContact.InitializeDamage(damage);
        }
    }

    public void InitializeStats(EnemyStatsData statsData)
    {
        if (statsData == null)
        {
            return;
        }

        InitializeStats(statsData.Health, statsData.Speed, statsData.Damage);
    }

    private void OnDisable()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.UnregisterEnemy(this);
        }
    }

    public void Tick(float deltaTime, Vector3 playerPosition, PlayerHealthController playerHealth)
    {
        if (_enemyFollow != null)
        {
            _enemyFollow.TickMovement(deltaTime, playerPosition);
        }

        if (_enemyDamageOnContact != null)
        {
            _enemyDamageOnContact.TickDamage(deltaTime, playerPosition, playerHealth);
        }
    }
}
