using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Stat
{
    public string statName;
    public string displayName;
    public float BaseValue;
    private List<float> additiveModifiers = new List<float>();
    private List<float> multiplicativeModifiers = new List<float>();

    public StatUpgrade upgradeParameters;

    public float Value
    {
        get
        {
            float finalValue = BaseValue;

            foreach (var add in additiveModifiers)
            {
                finalValue += add;
            }

            float multiplier = 1f;
            foreach (var mult in multiplicativeModifiers)
            {
                multiplier *= 1f + (mult / 100f);
            }
            finalValue *= multiplier;

            return finalValue;
        }
    }


    public void AddModifier(float value, ModifierType type)
    {
        if (type == ModifierType.Additive)
        {
            additiveModifiers.Add(value);
        }
        else
        {
            multiplicativeModifiers.Add(value);
        }
    }

    public void RemoveModifier(float value, ModifierType type)
    {
        if (type == ModifierType.Additive)
        {
            additiveModifiers.Remove(value);
        }
        else
        {
            multiplicativeModifiers.Remove(value);
        }
    }

    public void ClearModifiers()
    {
        additiveModifiers.Clear();
        multiplicativeModifiers.Clear();
    }
}

public enum ModifierType
{
    Additive,
    Multiplicative
}

[System.Serializable]
public class StatUpgrade
{
    public bool allowPercentageScaling = false;
    public bool allowFlatScaling = false;
    public float[] percentageScalingValues;
    public float[] flatScalingValues;

    public float GetRandomPercentageScalingValue()
    {
        if (percentageScalingValues.Length == 0)
        {
            
            return 0f;
        }

        int randomIndex = Random.Range(0, percentageScalingValues.Length);
        return percentageScalingValues[randomIndex];
    }

    public float GetRandomFlatScalingValue()
    {
        if (flatScalingValues.Length == 0)
        {
            return 0f;
        }

        int randomIndex = Random.Range(0, flatScalingValues.Length);
        return flatScalingValues[randomIndex];
    }
}