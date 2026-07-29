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
}


[System.Serializable]
public class SkillUISlot
{
    public GameObject root;
    public Image icon;

    public void SetActive(bool value)
    {
        root.SetActive(value);
    }

    public void SetSkill(ActiveSkillSO skill)
    {
        if (skill == null)
        {
            icon.enabled = false;
            return;
        }

        icon.sprite = skill.icon;
        icon.enabled = true;
    }
}