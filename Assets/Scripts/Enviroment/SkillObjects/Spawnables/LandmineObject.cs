using System.Collections;
using UnityEngine;

public class LandmineObject : MonoBehaviour, ISpawneable
{
    [Header("Landmine Settings")]
    [SerializeField] private bool pushEnemies = true;
    [SerializeField] private float armingDelay = 0.5f;

    [Header("Targeting & Effects")]
    [SerializeField] private LayerMask targetLayers = ~0;
    [SerializeField] private GameObject explosionVFX;

    private bool isArmed = false;
    private bool hasExploded = false;
    private SpawnParams _spawnParams;

    private void Start()
    {
        StartCoroutine(ArmMineRoutine());
    }

    public void Initialize(SpawnParams spawnParams)
    {
        _spawnParams = spawnParams;

    }


    private IEnumerator ArmMineRoutine()
    {
        yield return new WaitForSeconds(armingDelay);
        isArmed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckAndExplode(collision.gameObject);
    }

    private void CheckAndExplode(GameObject target)
    {
        if (!isArmed || hasExploded) return;

        if (target.GetComponentInParent<IDamageable>() != null || target.GetComponentInParent<IPusheable>() != null)
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        float radius = _spawnParams.radius;
        float damage = _spawnParams.damage;
        float pushForce = _spawnParams.pushForce;

        Collider[] hits = Physics.OverlapSphere(transform.position, radius, targetLayers);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            if (hit.TryGetComponent<IDamageable>(out IDamageable damageable) || hit.GetComponentInParent<IDamageable>() is IDamageable parentDamageable && (damageable = parentDamageable) != null)
            {
                damageable.TakeDamage(Mathf.RoundToInt(damage));
                Debug.Log("Damage dealt to: " + hit.gameObject.name);
            }

            if (pushEnemies)
            {
                if (hit.TryGetComponent<IPusheable>(out IPusheable pusheable) || hit.GetComponentInParent<IPusheable>() is IPusheable parentPusheable && (pusheable = parentPusheable) != null)
                {
                    pusheable.Push(transform.position, pushForce, attract: false);
                }
            }
        }

        Destroy(gameObject);
    }
}
