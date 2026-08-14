using UnityEngine;
public interface ISpawneable
{
    void Initialize(SpawnParams spawnParams);
}
[System.Serializable]
public class SpawnParams
{
    public bool affectPlayer = false;
    public AoEEffectType effectType;
    public float duration = -1;
    public Vector3 objectScale = Vector3.one;
    public float radius = -1;
    public float effectRate = 1f;
    public float damage;
    public float healAmount;
    public float slowAmount;
    public float slowDuration;
    public float stunDuration;
    public float pushForce;
    public float silenceDuration;
    public float blindDuration;
    public float vulnerabilityPercentage;
    public float vulnerabilityDuration;
}