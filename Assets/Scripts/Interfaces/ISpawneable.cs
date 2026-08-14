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
    public float effectRate = 1f;
    public float damage;
    public float healAmount;
    [Header("Slow Settings")]
    [Range(0f, 1f)]
    public float slowAmount;
    public float slowDuration;
    public float stunDuration;
    public float pushForce;
    public float pullForce;
    public float tauntDuration;
    public float silenceDuration;
    public float rootDuration;
    public float blindDuration;
}