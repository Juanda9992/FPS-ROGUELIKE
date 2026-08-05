using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public List<Upgrade> currentUpgrades = new List<Upgrade>();
    [SerializeField] private int numberOfUpgradesToGenerate = 3;
    [SerializeField] private UpgradeData[] selectedUpgradeArray;

    [SerializeField] private UpgradeManagerUI upgradeManagerUI;
    [ContextMenu("Generate Upgrades")]
    public void GenerateUpgrades()
    {
        currentUpgrades.Clear();

        selectedUpgradeArray = new UpgradeData[numberOfUpgradesToGenerate];

        for (int i = 0; i < numberOfUpgradesToGenerate; i++)
        {
            Stat randomStat = PlayerStatsManager.Instance.GetRandomStat();

            bool usedFlatScaling = GenerateRandomUpgradeValue(randomStat, out float upgradeValue);

            selectedUpgradeArray[i] = new UpgradeData
            {
                targetStat = randomStat,
                upgradeValue = upgradeValue,
                usedFlatScaling = usedFlatScaling
            };

        }
        upgradeManagerUI.DisplayUpgrades(selectedUpgradeArray);
    }

    private bool GenerateRandomUpgradeValue(Stat stat, out float upgradeValue) //Returns true if flat scaling was used, false if percentage scaling was used
    {
        upgradeValue = 0f;

        if (stat.upgradeParameters.allowFlatScaling && stat.upgradeParameters.allowPercentageScaling)
        {
            // Randomly choose between flat and percentage scaling
            bool useFlatScaling = Random.value > 0.5f;
            if (useFlatScaling)
            {
                upgradeValue = stat.upgradeParameters.GetRandomFlatScalingValue();
                return true;
            }
            else
            {
                upgradeValue = stat.upgradeParameters.GetRandomPercentageScalingValue();
                return false;
            }
        }

        if (stat.upgradeParameters.allowFlatScaling)
        {
            upgradeValue = stat.upgradeParameters.GetRandomFlatScalingValue();
            return true;
        }

        if (stat.upgradeParameters.allowPercentageScaling)
        {
            upgradeValue = stat.upgradeParameters.GetRandomPercentageScalingValue();
            return false;
        }

        return false;
    }
    public void SelectUpgrade(UpgradeData upgrade)
    {   
        Debug.Log($"Upgrade selected: {upgrade.targetStat.statName} with value {upgrade.upgradeValue} (Flat Scaling: {upgrade.usedFlatScaling})");
        //upgrade.Select();
    }

    [ContextMenu("Test Upgrade values not being zero")]
    public void TestUpgradeValues()
    {
        for(int i = 0; i < 1000; i++)
        {
            Stat randomStat = PlayerStatsManager.Instance.GetRandomStat();
            bool usedFlatScaling = GenerateRandomUpgradeValue(randomStat, out float upgradeValue);
            if(upgradeValue == 0f)
            {
                Debug.LogError($"Upgrade value is zero for stat {randomStat.statName} with flat scaling: {usedFlatScaling}");
            }
        }
    }
}

// Base class for all upgrades
public abstract class Upgrade
{
    public Stat targetStat;
    public float upgradeValue;
    public bool usedFlatScaling;
    // Method to apply the upgrade
    public virtual void Select()
    {
        // Default behavior (can be overridden)
        Debug.Log("Upgrade selected");
    }
}
[System.Serializable]
public class UpgradeData
{
    public Stat targetStat;
    public float upgradeValue;
    public bool usedFlatScaling;
}
// Placeholder for future ability upgrades
public class HabilityUpgrade : Upgrade
{
    public override void Select()
    {
        // Empty for now
        Debug.Log("Hability upgrade selected");
    }
}