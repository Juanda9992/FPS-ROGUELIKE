using UnityEngine;

public class PlayerSkillsManager : MonoBehaviour
{
    [SerializeField] private int maxSlots = 3;
    [Header("Skill Slots")]
    public SkillSlot[] slots;


    [SerializeField] private SkillUIManager skillUIManager;
    private void Awake()
    {
        slots = new SkillSlot[maxSlots];

        for (int i = 0; i < maxSlots; i++)
        {
            slots[i] = new SkillSlot();
        }
    }

    public bool CanAddItem()
    {
        foreach (var slot in slots)
        {
            if (!slot.HasSkill)
            {
                return true;
            }
        }

        return false;
    }

    public bool TryAddSkill(ActiveSkillSO skill)
    {
        if (!CanAddItem()) 
        {
            return false;
        }

        for(int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].HasSkill)
            {
                Debug.Log($"Adding skill {skill.skillName} to slot {i}");
                slots[i].SetSkill(skill);
                skillUIManager.TurnSkillSlotOn(i, skill);
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryUseSkill(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TryUseSkill(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TryUseSkill(2);
        }
    }

    void TryUseSkill(int index)
    {
        if (index < 0 || index >= slots.Length)
        {
            return;
        }

        slots[index].Use(gameObject);
    }
}

[System.Serializable]
public class SkillSlot
{
    public ActiveSkillSO skill;

    private float lastUseTime;

    public bool HasSkill => skill != null;

    public bool CanUse()
    {
        if (!HasSkill) return false;
        return Time.time >= lastUseTime + skill.cooldown;
    }

    public void Use(GameObject owner)
    {
        if (!CanUse()) return;

        skill.Activate(owner);
        lastUseTime = Time.time;
    }

    public void SetSkill(ActiveSkillSO newSkill)
    {
        skill = newSkill;
        lastUseTime = -999f;
    }

    public void Clear()
    {
        skill = null;
    }
}