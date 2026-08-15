using UnityEngine;

public class CameraBobbingController : MonoBehaviour
{
    [System.Serializable]
    public class MotionProfile
    {
        [SerializeField] private float _frequency = 5f;
        [SerializeField] private Vector3 _posAmplitude = new Vector3(0.05f, 0.05f, 0f);
        [SerializeField] private Vector3 _rotAmplitude = new Vector3(1.5f, 1.0f, 1.5f);

        public MotionProfile(float frequency, Vector3 posAmplitude, Vector3 rotAmplitude)
        {
            _frequency = frequency;
            _posAmplitude = posAmplitude;
            _rotAmplitude = rotAmplitude;
        }

        public float Frequency => _frequency;
        public Vector3 PosAmplitude => _posAmplitude;
        public Vector3 RotAmplitude => _rotAmplitude;
    }

    [Header("References")]
    [SerializeField] private FPSController _fpsController;
    [SerializeField] private Transform _motionTransform;

    [Header("Motion Profiles")]
    [SerializeField] private MotionProfile _idleProfile = new MotionProfile(2f, new Vector3(0.015f, 0.02f, 0f), new Vector3(0.5f, 0.3f, 0.2f));
    [SerializeField] private MotionProfile _walkProfile = new MotionProfile(7.5f, new Vector3(0.05f, 0.06f, 0.01f), new Vector3(1.5f, 1.0f, 1.5f));
    [SerializeField] private MotionProfile _runProfile = new MotionProfile(11f, new Vector3(0.09f, 0.11f, 0.03f), new Vector3(3.0f, 2.0f, 3.5f));

    [Header("Interpolation Speeds")]
    [SerializeField] private float _smoothSpeed = 12f;
    [SerializeField] private float _resetSpeed = 8f;

    private float _timer;
    private Vector3 _initialLocalPos;
    private Quaternion _initialLocalRot;
    private bool _initialized;

    public void InitializeReferences(FPSController fpsController, Transform motionTransform)
    {
        _fpsController = fpsController;
        _motionTransform = motionTransform;
        if (_motionTransform != null)
        {
            _initialLocalPos = _motionTransform.localPosition;
            _initialLocalRot = _motionTransform.localRotation;
            _initialized = true;
        }
    }

    private void Reset()
    {
        AutoFindReferences();
    }

    private void Awake()
    {
        AutoFindReferences();

        if (_motionTransform != null)
        {
            _initialLocalPos = _motionTransform.localPosition;
            _initialLocalRot = _motionTransform.localRotation;
            _initialized = true;
        }
    }

    private void AutoFindReferences()
    {
        if (_motionTransform == null)
        {
            _motionTransform = transform;
        }

        if (_fpsController == null)
        {
            _fpsController = GetComponentInParent<FPSController>();
            if (_fpsController == null)
            {
                _fpsController = Object.FindFirstObjectByType<FPSController>();
            }
        }
    }

    private void LateUpdate()
    {
        UpdateCameraMotion(Time.deltaTime);
    }

    private void UpdateCameraMotion(float deltaTime)
    {
        if (_fpsController == null || _motionTransform == null)
        {
            AutoFindReferences();
            if (_fpsController == null || _motionTransform == null)
            {
                return;
            }
        }

        if (!_initialized && _motionTransform != null)
        {
            _initialLocalPos = _motionTransform.localPosition;
            _initialLocalRot = _motionTransform.localRotation;
            _initialized = true;
        }

        bool isGrounded = _fpsController.IsGrounded;
        bool isMoving = _fpsController.IsMoving;
        bool isRunning = _fpsController.IsRunning;

        Vector3 targetPosOffset = Vector3.zero;
        Vector3 targetRotOffset = Vector3.zero;

        // Only calculate bobbing/sway offsets when the player is on the ground
        if (isGrounded)
        {
            MotionProfile targetProfile = _idleProfile;

            if (isMoving)
            {
                if (isRunning)
                {
                    targetProfile = _runProfile;
                }
                else
                {
                    targetProfile = _walkProfile;
                }
            }

            // Advance bobbing timer only while grounded
            _timer += deltaTime * targetProfile.Frequency;

            float sinTimer = Mathf.Sin(_timer);
            float cosTimer = Mathf.Cos(_timer);
            float sinDoubleTimer = Mathf.Sin(_timer * 2f);

            if (isMoving)
            {
                // Walk / Run Lissajous stride motion
                targetPosOffset = new Vector3(
                    cosTimer * targetProfile.PosAmplitude.x,
                    Mathf.Abs(sinTimer) * targetProfile.PosAmplitude.y,
                    sinDoubleTimer * targetProfile.PosAmplitude.z
                );

                targetRotOffset = new Vector3(
                    sinDoubleTimer * targetProfile.RotAmplitude.x,
                    cosTimer * targetProfile.RotAmplitude.y,
                    -cosTimer * targetProfile.RotAmplitude.z
                );
            }
            else
            {
                // Idle breathing sway
                targetPosOffset = new Vector3(
                    cosTimer * targetProfile.PosAmplitude.x,
                    sinTimer * targetProfile.PosAmplitude.y,
                    0f
                );

                targetRotOffset = new Vector3(
                    sinTimer * targetProfile.RotAmplitude.x,
                    cosTimer * targetProfile.RotAmplitude.y,
                    0f
                );
            }
        }

        // Blend smoothly to target offset (smoothly returns to initial local transform when in air)
        Vector3 targetLocalPos = _initialLocalPos + targetPosOffset;
        Quaternion targetLocalRot = _initialLocalRot * Quaternion.Euler(targetRotOffset);

        float lerpRate = (isGrounded && isMoving) ? _smoothSpeed : _resetSpeed;
        _motionTransform.localPosition = Vector3.Lerp(_motionTransform.localPosition, targetLocalPos, deltaTime * lerpRate);
        _motionTransform.localRotation = Quaternion.Slerp(_motionTransform.localRotation, targetLocalRot, deltaTime * lerpRate);
    }
}
