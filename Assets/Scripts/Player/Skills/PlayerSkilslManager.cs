using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillsManager : MonoBehaviour, IPausable
{
    [Header("Settings")]
    [SerializeField] private Stat _skillCooldownMultiplierStat;
    [SerializeField] private int _maxSlots = 3;

    [Header("Skill Slots")]
    [SerializeField] private SkillSlot[] _slots;
    [SerializeField] private SkillUIManager _skillUIManager;

    private PlayerInputActions _input;
    private bool _isGameStarted;

    public SkillSlot[] Slots => _slots;

    private void Awake()
    {
        _slots = new SkillSlot[_maxSlots];

        for (int i = 0; i < _maxSlots; i++)
        {
            _slots[i] = new SkillSlot();
        }

        _input = new PlayerInputActions();
        _input.Player.Skill1.performed += _ => TryUseSkill(0);
        _input.Player.Skill2.performed += _ => TryUseSkill(1);
        _input.Player.Skill3.performed += _ => TryUseSkill(2);
    }

    private void OnEnable()
    {
        PauseManager.Instance.OnPauseChanged += OnPauseChanged;
        _input.Enable();
    }

    private void OnDisable()
    {
        PauseManager.Instance.OnPauseChanged -= OnPauseChanged;
        _input.Disable();
    }

    private void Start()
    {
        GameEventsManager.Instance.OnGameStarted += HandleGameStarted;
        if (GameEventsManager.Instance.IsGameStarted)
        {
            HandleGameStarted();
        }
    }

    private void OnDestroy()
    {
        GameEventsManager.Instance.OnGameStarted -= HandleGameStarted;
        _input.Dispose();
    }

    #region Pause And Resume Methods
    private void OnPauseChanged(bool isPaused)
    {
        if (isPaused)
        {
            OnPause();
        }
        else
        {
            OnResume();
        }
    }

    public void OnPause()
    {
        if (_input != null)
        {
            _input.Player.Disable();
        }
    }

    public void OnResume()
    {
        if (_input != null)
        {
            _input.Player.Enable();
        }
    }
    #endregion

    private void HandleGameStarted()
    {
        _isGameStarted = true;
        _skillCooldownMultiplierStat = PlayerStatsManager.Instance.GetStatByName("CooldownMultiplier");
    }

    public bool CanAddItem()
    {
        if (_slots == null)
        {
            return false;
        }

        foreach (var slot in _slots)
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

        for (int i = 0; i < _slots.Length; i++)
        {
            if (!_slots[i].HasSkill)
            {
                Debug.Log($"Adding skill {instance.Data.skillName} to slot {i}");
                _slots[i].SetSkill(instance);
                if (_skillUIManager != null)
                {
                    _skillUIManager.TurnSkillSlotOn(i, instance);
                }
                return true;
            }
        }
        return false;
    }

    public void TryUseSkill(int index)
    {
        if (!_isGameStarted)
        {
            return;
        }

        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused)
        {
            return;
        }

        if (_slots == null || index < 0 || index >= _slots.Length)
        {
            return;
        }

        if (!_slots[index].CanUse())
        {
            Debug.Log($"Skill in slot {index} is on cooldown or not assigned.");
            return;
        }

        if (_slots[index].Use(gameObject))
        {
            if (_skillUIManager != null)
            {
                _skillUIManager.TriggerCooldown(index);
            }
        }
    }
}

[System.Serializable]
public class SkillSlot
{
    [SerializeField] private SkillInstance _skillInstance;

    public SkillInstance SkillInstance => _skillInstance;
    public bool HasSkill => _skillInstance != null && _skillInstance.Data != null;

    public bool CanUse()
    {
        if (!HasSkill)
        {
            return false;
        }
        return !_skillInstance.IsOnCooldown();
    }

    public bool Use(GameObject owner)
    {
        if (!CanUse())
        {
            return false;
        }

        return _skillInstance.TryActivate(owner);
    }

    public void SetSkill(SkillInstance newInstance)
    {
        _skillInstance = newInstance;
    }

    public void Clear()
    {
        _skillInstance = null;
    }
}