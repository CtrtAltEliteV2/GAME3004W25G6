using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	private CharacterController controller;
	private InputManager inputManager;
	private PlayerStats playerStats;
	private InventoryManager inventoryManager;
	private bool isInventoryOpen = false;

	[Header("Movement Properties")]
	[SerializeField] private float moveSpeed = 5.0f;
	[SerializeField] private float gravity = -9.81f;
	[SerializeField] private float jumpHeight = 1.5f;
	[SerializeField] private float smoothTimeGrounded = 0.1f;
	[SerializeField] private float smoothTimeAir = 0.3f;

	private Vector3 velocity;
	private Vector3 moveDirection;
	private Vector3 smoothMoveVelocity;

	[Header("Inventory & Held Item")]
	[Tooltip("The parent GameObject under which the held item prefab will be instantiated.")]
	[SerializeField] private GameObject heldItemParent;

	void Start()
	{
		controller = GetComponent<CharacterController>();
		inputManager = GetComponent<InputManager>();
		playerStats = GetComponent<PlayerStats>();
		inventoryManager = GetComponent<InventoryManager>();
		if (controller == null)
			Debug.LogError("CharacterController component not found.");

		if (heldItemParent == null)
			Debug.LogError("HeldItemParent GameObject is not assigned in the Inspector.");


		velocity = Vector3.zero;
		moveDirection = Vector3.zero;
		smoothMoveVelocity = Vector3.zero;

		// Initialize the inventory UI (which will also assign any starting items)
		if (inventoryManager != null)
		{
			inventoryManager.InitializeInventoryUI();
		}
		LockCursor();
	}

	void Update()
	{
		if (isInventoryOpen)
		{
			HandleInventoryInput();
			return;
		}
		if(inputManager.GetUseInput())
		{
			UseSelectedItem();
		}
		HandleHotbarInput();
		HandleInventoryInput();
		HandleMovement();
		HandleMouseLook();
	}
	void HandleInventoryInput()
	{
		if (inputManager.GetInventoryInput())
		{
			isInventoryOpen = !isInventoryOpen;
			if (isInventoryOpen)
			{
				UnlockCursor();
			}
			else
			{
				LockCursor();
			}
			inventoryManager.ToggleExtendedInventory();
		}
	}
	void HandleHotbarInput()
	{
		int slotInput = inputManager.GetHotbarSlotInput();
		if (slotInput != -1)
		{
			SelectHotbarSlot(slotInput);
		}
	}

	void SelectHotbarSlot(int index)
	{
		if (inventoryManager == null)
		{
			Debug.LogError("InventoryManager is not assigned.");
			return;
		}

		// Update the selected slot in the UI
		inventoryManager.SetSelectedHotbarSlot(index);

		// Remove any previously held item
		if (heldItemParent != null)
		{
			foreach (Transform child in heldItemParent.transform)
			{
				Destroy(child.gameObject);
			}
		}
		else
		{
			Debug.LogError("HeldItemParent GameObject is not assigned.");
			return;
		}

		// Get the InventoryItem from the selected slot and instantiate its prefab if available
		InventoryItem selectedItem = inventoryManager.GetItemInHotbar(index);
		if (selectedItem != null &&
			selectedItem.itemData != null &&
			selectedItem.itemData.itemPrefab != null)
		{
			Instantiate(selectedItem.itemData.itemPrefab, heldItemParent.transform);
		}
	}


	void HandleMovement()
	{
		Vector2 movementInput = inputManager.GetMovementInput();
		float moveX = movementInput.x;
		float moveZ = movementInput.y;

		// Calculate target move direction in world space
		Vector3 targetMoveDirection = (transform.right * moveX + transform.forward * moveZ).normalized * moveSpeed;

		// Use different smoothing times based on whether the player is grounded
		float smoothTime = controller.isGrounded ? smoothTimeGrounded : smoothTimeAir;
		moveDirection = Vector3.SmoothDamp(moveDirection, targetMoveDirection, ref smoothMoveVelocity, smoothTime);

		// Handle jumping
		if (inputManager.GetJumpInput() && controller.isGrounded)
		{
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
		}

		// Apply gravity
		velocity.y += gravity * Time.deltaTime;

		// Move the player
		Vector3 finalMoveDirection = moveDirection + Vector3.up * velocity.y;
		controller.Move(finalMoveDirection * Time.deltaTime);
	}

	void HandleMouseLook()
	{
		Vector2 mouseDelta = inputManager.GetMouseLookInput();
		transform.rotation = Quaternion.Euler(transform.eulerAngles.x - mouseDelta.y, transform.eulerAngles.y + mouseDelta.x, 0);
	}
	void LockCursor()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	void UnlockCursor()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
	void UseSelectedItem()
	{
		// Get the selected item from the inventory
		InventoryItem selectedItem = inventoryManager.GetCurrentItem();
		if (selectedItem == null || selectedItem.itemData == null)
		{
			Debug.Log("No item selected.");
			return;
		}
		Debug.Log("Using item: " + selectedItem.itemData.itemName);
	}
}
