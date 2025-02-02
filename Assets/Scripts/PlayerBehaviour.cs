using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Camera))]
public class PlayerBehaviour : MonoBehaviour
{
	private CharacterController controller;

	[Header("Movement Properties")]
	[SerializeField] private float moveSpeed = 5.0f;
	[SerializeField] private float gravity = -9.81f;
	[SerializeField] private float jumpHeight = 1.5f;

	private Vector3 velocity;
	private Vector3 moveDirection;
	private Vector3 smoothMoveVelocity;

	[Header("Mouse Look Properties")]
	[SerializeField] private float mouseSensitivity = 10.0f;
	[SerializeField] private float maxLookAngle = 90.0f;

	private float xRotation = 0.0f;

	void Start()
	{
		// Get components
		controller = GetComponent<CharacterController>();
		if (controller == null)
		{
			Debug.LogError("CharacterController component not found.");
		}


		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		velocity = Vector3.zero;
	}

	void Update()
	{
		HandleMovement();
		HandleMouseLook();
	}

	void HandleMovement()
	{

		float moveX = Input.GetAxisRaw("Horizontal");
		float moveZ = Input.GetAxisRaw("Vertical");

		// Calculate target movement direction based on player's orientation
		Vector3 targetMoveDirection = (transform.right * moveX + transform.forward * moveZ).normalized * moveSpeed;

		// Use a longer smooth time when in air for gradual deceleration
		float smoothTime = controller.isGrounded ? 0.1f : 0.3f;
		moveDirection = Vector3.SmoothDamp(moveDirection, targetMoveDirection, ref smoothMoveVelocity, smoothTime);

		if (Input.GetButtonDown("Jump") && controller.isGrounded)
		{
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
		}

		velocity.y += gravity * Time.deltaTime;

		// Combine horizontal smooth movement with vertical velocity
		Vector3 finalMoveDirection = moveDirection + Vector3.up * velocity.y;
		controller.Move(finalMoveDirection * Time.deltaTime);
	}

	void HandleMouseLook()
	{

		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

		transform.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y + mouseX, 0);
	}

}
