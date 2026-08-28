using System.Collections.Generic;
using UnityEngine;

public class PlayerStatVisualizerUI : MonoBehaviour
{
    [SerializeField] private GameObject statVisualizerPrefab;
    [SerializeField] private GameObject panelContainer;
    [SerializeField] private RectTransform statVisualizerParent;

    [SerializeField] private List<StatDisplaydataUI> statVisualizersList = new List<StatDisplaydataUI>();

    [SerializeField] private UpgradeManager upgradeManager;

    private void OnEnable()
    {
        upgradeManager.OnUpgradeSelected += UpdateStatDisplay;
        PauseManager.Instance.OnPauseChanged += OnPauseChanged;
        PlayerStatsManager.Instance.OnStatUpdated += UpdateStatDisplayByStat;
    }
    private void OnDisable()
    {
        upgradeManager.OnUpgradeSelected -= UpdateStatDisplay;
        PauseManager.Instance.OnPauseChanged -= OnPauseChanged;
        PlayerStatsManager.Instance.OnStatUpdated -= UpdateStatDisplayByStat;
    }
    private void OnPauseChanged(bool isPaused)
    {
        panelContainer.SetActive(isPaused);
    }

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
    public void UpdateStatDisplay(UpgradeData stat)
    {
        UpdateStatDisplayByStat(stat.targetStat);
    }

    public void UpdateStatDisplayByStat(Stat stat)
    {
        if (stat == null)
        {
            return;
        }

        foreach (StatDisplaydataUI statVisualizer in statVisualizersList)
        {
            if (statVisualizer.GetAttachedStat().statName == stat.statName)
            {
                statVisualizer.SetStat(stat);
                break;
            }
        }
    }
}
