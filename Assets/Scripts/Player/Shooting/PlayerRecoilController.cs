using UnityEngine;

public class PlayerRecoilController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _recoilTransform;

    [Header("Trauma Shake Settings")]
    [SerializeField] private float _traumaDecaySpeed = 3.0f;
    [SerializeField] private float _maxShakePosOffset = 0.035f;
    [SerializeField] private float _maxShakeRotOffset = 2.5f;
    [SerializeField] private float _shakeFrequency = 24f;

    private Vector3 _targetRecoilRotation;
    private Vector3 _currentRecoilRotation;
    private float _snappiness = 22f;
    private float _returnSpeed = 10f;

    private float _trauma = 0f;
    private float _shakeSeed;
    private Vector3 _initialLocalPos;
    private Quaternion _initialLocalRot;
    private bool _initialized;

    private float _currentSpread = 0f;
    private float _baseSpread = 0.4f;
    private float _maxSpread = 3.5f;
    private float _spreadRecoverySpeed = 5f;

    public float CurrentSpread => _currentSpread;

    private void Reset()
    {
        if (_recoilTransform == null)
        {
            _recoilTransform = transform;
        }
    }

    private void Awake()
    {
        _shakeSeed = Random.value * 100f;
        if (_recoilTransform == null)
        {
            _recoilTransform = transform;
        }

        _initialLocalPos = _recoilTransform.localPosition;
        _initialLocalRot = _recoilTransform.localRotation;
        _initialized = true;
    }

    public void InitializeReferences(Transform recoilTransform)
    {
        _recoilTransform = recoilTransform;
        if (_recoilTransform != null)
        {
            _initialLocalPos = _recoilTransform.localPosition;
            _initialLocalRot = _recoilTransform.localRotation;
            _initialized = true;
        }
    }

    public void ApplyRecoil(Weapon weapon)
    {
        if (weapon == null)
        {
            return;
        }

        _snappiness = weapon.recoilSnappiness;
        _returnSpeed = weapon.recoilReturnSpeed;
        _baseSpread = weapon.baseSpread;
        _maxSpread = weapon.maxSpread;
        _spreadRecoverySpeed = weapon.spreadRecoverySpeed;

        // Apply rotational kick: upward pitch + erratic horizontal yaw + erratic roll
        float pitchKick = -weapon.recoilPitchKick;
        float yawKick = Random.Range(-weapon.recoilYawKick, weapon.recoilYawKick);
        float rollKick = Random.Range(-weapon.recoilRollKick, weapon.recoilRollKick);

        _targetRecoilRotation += new Vector3(pitchKick, yawKick, rollKick);

        // Add shake trauma
        _trauma = Mathf.Clamp01(_trauma + weapon.cameraShakeStrength);

        // Increase spread bloom
        if (_currentSpread < weapon.baseSpread)
        {
            _currentSpread = weapon.baseSpread;
        }
        _currentSpread = Mathf.Min(_currentSpread + weapon.spreadIncreasePerShot, weapon.maxSpread);
    }

    public void ResetSpread(Weapon weapon)
    {
        if (weapon != null)
        {
            _baseSpread = weapon.baseSpread;
            _maxSpread = weapon.maxSpread;
            _spreadRecoverySpeed = weapon.spreadRecoverySpeed;
            _currentSpread = weapon.baseSpread;
        }
        else
        {
            _currentSpread = 0f;
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        // Recover spread bloom
        if (_currentSpread > _baseSpread)
        {
            _currentSpread = Mathf.MoveTowards(_currentSpread, _baseSpread, _spreadRecoverySpeed * deltaTime);
        }

        // Decay shake trauma
        if (_trauma > 0f)
        {
            _trauma = Mathf.Max(0f, _trauma - _traumaDecaySpeed * deltaTime);
        }

        // Smooth recoil rotation back to zero
        _targetRecoilRotation = Vector3.Lerp(_targetRecoilRotation, Vector3.zero, _returnSpeed * deltaTime);
        _currentRecoilRotation = Vector3.Slerp(_currentRecoilRotation, _targetRecoilRotation, _snappiness * deltaTime);
    }

    private void LateUpdate()
    {
        if (_recoilTransform == null)
        {
            return;
        }

        if (!_initialized)
        {
            _initialLocalPos = _recoilTransform.localPosition;
            _initialLocalRot = _recoilTransform.localRotation;
            _initialized = true;
        }

        // Calculate trauma shake
        float traumaSquare = _trauma * _trauma;
        Vector3 shakePosOffset = Vector3.zero;
        Vector3 shakeRotOffset = Vector3.zero;

        if (traumaSquare > 0.0001f)
        {
            float time = Time.time * _shakeFrequency + _shakeSeed;
            shakePosOffset = new Vector3(
                (Mathf.PerlinNoise(time, 0.0f) * 2f - 1f) * _maxShakePosOffset * traumaSquare,
                (Mathf.PerlinNoise(0.0f, time) * 2f - 1f) * _maxShakePosOffset * traumaSquare,
                (Mathf.PerlinNoise(time, time) * 2f - 1f) * _maxShakePosOffset * 0.5f * traumaSquare
            );

            shakeRotOffset = new Vector3(
                (Mathf.PerlinNoise(time + 10f, 0.0f) * 2f - 1f) * _maxShakeRotOffset * traumaSquare,
                (Mathf.PerlinNoise(0.0f, time + 20f) * 2f - 1f) * _maxShakeRotOffset * traumaSquare,
                (Mathf.PerlinNoise(time + 30f, time + 40f) * 2f - 1f) * _maxShakeRotOffset * traumaSquare
            );
        }

        // Combine recoil rotation with shake
        Quaternion recoilRot = Quaternion.Euler(_currentRecoilRotation + shakeRotOffset);

        _recoilTransform.localPosition = _initialLocalPos + shakePosOffset;
        _recoilTransform.localRotation = _initialLocalRot * recoilRot;
    }
}
