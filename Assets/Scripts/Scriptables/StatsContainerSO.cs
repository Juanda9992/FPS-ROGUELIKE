using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatsContainer", menuName = "ScriptableObjects/StatsContainerSO", order = 1)]
public class StatsContainerSO : ScriptableObject
{
    public List<Stat> stats = new List<Stat>();
}
