using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmuletInstance
{
    [SerializeField] private AmuletSO _data;
    [SerializeField] private List<StatModifierData> _modifiers = new List<StatModifierData>();

    public AmuletSO Data => _data;
    public IReadOnlyList<StatModifierData> Modifiers => _modifiers;

    public AmuletInstance(AmuletSO data, List<StatModifierData> modifiers)
    {
        _data = data;
        _modifiers = modifiers;
        if (_modifiers == null)
        {
            _modifiers = new List<StatModifierData>();
        }
    }

    public void ApplyModifiers(PlayerStatsManager statsManager)
    {
        foreach (var modifier in _modifiers)
        {
            Stat stat = statsManager.GetStatByName(modifier.StatName);
            if (stat != null)
            {
                float prevValue = stat.Value;
                stat.AddModifier(modifier.Value, modifier.ModifierType);

                if (string.Equals(modifier.StatName, "Shield", StringComparison.OrdinalIgnoreCase))
                {
                    float addedShield = stat.Value - prevValue;
                    if (addedShield > 0f && PlayerHealthController.Instance != null)
                    {
                        PlayerHealthController.Instance.RestoreShield(Mathf.RoundToInt(addedShield));
                    }
                }

                statsManager.NotifyStatUpdated(stat);
            }
        }
    }

    public void RemoveModifiers(PlayerStatsManager statsManager)
    {
        foreach (var modifier in _modifiers)
        {
            Stat stat = statsManager.GetStatByName(modifier.StatName);
            if (stat != null)
            {
                stat.RemoveModifier(modifier.Value, modifier.ModifierType);
                statsManager.NotifyStatUpdated(stat);
            }
        }
    }

    public string GetFormattedDescription()
    {
        if (_data != null && !string.IsNullOrEmpty(_data.Description))
        {
            return _data.Description;
        }

        List<string> lines = new List<string>();
        foreach (var modifier in _modifiers)
        {
            lines.Add(modifier.GetFormattedString());
        }

        return string.Join("\n", lines);
    }
}
