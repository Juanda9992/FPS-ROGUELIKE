using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmuletManager : MonoBehaviour
{
    public static PlayerAmuletManager Instance { get; private set; }


    [Header("Starting / Test Amulets")]
    [SerializeField] private List<AmuletSO> _startingAmulets = new List<AmuletSO>();

    [Header("Equipped Amulets (Runtime)")]
    [SerializeField] private List<AmuletInstance> _equippedAmulets = new List<AmuletInstance>();

    public event Action<AmuletInstance> OnAmuletEquipped;
    public event Action OnAmuletsChanged;

    public IReadOnlyList<AmuletInstance> EquippedAmulets => _equippedAmulets;
    public int EquippedCount => _equippedAmulets.Count;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {

        GameEventsManager.Instance.OnGameStarted += HandleGameStarted;
        if (GameEventsManager.Instance.IsGameStarted)
        {
            HandleGameStarted();
        }
    }

    private void OnDestroy()
    {
        GameEventsManager.Instance.OnGameStarted -= HandleGameStarted;
    }

    private void HandleGameStarted()
    {
        if (_startingAmulets != null && _startingAmulets.Count > 0)
        {
            foreach (var amuletSO in _startingAmulets)
            {
                if (amuletSO != null)
                {
                    EquipAmulet(amuletSO);
                }
            }
        }
    }

    public bool EquipAmulet(AmuletSO amuletSO)
    {
        if (amuletSO == null)
        {
            Debug.LogWarning("[PlayerAmuletManager] Cannot equip a null AmuletSO.");
            return false;
        }

        AmuletInstance instance = amuletSO.CreateInstance();
        return EquipAmulet(instance);
    }

    public bool EquipAmulet(AmuletInstance amuletInstance)
    {
        if (amuletInstance == null)
        {
            Debug.LogWarning("[PlayerAmuletManager] Cannot equip a null AmuletInstance.");
            return false;
        }

        _equippedAmulets.Add(amuletInstance);
        amuletInstance.ApplyModifiers(PlayerStatsManager.Instance);

        OnAmuletEquipped?.Invoke(amuletInstance);
        OnAmuletsChanged?.Invoke();

        Debug.Log($"[PlayerAmuletManager] Equipped amulet: {amuletInstance.Data?.DisplayName ?? "Amulet"} ({_equippedAmulets.Count})");
        return true;
    }
}
