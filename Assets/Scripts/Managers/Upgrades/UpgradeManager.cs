using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public List<Upgrade> currentUpgrades = new List<Upgrade>();
    [SerializeField] private int numberOfUpgradesToGenerate = 3;
    [SerializeField] private UpgradeData[] selectedUpgradeArray;

    [SerializeField] private UpgradeManagerUI upgradeManagerUI;
    [SerializeField] private PlayerExpManager playerExpManager;

    public event System.Action<UpgradeData> OnUpgradeSelected;

    [SerializeField] private int maxRerolls = 3;
    private int currentRerolls = 0;
    [ContextMenu("Generate Upgrades and Rerolls")]
    public void GenerateUpgradesAndRerolls()
    {
        currentRerolls = maxRerolls;
        GenerateUpgrades();
        upgradeManagerUI.UpdateRefreshAmmountText(currentRerolls, maxRerolls);
    }
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
        upgrade.Select();
        currentUpgrades.Add(upgrade);
        OnUpgradeSelected?.Invoke(upgrade);
    }

    [ContextMenu("Test Upgrade values not being zero")]
    public void TestUpgradeValues()
    {
        for (int i = 0; i < 1000; i++)
        {
            Stat randomStat = PlayerStatsManager.Instance.GetRandomStat();
            bool usedFlatScaling = GenerateRandomUpgradeValue(randomStat, out float upgradeValue);
            if (upgradeValue == 0f)
            {
                Debug.LogError($"Upgrade value is zero for stat {randomStat.statName} with flat scaling: {usedFlatScaling}");
            }
        }
    }

    public void RefreshUpgrades()
    {
        if (currentRerolls > 0)
        {
            currentRerolls--;
            GenerateUpgrades();
        }
        else
        {
            Debug.Log("No rerolls left.");
        }
        upgradeManagerUI.UpdateRefreshAmmountText(currentRerolls, maxRerolls);
    }

    void OnEnable()
    {
        playerExpManager.OnLevelUp+=_=> GenerateUpgradesAndRerolls();
    }
    void OnDisable()
    {
        playerExpManager.OnLevelUp-=_=> GenerateUpgradesAndRerolls();
    }
}

// Base class for all upgrades
public abstract class Upgrade
{
    // Method to apply the upgrade
    public virtual void Select()
    {
        // Default behavior (can be overridden)
        Debug.Log("Upgrade selected");
    }
}
[System.Serializable]
public class UpgradeData : Upgrade
{
    public Stat targetStat;
    public float upgradeValue;
    public bool usedFlatScaling;

    public override void Select()
    {

        if (usedFlatScaling)
        {
            targetStat.AddModifier(upgradeValue, ModifierType.Additive);
        }
        else
        {
            targetStat.AddModifier(upgradeValue, ModifierType.Multiplicative);
        }
        Debug.Log($"Applied upgrade: {upgradeValue} to stat: {targetStat.statName} using {(usedFlatScaling ? "flat" : "percentage")} scaling.");

    }
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