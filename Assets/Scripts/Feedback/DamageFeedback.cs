using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Renderer))]
public class DamageFeedback : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;
    private Renderer rend;
    private Material materialInstance;
    private Color originalColor;

    private Tween currentTween;

    private void Awake()
    {
        rend = GetComponent<Renderer>();

        materialInstance = rend.material;
        originalColor = materialInstance.color;
    }

    /// <summary>
    /// Call this when the player takes damage
    /// </summary>
    public void PlayDamageFlash()
    {
        currentTween?.Kill();

        currentTween = DOTween.Sequence()
            .AppendCallback(() => materialInstance.color = damageColor)
            .Append(materialInstance.DOColor(originalColor, flashDuration))
            .SetTarget(this);
    }

    private void OnDestroy()
    {
        currentTween?.Kill();
    }
}