using UnityEngine;

public class AoEObject : MonoBehaviour, ISpawneable
{
    [SerializeField] private AoEEffectType effectType;
    [SerializeField] private Renderer renderer;

    public void Initialize(SpawnParams spawnParams)
    {
        effectType = spawnParams.effectType;
        Color color = GetEffectColor();
        color.a = 0.5f;
        renderer.material.color = color;
        transform.localScale = spawnParams.objectScale;
    }

    private Color GetEffectColor()
    {
        switch (effectType)
        {
            case AoEEffectType.Damage:
                return Color.red;
            case AoEEffectType.Heal:
                return Color.green;
            case AoEEffectType.Slow:
                return Color.blue;
            case AoEEffectType.Stun:
                return Color.yellow;
            case AoEEffectType.Push:
                return Color.cyan;
            case AoEEffectType.Pull:
                return Color.magenta;
            case AoEEffectType.Taunt:
                return Color.white;
            case AoEEffectType.Silence:
                return Color.gray;
            case AoEEffectType.Root:
                return Color.black;
            case AoEEffectType.Blind:
                return Color.black;
            default:
                return Color.white;
        }
    }
}

public enum AoEEffectType
{
    Damage,
    Heal,
    Slow,
    Stun,
    Push,
    Pull,
    Taunt,
    Silence,
    Root,
    Blind
}
