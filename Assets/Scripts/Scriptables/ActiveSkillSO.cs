using UnityEngine;

public abstract class ActiveSkillSO : ScriptableObject
{
    [Header("Base Info")]
    public string skillName;
    public Sprite icon;

    [Header("Settings")]
    public float cooldown = 1f;

    // Método principal
    public abstract void Activate(GameObject owner);
}