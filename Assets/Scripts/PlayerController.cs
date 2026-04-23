using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;
    public float mouseSensitivity = 2f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Camera cam;
    private float verticalRotation = 0f;

    private float originalHeight;
    private bool isCrouching = false;
    private Vector3 velocity;

    private float crouchHeight = 0.9f;
    private float standHeight = 2f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;

        originalHeight = controller.height;
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

        float currentSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }

        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded)
        {
            velocity.y = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
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