using UnityEngine;
using System;
public class PlayerExpManager : MonoBehaviour
{
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int currentLevel = 1;
    public float expToNextLevel = 100f;
    [SerializeField] private float expToNextLevelMultiplier = 1.5f;

    public event Action<int> OnExpChanged;
    public event Action<int> OnLevelUp;

    [SerializeField] private Stat _experienceMultiplierStat;

    public int CurrentExp => currentExp;
    public int CurrentLevel => currentLevel;

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
        _experienceMultiplierStat = PlayerStatsManager.Instance.GetStatByName("ExperienceMultiplier");
        OnExpChanged?.Invoke(currentExp);
    }
    public void AddExperience(int amount)
    {
        float multiplier = 1f;
        if (_experienceMultiplierStat != null)
        {
            multiplier = _experienceMultiplierStat.Value;
            Debug.Log($"[PlayerExpManager] Multiplier: {multiplier}");
        }

        int finalExp = Mathf.RoundToInt(amount * multiplier);
        currentExp += finalExp;
        OnExpChanged?.Invoke(currentExp);
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (currentExp >= expToNextLevel)
        {
            currentExp -= Mathf.RoundToInt(expToNextLevel);
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        OnLevelUp?.Invoke(currentLevel);
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expToNextLevelMultiplier);
        CheckLevelUp();
        OnExpChanged?.Invoke(currentExp);
    }

    #region Context Menu Tests
    [ContextMenu("Test Level Up")]
    private void TestLevelUp()
    {
        LevelUp();
    }
    #endregion
}
