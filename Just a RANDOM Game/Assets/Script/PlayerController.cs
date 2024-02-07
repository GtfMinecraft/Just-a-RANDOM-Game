using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float groundAcceleration; // acceleration while grounded
	[SerializeField] private float airAcceleration; // acceleration while not grounded
	[SerializeField] private float maxSpeed; // maximum speed of player
	[SerializeField] private float jumpSpeed; // initial speed of the jump
	[SerializeField] private float gravity; // rate at which player accelerates downwards
	[SerializeField] private float groundFriction; // rate at which player slows down horizontally while grounded
	[SerializeField] private float airResistance; // rate at which player slows down horizontally while not grounded
	[SerializeField] private float mouseSensitivity; // how fast camera turns in respect to mouse movement
	[SerializeField] private Camera playerView;

	private Vector2 currentMovement;
	private Vector3 currentVelocity;
	private bool isGrounded;
	private float currentHorizontalSpeed;
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
		playerCharacterController.Move(currentVelocity * Time.deltaTime);
		currentVelocity = UpdateVelocity(currentVelocity);

		// handle player rotation
		playerView.transform.localEulerAngles = new Vector3(currentLookAngle, 0, 0);
	}

	private Vector3 UpdateVelocity(Vector3 currentVelocity) 
	{
		Vector3 targetVelocity = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * new Vector3(currentMovement.x, 0, currentMovement.y) * maxSpeed;
		Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);

		isGrounded = playerCharacterController.isGrounded;

		// update horizontal velocity
		if (targetVelocity == Vector3.zero)
			horizontalVelocity *= Mathf.Pow(0.5f, (isGrounded ? groundFriction : airResistance) * Time.deltaTime);
		else if (isGrounded) 
		{
			currentHorizontalSpeed += groundAcceleration * Time.deltaTime;
			horizontalVelocity = currentHorizontalSpeed * targetVelocity;
		}
		else
			horizontalVelocity += targetVelocity * airAcceleration * Time.deltaTime;
			
		// update vertical velocity
		// slight downward velocity is still needed while grounded to make playerCharacterController.isGrounded work
		if (isGrounded)
			currentVelocity.y = 0f;
		currentVelocity.y -= gravity * Time.deltaTime;

		// limit speed
		horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeed);

		return new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
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
		if (isGrounded && ctx.performed)
			currentVelocity.y = jumpSpeed;
	}
}
