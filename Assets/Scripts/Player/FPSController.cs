using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FPSController : MonoBehaviour, IPusheable
{
    [Header("Movimiento")]
    [SerializeField] public Stat walkSpeedStat;
    [SerializeField] public Stat runSpeedStat;

    [Header("Salto")]
    [SerializeField] private Stat jumpForceStat;
    [SerializeField] private Stat jumpCountStat;
    private int currentJumpCount;
    [SerializeField] private float gravity = -9.81f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 1.25f;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundMask = ~0;
    private bool isGrounded;

    [Header("Cámara")]
    [SerializeField] public Transform cameraPivot;
    [SerializeField] public float mouseSensitivity = 100f;

    private Rigidbody rb;
    private PlayerInputActions input;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float xRotation;
    private bool isRunning;
    private bool _isGameStarted;

    public bool IsGrounded => isGrounded;
    public bool IsRunning => isRunning;
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;
    public Vector2 MoveInput => moveInput;
    public Vector3 Velocity => rb != null ? rb.velocity : Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        input = new PlayerInputActions();

        // INPUTS
        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        input.Player.Run.performed += _ => isRunning = true;
        input.Player.Run.canceled += _ => isRunning = false;

        input.Player.Jump.performed += _ => Jump();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.OnGameStarted += HandleGameStarted;
            if (GameEventsManager.Instance.IsGameStarted)
            {
                HandleGameStarted();
            }
            else
            {
                CursorManager.SetCursorVisible(true);
            }
        }
        else
        {
            HandleGameStarted();
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.OnGameStarted -= HandleGameStarted;
        }
    }

    private void HandleGameStarted()
    {
        _isGameStarted = true;
        CursorManager.SetCursorVisible(false);

        walkSpeedStat = PlayerStatsManager.Instance.GetStatByName("WalkSpeed");
        runSpeedStat = PlayerStatsManager.Instance.GetStatByName("RunSpeed");

        jumpForceStat = PlayerStatsManager.Instance.GetStatByName("JumpHeight");
        jumpCountStat = PlayerStatsManager.Instance.GetStatByName("JumpCount");

        if (jumpCountStat != null)
        {
            currentJumpCount = Mathf.RoundToInt(jumpCountStat.Value);
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
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        RaycastHit[] hits = Physics.SphereCastAll(origin, groundCheckRadius, Vector3.down, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
        
        bool foundGround = false;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && hits[i].collider.transform.root != transform.root)
            {
                foundGround = true;
                break;
            }
        }

        isGrounded = foundGround;
        if (isGrounded && jumpCountStat != null)
        {
            currentJumpCount = Mathf.RoundToInt(jumpCountStat.Value);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawWireSphere(origin + Vector3.down * groundCheckDistance, groundCheckRadius);
    }

    private void Move()
    {
        if (walkSpeedStat == null || runSpeedStat == null)
        {
            return;
        }

        float speed = walkSpeedStat.Value;

        if (isRunning)
        {
            speed = runSpeedStat.Value;
        }

        Vector3 moveDir = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        Vector3 targetVelocity = moveDir * speed;

        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    private void Jump()
    {
        if (!_isGameStarted || jumpForceStat == null)
        {
            return;
        }

        if (isGrounded || currentJumpCount > 0)
        {
            float yVelocity = Mathf.Sqrt(jumpForceStat.Value * -2f * gravity);
            rb.velocity = new Vector3(rb.velocity.x, yVelocity, rb.velocity.z);
            currentJumpCount--;

            if (GameEventsManager.Instance != null)
            {
                GameEventsManager.Instance.TriggerPlayerJump();
            }
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
        rb.AddForce(pushDirection * strenght, ForceMode.Impulse);
    }
}