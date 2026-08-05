using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] public Stat walkSpeedStat;
    [SerializeField] public Stat runSpeedStat;
    [SerializeField] public Stat crouchSpeedStat;

    [Header("Salto")]
    [SerializeField] private Stat jumpForceStat; 
    [SerializeField] private Stat jumpCountStat; 
    private int currentJumpCount;
    [SerializeField] private float gravity = -9.81f;

    [Header("Cámara")]
    [SerializeField] public Transform cameraPivot;
    [SerializeField] public float mouseSensitivity = 100f;

    [Header("Agacharse")]
    [SerializeField] public float crouchHeight = 1f;
    [SerializeField] public float normalHeight = 2f;

    private CharacterController controller;
    private PlayerInputActions input;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float yVelocity;
    private float xRotation;

    private bool isRunning;
    private bool isCrouching;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new PlayerInputActions();

        // INPUTS
        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        input.Player.Run.performed += _ => isRunning = true;
        input.Player.Run.canceled += _ => isRunning = false;

        input.Player.Crouch.performed += _ => ToggleCrouch();

        input.Player.Jump.performed += _ => Jump();
    }

    void OnEnable()
    {
        input.Enable();
    }
    void OnDisable() 
    {
        input.Disable();
    }
    void Start()
    {
        CursorManager.SetCursorVisible(false);

        walkSpeedStat = PlayerStatsManager.Instance.GetStatByName("WalkSpeed");
        runSpeedStat = PlayerStatsManager.Instance.GetStatByName("RunSpeed");
        crouchSpeedStat = PlayerStatsManager.Instance.GetStatByName("CrouchSpeed");

        jumpForceStat = PlayerStatsManager.Instance.GetStatByName("JumpHeight");
        jumpCountStat = PlayerStatsManager.Instance.GetStatByName("JumpCount");

        currentJumpCount = Mathf.RoundToInt(jumpCountStat.Value);
    }

    void Update()
    {
        Look();
        Move();
        ApplyGravity();

        if(controller.isGrounded)
        {
            currentJumpCount = Mathf.RoundToInt(jumpCountStat.Value);
        }
    }

    void Look()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        float speed = walkSpeedStat.Value;

        if (isCrouching)
        {
            speed = crouchSpeedStat.Value;
        }
        else if (isRunning)
        {
            speed = runSpeedStat.Value;
        }

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);
    }

    void Jump()
    {
        if ((controller.isGrounded || currentJumpCount > 0) && !isCrouching)
        {
            yVelocity = Mathf.Sqrt(jumpForceStat.Value * -2f * gravity);
            currentJumpCount--;
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
        }
        yVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * yVelocity * Time.deltaTime);
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        controller.height = isCrouching ? crouchHeight : normalHeight;

        cameraPivot.localPosition = new Vector3(
            0,
            isCrouching ? crouchHeight / 2f : normalHeight / 2f,
            0
        );
    }
}