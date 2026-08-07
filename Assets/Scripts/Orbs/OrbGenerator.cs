using UnityEngine;

public class OrbGenerator : MonoBehaviour
{
    [Header("Orb Prefabs")]
    [SerializeField] private OrbBase[] orbPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private int minOrbs = 1;
    [SerializeField] private int maxOrbs = 4;
    [SerializeField] private float spawnRadius = 0.8f;
    [SerializeField] private float burstForce = 3f;

    /// <summary>
    /// Spawns a random amount of random orb prefabs at the current position.
    /// </summary>
    public void SpawnOrbs()
    {
        SpawnOrbs(transform.position);
    }

    /// <summary>
    /// Spawns a random amount of random orb prefabs at the specified position.
    /// </summary>
    public void SpawnOrbs(Vector3 position)
    {
        if (orbPrefabs == null || orbPrefabs.Length == 0)
        {
            Debug.LogWarning($"[OrbGenerator] No orb prefabs assigned on {gameObject.name}. Cannot spawn orbs.");
            return;
        }

        int amountToSpawn = Random.Range(minOrbs, maxOrbs + 1);

        for (int i = 0; i < amountToSpawn; i++)
        {
            int randomIndex = Random.Range(0, orbPrefabs.Length);
            OrbBase prefabToSpawn = orbPrefabs[randomIndex];

            if (prefabToSpawn == null) continue;

            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = Mathf.Abs(randomOffset.y); // Keep offset at or above ground level
            Vector3 spawnPos = position + randomOffset;

            OrbBase orb = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

            Rigidbody rb = orb.GetRb();
            if (rb != null)
            {
                Vector3 forceDirection = (randomOffset + Vector3.up).normalized;
                rb.AddForce(forceDirection * burstForce, ForceMode.Impulse);
            }
        }
    }
}
