using System;
using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get; private set; }

    [Header("Combat Stats")]
    [SerializeField] private int _enemiesKilled;
    [SerializeField] private int _damageTaken;
    [SerializeField] private int _damageDealtToEnemies;

    [Header("Action Stats")]
    [SerializeField] private int _shotsFired;
    [SerializeField] private int _reloadCount;
    [SerializeField] private int _jumpCount;
    [SerializeField] private int _enemiesSpawned;

    // Events for UI or external systems
    public event Action<int> OnEnemiesKilledChanged;
    public event Action<int> OnDamageTakenChanged;
    public event Action<int> OnDamageDealtChanged;
    public event Action<int> OnShotsFiredChanged;
    public event Action<int> OnReloadCountChanged;
    public event Action<int> OnJumpCountChanged;
    public event Action<int> OnEnemiesSpawnedChanged;
    public event Action OnStatsUpdated;

    // Properties for read-only access
    public int EnemiesKilled
    {
        get => _enemiesKilled;
    }

    public int DamageTaken
    {
        get => _damageTaken;
    }

    public int DamageDealtToEnemies
    {
        get => _damageDealtToEnemies;
    }

    public int ShotsFired
    {
        get => _shotsFired;
    }

    public int ReloadCount
    {
        get => _reloadCount;
    }

    public int JumpCount
    {
        get => _jumpCount;
    }

    public int EnemiesSpawned
    {
        get => _enemiesSpawned;
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

    private void Start()
    {
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.OnGameStarted += HandleGameStarted;
            GameEventsManager.Instance.OnPlayerShot += HandlePlayerShot;
            GameEventsManager.Instance.OnPlayerReload += HandlePlayerReload;
            GameEventsManager.Instance.OnPlayerJump += HandlePlayerJump;
            GameEventsManager.Instance.OnPlayerTakeDamage += HandlePlayerTakeDamage;
            GameEventsManager.Instance.OnEnemyKilled += HandleEnemyKilled;
            GameEventsManager.Instance.OnEnemySpawned += HandleEnemySpawned;
            GameEventsManager.Instance.OnEnemyTakeDamage += HandleEnemyTakeDamage;

            if (GameEventsManager.Instance.IsGameStarted)
            {
                HandleGameStarted();
            }
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.OnGameStarted -= HandleGameStarted;
            GameEventsManager.Instance.OnPlayerShot -= HandlePlayerShot;
            GameEventsManager.Instance.OnPlayerReload -= HandlePlayerReload;
            GameEventsManager.Instance.OnPlayerJump -= HandlePlayerJump;
            GameEventsManager.Instance.OnPlayerTakeDamage -= HandlePlayerTakeDamage;
            GameEventsManager.Instance.OnEnemyKilled -= HandleEnemyKilled;
            GameEventsManager.Instance.OnEnemySpawned -= HandleEnemySpawned;
            GameEventsManager.Instance.OnEnemyTakeDamage -= HandleEnemyTakeDamage;
        }
    }

    public void ResetStats()
    {
        _enemiesKilled = 0;
        _damageTaken = 0;
        _damageDealtToEnemies = 0;
        _shotsFired = 0;
        _reloadCount = 0;
        _jumpCount = 0;
        _enemiesSpawned = 0;

        OnEnemiesKilledChanged?.Invoke(_enemiesKilled);
        OnDamageTakenChanged?.Invoke(_damageTaken);
        OnDamageDealtChanged?.Invoke(_damageDealtToEnemies);
        OnShotsFiredChanged?.Invoke(_shotsFired);
        OnReloadCountChanged?.Invoke(_reloadCount);
        OnJumpCountChanged?.Invoke(_jumpCount);
        OnEnemiesSpawnedChanged?.Invoke(_enemiesSpawned);
        OnStatsUpdated?.Invoke();
    }

    private void HandleGameStarted()
    {
        ResetStats();
    }

    private void HandlePlayerShot()
    {
        _shotsFired++;
        OnShotsFiredChanged?.Invoke(_shotsFired);
        OnStatsUpdated?.Invoke();
    }

    private void HandlePlayerReload()
    {
        _reloadCount++;
        OnReloadCountChanged?.Invoke(_reloadCount);
        OnStatsUpdated?.Invoke();
    }

    private void HandlePlayerJump()
    {
        _jumpCount++;
        OnJumpCountChanged?.Invoke(_jumpCount);
        OnStatsUpdated?.Invoke();
    }

    private void HandlePlayerTakeDamage(int damage)
    {
        _damageTaken += damage;
        OnDamageTakenChanged?.Invoke(_damageTaken);
        OnStatsUpdated?.Invoke();
    }

    private void HandleEnemyKilled(GameObject enemy)
    {
        _enemiesKilled++;
        OnEnemiesKilledChanged?.Invoke(_enemiesKilled);
        OnStatsUpdated?.Invoke();
    }

    private void HandleEnemySpawned(GameObject enemy)
    {
        _enemiesSpawned++;
        OnEnemiesSpawnedChanged?.Invoke(_enemiesSpawned);
        OnStatsUpdated?.Invoke();
    }

    private void HandleEnemyTakeDamage(int damage)
    {
        _damageDealtToEnemies += damage;
        OnDamageDealtChanged?.Invoke(_damageDealtToEnemies);
        OnStatsUpdated?.Invoke();
    }
}
