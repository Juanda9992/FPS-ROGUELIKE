using UnityEngine;
using UnityEngine.Serialization;

public class OrbGenerator : MonoBehaviour
{
    [Header("Orb Prefabs")]
    [SerializeField] private OrbBase[] _orbPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private int _minOrbs = 1;
    [SerializeField] private int _maxOrbs = 4;
    [SerializeField] private float _spawnRadius = 0.8f;
    [SerializeField] private float _burstForce = 3f;

    [Header("Dynamic Spawn Thresholds")]
    [SerializeField][Range(0.1f, 1f)] private float _healthSpawnThreshold = 1.0f;
    [SerializeField][Range(0.1f, 1f)] private float _shieldSpawnThreshold = 1.0f;
    [SerializeField] private int _maxSelectionAttempts = 20;

    private PlayerHealthController _playerHealth;
    private PlayerGrenadeController _playerGrenade;
    private PlayerWeaponManager _playerWeapon;
    private bool _hasFoundPlayer;

    /// <summary>
    /// Spawns a random amount of dynamically filtered orb prefabs at the current position.
    /// </summary>
    public void SpawnOrbs()
    {
        SpawnOrbs(transform.position);
    }

    /// <summary>
    /// Spawns a random amount of dynamically filtered orb prefabs at the specified position.
    /// </summary>
    public void SpawnOrbs(Vector3 position)
    {
        if (_orbPrefabs == null || _orbPrefabs.Length == 0)
        {
            Debug.LogWarning($"[OrbGenerator] No orb prefabs assigned on {gameObject.name}. Cannot spawn orbs.");
            return;
        }

        CachePlayerReferences();

        int amountToSpawn = Random.Range(_minOrbs, _maxOrbs + 1);

        for (int i = 0; i < amountToSpawn; i++)
        {
            OrbBase prefabToSpawn = null;
            int attempts = 0;

            do
            {
                int randomIndex = Random.Range(0, _orbPrefabs.Length);
                prefabToSpawn = _orbPrefabs[randomIndex];
                attempts++;
            }
            while (!IsOrbValid(prefabToSpawn) && attempts < _maxSelectionAttempts);

            // Fallback: If after max attempts the selected orb is still invalid, find any valid orb
            if (!IsOrbValid(prefabToSpawn))
            {
                prefabToSpawn = GetFallbackOrb();
            }

            if (prefabToSpawn == null)
            {
                continue;
            }

            Vector3 randomOffset = Random.insideUnitSphere * _spawnRadius;
            randomOffset.y = Mathf.Abs(randomOffset.y); // Keep offset at or above ground level
            Vector3 spawnPos = position + randomOffset;

            OrbBase orb = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

            Rigidbody rb = orb.GetRb();
            if (rb != null)
            {
                Vector3 forceDirection = (randomOffset + Vector3.up).normalized;
                rb.AddForce(forceDirection * _burstForce, ForceMode.Impulse);
            }
        }
    }

    private void CachePlayerReferences()
    {
        if (_hasFoundPlayer && _playerHealth != null)
        {
            return;
        }

        if (PlayerHealthController.Instance != null)
        {
            _playerHealth = PlayerHealthController.Instance;
        }

        GameObject playerObj = null;
        if (EnemyManager.Instance != null && EnemyManager.Instance.PlayerTransform != null)
        {
            playerObj = EnemyManager.Instance.PlayerTransform.gameObject;
        }
        else
        {
            playerObj = GameObject.FindGameObjectWithTag("Player");
        }

        if (playerObj != null)
        {
            playerObj.TryGetComponent<PlayerHealthController>(out _playerHealth);
            playerObj.TryGetComponent<PlayerGrenadeController>(out _playerGrenade);
            playerObj.TryGetComponent<PlayerWeaponManager>(out _playerWeapon);
            _hasFoundPlayer = true;
        }
    }

    private bool IsOrbValid(OrbBase orbPrefab)
    {
        if (orbPrefab == null)
        {
            return false;
        }

        CachePlayerReferences();

        switch (orbPrefab.Type)
        {
            case OrbType.Health:
                if (_playerHealth != null)
                {
                    int healthThresholdValue = Mathf.RoundToInt(_playerHealth.MaxHealth * _healthSpawnThreshold);
                    return _playerHealth.Health < healthThresholdValue;
                }
                return true;

            case OrbType.Shield:
                if (_playerHealth != null)
                {
                    // If player hasn't unlocked shield, shield orbs never spawn
                    if (_playerHealth.MaxShield <= 0)
                    {
                        return false;
                    }

                    int shieldThresholdValue = Mathf.RoundToInt(_playerHealth.MaxShield * _shieldSpawnThreshold);
                    return _playerHealth.CurrentShield < shieldThresholdValue;
                }
                return false;

            case OrbType.Ammo:
                if (orbPrefab is AmmoOrb ammoOrb)
                {
                    if (ammoOrb.IsGrenadeReload)
                    {
                        if (_playerGrenade != null)
                        {
                            return !_playerGrenade.IsAmmoFull;
                        }
                        return true;
                    }
                    else
                    {
                        if (_playerWeapon != null)
                        {
                            var weaponInstance = _playerWeapon.GetCurrentWeaponInstance();
                            if (weaponInstance != null)
                            {
                                return weaponInstance.currentReserveAmmo < weaponInstance.maxAmmo;
                            }
                        }
                        return true;
                    }
                }
                return true;

            case OrbType.Experience:
            default:
                return true;
        }
    }

    private OrbBase GetFallbackOrb()
    {
        for (int i = 0; i < _orbPrefabs.Length; i++)
        {
            if (_orbPrefabs[i] != null && IsOrbValid(_orbPrefabs[i]))
            {
                return _orbPrefabs[i];
            }
        }

        bool isShieldUnlocked = _playerHealth != null && _playerHealth.MaxShield > 0;

        for (int i = 0; i < _orbPrefabs.Length; i++)
        {
            if (_orbPrefabs[i] != null && _orbPrefabs[i].Type == OrbType.Experience)
            {
                return _orbPrefabs[i];
            }
        }

        for (int i = 0; i < _orbPrefabs.Length; i++)
        {
            if (_orbPrefabs[i] != null)
            {
                if (_orbPrefabs[i].Type == OrbType.Shield && !isShieldUnlocked)
                {
                    continue;
                }
                return _orbPrefabs[i];
            }
        }

        return null;
    }
}
