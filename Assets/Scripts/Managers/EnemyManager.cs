using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("Player References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private PlayerHealthController _playerHealth;

    private readonly List<EnemyBrain> _activeEnemies = new List<EnemyBrain>(500);

    public Transform PlayerTransform
    {
        get => _playerTransform;
    }

    public PlayerHealthController PlayerHealth
    {
        get => _playerHealth;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (_playerTransform == null)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        Vector3 playerPosition = _playerTransform.position;

        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            if (_activeEnemies[i] == null)
            {
                _activeEnemies.RemoveAt(i);
                continue;
            }

            _activeEnemies[i].Tick(deltaTime, playerPosition, _playerHealth);
        }
    }

    public void RegisterEnemy(EnemyBrain enemy)
    {
        if (enemy == null)
        {
            return;
        }

        if (!_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(EnemyBrain enemy)
    {
        if (enemy == null)
        {
            return;
        }

        _activeEnemies.Remove(enemy);
    }
}
