using UnityEngine;

public class PlayerSkillsManager : MonoBehaviour
{
    [SerializeField] private Stat skillCooldownMultiplierStat;
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

    private bool _isGameStarted;

    private void Start()
    {
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.OnGameStarted += HandleGameStarted;
            if (GameEventsManager.Instance.IsGameStarted)
            {
                HandleGameStarted();
            }
        }
        else
        {
            HandleGameStarted();
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.OnGameStarted -= HandleGameStarted;
        }
    }

    private void HandleGameStarted()
    {
        _isGameStarted = true;
        skillCooldownMultiplierStat = PlayerStatsManager.Instance.GetStatByName("CooldownMultiplier");
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
        if (skill == null || !CanAddItem())
        {
            return false;
        }

        SkillInstance instance = skill.CreateInstance();
        return TryAddSkillInstance(instance);
    }

    public bool TryAddSkillInstance(SkillInstance instance)
    {
        if (instance == null || !CanAddItem())
        {
            return false;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].HasSkill)
            {
                Debug.Log($"Adding skill {instance.Data.skillName} to slot {i}");
                slots[i].SetSkill(instance);
                if (skillUIManager != null)
                {
                    skillUIManager.TurnSkillSlotOn(i, instance);
                }
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (!_isGameStarted)
        {
            return;
        }
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

    private void TryUseSkill(int index)
    {
        if (slots == null || index < 0 || index >= slots.Length)
        {
            return;
        }

        if (!slots[index].CanUse())
        {
            Debug.Log($"Skill in slot {index} is on cooldown or not assigned.");
            return;
        }

        if (slots[index].Use(gameObject))
        {
            if (skillUIManager != null)
            {
                skillUIManager.TriggerCooldown(index);
            }
        }
    }
}

[System.Serializable]
public class SkillSlot
{
    public SkillInstance skillInstance;

    public bool HasSkill => skillInstance != null && skillInstance.Data != null;

    public bool CanUse()
    {
        if (!HasSkill)
        {
            return false;
        }
        return !skillInstance.IsOnCooldown();
    }

    public bool Use(GameObject owner)
    {
        if (!CanUse())
        {
            return false;
        }

        return skillInstance.TryActivate(owner);
    }

    public void SetSkill(SkillInstance newInstance)
    {
        skillInstance = newInstance;
    }

    public void Clear()
    {
        skillInstance = null;
    }
}