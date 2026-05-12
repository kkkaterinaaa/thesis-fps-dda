using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;
    public float mouseSensitivity = 2f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("Jump feel")]
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;
    private float coyoteTimer;
    private float jumpBufferTimer;

    private CharacterController controller;
    private Camera cam;
    private float verticalRotation = 0f;

    private float originalHeight;
    private bool isCrouching = false;
    private Vector3 velocity;

    private float crouchHeight = 0.9f;
    private float standHeight = 2f;

    [Header("Audio")]
    public PlayerAudio playerAudio;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;

        originalHeight = controller.height;

        if (playerAudio == null) playerAudio = GetComponent<PlayerAudio>();
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleActions();
    }

    void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
        cam.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = speed;
        if (sprinting) currentSpeed = sprintSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        bool grounded = controller.isGrounded;

        if (grounded) coyoteTimer = coyoteTime;
        else          coyoteTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Jump")) jumpBufferTimer = jumpBufferTime;
        else                             jumpBufferTimer -= Time.deltaTime;

        bool jumped = false;
        if (grounded && velocity.y < 0f)
            velocity.y = -2f;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumped = true;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        if (!grounded)
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (playerAudio != null)
        {
            float hSpeed = move.magnitude * currentSpeed;
            PlayerAudio.MotionKind kind;
            if (hSpeed < 0.1f)       kind = PlayerAudio.MotionKind.Idle;
            else if (isCrouching)    kind = PlayerAudio.MotionKind.Crouch;
            else if (sprinting)      kind = PlayerAudio.MotionKind.Sprint;
            else                     kind = PlayerAudio.MotionKind.Walk;

            playerAudio.TickFootsteps(controller.isGrounded, hSpeed, kind);
            if (jumped) playerAudio.PlayJump();
        }
    }

    void HandleActions()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!isCrouching)
            {
                isCrouching = true;
                controller.height = crouchHeight;
            }
        }
        else
        {
            if (isCrouching)
            {
                isCrouching = false;
                controller.height = standHeight;
            }
        }

        if (Input.GetMouseButton(1))
        {
            cam.fieldOfView = 60f;
        }
        else
        {
            cam.fieldOfView = 90f;
        }

    }
}