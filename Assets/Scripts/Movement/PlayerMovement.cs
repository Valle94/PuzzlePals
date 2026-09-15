using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Camera camera;
    [SerializeField] CharacterController characterController;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float horizontalRotationSpeed = 15f;
    [SerializeField] private float verticalRotationSpeed = 20f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 moveDirection;
    private Vector3 playerRotation;
    private float verticalVelocity;
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize Components
        if (characterController == null) { characterController = GetComponent<CharacterController>(); }
        if (camera == null) { camera = GetComponentInChildren<Camera>(); }

        // Set cursor behavior
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
        CalcVerticalVelocity();
    }

    // This method handles player movement in the x/z plane.
    private void Move()
    {
        Vector3 forward = transform.forward;        // Get world vector for forward direction
        Vector3 right = transform.right;            // Get world vector for right direction
        
        // Calulate directional vector
        moveDirection = (forward * moveInput.y + right * moveInput.x) * moveSpeed;  

        moveDirection.y = verticalVelocity;         // Set vertical movement to vertical velocity
        characterController.Move(moveDirection * Time.deltaTime); // Execute movement normalized by deltaTime
    }

    // This method handles horizontal and vertical rotation of the player and camera
    private void Rotate()
    {
        // Rotate the player around the y-axis (left/right)
        transform.Rotate(0f, playerRotation.y * horizontalRotationSpeed * Time.deltaTime, 0f);

        // Rotate just the camera around the x-axis (up/down)
        camera.transform.Rotate(playerRotation.x * verticalRotationSpeed * Time.deltaTime, 0f, 0f);
    }

    // This method calculates the vertical velocity of the player
    private void CalcVerticalVelocity()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small downward force to keep grounded state sticky
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    // The following methods are action calls using the InputSystemActions
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            // Physics formula for jump height: sqrt(height * -2 * gravity)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
        playerRotation = new Vector3(-lookInput.y, lookInput.x, 0f);
        Debug.Log(lookInput);
    }
}