using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerBehaviour : MonoBehaviour
{
	// Components
	private CharacterController controller;
	private Transform cameraTransform;

	// Movement properties
	[Header("Movement Properties")]
	[SerializeField] private float moveSpeed = 5.0f;
	[SerializeField] private float gravity = -9.81f;
	[SerializeField] private float jumpHeight = 1.5f;

	private Vector3 velocity;
	private Vector3 moveDirection;
	private Vector3 smoothMoveVelocity;
	private bool isGrounded;

	// Mouse look properties
	[Header("Mouse Look Properties")]
	[SerializeField] private float mouseSensitivity = 10.0f;
	[SerializeField] private float maxLookAngle = 90.0f;

	private float xRotation = 0.0f; // Vertical rotation

	// Ground detection properties
	[Header("Ground Detection Properties")]
	[SerializeField] private Transform groundCheck;
	[SerializeField] private float groundDistance = 1.1f;
	[SerializeField] private LayerMask groundMask;

	void Start()
	{
		// Get components
		controller = GetComponent<CharacterController>();
		if (controller == null)
		{
			Debug.LogError("CharacterController component not found.");
		}

		cameraTransform = GetComponentInChildren<Camera>()?.transform;
		if (cameraTransform == null)
		{
			Debug.LogError("No camera found as a child of the player. Please ensure the Main Camera is a child of the player.");
		}
		else
		{
			// Initialize camera rotation to look at eye level
			cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
		}

		// Lock and hide the cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		if (groundCheck == null)
		{
			Debug.LogError("GroundCheck is not assigned in the Inspector.");
		}
	}

	void Update()
	{
		HandleMovement();
		HandleMouseLook();
	}

	void HandleMovement()
	{
		// Ground Check
		isGrounded = groundCheck != null && Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

		if (isGrounded && velocity.y < 0)
		{
			velocity.y = -2f; // Small downward force to keep grounded
		}

		// Input
		float moveX = Input.GetAxisRaw("Horizontal");
		float moveZ = Input.GetAxisRaw("Vertical");

		// Calculate target movement direction based on player's orientation
		Vector3 targetMoveDirection = (transform.right * moveX + transform.forward * moveZ).normalized * moveSpeed;

		// Smooth movement
		moveDirection = Vector3.SmoothDamp(moveDirection, targetMoveDirection, ref smoothMoveVelocity, 0.1f);

		// Move the player
		controller.Move(moveDirection * Time.deltaTime);

		// Jump
		if (Input.GetButtonDown("Jump") && isGrounded)
		{
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
		}

		// Gravity
		velocity.y += gravity * Time.deltaTime;

		// Apply vertical movement separately
		controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
	}

	void HandleMouseLook()
	{
		if (cameraTransform == null)
		{
			return;
		}

		// Mouse Input
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

		// Rotate player horizontally
		transform.Rotate(Vector3.up * mouseX);

		// Rotate camera vertically
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

		cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
	}

	void OnDrawGizmos()
	{
		if (groundCheck != null)
		{
			// Visualize ground check sphere
			Gizmos.color = isGrounded ? Color.green : Color.red;
			Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
		}
	}
}
