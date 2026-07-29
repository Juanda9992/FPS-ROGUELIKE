using UnityEngine;
[CreateAssetMenu(fileName = "SkillPrintMessage", menuName = "Skills/SkillPrintMessage")]
public class SkillPrintMessage : ActiveSkillSO
{
    [SerializeField] private string message;
    public override void Activate(GameObject owner)
    {
        Debug.Log(message);
    }
}
