using UnityEngine;
using System.Collections.Generic;
public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }
    [SerializeField] private StatsContainerSO statsContainer;

    [SerializeField] private List<Stat> stats = new List<Stat>();

    [SerializeField] private PlayerStatVisualizerUI statVisualizerUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        CopyStatsFromContainer();

        statVisualizerUI.CreateStatsOnPanel(stats);
    }
    public Stat GetStatByName(string statName)
    {
        return stats.Find(s => string.Equals(s.statName, statName, System.StringComparison.OrdinalIgnoreCase));
    }

    private void CopyStatsFromContainer()
    {
        stats.Clear();
        foreach (var stat in statsContainer.stats)
        {
            Stat newStat = new Stat
            {
                statName = stat.statName,
                displayName = stat.displayName,
                BaseValue = stat.BaseValue,
                upgradeParameters = stat.upgradeParameters
            };
            stats.Add(newStat);
        }
    }

    public event System.Action<Stat> OnStatUpdated;

    public IReadOnlyList<Stat> AllStats => stats;

    public void NotifyStatUpdated(Stat stat)
    {
        if (stat != null)
        {
            OnStatUpdated?.Invoke(stat);
        }
    }

    public void NotifyAllStatsUpdated()
    {
        foreach (var stat in stats)
        {
            OnStatUpdated?.Invoke(stat);
        }
    }

    public Stat GetRandomStat()
    {
        if (stats.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, stats.Count);
        return stats[randomIndex];
    }
}