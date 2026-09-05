using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CapsuleSlotUI : MonoBehaviour
{
    [Header("Slot Title & Details")]
    [SerializeField] private TextMeshProUGUI _slotTitleText;
    [SerializeField] private TextMeshProUGUI _selectedCapsuleNameText;
    [SerializeField] private Image _selectedCapsuleIcon;

    [Header("Button & Visuals")]
    [SerializeField] private Button _slotButton;
    [SerializeField] private Image _slotBackgroundImage;
    [SerializeField] private Color _normalColor = new Color(0.22f, 0.22f, 0.28f, 1f);
    [SerializeField] private Color _activeColor = new Color(0.35f, 0.5f, 0.75f, 1f);

    public event Action OnSlotClicked;

    private void Awake()
    {
        _slotButton.onClick.AddListener(HandleSlotClicked);

        if (_slotTitleText != null && string.IsNullOrEmpty(_slotTitleText.text))
        {
            _slotTitleText.text = "Capsule";
        }
    }

    private void OnDestroy()
    {
        _slotButton.onClick.RemoveListener(HandleSlotClicked);
    }

    public void UpdateSlot(SpawnObjectSkill selectedCapsule)
    {
        if (selectedCapsule != null)
        {
            string displayName = !string.IsNullOrEmpty(selectedCapsule.skillName)
                ? selectedCapsule.skillName
                : selectedCapsule.name;

            _selectedCapsuleNameText.text = displayName;

            if (selectedCapsule.icon != null)
            {
                _selectedCapsuleIcon.sprite = selectedCapsule.icon;
                _selectedCapsuleIcon.enabled = true;
            }
            else
            {
                _selectedCapsuleIcon.enabled = false;
            }
        }
        else
        {
            _selectedCapsuleNameText.text = "None";
            _selectedCapsuleIcon.enabled = false;
        }
    }

    public void SetHighlighted(bool isHighlighted)
    {
        _slotBackgroundImage.color = isHighlighted ? _activeColor : _normalColor;
    }

    private void HandleSlotClicked()
    {
        OnSlotClicked?.Invoke();
    }
}
