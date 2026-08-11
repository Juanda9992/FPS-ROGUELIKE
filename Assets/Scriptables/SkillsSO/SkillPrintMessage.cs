using UnityEngine;
[CreateAssetMenu(fileName = "SkillPrintMessage", menuName = "Skills/SkillPrintMessage")]
public class SkillPrintMessage : ActiveSkillSO
{
    [SerializeField] private string message;
    public override void Activate(GameObject owner, SkillInstance instance = null)
    {
        string levelInfo = instance != null ? $" [Lvl {instance.Level}]" : "";
        Debug.Log($"{message}{levelInfo}");
    }
}
