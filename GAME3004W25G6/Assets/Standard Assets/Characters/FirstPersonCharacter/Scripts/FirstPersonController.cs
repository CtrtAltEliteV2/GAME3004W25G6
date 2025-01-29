using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckRadius = 0.4f;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float mouseSensitivity = 100f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private CharacterController characterController;
    private AudioSource audioSource;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning;
    private float verticalRotation;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction runAction;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        playerInput = new PlayerInput();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        runAction = playerInput.actions["Run"];
    }

   
    private void Update()
    {
        HandleMovement();
        HandleCameraRotation();
    }

    private void HandleMovement()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            PlaySound(landSound);
        }

        // Movement Input
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        float speed = runAction.IsPressed() ? runSpeed : walkSpeed;
        characterController.Move(move * speed * Time.deltaTime);

        // Jump Input
        if (jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            PlaySound(jumpSound);
        }

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Footstep Sounds
        if (isGrounded && characterController.velocity.magnitude > 0.2f && !audioSource.isPlaying)
        {
            PlayFootstepSound();
        }
    }

    private void HandleCameraRotation()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>() * mouseSensitivity * Time.deltaTime;

        verticalRotation -= lookInput.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * lookInput.x);
    }

    private void PlayFootstepSound()
    {
        if (footstepSounds.Length == 0) return;

        int index = UnityEngine.Random.Range(0, footstepSounds.Length);
        PlaySound(footstepSounds[index]);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
