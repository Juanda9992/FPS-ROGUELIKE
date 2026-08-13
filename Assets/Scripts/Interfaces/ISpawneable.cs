using UnityEngine;
public interface ISpawneable
{
    void Initialize(SpawnParams spawnParams);
}
[System.Serializable]
public class SpawnParams
{
    public AoEEffectType effectType;
    public float duration = -1;
    public Vector3 objectScale = Vector3.one;
}