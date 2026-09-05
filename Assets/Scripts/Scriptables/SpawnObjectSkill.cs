using UnityEngine;

public enum PlacementMode
{
    Raycast,
    PhysicsForce,
    AtOwnerPosition
}

[CreateAssetMenu(fileName = "SpawnObjectSkill", menuName = "Skills/Spawn Object Skill")]
public class SpawnObjectSkill : ActiveSkillSO
{
    [Header("Spawn Settings")]
    [Tooltip("Prefab to instantiate when skill is activated.")]
    public GameObject prefab;

    [Tooltip("Placement mode: Raycast onto surfaces, throw with physics force, or spawn directly at owner.")]
    public PlacementMode placementMode = PlacementMode.Raycast;

    [Tooltip("Position offset relative to spawn point.")]
    public Vector3 positionOffset = Vector3.zero;

    [Header("Raycast Settings")]
    [Tooltip("Maximum distance for Raycast placement mode.")]
    public float maxRaycastDistance = 30f;

    [Tooltip("Layer mask for ground/surface detection in Raycast mode.")]
    public LayerMask raycastMask = ~0;

    [Header("Physics Force Settings")]
    [Tooltip("Forward impulse force applied to Rigidbody in PhysicsForce placement mode.")]
    public float throwForce = 15f;

    [Tooltip("Upward impulse force applied to Rigidbody in PhysicsForce placement mode.")]
    public float upwardForce = 2f;

    [Header("Custom Parameters")]
    [SerializeField] private SpawnParams spawnParams;

    public SpawnParams SpawnParams => spawnParams;


    public override void Activate(GameObject owner, SkillInstance instance = null)
    {
        Vector3 spawnPosition = owner.transform.position + owner.transform.TransformDirection(positionOffset);
        Quaternion spawnRotation = owner.transform.rotation;

        Transform aimTransform = Camera.main != null ? Camera.main.transform : owner.transform;

        switch (placementMode)
        {
            case PlacementMode.Raycast:
                if (Physics.Raycast(aimTransform.position, aimTransform.forward, out RaycastHit hit, maxRaycastDistance, raycastMask))
                {
                    spawnPosition = hit.point + positionOffset;
                    spawnRotation = Quaternion.LookRotation(hit.normal);
                }
                else
                {
                    spawnPosition = aimTransform.position + (aimTransform.forward * maxRaycastDistance) + positionOffset;
                }
                break;

            case PlacementMode.PhysicsForce:
                spawnPosition = aimTransform.position + (aimTransform.forward * 1.5f) + positionOffset;
                spawnRotation = aimTransform.rotation;
                break;

            case PlacementMode.AtOwnerPosition:
                spawnPosition = owner.transform.position + owner.transform.TransformDirection(positionOffset);
                spawnRotation = owner.transform.rotation;
                break;
        }

        // Instantiate Object
        GameObject spawnedObject = Instantiate(prefab, spawnPosition, spawnRotation);

        if (spawnedObject.TryGetComponent<ISpawneable>(out ISpawneable spawneable))
        {
            spawneable.Initialize(spawnParams);
        }

        // Apply physical impulse if in PhysicsForce mode
        if (placementMode == PlacementMode.PhysicsForce && spawnedObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            Vector3 impulse = (aimTransform.forward * throwForce) + (Vector3.up * upwardForce);
            rb.AddForce(impulse, ForceMode.Impulse);
        }
    }
}
