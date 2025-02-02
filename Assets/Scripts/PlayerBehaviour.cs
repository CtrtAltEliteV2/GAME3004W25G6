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

		cameraTransform = GetComponent<Camera>()?.transform;
		if (cameraTransform == null)
		{
			Debug.LogError("Camera component not found on the Player GameObject.");
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

		 // Use a longer smooth time when in air for gradual deceleration
		float smoothTime = isGrounded ? 0.1f : 0.3f;
		moveDirection = Vector3.SmoothDamp(moveDirection, targetMoveDirection, ref smoothMoveVelocity, smoothTime);

		// Jump
		if (Input.GetButtonDown("Jump") && isGrounded)
		{
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
		}

		// Gravity
		velocity.y += gravity * Time.deltaTime;

		 // Combine horizontal smooth movement with vertical velocity
		Vector3 finalMoveDirection = new Vector3(moveDirection.x, velocity.y, moveDirection.z);
		controller.Move(finalMoveDirection * Time.deltaTime);
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
		xRotation -= mouseY;
    	xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);


    	transform.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y + mouseX, 0f);
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
