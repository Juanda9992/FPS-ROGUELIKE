using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private EnemyFollow _enemyFollow;
    [SerializeField] private EnemyDamageOnContact _enemyDamageOnContact;

    public EnemyFollow EnemyFollow
    {
        get => _enemyFollow;
    }

    public EnemyDamageOnContact EnemyDamageOnContact
    {
        get => _enemyDamageOnContact;
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
