using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnData", menuName = "Scriptables/Enemy/Spawn Data")]
public class EnemySpawnDataSO : ScriptableObject
{
    [Header("Enemy Prefab")]
    [SerializeField] private GameObject _enemyPrefab;

    [Header("Enemy Stats")]
    [SerializeField] private EnemyStatsData _enemyStatsData;

    [Header("Spawn Settings")]
    [SerializeField] private int _baseWeight = 10;
    [SerializeField] private int _minTimeSecondsToSpawn = 0;
    [SerializeField] private AnimationCurve _weightMultiplierOverTime = AnimationCurve.Linear(0f, 1f, 600f, 2f);

    public GameObject EnemyPrefab
    {
        get => _enemyPrefab;
    }

    public EnemyStatsData EnemyStatsData
    {
        get => _enemyStatsData;
    }

    public int BaseWeight
    {
        get => _baseWeight;
    }

    public int MinTimeSecondsToSpawn
    {
        get => _minTimeSecondsToSpawn;
    }

    public float GetCurrentWeight(int elapsedSeconds)
    {
        if (elapsedSeconds < _minTimeSecondsToSpawn)
        {
            return 0f;
        }

        if (_weightMultiplierOverTime != null && _weightMultiplierOverTime.length > 0)
        {
            return _baseWeight * _weightMultiplierOverTime.Evaluate(elapsedSeconds);
        }

        return _baseWeight;
    }
}
