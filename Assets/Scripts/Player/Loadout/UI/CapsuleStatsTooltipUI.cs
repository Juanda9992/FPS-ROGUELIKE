using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CapsuleStatsTooltipUI : MonoBehaviour
{
    [Header("Root Panel")]
    [SerializeField] private GameObject _panelRoot;

    [Header("Header Elements")]
    [SerializeField] private TextMeshProUGUI _capsuleTitleText;
    [SerializeField] private Image _capsuleIcon;

    [Header("Description & Stats")]
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _statsDetailsText;

    [Header("Positioning Settings")]
    [SerializeField] private bool _useDynamicPositioning = false;
    [SerializeField] private Vector2 _offsetFromTarget = new Vector2(20f, 0f);

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = transform as RectTransform;
        _panelRoot.SetActive(false);
    }

    public void Show(SpawnObjectSkill capsule, RectTransform targetItem = null)
    {
        if (capsule == null)
        {
            Hide();
            return;
        }

        PopulateStats(capsule);

        if (_useDynamicPositioning && targetItem != null && _rectTransform != null)
        {
            Vector3[] corners = new Vector3[4];
            targetItem.GetWorldCorners(corners);
            Vector3 targetCenterRight = (corners[2] + corners[3]) * 0.5f;
            _rectTransform.position = targetCenterRight + new Vector3(_offsetFromTarget.x, _offsetFromTarget.y, 0f);
        }

        _panelRoot.SetActive(true);
    }

    public void Hide()
    {
        _panelRoot.SetActive(false);
    }

    private void PopulateStats(SpawnObjectSkill capsule)
    {
        string displayName = !string.IsNullOrEmpty(capsule.skillName) ? capsule.skillName : capsule.name;
        _capsuleTitleText.text = displayName;

        SpawnParams parameters = capsule.SpawnParams;

        if (capsule.icon != null)
        {
            _capsuleIcon.sprite = capsule.icon;
            _capsuleIcon.enabled = true;
        }
        else
        {
            _capsuleIcon.enabled = false;
        }

        string desc = !string.IsNullOrEmpty(capsule.description)
                ? capsule.description
                : $"Deployable capsule effect.";
        _descriptionText.text = desc;

        _statsDetailsText.text = BuildStatsString(capsule, parameters);
    }

    private string BuildStatsString(SpawnObjectSkill capsule, SpawnParams parameters)
    {
        StringBuilder sb = new StringBuilder();

        // Base Skill Settings
        sb.AppendLine($"<color=#70C0FF><b>Cooldown:</b></color> {capsule.cooldown:0.#}s");

        if (parameters != null)
        {
            if (parameters.duration > 0)
            {
                sb.AppendLine($"<color=#70C0FF><b>Duration:</b></color> {parameters.duration:0.#}s");
            }

            if (parameters.radius > 0)
            {
                sb.AppendLine($"<color=#70C0FF><b>Radius:</b></color> {parameters.radius:0.#}m");
            }

            if (parameters.effectRate > 0)
            {
                sb.AppendLine($"<color=#70C0FF><b>Tick Rate:</b></color> every {parameters.effectRate:0.##}s");
            }

            // Effect specifics
            if (parameters.damage > 0)
            {
                sb.AppendLine($"<color=#FF6E6E><b>Damage:</b></color> {parameters.damage:0.#}");
            }

            if (parameters.healAmount > 0)
            {
                sb.AppendLine($"<color=#6EFF89><b>Heal Amount:</b></color> {parameters.healAmount:0.#}");
            }

            if (parameters.slowAmount > 0 || parameters.slowDuration > 0)
            {
                sb.AppendLine($"<color=#FFD56E><b>Slow:</b></color> {parameters.slowAmount:0.#}% ({parameters.slowDuration:0.#}s)");
            }

            if (parameters.stunDuration > 0)
            {
                sb.AppendLine($"<color=#FFD56E><b>Stun Duration:</b></color> {parameters.stunDuration:0.#}s");
            }

            if (parameters.pushForce > 0)
            {
                sb.AppendLine($"<color=#E389FF><b>Push Force:</b></color> {parameters.pushForce:0.#}");
            }

            if (parameters.silenceDuration > 0)
            {
                sb.AppendLine($"<color=#D488FF><b>Silence Duration:</b></color> {parameters.silenceDuration:0.#}s");
            }

            if (parameters.blindDuration > 0)
            {
                sb.AppendLine($"<color=#88D4FF><b>Blind Duration:</b></color> {parameters.blindDuration:0.#}s");
            }

            if (parameters.vulnerabilityPercentage > 0 || parameters.vulnerabilityDuration > 0)
            {
                sb.AppendLine($"<color=#FFAA70><b>Vulnerability:</b></color> +{parameters.vulnerabilityPercentage:0.#}% ({parameters.vulnerabilityDuration:0.#}s)");
            }
        }

        return sb.ToString().TrimEnd();
    }
}
