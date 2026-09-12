using UnityEngine;

public class EnemyEffectVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private DamageFeedback _damageFeedback;

    [Header("Effect Colors")]
    [SerializeField] private Color _damageColor = new Color(1f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color _slownessColor = new Color(0f, 0.82f, 1f, 1f);
    [SerializeField] private Color _weaknessColor = new Color(0.7f, 0.15f, 1f, 1f);
    [SerializeField] private Color _stunColor = new Color(1f, 0.9f, 0.1f, 1f);

    [Header("Current Status")]
    [SerializeField] private EnemyEffectType _currentEffectType = EnemyEffectType.None;

    public EnemyEffectType CurrentEffectType
    {
        get => _currentEffectType;
    }

    public void ApplyVisuals(EnemyEffectType effectType)
    {
        _currentEffectType = effectType;

        if (effectType == EnemyEffectType.None)
        {
            return;
        }

        Color targetColor = GetColorForEffect(effectType);

        if (_damageFeedback == null)
        {
            TryGetComponent<DamageFeedback>(out _damageFeedback);
        }

        if (_renderer == null)
        {
            TryGetComponent<Renderer>(out _renderer);
            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
            }
        }

        if (_damageFeedback != null)
        {
            _damageFeedback.SetOriginalColor(targetColor);
        }
        else if (_renderer != null)
        {
            _renderer.material.color = targetColor;
        }
    }

    public Color GetColorForEffect(EnemyEffectType effectType)
    {
        switch (effectType)
        {
            case EnemyEffectType.Damage:
                return _damageColor;
            case EnemyEffectType.Slowness:
                return _slownessColor;
            case EnemyEffectType.Weakness:
                return _weaknessColor;
            case EnemyEffectType.Stun:
                return _stunColor;
            default:
                return Color.white;
        }
    }
}
