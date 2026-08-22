using UnityEngine;
using UnityEngine.Pool;

public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager Instance { get; private set; }

    [Header("Prefab References")]
    [SerializeField] private DamagePopup _damagePopupPrefab;

    [Header("Pool Configuration")]
    [SerializeField] private int _defaultPoolCapacity = 50;
    [SerializeField] private int _maxPoolSize = 200;

    [Header("Color Settings")]
    [SerializeField] private Color _normalDamageColor = Color.white;
    [SerializeField] private Color _critDamageColor = Color.yellow;

    private ObjectPool<DamagePopup> _pool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _pool = new ObjectPool<DamagePopup>(
            createFunc: CreatePooledItem,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: false,
            defaultCapacity: _defaultPoolCapacity,
            maxSize: _maxPoolSize
        );
    }

    private DamagePopup CreatePooledItem()
    {
        DamagePopup popup = Instantiate(_damagePopupPrefab, transform);
        return popup;
    }

    private void OnTakeFromPool(DamagePopup popup)
    {
        popup.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(DamagePopup popup)
    {
        popup.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(DamagePopup popup)
    {
        Destroy(popup.gameObject);
    }

    public void SpawnDamageNumber(int damage, Vector3 worldPosition, bool isCrit = false)
    {
        if (damage <= 0)
        {
            return;
        }

        DamagePopup popup = _pool.Get();
        Color color = isCrit ? _critDamageColor : _normalDamageColor;

        Vector3 scatterOffset = new Vector3(
            Random.Range(-0.25f, 0.25f),
            Random.Range(0f, 0.3f),
            Random.Range(-0.25f, 0.25f)
        );

        popup.Setup(damage, worldPosition + scatterOffset, color, ReturnToPool);
    }

    private void ReturnToPool(DamagePopup popup)
    {
        _pool.Release(popup);
    }
}
