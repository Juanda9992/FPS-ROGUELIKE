using UnityEngine;

public class EffectCapsule : MonoBehaviour, ISpawneable
{
    [Header("AoE Prefab Settings")]
    [SerializeField] private GameObject aoePrefab;

    [Header("Spawn Behavior")]
    [SerializeField] private bool spawnOnCollision = false;
    [SerializeField] private bool destroyOnSpawn = false;

    private SpawnParams _spawnParams;
    private bool _hasSpawnedAoE = false;
    public void Initialize(SpawnParams spawnParams)
    {
        _spawnParams = spawnParams;
    }

    public void CreateAoEObject()
    {
        if (_hasSpawnedAoE)
        {
            return;
        }
        _hasSpawnedAoE = true;

        GameObject spawnedAoE = Instantiate(aoePrefab, transform.position, Quaternion.identity);

        if (spawnedAoE.TryGetComponent<ISpawneable>(out ISpawneable spawneable))
        {
            spawneable.Initialize(_spawnParams);
        }

        if (destroyOnSpawn)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (spawnOnCollision && !_hasSpawnedAoE)
        {
            CreateAoEObject();
        }
    }
}
