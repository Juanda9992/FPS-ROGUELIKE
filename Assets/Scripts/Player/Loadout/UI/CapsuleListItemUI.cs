using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CapsuleListItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _capsuleNameText;
    [SerializeField] private Button _itemButton;
    [SerializeField] private Image _backgroundImage;

    [Header("Visual Colors")]
    [SerializeField] private Color _normalColor = new Color(0.18f, 0.18f, 0.22f, 0.9f);
    [SerializeField] private Color _selectedColor = new Color(0.2f, 0.75f, 0.45f, 1f);
    [SerializeField] private Color _hoverColor = new Color(0.3f, 0.35f, 0.45f, 1f);

    private SpawnObjectSkill _attachedCapsule;
    private Action<CapsuleListItemUI, SpawnObjectSkill> _onSelectedCallback;
    private Action<SpawnObjectSkill, RectTransform> _onHoverEnterCallback;
    private Action<SpawnObjectSkill> _onHoverExitCallback;
    private bool _isSelected;

    public SpawnObjectSkill AttachedCapsule => _attachedCapsule;
    public RectTransform RectTransform => transform as RectTransform;

    private void Awake()
    {
        _itemButton.onClick.AddListener(OnItemClicked);
    }

    private void OnDestroy()
    {
        _itemButton.onClick.RemoveListener(OnItemClicked);
    }

    public void SetCapsuleData(
        SpawnObjectSkill capsule,
        Action<CapsuleListItemUI, SpawnObjectSkill> onSelectedCallback,
        Action<SpawnObjectSkill, RectTransform> onHoverEnterCallback,
        Action<SpawnObjectSkill> onHoverExitCallback,
        bool isSelected = false)
    {
        _attachedCapsule = capsule;
        _onSelectedCallback = onSelectedCallback;
        _onHoverEnterCallback = onHoverEnterCallback;
        _onHoverExitCallback = onHoverExitCallback;

        string displayName = !string.IsNullOrEmpty(capsule.skillName) ? capsule.skillName : capsule.name;
        _capsuleNameText.text = displayName;

        SetSelected(isSelected);
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        UpdateVisuals();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isSelected && _backgroundImage != null)
        {
            _backgroundImage.color = _hoverColor;
        }

        _onHoverEnterCallback?.Invoke(_attachedCapsule, RectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateVisuals();
        _onHoverExitCallback?.Invoke(_attachedCapsule);
    }

    private void OnItemClicked()
    {
        _onSelectedCallback?.Invoke(this, _attachedCapsule);
    }

    private void UpdateVisuals()
    {
        _backgroundImage.color = _isSelected ? _selectedColor : _normalColor;
    }
}
