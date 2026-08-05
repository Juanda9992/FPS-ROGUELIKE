using UnityEngine;
using TMPro;
public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI upgradeValueText;

    private UpgradeData attachedUpgradeData;
    public void SetUpgradeData(UpgradeData upgrade)
    {
        attachedUpgradeData = upgrade;
        statNameText.text = upgrade.targetStat.displayName;
        upgradeValueText.text = FormatUpgradeValue(upgrade.upgradeValue, upgrade.usedFlatScaling);
    }

    private string FormatUpgradeValue(float value, bool usedFlatScaling)
    {
        if (usedFlatScaling)
        {
            return $"+{value.ToString("0.##")}";
        }
        else
        {
            return $"{value.ToString("0.##")}%";
        }
    }

    public void OnCardSelected()
    {
        UpgradeManagerUI.Instance.OnUpgradeCardSelected(attachedUpgradeData);
    }


}
