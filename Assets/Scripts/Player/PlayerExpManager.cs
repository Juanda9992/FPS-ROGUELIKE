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
