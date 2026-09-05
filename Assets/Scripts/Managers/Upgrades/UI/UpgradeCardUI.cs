using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI upgradeValueText;

    [Header("Visual Elements")]
    [SerializeField] private Image cardBackgroundImage;
    [SerializeField] private Button cardButton;

    [Header("Selection Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.25f, 0.75f, 0.45f, 1f);

    private UpgradeData attachedUpgradeData;

    public void SetUpgradeData(UpgradeData upgrade)
    {
        attachedUpgradeData = upgrade;
        statNameText.text = upgrade.targetStat.displayName;
        upgradeValueText.text = FormatUpgradeValue(upgrade.upgradeValue, upgrade.usedFlatScaling);
        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        Color targetColor = isSelected ? selectedColor : normalColor;

        if (cardBackgroundImage != null)
        {
            cardBackgroundImage.color = targetColor;
        }

        if (cardButton != null)
        {
            ColorBlock colors = cardButton.colors;
            colors.normalColor = targetColor;
            colors.selectedColor = targetColor;
            colors.highlightedColor = isSelected ? selectedColor * 1.1f : normalColor * 0.95f;
            cardButton.colors = colors;
        }
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
        UpgradeManagerUI.Instance.OnUpgradeCardSelected(this, attachedUpgradeData);
    }
}
