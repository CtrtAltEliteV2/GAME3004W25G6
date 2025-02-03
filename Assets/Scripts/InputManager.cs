using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	[SerializeField] private float mouseSensitivity = 10f;

	public Vector2 GetMovementInput()
	{
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		return new Vector2(horizontal, vertical);
	}

	public bool GetJumpInput()
	{
		return Input.GetButtonDown("Jump");
	}

	public Vector2 GetMouseLookInput()
	{
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
		return new Vector2(mouseX, mouseY);
	}

	public int GetHotbarSlotInput()
	{
		// Check keys 1-9 (which map to slot indices 0–8)
		for (int i = 1; i <= 9; i++)
		{
			if (Input.GetKeyDown(i.ToString()))
			{
				return i - 1; // convert to 0-indexed
			}
		}
		// Check key "0" for the tenth slot (index 9)
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			return 9;
		}
		return -1;
	}

	internal bool GetInventoryInput()
	{
		return Input.GetKeyDown(KeyCode.I);
	}
}
