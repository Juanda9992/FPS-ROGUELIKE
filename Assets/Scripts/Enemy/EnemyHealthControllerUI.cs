using UnityEngine;

public class EnemyHealthControllerUI : MonoBehaviour
{
    [Header("Sprite References")]
    [SerializeField] private SpriteRenderer _fillRenderer;
    [SerializeField] private SpriteRenderer _backgroundRenderer;

    private Transform _mainCameraTransform;
    private Vector3 _initialFillScale;
    private Vector3 _initialFillLocalPosition;

    private void Start()
    {
        _mainCameraTransform = Camera.main.transform;
        _initialFillScale = _fillRenderer.transform.localScale;
        _initialFillLocalPosition = _fillRenderer.transform.localPosition;
    }

    private void LateUpdate()
    {
        _backgroundRenderer.transform.rotation = _mainCameraTransform.rotation;
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        float ratio = Mathf.Clamp01((float)currentHealth / maxHealth);

        bool shouldShow = currentHealth < maxHealth && currentHealth > 0;
        _backgroundRenderer.enabled = shouldShow;
        _fillRenderer.enabled = shouldShow;

        if (shouldShow)
        {
            Vector3 newScale = _initialFillScale;
            newScale.x = _initialFillScale.x * ratio;
            _fillRenderer.transform.localScale = newScale;
        }
    }
}
