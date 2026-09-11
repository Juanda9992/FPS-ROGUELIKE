using UnityEngine;

public class EnemyFlyingFollow : EnemyFollow
{
    [Header("Flying Locomotion Settings")]
    [SerializeField] private float _hoverHeight = 3.5f;
    [SerializeField] private float _bobbingFrequency = 2f;
    [SerializeField] private float _bobbingAmplitude = 0.4f;

    private float _bobbingTimer;

    public float HoverHeight
    {
        get => _hoverHeight;
        set => _hoverHeight = value;
    }

    public float BobbingFrequency
    {
        get => _bobbingFrequency;
        set => _bobbingFrequency = value;
    }

    public float BobbingAmplitude
    {
        get => _bobbingAmplitude;
        set => _bobbingAmplitude = value;
    }

    public override void TickMovement(float deltaTime, Vector3 playerPosition)
    {
        ApplyPushMovement(deltaTime);

        if (IsBlind)
        {
            return;
        }

        _bobbingTimer += deltaTime * _bobbingFrequency;
        float verticalBobbing = Mathf.Sin(_bobbingTimer) * _bobbingAmplitude;

        Vector3 targetPosition = playerPosition + Vector3.up * (_hoverHeight + verticalBobbing);
        Vector3 displacementToTarget = targetPosition - transform.position;

        if (displacementToTarget.sqrMagnitude > 0.01f)
        {
            Vector3 moveDirection = displacementToTarget.normalized;
            transform.position += moveDirection * Speed * deltaTime;
        }

        Vector3 lookDirection = (playerPosition - transform.position).normalized;
        if (lookDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * deltaTime);
        }
    }

    #region Context Menu Tests
    [ContextMenu("Test Apply Stun (2s)")]
    private void TestApplyStunContextMenu()
    {
        ApplyStunEffect(2f);
    }

    [ContextMenu("Test Apply Slow (50% for 3s)")]
    private void TestApplySlowContextMenu()
    {
        ApplySlowEffect(3f, 0.5f);
    }

    [ContextMenu("Test Blind (3s)")]
    private void TestBlindContextMenu()
    {
        Blind(3f);
    }
    #endregion
}
