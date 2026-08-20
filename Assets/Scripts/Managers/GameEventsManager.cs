using System;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Instance { get; private set; }

    public event Action OnGameStarted;
    public event Action OnPlayerShot;
    public event Action OnPlayerReload;
    public event Action OnPlayerJump;
    public event Action<int> OnPlayerTakeDamage;
    public event Action<GameObject> OnEnemyKilled;
    public event Action<GameObject> OnEnemySpawned;
    public event Action<int> OnEnemyTakeDamage;
    public event Action OnPlayerDied;

    [Header("Debug / State")]
    [SerializeField] private bool _isGameStarted;

    public bool IsGameStarted
    {
        get => _isGameStarted;
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
        if (!_isGameStarted)
        {
            CursorManager.SetCursorVisible(true);
        }
    }

    public void StartGame()
    {
        if (_isGameStarted)
        {
            return;
        }

        _isGameStarted = true;
        OnGameStarted?.Invoke();
    }

    public void TriggerPlayerShot()
    {
        OnPlayerShot?.Invoke();
    }

    public void TriggerPlayerReload()
    {
        OnPlayerReload?.Invoke();
    }

    public void TriggerPlayerJump()
    {
        OnPlayerJump?.Invoke();
    }

    public void TriggerPlayerTakeDamage(int damage)
    {
        OnPlayerTakeDamage?.Invoke(damage);
    }

    public void TriggerEnemyKilled(GameObject enemy)
    {
        OnEnemyKilled?.Invoke(enemy);
    }

    public void TriggerEnemySpawned(GameObject enemy)
    {
        OnEnemySpawned?.Invoke(enemy);
    }

    public void TriggerEnemyTakeDamage(int damage)
    {
        OnEnemyTakeDamage?.Invoke(damage);
    }

    public void TriggerPlayerDeath()
    {
        OnPlayerDied?.Invoke();
    }
}
