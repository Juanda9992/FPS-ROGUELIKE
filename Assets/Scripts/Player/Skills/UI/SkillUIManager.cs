using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    [Header("UI Slots")]
    public SkillUISlot[] slots;

    [Header("Capsule Slot")]
    [SerializeField] private SkillUISlot capsuleSlot;

    private void Awake()
    {
        if (slots != null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                {
                    slots[i].SetActive(false);
                }
            }
        }

        capsuleSlot.SetActive(false);
    }

    public void Update()
    {
        if (slots != null)
        {
            foreach (var slot in slots)
            {
                if (slot != null)
                {
                    slot.UpdateCooldown();
                }
            }
        }

        capsuleSlot.UpdateCooldown();
    }

    public void TurnSkillSlotOn(int index, SkillInstance skillInstance)
    {
        if (slots == null || index < 0 || index >= slots.Length)
        {
            Debug.LogWarning($"Index {index} is out of bounds for skill slots.");
            return;
        }

        SkillUISlot slot = slots[index];
        if (slot != null)
        {
            slot.SetActive(true);
            slot.SetSkill(skillInstance);
        }
    }

    public void TriggerCooldown(int index)
    {
        if (slots == null || index < 0 || index >= slots.Length)
        {
            Debug.LogWarning($"Index {index} is out of bounds for skill slots.");
            return;
        }

        SkillUISlot slot = slots[index];
        if (slot != null)
        {
            slot.TriggerCooldown();
        }
    }

    public void SetCapsuleSlot(SkillInstance skillInstance)
    {
        capsuleSlot.SetActive(true);
        capsuleSlot.SetSkill(skillInstance);
    }

    public void TriggerCapsuleCooldown()
    {
        capsuleSlot.TriggerCooldown();
    }
}

[System.Serializable]
public class SkillUISlot
{
    public GameObject root;
    public Image icon;

    [Header("Cooldown UI")]
    public Image cooldownFill;
    private SkillInstance currentSkillInstance;

    public void SetActive(bool value)
    {
        if (root != null)
        {
            root.SetActive(value);
        }
    }

    public void SetSkill(SkillInstance instance)
    {
        currentSkillInstance = instance;

        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = 0;
        }

        if (instance == null || instance.Data == null)
        {
            if (icon != null)
            {
                icon.enabled = false;
            }
            return;
        }

        if (icon != null)
        {
            icon.sprite = instance.Data.icon;
            icon.enabled = true;
        }
    }

    public void TriggerCooldown()
    {
        if (currentSkillInstance == null) return;

        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = 1f;
        }
    }

    public void UpdateCooldown()
    {
        if (currentSkillInstance == null) return;

        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = currentSkillInstance.GetCooldownProgress();
        }
    }
}