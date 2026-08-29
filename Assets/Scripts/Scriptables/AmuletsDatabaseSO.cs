using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAmuletsDatabase", menuName = "ScriptableObjects/Amulets/AmuletsDatabaseSO", order = 2)]
public class AmuletsDatabaseSO : ScriptableObject
{
    [Header("Amulets Collection")]
    [SerializeField] private List<AmuletSO> _amulets = new List<AmuletSO>();

    public IReadOnlyList<AmuletSO> Amulets => _amulets;
    public int Count => _amulets.Count;

    private void OnValidate()
    {
        RemoveDuplicates();
    }

    public void RemoveDuplicates()
    {
        if (_amulets == null || _amulets.Count == 0)
        {
            return;
        }

        HashSet<AmuletSO> seen = new HashSet<AmuletSO>();
        List<AmuletSO> uniqueList = new List<AmuletSO>();
        bool hadDuplicates = false;

        for (int i = 0; i < _amulets.Count; i++)
        {
            AmuletSO amulet = _amulets[i];
            if (amulet != null)
            {
                if (!seen.Add(amulet))
                {
                    hadDuplicates = true;
                    Debug.LogWarning($"[AmuletsDatabaseSO] Duplicate amulet removed: '{amulet.DisplayName}' in '{name}'.");
                }
                else
                {
                    uniqueList.Add(amulet);
                }
            }
            else
            {
                uniqueList.Add(null);
            }
        }

        if (hadDuplicates)
        {
            _amulets = uniqueList;
        }
    }

    public List<AmuletSO> GetAmuletsByRarity(AmuletRarity rarity)
    {
        List<AmuletSO> result = new List<AmuletSO>();
        for (int i = 0; i < _amulets.Count; i++)
        {
            if (_amulets[i] != null && _amulets[i].Rarity == rarity)
            {
                result.Add(_amulets[i]);
            }
        }

        return result;
    }

    #region ContextMenu Tests
    [ContextMenu("Clean & Remove Duplicates")]
    private void ContextMenuRemoveDuplicates()
    {
        RemoveDuplicates();
    }
    #endregion
}
