using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Camera))]
public class PlayerBehaviour : MonoBehaviour
{
	private CharacterController controller;
	[SerializeField] GameObject heldItemParent;

	[Header("Movement Properties")]
	[SerializeField] private float moveSpeed = 5.0f;
	[SerializeField] private float gravity = -9.81f;
	[SerializeField] private float jumpHeight = 1.5f;
	[SerializeField] private float smoothTimeGrounded = 0.1f;
	[SerializeField] private float smoothTimeAir = 0.3f;

	private Vector3 velocity;
	private Vector3 moveDirection;
	private Vector3 smoothMoveVelocity;

	[Header("Mouse Look Properties")]
	[SerializeField] private float mouseSensitivity = 10.0f;

	[Header("Player Properties")]
	[SerializeField] private float playerHealth = 100.0f;
	[SerializeField] private float playerHunger = 100.0f;
	[SerializeField] private float playerThirst = 100.0f;
	[SerializeField] private float playerStamina = 100.0f;

	[Header("Inventory Properties")]
	//Will make inventory its own class later
	[SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();




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
		HandleNumberKeyPress();
		HandleMovement();
		HandleMouseLook();
	}

	void HandleNumberKeyPress()
	{
		for (int i = 0; i <= 9; i++)
		{
			if (Input.GetKeyDown(i.ToString()))
			{
				Debug.Log("Pressed " + i);
				//This will be changed later just temporary testing
				//Destroy(heldItemParent.transform.GetChild(0).gameObject);
				if(i > inventory.Count)
				{
					return;
				}
				if (heldItemParent.transform.childCount > 0)
				{
					Destroy(heldItemParent.transform.GetChild(0).gameObject);
				}
				GameObject newItem = Instantiate(inventory[i - 1].itemPrefab, heldItemParent.transform);
			}
		}
	}

	void HandleMovement()
	{

		float moveX = Input.GetAxisRaw("Horizontal");
		float moveZ = Input.GetAxisRaw("Vertical");

		// Calculate target movement direction based on player's orientation
		Vector3 targetMoveDirection = (transform.right * moveX + transform.forward * moveZ).normalized * moveSpeed;

		// Use a longer smooth time when in air for gradual deceleration
		float smoothTime = controller.isGrounded ? smoothTimeGrounded : smoothTimeAir;
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
		//Input internally uses deltaTime already
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

		transform.rotation = Quaternion.Euler(transform.eulerAngles.x - mouseY, transform.eulerAngles.y + mouseX, 0);
	}

}
