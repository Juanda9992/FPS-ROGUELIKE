using UnityEngine;

public enum SpawnOriginMode
{
    OwnerPosition,
    CameraPosition,
    GroundAtCrosshair
}

public enum AimMode
{
    CrosshairTarget,    // Raycasts from camera to find exact point aimed at
    CameraForward,      // Pure forward vector of camera
    OwnerForward,       // Pure forward vector of owner transform
    CustomDirection     // Fixed direction vector
}

public enum ProjectilePattern
{
    Single,
    ConeSpread,         // Fan / Shotgun spread
    RadialNova          // 360-degree circle around origin
}

[CreateAssetMenu(fileName = "SkillProjectileSO", menuName = "Skills/SkillProjectileSO")]
public class SkillProjectileSO : ActiveSkillSO
{
    [Header("Projectile Config")]
    [SerializeField] private ProjectileConfig projConfig;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Spawn Position")]
    [SerializeField] private SpawnOriginMode spawnOrigin = SpawnOriginMode.OwnerPosition;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 1.2f, 0.5f);
    [SerializeField] private LayerMask raycastMask = ~0;

    [Header("Aiming & Direction")]
    [SerializeField] private AimMode aimMode = AimMode.CrosshairTarget;
    [SerializeField] private Vector3 customDirection = Vector3.forward;
    [SerializeField] private float maxAimDistance = 100f;
    [SerializeField] private float randomSpreadAngle = 0f; // Random accuracy spread

    [Header("Multi-Projectile Pattern")]
    [SerializeField] private ProjectilePattern pattern = ProjectilePattern.Single;
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float coneSpreadAngle = 30f; // Total arc angle for ConeSpread

    public override void Activate(GameObject owner, SkillInstance instance = null)
    {
        if (projectilePrefab == null || owner == null) return;

        Camera mainCam = Camera.main;
        Vector3 spawnPos = CalculateSpawnPosition(owner, mainCam);
        Vector3 baseAimDirection = CalculateBaseAimDirection(owner, mainCam, spawnPos);
        int damage = instance != null ? Mathf.RoundToInt(instance.GetEffectiveDamage()) : Mathf.RoundToInt(baseDamage);

        // Spawn projectiles according to configured pattern
        switch (pattern)
        {
            case ProjectilePattern.Single:
                SpawnSingleProjectile(spawnPos, baseAimDirection);
                break;

            case ProjectilePattern.ConeSpread:
                SpawnConePattern(spawnPos, baseAimDirection);
                break;

            case ProjectilePattern.RadialNova:
                SpawnRadialPattern(spawnPos);
                break;
        }
    }

    private Vector3 CalculateSpawnPosition(GameObject owner, Camera mainCam)
    {
        switch (spawnOrigin)
        {
            case SpawnOriginMode.CameraPosition:
                if (mainCam != null)
                    return mainCam.transform.position + mainCam.transform.TransformDirection(spawnOffset);
                break;

            case SpawnOriginMode.GroundAtCrosshair:
                if (mainCam != null && Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit, maxAimDistance, raycastMask))
                {
                    Debug.Log("hit.point: " + hit.point);
                    return hit.point + spawnOffset;
                }
                break;

            case SpawnOriginMode.OwnerPosition:
            default:
                break;
        }

        return owner.transform.position + owner.transform.TransformDirection(spawnOffset);
    }

    private Vector3 CalculateBaseAimDirection(GameObject owner, Camera mainCam, Vector3 spawnPos)
    {
        switch (aimMode)
        {
            case AimMode.CrosshairTarget:
                if (mainCam != null)
                {
                    Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                    Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, raycastMask)
                        ? hit.point
                        : ray.GetPoint(maxAimDistance);

                    return (targetPoint - spawnPos).normalized;
                }
                break;

            case AimMode.CameraForward:
                if (mainCam != null)
                    return mainCam.transform.forward;
                break;

            case AimMode.CustomDirection:
                return owner.transform.TransformDirection(customDirection.normalized);

            case AimMode.OwnerForward:
            default:
                break;
        }

        return owner.transform.forward;
    }

    private void SpawnSingleProjectile(Vector3 position, Vector3 direction)
    {
        Vector3 finalDir = ApplyRandomSpread(direction, randomSpreadAngle);
        InstantiateAndInitialize(position, finalDir);
    }

    private void SpawnConePattern(Vector3 position, Vector3 baseDirection)
    {
        int count = Mathf.Max(1, projectileCount);
        if (count == 1)
        {
            SpawnSingleProjectile(position, baseDirection);
            return;
        }

        float stepAngle = coneSpreadAngle / (count - 1);
        float startAngle = -coneSpreadAngle / 2f;

        for (int i = 0; i < count; i++)
        {
            float currentAngle = startAngle + (stepAngle * i);
            Quaternion rotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
            Vector3 spreadDir = rotation * baseDirection;
            Vector3 finalDir = ApplyRandomSpread(spreadDir, randomSpreadAngle);

            InstantiateAndInitialize(position, finalDir);
        }
    }

    private void SpawnRadialPattern(Vector3 position)
    {
        int count = Mathf.Max(1, projectileCount);
        float stepAngle = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = stepAngle * i;
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            Vector3 finalDir = ApplyRandomSpread(dir, randomSpreadAngle);

            InstantiateAndInitialize(position, finalDir);
        }
    }

    private Vector3 ApplyRandomSpread(Vector3 dir, float angle)
    {
        if (angle <= 0f) return dir;
        return Quaternion.Euler(
            Random.Range(-angle, angle),
            Random.Range(-angle, angle),
            0f
        ) * dir;
    }

    private void InstantiateAndInitialize(Vector3 position, Vector3 direction)
    {
        Quaternion rotation = direction != Vector3.zero ? Quaternion.LookRotation(direction) : Quaternion.identity;
        GameObject projObj = Instantiate(projectilePrefab, position, rotation);



        if (projObj.TryGetComponent<GenericProjectileMovement>(out var movement))
        {
            movement.Initialize(projConfig, direction);
        }
    }
}
