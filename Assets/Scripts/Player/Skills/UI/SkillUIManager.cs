using UnityEngine;
using UnityEngine.UI;
public class SkillUIManager : MonoBehaviour
{
    [Header("UI Slots")]
    public SkillUISlot[] slots;

    private void Awake()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetActive(false);
        }
    }

    public void Update()
    {
        foreach (var slot in slots)
        {
            slot.UpdateCooldown();
        }
    }
    public void TurnSkillSlotOn(int index, ActiveSkillSO skill)
    {
        if (index < 0 || index >= slots.Length)
        {
            Debug.LogWarning($"Index {index} is out of bounds for skill slots.");
            return;
        }

        SkillUISlot slot = slots[index];

        slot.SetActive(true);
        slot.SetSkill(skill);
    }

    public void TriggerCooldown(int index)
    {
        if (index < 0 || index >= slots.Length)
        {
            Debug.LogWarning($"Index {index} is out of bounds for skill slots.");
            return;
        }

        SkillUISlot slot = slots[index];
        slot.TriggerCooldown();
    }
}

[System.Serializable]
public class SkillUISlot
{
    public GameObject root;
    public Image icon;

    [Header("Cooldown UI")]
    public Image cooldownFill;
    private ActiveSkillSO currentSkill;
    private float lastUseTime;

    public void SetActive(bool value)
    {
        root.SetActive(value);
    }

    public void SetSkill(ActiveSkillSO skill)
    {
        currentSkill = skill;

        cooldownFill.fillAmount = 0;
        if (skill == null)
        {
            icon.enabled = false;
            return;
        }

        icon.sprite = skill.icon;
        icon.enabled = true;
    }

    public void TriggerCooldown()
    {
        if (currentSkill == null) 
        {
            return;
        }

        lastUseTime = Time.time;
        cooldownFill.fillAmount = 1f;
    }

    public void UpdateCooldown()
    {
        if (currentSkill == null) 
        {
            return;
        }

        float elapsed = Time.time - lastUseTime;
        float cd = GetEffectiveCooldown(currentSkill);

        if (elapsed >= cd)
        {
            cooldownFill.fillAmount = 0;
            return;
        }

        float normalized = 1f - (elapsed / cd);
        cooldownFill.fillAmount = normalized;
    }

    private float GetEffectiveCooldown(ActiveSkillSO skill)
    {
        if (skill == null)
        {
            return 0f;
        }

        var cooldownMultiplierStat = PlayerStatsManager.Instance.GetStatByName("CooldownMultiplier");

        float multiplier = cooldownMultiplierStat.Value;

        multiplier = Mathf.Max(multiplier, 0.0001f);

        //Debug.Log($"Real Cooldown: {skill.cooldown / multiplier}");
        return skill.cooldown / multiplier;
    }
}