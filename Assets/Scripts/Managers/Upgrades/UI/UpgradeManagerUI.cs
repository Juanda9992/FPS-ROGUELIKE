using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpgradeManagerUI : MonoBehaviour
{
    public static UpgradeManagerUI Instance { get; private set; }
    [Header("Upgrade UI Elements")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject upgradeCardPrefab;
    [SerializeField] private RectTransform upgradeCardContainer;

    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button selectUpgradeButton;

    [Header("Refresh Upgrades Logic")]
    [SerializeField] private Button refreshUpgradesButton;
    [SerializeField] private TextMeshProUGUI refreshAmmountText;
    [SerializeField] private UpgradeManager upgradeManager;
    private UpgradeData selectedUpgrade;
    private void Awake()
    {
        upgradeButton.gameObject.SetActive(false);
        upgradePanel.SetActive(false);

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        selectUpgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        refreshUpgradesButton.onClick.AddListener(OnRefreshUpgradesButtonClicked);
    }
    public void DisplayUpgrades(UpgradeData[] upgrades)
    {
        ClearUpgradeCards();

        upgradePanel.SetActive(true);
        CursorManager.SetCursorVisible(true);
        Time.timeScale = 0f;

        foreach (var upgrade in upgrades)
        {
            GameObject card = Instantiate(upgradeCardPrefab, upgradeCardContainer);
            UpgradeCardUI cardUI = card.GetComponent<UpgradeCardUI>();
            cardUI.SetUpgradeData(upgrade);
        }
    }

    private void ClearUpgradeCards()
    {
        foreach (Transform child in upgradeCardContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnUpgradeCardSelected(UpgradeData upgrade)
    {
        selectedUpgrade = upgrade;
        upgradeButton.gameObject.SetActive(true);
    }

    private void OnUpgradeButtonClicked()
    {
        if (selectedUpgrade != null)
        {
            upgradeManager.SelectUpgrade(selectedUpgrade);
            ClearUpgradeCards();
            upgradeButton.gameObject.SetActive(false);
            CursorManager.SetCursorVisible(false);
            upgradePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void OnRefreshUpgradesButtonClicked()
    {
        upgradeManager.RefreshUpgrades();
    }

    public void UpdateRefreshAmmountText(int currentRerolls, int maxRerolls)
    {
        refreshAmmountText.text = $"Refreshes: {currentRerolls}/{maxRerolls}";
        refreshUpgradesButton.interactable = currentRerolls > 0;
    }
}
