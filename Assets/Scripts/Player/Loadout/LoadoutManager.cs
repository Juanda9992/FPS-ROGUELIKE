using System;
using UnityEngine;

public class LoadoutManager : MonoBehaviour
{
    public static LoadoutManager Instance { get; private set; }

    [Header("Available Capsules")]
    [SerializeField] private SpawnObjectSkill[] _availableCapsules;
    [SerializeField] private SpawnObjectSkill _selectedCapsule;

    [Header("References")]
    [SerializeField] private PlayerSkillsManager _playerSkillsManager;

    public SpawnObjectSkill[] AvailableCapsules => _availableCapsules;
    public SpawnObjectSkill SelectedCapsule => _selectedCapsule;

    public event Action<SpawnObjectSkill> OnCapsuleSelected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (_selectedCapsule == null && _availableCapsules != null && _availableCapsules.Length > 0)
        {
            _selectedCapsule = _availableCapsules[0];
        }
    }

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
    }

    private void OnDestroy()
    {
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.OnGameStarted -= HandleGameStarted;
        }
    }

    public void SelectCapsule(SpawnObjectSkill capsule)
    {
        if (capsule == null)
        {
            return;
        }

        _selectedCapsule = capsule;
        OnCapsuleSelected?.Invoke(_selectedCapsule);
    }

    private void HandleGameStarted()
    {
        EquipSelectedCapsuleToPlayer();
    }

    public void EquipSelectedCapsuleToPlayer()
    {
        if (_selectedCapsule == null)
        {
            if (_availableCapsules != null && _availableCapsules.Length > 0)
            {
                _selectedCapsule = _availableCapsules[0];
            }
            else
            {
                Debug.LogWarning("No capsule available in LoadoutManager to equip.");
                return;
            }
        }

        _playerSkillsManager = FindFirstObjectByType<PlayerSkillsManager>();
        _playerSkillsManager.SetCapsule(_selectedCapsule);
        Debug.Log($"Equipped capsule {_selectedCapsule.skillName} to Player Q slot.");
    }

    #region ContextMenu Tests
    [ContextMenu("Test Select Next Capsule")]
    private void TestSelectNextCapsule()
    {
        if (_availableCapsules == null || _availableCapsules.Length == 0)
        {
            Debug.LogWarning("LoadoutManager: No available capsules to cycle through.");
            return;
        }

        int currentIndex = Array.IndexOf(_availableCapsules, _selectedCapsule);
        int nextIndex = (currentIndex + 1) % _availableCapsules.Length;
        SelectCapsule(_availableCapsules[nextIndex]);
        Debug.Log($"LoadoutManager Test: Selected capsule {nextIndex} -> {_selectedCapsule.skillName}");
    }

    [ContextMenu("Test Equip Selected Capsule")]
    private void TestEquipSelectedCapsule()
    {
        EquipSelectedCapsuleToPlayer();
    }

    [ContextMenu("Test Log Available Capsules")]
    private void TestLogAvailableCapsules()
    {
        if (_availableCapsules == null || _availableCapsules.Length == 0)
        {
            Debug.Log("LoadoutManager: No capsules registered in AvailableCapsules array.");
            return;
        }

        for (int i = 0; i < _availableCapsules.Length; i++)
        {
            SpawnObjectSkill capsule = _availableCapsules[i];
            string capsuleName = capsule != null ? (!string.IsNullOrEmpty(capsule.skillName) ? capsule.skillName : capsule.name) : "NULL";
            bool isCurrent = capsule == _selectedCapsule;
            Debug.Log($"[{i}] {capsuleName} {(isCurrent ? "(SELECTED)" : "")}");
        }
    }
    #endregion
}
