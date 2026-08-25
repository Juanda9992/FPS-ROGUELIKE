using UnityEngine;

public class EnemyClusterManager : MonoBehaviour
{
    public static EnemyClusterManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private EnemySpawner _enemySpawner;

    [Header("Cluster Settings")]
    [SerializeField] private int _minClusterSize = 10;
    [SerializeField] private int _maxClusterSize = 50;
    [SerializeField] private AnimationCurve _clusterSizeMultiplier = AnimationCurve.Linear(0f, 1f, 600f, 2.5f);
    [SerializeField] private bool _ignoreMaxActiveEnemies = true;

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
        _enemySpawner = EnemySpawner.Instance;
        TimeManager.Instance.OnMinuteElapsed += HandleMinuteElapsed;
    }

    private void OnDestroy()
    {
        TimeManager.Instance.OnMinuteElapsed -= HandleMinuteElapsed;
    }

    private void HandleMinuteElapsed(int minute)
    {
        SpawnCluster();
        GameScreenAdviceManager.Instance.SetMessage($"A horde of enemies is coming!", Color.red);
    }

    public void SpawnCluster()
    {
        float totalSeconds = TimeManager.Instance.TotalSeconds;
        float sizeMultiplier = _clusterSizeMultiplier.Evaluate(totalSeconds);
        int randomClusterSize = Random.Range(_minClusterSize, _maxClusterSize);
        int clusterCount = Mathf.Max(_minClusterSize, Mathf.RoundToInt(randomClusterSize * sizeMultiplier));

        _enemySpawner.SpawnCluster(clusterCount, _ignoreMaxActiveEnemies);
    }

    public void SpawnCluster(int count)
    {
        _enemySpawner.SpawnCluster(count, _ignoreMaxActiveEnemies);
    }

    [ContextMenu("Spawn Cluster Now")]
    private void SpawnClusterContextMenu()
    {
        SpawnCluster();
    }
}
