using System.Collections.Generic;
[System.Serializable]
public class Stat
{
    public string statName;
    public float BaseValue;
    private List<float> additiveModifiers = new List<float>();
    private List<float> multiplicativeModifiers = new List<float>();

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
                multiplier += mult;
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