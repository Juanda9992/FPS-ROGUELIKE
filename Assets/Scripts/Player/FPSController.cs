using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float crouchSpeed = 2.5f;

    [Header("Salto")]
    public float jumpForce = 1.5f;
    public float gravity = -9.81f;

    [Header("Cámara")]
    public Transform cameraPivot;
    public float mouseSensitivity = 100f;

    [Header("Agacharse")]
    public float crouchHeight = 1f;
    public float normalHeight = 2f;

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
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Look();
        Move();
        ApplyGravity();
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
        float speed = walkSpeed;

        if (isCrouching)
        {
            speed = crouchSpeed;
        }
        else if (isRunning)
        {
            speed = runSpeed;
        }

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);
    }

    void Jump()
    {
        if (controller.isGrounded && !isCrouching)
        {
            yVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
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