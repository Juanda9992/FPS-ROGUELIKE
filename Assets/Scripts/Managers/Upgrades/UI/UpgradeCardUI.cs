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
        upgradeValueText.text = upgrade.upgradeValue.ToString("F2");
    }

    public void OnCardSelected()
    {
        UpgradeManagerUI.Instance.OnUpgradeCardSelected(attachedUpgradeData);
    }


}
