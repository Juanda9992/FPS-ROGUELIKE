using UnityEngine;

public class PlayerSkillSetter : MonoBehaviour
{
    [SerializeField] private ActiveSkillSO[] testSkillArray;
    private PlayerSkillsManager skillsManager;

    private void Awake()
    {
        skillsManager = GetComponent<PlayerSkillsManager>();
    }

    private void Start()
    {
        AddTestSkill();
    }

    public bool TryAddSkill(ActiveSkillSO skillSlotSO)
    {
        if (skillSlotSO == null)
        {
            Debug.LogWarning("SkillSlotSO is null. Cannot add skill.");
            return false;
        }

        if (!skillsManager.CanAddItem())
        {
            Debug.LogWarning("No available skill slots. Cannot add skill.");
            return false;
        }

        return skillsManager.TryAddSkill(skillSlotSO);
    }

    [ContextMenu("Add Test Skill")]
    private void AddTestSkill()
    {
        foreach (var skill in testSkillArray)
        {
            TryAddSkill(skill);
        }
    }
}