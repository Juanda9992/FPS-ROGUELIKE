using System.Collections.Generic;
using UnityEngine;

public class PlayerStatVisualizerUI : MonoBehaviour
{
    [SerializeField] private GameObject statVisualizerPrefab;
    [SerializeField] private RectTransform statVisualizerParent;

    [SerializeField] private List<StatDisplaydataUI> statVisualizersList = new List<StatDisplaydataUI>();

    [SerializeField] private UpgradeManager upgradeManager;
    
    public void CreateStatsOnPanel(List<Stat> stats)
    {
        foreach (var stat in stats)
        {
            GameObject statVisualizerGO = Instantiate(statVisualizerPrefab, statVisualizerParent);
            StatDisplaydataUI statVisualizer = statVisualizerGO.GetComponent<StatDisplaydataUI>();
            statVisualizersList.Add(statVisualizer);
            statVisualizer.SetStat(stat);
        }
    }

    private System.Collections.IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        CursorManager.SetCursorVisible(true);
    }

    public void UpdateStatDisplay(UpgradeData stat)
    {
        foreach (StatDisplaydataUI statVisualizer in statVisualizersList)
        {
            if (statVisualizer.GetAttachedStat().statName == stat.targetStat.statName)
            {
                statVisualizer.SetStat(stat.targetStat);
                break;
            }
        }
    }
    void OnEnable()
    {
        upgradeManager.OnUpgradeSelected += UpdateStatDisplay;
    }

    void OnDisable()
    {
        upgradeManager.OnUpgradeSelected -= UpdateStatDisplay;
    }
}
