using UnityEngine;
using System.Collections.Generic;
public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }
    [SerializeField] private StatsContainerSO statsContainer;

    [SerializeField] private List<Stat> stats = new List<Stat>();

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
    }
    public Stat GetStatByName(string statName)
    {
        return stats.Find(s => s.statName == statName);
    }

    private void CopyStatsFromContainer()
    {
        stats.Clear();
        foreach (var stat in statsContainer.stats)
        {
            Stat newStat = new Stat
            {
                statName = stat.statName,
                BaseValue = stat.BaseValue,
                upgradeParameters = stat.upgradeParameters
            };
            stats.Add(newStat);
        }
    }
}