using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameScreenAdviceManager : MonoBehaviour
{
    public static GameScreenAdviceManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _adviceText;

    [Header("Timing Settings")]
    [SerializeField] private float _fadeInDuration = 0.5f;
    [SerializeField] private float _displayDuration = 2.5f;
    [SerializeField] private float _fadeOutDuration = 0.5f;

    [Header("Animation Settings")]
    [SerializeField] private Ease _fadeInEase = Ease.OutQuad;
    [SerializeField] private Ease _fadeOutEase = Ease.InQuad;

    private Sequence _currentSequence;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _adviceText.alpha = 0f;
    }

    public void SetMessage(string text, Color color)
    {
        _currentSequence?.Kill();

        _adviceText.text = text;
        _adviceText.color = new Color(color.r, color.g, color.b, 0f);

        _currentSequence = DOTween.Sequence()
            .Append(_adviceText.DOFade(1f, _fadeInDuration).SetEase(_fadeInEase))
            .AppendInterval(_displayDuration)
            .Append(_adviceText.DOFade(0f, _fadeOutDuration).SetEase(_fadeOutEase))
            .SetTarget(this);
    }

    public void SetMessage(string text)
    {
        SetMessage(text, Color.white);
    }

    private void OnDestroy()
    {
        _currentSequence?.Kill();
    }

    [ContextMenu("Test Advice Message")]
    private void TestAdviceMessage()
    {
        SetMessage("A horde is coming!", Color.red);
    }
}
