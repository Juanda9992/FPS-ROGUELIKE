using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FPSController : MonoBehaviour, IPusheable
{
    [Header("Movimiento")]
    [SerializeField] private Stat _movementSpeedStat;
    [SerializeField] private float _sprintMultiplier = 1.5f;

    [Header("Salto")]
    [SerializeField] private Stat _jumpForceStat;
    [SerializeField] private Stat _jumpCountStat;
    private int _currentJumpCount;
    [SerializeField] private float _gravity = -9.81f;

    [Header("Ground Check")]
    [SerializeField] private float _groundCheckDistance = 1.25f;
    [SerializeField] private float _groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask _groundMask = ~0;
    private bool _isGrounded;

    [Header("Cámara")]
    [SerializeField] public Transform cameraPivot;
    [SerializeField] public float mouseSensitivity = 100f;

    private Rigidbody _rb;
    private PlayerInputActions _input;

    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private float _xRotation;
    private bool _isRunning;
    private bool _isGameStarted;

    public bool IsGrounded => _isGrounded;
    public bool IsRunning => _isRunning;
    public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;
    public Vector2 MoveInput => _moveInput;
    public Vector3 Velocity => _rb != null ? _rb.velocity : Vector3.zero;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _input = new PlayerInputActions();

        // INPUTS
        _input.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _input.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _input.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _input.Player.Look.canceled += ctx => _lookInput = Vector2.zero;

        _input.Player.Run.performed += _ => _isRunning = true;
        _input.Player.Run.canceled += _ => _isRunning = false;

        _input.Player.Jump.performed += _ => Jump();
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void Start()
    {
        GameEventsManager.Instance.OnGameStarted += HandleGameStarted;
        if (GameEventsManager.Instance.IsGameStarted)
        {
            HandleGameStarted();
        }

        CursorManager.SetCursorVisible(true);
    }

    private void OnDestroy()
    {
        GameEventsManager.Instance.OnGameStarted -= HandleGameStarted;
        _input.Dispose();
    }

    private void HandleGameStarted()
    {
        _isGameStarted = true;
        CursorManager.SetCursorVisible(false);

        _movementSpeedStat = PlayerStatsManager.Instance.GetStatByName("MovementSpeed");

        _jumpForceStat = PlayerStatsManager.Instance.GetStatByName("JumpHeight");
        _jumpCountStat = PlayerStatsManager.Instance.GetStatByName("JumpCount");

        if (_jumpCountStat != null)
        {
            _currentJumpCount = Mathf.RoundToInt(_jumpCountStat.Value);
        }
    }

    private void Update()
    {
        if (!_isGameStarted)
        {
            return;
        }

        Look();
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        if (!_isGameStarted)
        {
            return;
        }

        Move();
    }

    private void Look()
    {
        float mouseX = _lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = _lookInput.y * mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        cameraPivot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        RaycastHit[] hits = Physics.SphereCastAll(origin, _groundCheckRadius, Vector3.down, _groundCheckDistance, _groundMask, QueryTriggerInteraction.Ignore);

        bool foundGround = false;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && hits[i].collider.transform.root != transform.root)
            {
                foundGround = true;
                break;
            }
        }

        _isGrounded = foundGround;
        if (_isGrounded && _jumpCountStat != null)
        {
            _currentJumpCount = Mathf.RoundToInt(_jumpCountStat.Value);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawWireSphere(origin + Vector3.down * _groundCheckDistance, _groundCheckRadius);
    }

    private void Move()
    {
        if (_movementSpeedStat == null)
        {
            return;
        }

        float speed = _movementSpeedStat.Value;

        if (_isRunning)
        {
            speed *= _sprintMultiplier;
        }

        Vector3 moveDir = (transform.right * _moveInput.x + transform.forward * _moveInput.y).normalized;
        Vector3 targetVelocity = moveDir * speed;

        _rb.velocity = new Vector3(targetVelocity.x, _rb.velocity.y, targetVelocity.z);
    }

    private void Jump()
    {
        if (!_isGameStarted || _jumpForceStat == null)
        {
            return;
        }

        if (_isGrounded || _currentJumpCount > 0)
        {
            float yVelocity = Mathf.Sqrt(_jumpForceStat.Value * -2f * _gravity);
            _rb.velocity = new Vector3(_rb.velocity.x, yVelocity, _rb.velocity.z);
            _currentJumpCount--;

            GameEventsManager.Instance.TriggerPlayerJump();
        }
    }

    public void Push(Vector3 center, float strenght, bool attract = false)
    {
        Vector3 pushDirection = attract ? (center - transform.position) : (transform.position - center);
        if (pushDirection.sqrMagnitude < 0.0001f)
        {
            pushDirection = attract ? -transform.forward : transform.forward;
        }
        else
        {
            pushDirection.Normalize();
        }
        _rb.AddForce(pushDirection * strenght, ForceMode.Impulse);
    }
}