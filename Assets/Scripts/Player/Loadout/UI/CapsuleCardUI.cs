using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CapsuleCardUI : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI _capsuleNameText;
    [SerializeField] private TextMeshProUGUI _cooldownText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [Header("Visual Elements")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _cardBackgroundImage;
    [SerializeField] private Button _cardButton;

    [Header("Selection Colors")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _selectedColor = new Color(0.25f, 0.75f, 0.45f, 1f);

    private SpawnObjectSkill _attachedCapsule;
    private Action<CapsuleCardUI, SpawnObjectSkill> _onSelectedCallback;

    public SpawnObjectSkill AttachedCapsule => _attachedCapsule;

    private void Awake()
    {
        _cardButton.onClick.AddListener(OnCardClicked);
    }

    private void OnDestroy()
    {
        _cardButton.onClick.RemoveListener(OnCardClicked);
    }

    public void SetCapsuleData(SpawnObjectSkill capsule, Action<CapsuleCardUI, SpawnObjectSkill> onSelectedCallback, bool isSelected = false)
    {
        _attachedCapsule = capsule;
        _onSelectedCallback = onSelectedCallback;

        _capsuleNameText.text = capsule.skillName;

        _cooldownText.text = $"Cooldown: {capsule.cooldown:0.#}s";

        _descriptionText.text = !string.IsNullOrEmpty(capsule.description)
            ? capsule.description
            : $"Launches a {capsule.skillName} area effect.";

        if (capsule.icon != null)
        {
            _iconImage.sprite = capsule.icon;
            _iconImage.enabled = true;
        }
        else
        {
            _iconImage.enabled = false;
        }

        SetSelected(isSelected);
    }

    public void SetSelected(bool isSelected)
    {
        Color targetColor = isSelected ? _selectedColor : _normalColor;

        _cardBackgroundImage.color = targetColor;

        ColorBlock colors = _cardButton.colors;
        colors.normalColor = targetColor;
        colors.selectedColor = targetColor;
        colors.highlightedColor = isSelected ? _selectedColor * 1.1f : _normalColor * 0.95f;
        _cardButton.colors = colors;
    }

    public void OnCardClicked()
    {
        _onSelectedCallback?.Invoke(this, _attachedCapsule);
    }
}
