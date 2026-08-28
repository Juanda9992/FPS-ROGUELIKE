using System;
using System.Collections.Generic;
using UnityEngine;

public class AmuletCreator : MonoBehaviour
{
    public static AmuletCreator Instance { get; private set; }

    [Header("Available Amulets")]
    [SerializeField] private List<AmuletSO> _availableAmulets = new List<AmuletSO>();

    [Header("Rarity Spawn Weights")]
    [SerializeField] private List<RarityWeight> _rarityWeights = new List<RarityWeight>
    {
        new RarityWeight(AmuletRarity.Common, 50f),
        new RarityWeight(AmuletRarity.Uncommon, 25f),
        new RarityWeight(AmuletRarity.Rare, 15f),
        new RarityWeight(AmuletRarity.Epic, 8f),
        new RarityWeight(AmuletRarity.Legendary, 2f)
    };

    public event Action<AmuletInstance> OnAmuletSelected;

    public IReadOnlyList<AmuletSO> AvailableAmulets => _availableAmulets;

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

    public AmuletInstance CreateRandomAmulet()
    {
        if (_availableAmulets == null || _availableAmulets.Count == 0)
        {
            Debug.LogWarning("[AmuletCreator] No amulets available in _availableAmulets list.");
            return null;
        }

        AmuletRarity selectedRarity = GetRandomRarity();
        List<AmuletSO> matchingAmulets = _availableAmulets.FindAll(a => a != null && a.Rarity == selectedRarity);

        AmuletSO selectedAmuletSO;

        if (matchingAmulets.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, matchingAmulets.Count);
            selectedAmuletSO = matchingAmulets[randomIndex];
        }
        else
        {
            // Fallback if no amulet exists for the rolled rarity
            int randomIndex = UnityEngine.Random.Range(0, _availableAmulets.Count);
            selectedAmuletSO = _availableAmulets[randomIndex];
        }

        return selectedAmuletSO.CreateInstance();
    }

    public List<AmuletInstance> CreateRandomAmulets(int count, bool allowDuplicates = false)
    {
        List<AmuletInstance> generatedList = new List<AmuletInstance>();

        if (_availableAmulets == null || _availableAmulets.Count == 0)
        {
            return generatedList;
        }

        List<AmuletSO> pool = new List<AmuletSO>(_availableAmulets);

        for (int i = 0; i < count; i++)
        {
            if (pool.Count == 0 && !allowDuplicates)
            {
                break;
            }

            AmuletRarity selectedRarity = GetRandomRarity();
            List<AmuletSO> matchingAmulets = pool.FindAll(a => a != null && a.Rarity == selectedRarity);

            AmuletSO selectedSO;

            if (matchingAmulets.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, matchingAmulets.Count);
                selectedSO = matchingAmulets[randomIndex];
            }
            else
            {
                int randomIndex = UnityEngine.Random.Range(0, pool.Count);
                selectedSO = pool[randomIndex];
            }

            if (selectedSO != null)
            {
                generatedList.Add(selectedSO.CreateInstance());
                if (!allowDuplicates)
                {
                    pool.Remove(selectedSO);
                }
            }
        }

        return generatedList;
    }

    public void SelectAmulet(AmuletInstance amuletInstance)
    {
        if (amuletInstance == null)
        {
            return;
        }

        PlayerAmuletManager.Instance.EquipAmulet(amuletInstance);
        OnAmuletSelected?.Invoke(amuletInstance);
        Debug.Log($"[AmuletCreator] Selected amulet: {amuletInstance.Data?.DisplayName ?? "Unknown"}");
    }

    public AmuletRarity GetRandomRarity()
    {
        float totalWeight = 0f;
        foreach (var rw in _rarityWeights)
        {
            totalWeight += rw.Weight;
        }

        if (totalWeight <= 0f)
        {
            return AmuletRarity.Common;
        }

        float randomVal = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var rw in _rarityWeights)
        {
            cumulative += rw.Weight;
            if (randomVal <= cumulative)
            {
                return rw.Rarity;
            }
        }

        return AmuletRarity.Common;
    }

    #region ContextMenu Tests
    [ContextMenu("Test Create Random Amulet")]
    private void TestCreateRandomAmulet()
    {
        AmuletInstance amulet = CreateRandomAmulet();
        if (amulet != null)
        {
            Debug.Log($"[AmuletCreator Test] Generated: {amulet.Data?.DisplayName} ({amulet.Data?.Rarity})\n{amulet.GetFormattedDescription()}");
        }
    }

    [ContextMenu("Test Select Random Amulet")]
    private void TestSelectRandomAmulet()
    {
        AmuletInstance amulet = CreateRandomAmulet();
        if (amulet != null)
        {
            SelectAmulet(amulet);
        }
    }

    [ContextMenu("Test 1000 Rolls Distribution")]
    private void TestDistribution()
    {
        Dictionary<AmuletRarity, int> counts = new Dictionary<AmuletRarity, int>();
        foreach (AmuletRarity r in Enum.GetValues(typeof(AmuletRarity)))
        {
            counts[r] = 0;
        }

        for (int i = 0; i < 1000; i++)
        {
            AmuletRarity rolled = GetRandomRarity();
            counts[rolled]++;
        }

        foreach (var pair in counts)
        {
            Debug.Log($"[Distribution 1000 Rolls] {pair.Key}: {pair.Value} ({pair.Value / 10f:F1}%)");
        }
    }
    #endregion
}

[System.Serializable]
public class RarityWeight
{
    [SerializeField] private AmuletRarity _rarity;
    [SerializeField] private float _weight;

    public AmuletRarity Rarity => _rarity;
    public float Weight => _weight;

    public RarityWeight(AmuletRarity rarity, float weight)
    {
        _rarity = rarity;
        _weight = weight;
    }
}
