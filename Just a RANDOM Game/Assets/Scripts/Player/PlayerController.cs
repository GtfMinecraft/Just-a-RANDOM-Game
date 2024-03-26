using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
	[Header("Movement")]
    [SerializeField] private float groundAcceleration; // acceleration while grounded
	[SerializeField] private float airAcceleration; // acceleration while not grounded
	[SerializeField] private float maxSpeed; // maximum speed of player
	[SerializeField] private float jumpSpeed; // initial speed of the jump
	[SerializeField] private float dashSpeed;
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float runMultiplier;
    [SerializeField] private float gravity; // rate at which player accelerates downwards
	[SerializeField] private float groundFriction; // rate at which player slows down horizontally while grounded
	[SerializeField] private float airResistance; // rate at which player slows down horizontally while not grounded

	public Transform orientation;
	public Transform playerObj;

	[Header("Pick Item")]
    public Vector3 boxCastSize;

    [Header("Bot")]
    public GameObject bot;

	[HideInInspector]
    public bool canMove = true;
    [HideInInspector]
    public bool canRotate = true;
    [HideInInspector]
    public bool canControl = true;

    private Vector2 currentMovement;
	private Vector3 currentVelocity;
	private Vector3 targetVelocity;
	private Vector3 horizontalVelocity;
    private bool isGrounded;
    private bool isRunning;
	private bool isJumping;
	private bool isDashing;
	private bool isPicking = false;
	private bool nowDashing = false;
    private float dashTimer;
	private float dashTime;
	private float dashCooldownTracker = 0f;

	private CharacterController playerCharacterController;

	private void Awake()
	{
		playerCharacterController = GetComponent<CharacterController>();

		dashTime = dashDistance / dashSpeed;
    }

    private void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

    private void Update()
	{
        isGrounded = playerCharacterController.isGrounded;

        if (canMove)
		{
            UpdateVelocity();

            // handle movement
            playerCharacterController.Move(currentVelocity * Time.deltaTime);
        }

		if (canControl)
		{
            // pick up item
            if (isPicking)
            {
                Collider[] hits;

                hits = Physics.OverlapBox(playerObj.position + boxCastSize.z / 2 * playerObj.forward, boxCastSize / 2, playerObj.rotation);

                foreach (Collider hit in hits)
                {
                    if (hit.GetComponent<Interactable>() != null)
                    {
                        hit.GetComponent<Interactable>().Interact();
                    }
                }
            }
        }
	}

	private void UpdateVelocity()
	{
        float speedMultiplier = isRunning ? runMultiplier : 1f;

		if (!nowDashing)
		{
			targetVelocity = Quaternion.AngleAxis(orientation.eulerAngles.y, Vector3.up) * new Vector3(currentMovement.x, 0, currentMovement.y) * maxSpeed * speedMultiplier;
			horizontalVelocity = new Vector3(playerCharacterController.velocity.x, 0, playerCharacterController.velocity.z);

			if (isGrounded && isJumping)
			{
				currentVelocity.y = jumpSpeed;
			}

			// update horizontal velocity
			if (targetVelocity == Vector3.zero)
				horizontalVelocity *= Mathf.Pow(0.5f, (isGrounded ? groundFriction : airResistance) * Time.deltaTime);
			else if (isGrounded)
			{
				horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetVelocity, groundAcceleration * speedMultiplier * Time.deltaTime);
			}
			else
			{
				horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetVelocity, airAcceleration * speedMultiplier * Time.deltaTime);
			}

            // update vertical velocity
            if (!isGrounded)
                currentVelocity.y -= gravity * Time.deltaTime;

			// limit speed
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeed * speedMultiplier);
        }

        if (nowDashing)
        {
			if(Time.time - dashTimer <= dashTime)
			{
				if(targetVelocity == Vector3.zero)
					horizontalVelocity = playerObj.forward * dashSpeed;
				else
	                horizontalVelocity = targetVelocity.normalized * dashSpeed;
            }
			else
			{
				nowDashing = false;
				dashCooldownTracker = dashCooldown;
			}
        }

        if (dashCooldownTracker > 0f)
		{
            dashCooldownTracker -= Time.deltaTime;
        }
        else if (dashCooldownTracker <= 0f && isGrounded && isDashing && !nowDashing)
		{
			nowDashing = true;
			dashTimer = Time.time;
        }

        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, dashSpeed);

		currentVelocity = new Vector3(horizontalVelocity.x, (nowDashing ? 0 : currentVelocity.y), horizontalVelocity.z);
	}

	public void MovementHandler(InputAction.CallbackContext ctx) 
	{
		currentMovement = ctx.ReadValue<Vector2>();
    }

	public void JumpHandler(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			isJumping = true;
		}
		else if(ctx.canceled)
		{
			isJumping = false;
		}
	}

	public void RunHandler(InputAction.CallbackContext ctx)
	{
        if (ctx.performed)
        {
            isRunning = true;
        }
        else if (ctx.canceled)
        {
            isRunning = false;
        }
    }

    public void DashHandler(InputAction.CallbackContext ctx)
    {
		if(ctx.performed)
		{
			isDashing = true;
		}
		else if(ctx.canceled)
		{
			isDashing = false;
		}
    }

    public void PickItemHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isPicking = true;
        }
        else if (ctx.canceled)
        {
            isPicking = false;
        }
    }

    public void EatHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canControl)
        {
            InterfaceHandler.instance.inventoryCanvas.ChangeToolInventory(InventoryTypes.food);
            PlayerItemController.instance.SwapHandItem(PlayerItemController.instance.defaultItems[6]);
            //eat anim
        }
        else if (ctx.canceled)
        {
            InterfaceHandler.instance.inventoryCanvas.ChangeToolInventory(PlayerChunkInteraction.instance.currentChunk);
        }
    }

    public void CallBotHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canControl)
        {
            //anim
            bot.GetComponent<BotMovement>().Call();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 boxCastOrigin = playerObj.position + boxCastSize.z / 2 * playerObj.forward;
        Quaternion boxCastRotation = playerObj.rotation;

        // BoxCast Gizmos
        Gizmos.matrix = Matrix4x4.TRS(boxCastOrigin, boxCastRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxCastSize);
        Gizmos.matrix = Matrix4x4.identity; // Reset Gizmos matrix
    }
}
