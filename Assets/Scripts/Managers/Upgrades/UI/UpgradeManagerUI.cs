using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private UpgradeData _selectedUpgrade;
    private UpgradeCardUI _selectedCardUI;
    private readonly List<UpgradeCardUI> _activeCardUIs = new List<UpgradeCardUI>();

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
        ClearSelection();
        ClearUpgradeCards();

        upgradePanel.SetActive(true);
        CursorManager.SetCursorVisible(true);
        Time.timeScale = 0f;

        foreach (var upgrade in upgrades)
        {
            GameObject card = Instantiate(upgradeCardPrefab, upgradeCardContainer);
            UpgradeCardUI cardUI = card.GetComponent<UpgradeCardUI>();
            cardUI.SetUpgradeData(upgrade);
            _activeCardUIs.Add(cardUI);
        }
    }

    private void ClearUpgradeCards()
    {
        _activeCardUIs.Clear();
        foreach (Transform child in upgradeCardContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void ClearSelection()
    {
        _selectedUpgrade = null;
        if (_selectedCardUI != null)
        {
            _selectedCardUI.SetSelected(false);
            _selectedCardUI = null;
        }

        foreach (var cardUI in _activeCardUIs)
        {
            if (cardUI != null)
            {
                cardUI.SetSelected(false);
            }
        }

        upgradeButton.gameObject.SetActive(false);
    }

    public void OnUpgradeCardSelected(UpgradeCardUI cardUI, UpgradeData upgrade)
    {
        if (_selectedCardUI != null && _selectedCardUI != cardUI)
        {
            _selectedCardUI.SetSelected(false);
        }

        _selectedCardUI = cardUI;
        _selectedUpgrade = upgrade;

        _selectedCardUI.SetSelected(true);

        upgradeButton.gameObject.SetActive(true);
    }

    private void OnUpgradeButtonClicked()
    {
        if (_selectedUpgrade != null)
        {
            upgradeManager.SelectUpgrade(_selectedUpgrade);
            ClearSelection();
            ClearUpgradeCards();
            CursorManager.SetCursorVisible(false);
            upgradePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void OnRefreshUpgradesButtonClicked()
    {
        ClearSelection();
        upgradeManager.RefreshUpgrades();
    }

    public void UpdateRefreshAmmountText(int currentRerolls, int maxRerolls)
    {
        refreshAmmountText.text = $"Refreshes: {currentRerolls}/{maxRerolls}";
        refreshUpgradesButton.interactable = currentRerolls > 0;
    }
}
