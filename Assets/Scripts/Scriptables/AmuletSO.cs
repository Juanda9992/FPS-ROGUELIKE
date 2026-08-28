using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAmulet", menuName = "ScriptableObjects/Amulets/AmuletSO", order = 1)]
public class AmuletSO : ScriptableObject
{
    [Header("General Info")]
    [SerializeField] private string _amuletName = "New Amulet";
    [SerializeField] private string _displayName = "New Amulet";
    [TextArea(2, 4)]
    [SerializeField] private string _description = "";
    [SerializeField] private Sprite _icon;
    [SerializeField] private AmuletRarity _rarity = AmuletRarity.Common;

    [Header("Fixed Stat Modifiers")]
    [SerializeField] private List<StatModifierData> _statModifiers = new List<StatModifierData>();

    public string AmuletName => _amuletName;
    public string DisplayName => string.IsNullOrEmpty(_displayName) ? _amuletName : _displayName;
    public string Description => _description;
    public Sprite Icon => _icon;
    public AmuletRarity Rarity => _rarity;
    public IReadOnlyList<StatModifierData> StatModifiers => _statModifiers;

    public AmuletInstance CreateInstance()
    {
        List<StatModifierData> activeModifiers = new List<StatModifierData>();

        foreach (var modifier in _statModifiers)
        {
            activeModifiers.Add(new StatModifierData(modifier.StatName, modifier.Value, modifier.ModifierType));
        }

        return new AmuletInstance(this, activeModifiers);
    }

    public string GetFormattedDescription()
    {
        if (!string.IsNullOrEmpty(_description))
        {
            return _description;
        }

        List<string> lines = new List<string>();
        foreach (var modifier in _statModifiers)
        {
            lines.Add(modifier.GetFormattedString());
        }

        return string.Join("\n", lines);
    }
}

public enum AmuletRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class StatModifierData
{
    [SerializeField] private string _statName;
    [SerializeField] private float _value;
    [SerializeField] private ModifierType _modifierType;

    public string StatName => _statName;
    public float Value => _value;
    public ModifierType ModifierType => _modifierType;

    public StatModifierData()
    {
    }

    public StatModifierData(string statName, float value, ModifierType modifierType)
    {
        _statName = statName;
        _value = value;
        _modifierType = modifierType;
    }

    public string GetFormattedString()
    {
        string sign = _value >= 0f ? "+" : "";
        if (_modifierType == ModifierType.Additive)
        {
            return $"{sign}{_value:0.##} {_statName}";
        }
        else
        {
            return $"{sign}{_value:0.##}% {_statName}";
        }
    }
}