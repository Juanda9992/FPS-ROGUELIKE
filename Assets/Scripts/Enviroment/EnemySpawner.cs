using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Target & Positioning")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _minSpawnRadius = 15f;
    [SerializeField] private float _maxSpawnRadius = 25f;

    [Header("Spawn Timing & Scaling")]
    [SerializeField] private float _baseSpawnInterval = 4f;
    [SerializeField] private float _minSpawnInterval = 0.5f;
    [SerializeField] private AnimationCurve _spawnRateProgression = AnimationCurve.EaseInOut(0f, 1f, 600f, 0.25f);
    [SerializeField] private AnimationCurve _enemyHealthOverTime = AnimationCurve.Linear(0f, 1f, 600f, 2f);
    [SerializeField] private AnimationCurve _enemyDamageOverTime = AnimationCurve.Linear(0f, 1f, 600f, 2f);
    [SerializeField] private Vector2Int _packSizeRange = new Vector2Int(1, 3);

    [Header("Enemy Pool & Limits")]
    [SerializeField] private List<EnemySpawnDataSO> _enemySpawnDataList = new List<EnemySpawnDataSO>();
    [SerializeField] private int _maxActiveEnemies = 50;

    public static EnemySpawner Instance { get; private set; }

    private float _spawnTimer;
    private float _currentInterval;
    private int _currentActiveEnemies;
    private bool _isGameStarted;

    public int CurrentActiveEnemies
    {
        get => _currentActiveEnemies;
    }

    public int MaxActiveEnemies
    {
        get => _maxActiveEnemies;
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
        }
    }

    private void HandleGameStarted()
    {
        _isGameStarted = true;
        _spawnTimer = 0f;
        SetNextSpawnInterval();
    }

    private void Update()
    {
        if (!_isGameStarted)
        {
            return;
        }

        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _currentInterval)
        {
            _spawnTimer = 0f;
            SetNextSpawnInterval();
            TrySpawnCluster();
        }
    }

    private void SetNextSpawnInterval()
    {
        float timeMultiplier = _spawnRateProgression.Evaluate(TimeManager.Instance.TotalSeconds);
        float calculatedInterval = Mathf.Max(_minSpawnInterval, _baseSpawnInterval * timeMultiplier);

        // Add 20% random jitter so intervals do not feel robotic
        _currentInterval = calculatedInterval * Random.Range(0.8f, 1.2f);
    }

    private void TrySpawnCluster()
    {
        if (_currentActiveEnemies >= _maxActiveEnemies)
        {
            return;
        }

        int clusterSize = Random.Range(_packSizeRange.x, _packSizeRange.y + 1);
        SpawnCluster(clusterSize, false);
    }

    public void SpawnCluster(int count, bool ignoreMaxLimit = false)
    {
        for (int i = 0; i < count; i++)
        {
            if (!ignoreMaxLimit && _currentActiveEnemies >= _maxActiveEnemies)
            {
                break;
            }

            SpawnSingleEnemy();
        }
    }

    public void SpawnSingleEnemy()
    {
        EnemySpawnDataSO spawnData = SelectEnemySpawnData();
        if (spawnData == null || spawnData.EnemyPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject enemyObj = Instantiate(spawnData.EnemyPrefab, spawnPosition, Quaternion.identity);
        _currentActiveEnemies++;

        if (enemyObj.TryGetComponent<EnemyBrain>(out var enemyBrain))
        {
            if (spawnData.EnemyStatsData != null)
            {
                float totalSeconds = TimeManager.Instance.TotalSeconds;
                float healthMultiplier = _enemyHealthOverTime.Evaluate(totalSeconds);
                float damageMultiplier = _enemyDamageOverTime.Evaluate(totalSeconds);

                int scaledHealth = Mathf.RoundToInt(spawnData.EnemyStatsData.Health * healthMultiplier);
                int scaledDamage = Mathf.RoundToInt(spawnData.EnemyStatsData.Damage * damageMultiplier);

                enemyBrain.InitializeStats(scaledHealth, spawnData.EnemyStatsData.Speed, scaledDamage);
            }

            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.RegisterEnemy(enemyBrain);
            }
        }

        if (enemyObj.TryGetComponent<EnemyHealthController>(out var healthController))
        {
            healthController.OnDeath += HandleEnemyDeath;
        }

        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.TriggerEnemySpawned(enemyObj);
        }
    }

    private EnemySpawnDataSO SelectEnemySpawnData()
    {
        float totalWeight = 0f;

        List<KeyValuePair<EnemySpawnDataSO, float>> availableEnemies = new List<KeyValuePair<EnemySpawnDataSO, float>>();

        for (int i = 0; i < _enemySpawnDataList.Count; i++)
        {
            EnemySpawnDataSO spawnData = _enemySpawnDataList[i];
            if (spawnData == null || spawnData.EnemyPrefab == null)
            {
                continue;
            }

            float weight = spawnData.GetCurrentWeight(TimeManager.Instance.TotalSeconds);
            if (weight > 0f)
            {
                totalWeight += weight;
                availableEnemies.Add(new KeyValuePair<EnemySpawnDataSO, float>(spawnData, weight));
            }
        }

        if (availableEnemies.Count == 0 || totalWeight <= 0f)
        {
            return null;
        }

        float randomRoll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < availableEnemies.Count; i++)
        {
            cumulative += availableEnemies[i].Value;
            if (randomRoll <= cumulative)
            {
                return availableEnemies[i].Key;
            }
        }

        return availableEnemies[0].Key;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(_minSpawnRadius, _maxSpawnRadius);

        Vector3 offset = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
        return _playerTransform.position + offset;
    }

    private void HandleEnemyDeath()
    {
        _currentActiveEnemies = Mathf.Max(0, _currentActiveEnemies - 1);
    }
}
