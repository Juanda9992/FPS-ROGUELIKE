using UnityEngine;

[System.Serializable]
public class SkillInstance
{
    public ActiveSkillSO Data { get; private set; }
    public int Level { get; private set; } = 1;
    public float LastUseTime { get; private set; } = -999f;

    public SkillInstance(ActiveSkillSO data)
    {
        Data = data;
        Level = 1;
        LastUseTime = -999f;
    }

    public float GetEffectiveCooldown()
    {
        Stat cooldownMultiplierStat = PlayerStatsManager.Instance.GetStatByName("CooldownMultiplier");
        return Data.cooldown / cooldownMultiplierStat.Value;
    }

    public float GetEffectiveDamage()
    {
        Stat damageMultiplierStat = PlayerStatsManager.Instance.GetStatByName("DamageMultiplier");

        float mult = damageMultiplierStat.Value;

        return Data.baseDamage * mult;
    }

    public bool IsOnCooldown()
    {
        return Time.time < LastUseTime + GetEffectiveCooldown();
    }

    public float GetCooldownProgress()
    {
        if (Data == null || !IsOnCooldown())
        {
            return 0f;
        }

        float cd = GetEffectiveCooldown();
        if (cd <= 0f)
        {
            return 0f;
        }

        float elapsed = Time.time - LastUseTime;
        return Mathf.Clamp01(1f - (elapsed / cd));
    }

    public bool TryActivate(GameObject owner)
    {
        if (Data == null || IsOnCooldown())
        {
            return false;
        }

        LastUseTime = Time.time;
        Data.Activate(owner, this);
        return true;
    }
}
