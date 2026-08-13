using UnityEngine;

public abstract class ActiveSkillSO : ScriptableObject
{
    [Header("Base Info")]
    public string skillId;
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Settings")]
    public float cooldown = 1f;

    public virtual SkillInstance CreateInstance()
    {
        return new SkillInstance(this);
    }

    // Método principal
    public abstract void Activate(GameObject owner, SkillInstance instance = null);
}