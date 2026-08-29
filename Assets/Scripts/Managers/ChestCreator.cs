using UnityEngine;

public class ChestCreator : MonoBehaviour
{
    public static ChestCreator Instance { get; private set; }

    [Header("Prefab Reference")]
    [SerializeField] private GameObject _chestPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _minSpawnRadius = 5f;
    [SerializeField] private float _maxSpawnRadius = 20f;
    [SerializeField] private LayerMask _groundLayer = ~0;
    [SerializeField] private float _raycastHeight = 50f;

    public GameObject ChestPrefab => _chestPrefab;

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

    public GameObject GenerateChest(Vector3 position, Quaternion rot)
    {
        if (_chestPrefab == null)
        {
            Debug.LogWarning("[ChestCreator] ChestPrefab is not assigned.");
            return null;
        }

        GameObject chestInstance = Instantiate(_chestPrefab, position, rot);
        Debug.Log($"[ChestCreator] Chest spawned at {position}");
        return chestInstance;
    }

    public GameObject GenerateChestOnRandomPlace()
    {
        Vector3 origin = transform.position;

        if (_playerTransform != null)
        {
            origin = _playerTransform.position;
        }
        else
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                _playerTransform = playerObj.transform;
                origin = _playerTransform.position;
            }
        }

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(_minSpawnRadius, _maxSpawnRadius);

        Vector3 offset = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
        Vector3 targetPos = origin + offset;

        Vector3 rayStart = targetPos + Vector3.up * _raycastHeight;
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
        {
            targetPos = hit.point;
        }

        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        return GenerateChest(targetPos, randomRotation);
    }
    public bool TrySpawnChestFromKill(Vector3 position)
    {
        Stat luckStat = PlayerStatsManager.Instance.GetStatByName("Luck");
        float luckValue = luckStat != null ? luckStat.Value : 0f;

        if (luckValue <= 0f)
        {
            return false;
        }

        float roll = Random.Range(0f, 100f);
        if (roll < luckValue)
        {
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            GenerateChest(position, randomRotation);
            return true;
        }

        return false;
    }

    #region ContextMenu Tests
    [ContextMenu("Test Generate Chest At Position")]
    private void TestGenerateChest()
    {
        Vector3 spawnPos = transform.position + transform.forward * 3f;
        GenerateChest(spawnPos, Quaternion.identity);
    }

    [ContextMenu("Test Generate Chest On Random Place")]
    private void TestGenerateChestOnRandomPlace()
    {
        GenerateChestOnRandomPlace();
    }

    [ContextMenu("Test Try Spawn Chest From Kill")]
    private void TestTrySpawnChestFromKill()
    {
        Vector3 spawnPos = transform.position + transform.forward * 3f;
        bool spawned = TrySpawnChestFromKill(spawnPos);
        Debug.Log($"[ChestCreator Test] TrySpawnChestFromKill result: {spawned}");
    }
    #endregion
}
