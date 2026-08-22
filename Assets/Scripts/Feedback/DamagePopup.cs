using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text _text;

    [Header("Animation Settings")]
    [SerializeField] private float _duration = 0.6f;
    [SerializeField] private float _floatDistance = 1.2f;
    [SerializeField] private float _fadeDuration = 0.25f;
    [SerializeField] private float _punchScaleAmount = 0.35f;

    private Sequence _animationSequence;
    private Transform _cameraTransform;
    private Vector3 _initialScale;
    private System.Action<DamagePopup> _onComplete;

    private void Awake()
    {
        _initialScale = transform.localScale;
        if (Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }
    }

    public void Setup(int damage, Vector3 worldPosition, Color textColor, System.Action<DamagePopup> onComplete)
    {
        _onComplete = onComplete;

        _animationSequence?.Kill();

        transform.position = worldPosition;
        transform.localScale = _initialScale;

        transform.rotation = _cameraTransform.rotation;

        _text.color = textColor;
        _text.SetText("{0}", damage);

        _animationSequence = DOTween.Sequence()
            .Append(transform.DOPunchScale(Vector3.one * _punchScaleAmount, 0.2f, 8, 0.5f))
            .Join(transform.DOMoveY(worldPosition.y + _floatDistance, _duration).SetEase(Ease.OutCubic))
            .Join(_text.DOFade(0f, _fadeDuration).SetDelay(_duration - _fadeDuration).SetEase(Ease.InQuad))
            .OnComplete(() =>
            {
                _onComplete?.Invoke(this);
            })
            .SetTarget(this);
    }

    private void OnDestroy()
    {
        _animationSequence?.Kill();
    }
}
