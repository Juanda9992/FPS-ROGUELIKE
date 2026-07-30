using UnityEngine;
using System.Collections.Generic;
public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }
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
    }
    public Stat GetStatByName(string statName)
    {
        return stats.Find(s => s.statName == statName);
    }
}