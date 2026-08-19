using UnityEngine;
using System;
public class PlayerExpManager : MonoBehaviour
{
    [SerializeField] private int currentExp = 0;
    public int expToNextLevel = 100;
    [SerializeField] private int currentLevel = 1;

    public event Action<int> OnExpChanged; 
    public event Action<int> OnLevelUp;

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
        OnExpChanged?.Invoke(currentExp);
    }
    public void AddExperience(int amount)
    {
        currentExp += amount;
        OnExpChanged?.Invoke(currentExp);
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        OnLevelUp?.Invoke(currentLevel);
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.5f);
        CheckLevelUp();
        OnExpChanged?.Invoke(currentExp);
    }
    [ContextMenu("Add 50 Exp")]
    private void TestAddExp()
    {
        AddExperience(50);
    }

}
