using UnityEngine;
using TMPro;
public class StatDisplaydataUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statValueText;
    private Stat attachedStat;

    public void SetStat(Stat stat)
    {
        attachedStat = stat;
        statNameText.text = stat.displayName;
        statValueText.text = stat.Value.ToString("F2");
    }

    public Stat GetAttachedStat()
    {
        return attachedStat;
    }
}
