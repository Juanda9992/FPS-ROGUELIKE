using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI upgradeValueText;

    public void SetUpgradeData(UpgradeData upgrade)
    {
        statNameText.text = upgrade.targetStat.displayName;
        upgradeValueText.text = upgrade.upgradeValue.ToString("F2");
    }
}
