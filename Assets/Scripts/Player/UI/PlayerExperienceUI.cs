using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExperienceUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerExpManager playerExpManager;
    [SerializeField] private Image expFillImage;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _expText;

    [Header("Formatting")]
    [SerializeField] private string _levelFormat = "Lvl. {0}";

    private void OnEnable()
    {
        playerExpManager.OnExpChanged += UpdateExpUI;
        playerExpManager.OnLevelUp += UpdateLevelUI;

        UpdateExpUI(playerExpManager.CurrentExp);
        UpdateLevelUI(playerExpManager.CurrentLevel);
    }

    private void OnDisable()
    {
        playerExpManager.OnExpChanged -= UpdateExpUI;
        playerExpManager.OnLevelUp -= UpdateLevelUI;
    }

    private void UpdateExpUI(int currentExp)
    {
        float fillAmount = (float)currentExp / playerExpManager.expToNextLevel;
        expFillImage.DOFillAmount(fillAmount, 0.5f).SetEase(Ease.InOutQuad);
        _expText.text = $"{currentExp}/{Mathf.RoundToInt(playerExpManager.expToNextLevel)}";
    }

    private void UpdateLevelUI(int currentLevel)
    {

        if (!string.IsNullOrEmpty(_levelFormat))
        {
            _levelText.text = string.Format(_levelFormat, currentLevel);
        }
        else
        {
            _levelText.text = currentLevel.ToString();
        }
    }

    #region Context Menu Tests
    [ContextMenu("Test Update Level 5")]
    private void TestUpdateLevel()
    {
        UpdateLevelUI(5);
    }
    #endregion
}
