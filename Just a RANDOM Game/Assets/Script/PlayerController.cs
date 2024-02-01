using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float speed;
	[SerializeField] private float jumpSpeed;
	[SerializeField] private float gravity;
	[SerializeField] private float mouseSensitivity;
	[SerializeField] private Camera playerView;

	private Vector2 currentMovement;
	private float verticalVelocity = 0f;
	private GameObject currentlyHolding;
	private float currentLookAngle = 0f;

	private CharacterController playerCharacterController;

	private void Awake()
	{
		playerCharacterController = GetComponent<CharacterController>();
	}

	private void Update()
	{
		// handle movement
		Vector3 horizontalVelocity = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * new Vector3(currentMovement.x, 0, currentMovement.y) * speed;
		playerCharacterController.Move(new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z) * Time.deltaTime);

		// handle camera rotation
		playerView.transform.localEulerAngles = new Vector3(currentLookAngle, 0, 0);

		// update vertical velocity
		// THIS NEEDS TO HAPPEN AFTER MOVEMENT IS HANDLED im fucking dumb :/
		if (playerCharacterController.isGrounded)
			verticalVelocity = 0f;
		verticalVelocity -= gravity * Time.deltaTime;
	}

	public void MovementHandler(InputAction.CallbackContext ctx) 
	{
		currentMovement = ctx.ReadValue<Vector2>();
	}

	public void LookHandler(InputAction.CallbackContext ctx)
	{
		Vector2 lookRotation = ctx.ReadValue<Vector2>() * mouseSensitivity;

		// character rotation
		transform.eulerAngles += new Vector3(0, lookRotation.x, 0);

		// camera rotation
		currentLookAngle -= lookRotation.y;

		// normalize angle
		if (currentLookAngle < 0f)
			currentLookAngle += 360f;
		else if (currentLookAngle > 360f)
			currentLookAngle -= 360f;

		// clamp angle
		if (currentLookAngle > 90f && currentLookAngle < 180f)
			currentLookAngle = 90f;
		else if (currentLookAngle > 180f && currentLookAngle < 270f)
			currentLookAngle = 270f;
	}

	public void JumpHandler(InputAction.CallbackContext ctx)
	{
		if (playerCharacterController.isGrounded && ctx.performed) {
			verticalVelocity = jumpSpeed;
		}
	}
}
