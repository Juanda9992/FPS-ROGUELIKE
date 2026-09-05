using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutPanelUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject _panelRoot;

    [Header("Capsule Slot")]
    [SerializeField] private CapsuleSlotUI _capsuleSlotUI;

    [Header("Capsule Scroll List")]
    [SerializeField] private GameObject _selectionListRoot;
    [SerializeField] private RectTransform _capsuleListContainer;
    [SerializeField] private GameObject _capsuleListItemPrefab;

    [Header("Hover Stats Tooltip")]
    [SerializeField] private CapsuleStatsTooltipUI _statsTooltipUI;

    [Header("Navigation Buttons")]
    [SerializeField] private Button _backButton;

    [Header("Managers")]
    [SerializeField] private LoadoutManager _loadoutManager;

    private readonly List<CapsuleListItemUI> _activeListItems = new List<CapsuleListItemUI>();
    private CapsuleListItemUI _selectedItemUI;
    private bool _isListOpen;

    private void Awake()
    {
        _panelRoot.SetActive(false);
        _selectionListRoot.SetActive(false);
        _backButton.onClick.AddListener(ClosePanel);
        _capsuleSlotUI.OnSlotClicked += ToggleSelectionList;
    }

    private void OnDestroy()
    {
        _backButton.onClick.RemoveListener(ClosePanel);
        _capsuleSlotUI.OnSlotClicked -= ToggleSelectionList;
    }

    public void OpenPanel()
    {
        _panelRoot.SetActive(true);
        CloseSelectionList();
        UpdateCapsuleSlotView();
    }

    public void ClosePanel()
    {
        CloseSelectionList();

        _panelRoot.SetActive(false);
    }

    public void ToggleSelectionList()
    {
        if (_isListOpen)
        {
            CloseSelectionList();
        }
        else
        {
            OpenSelectionList();
        }
    }

    public void OpenSelectionList()
    {
        _isListOpen = true;
        _selectionListRoot.SetActive(true);

        _capsuleSlotUI.SetHighlighted(true);

        PopulateList();
    }

    public void CloseSelectionList()
    {
        _isListOpen = false;

        _selectionListRoot.SetActive(false);

        _capsuleSlotUI.SetHighlighted(false);

        _statsTooltipUI.Hide();
    }

    public void PopulateList()
    {
        ClearList();

        SpawnObjectSkill[] capsules = _loadoutManager.AvailableCapsules;
        if (capsules == null || capsules.Length == 0)
        {
            return;
        }

        SpawnObjectSkill currentSelected = _loadoutManager.SelectedCapsule;

        foreach (var capsule in capsules)
        {
            if (capsule == null)
            {
                continue;
            }

            GameObject itemGO = Instantiate(_capsuleListItemPrefab, _capsuleListContainer);
            if (itemGO.TryGetComponent<CapsuleListItemUI>(out var itemUI))
            {
                bool isSelected = (currentSelected != null && currentSelected == capsule);
                itemUI.SetCapsuleData(capsule, HandleItemSelected, HandleItemHoverEnter, HandleItemHoverExit, isSelected);
                _activeListItems.Add(itemUI);

                if (isSelected)
                {
                    _selectedItemUI = itemUI;
                }
            }
        }
    }

    private void ClearList()
    {
        _activeListItems.Clear();
        _selectedItemUI = null;

        if (_capsuleListContainer != null)
        {
            foreach (Transform child in _capsuleListContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void HandleItemSelected(CapsuleListItemUI itemUI, SpawnObjectSkill capsule)
    {
        if (_selectedItemUI != null && _selectedItemUI != itemUI)
        {
            _selectedItemUI.SetSelected(false);
        }

        _selectedItemUI = itemUI;
        if (_selectedItemUI != null)
        {
            _selectedItemUI.SetSelected(true);
        }

        CloseSelectionList();

        _loadoutManager.SelectCapsule(capsule);
        UpdateCapsuleSlotView();
    }

    private void HandleItemHoverEnter(SpawnObjectSkill capsule, RectTransform itemRect)
    {
        _statsTooltipUI.Show(capsule, itemRect);
    }

    private void HandleItemHoverExit(SpawnObjectSkill capsule)
    {
        _statsTooltipUI.Hide();
    }

    private void UpdateCapsuleSlotView()
    {
        _capsuleSlotUI.UpdateSlot(_loadoutManager.SelectedCapsule);
    }
}
