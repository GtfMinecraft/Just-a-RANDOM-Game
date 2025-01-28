using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Camera playerCam;

	[Header("Movement")]
    [SerializeField] private float groundAcceleration; // acceleration while grounded
	[SerializeField] private float airAcceleration; // acceleration while not grounded
	[SerializeField] private float maxSpeed; // maximum speed of player
	[SerializeField] private float jumpSpeed; // initial speed of the jump
    [SerializeField] private bool dashInAir;
	[SerializeField] private float dashSpeed; // *** speed and cooldown have to be in sync with anim
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float runMultiplier;
    [SerializeField] private float gravity; // rate at which player accelerates downwards
	[SerializeField] private float groundFriction; // rate at which player slows down horizontally while grounded
	[SerializeField] private float airResistance; // rate at which player slows down horizontally while not grounded
    [SerializeField] private float midAirUseDistance; // the distance above ground where player can use items
    
    public Transform orientation;
	public Transform playerObj;

    [Header("Interact")]
    public Vector3 boxCastSize;

    [Header("Animation")]
    public Animator anim;
    public float animTranceDelay = 0.2f;
    private enum AnimTrance
    {
        None,
        Dash,
        Interact,
        RightHand,
        LeftHand,
    }
    private AnimTrance animTrance = AnimTrance.None;

    [Header("Bot")]
    public GameObject bot;

	[HideInInspector]
    public bool canMove = true;
    [HideInInspector]
    public bool canRotate = true;
    [HideInInspector]
    public bool canControl = true;

    [HideInInspector]
    public Vector2 currentMovement;
	private Vector3 currentVelocity;
	private Vector3 targetVelocity;
	private Vector3 horizontalVelocity;
    private bool isGrounded;
    private bool isRunning;
	private bool isJumping;
	private bool isInteracting = false;
    [HideInInspector]
    public bool forcedInteraction = false;
    private bool isDashing;
    private bool nowDashing = false;
    private float dashTimer;
	private float dashCooldownTracker = 0f;
    private bool usingRight;
    private bool nowRight = false;
    private bool usingLeft;
    private bool nowLeft = false;

	private CharacterController playerCharacterController;

	private void Awake()
	{
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        playerCharacterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

    private void Update()
	{
        isGrounded = playerCharacterController.isGrounded;
        int playerAction = anim.GetInteger("PlayerAction");
        bool canInteract = false;

        canInteract = InteractablePrompt();

        if (canControl)
		{
            // interact
            if (isInteracting && canInteract)
            {
                Interact();
            }

            // possibly implement double-wielding
            if (usingRight && (playerAction == 0 || playerAction == 2 || playerAction == 4))
            {
                UseItem(true);
            }

            if (usingLeft && (playerAction == 0 || playerAction == 2 || playerAction == 3))
            {
                UseItem(false);
            }
        }

        RunAnimTrance();
    }

    private void FixedUpdate()
    {
        if (MovementIsEnable())
        {
            UpdateVelocity();

            // handle movement
            playerCharacterController.Move(currentVelocity * Time.deltaTime);
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
        else if (anim.GetInteger("PlayerAction") == 1 && !nowDashing && dashCooldownTracker <= 0f)
        {
            anim.SetInteger("PlayerAction", 0);
        }
        else if (anim.GetInteger("PlayerAction") == 0 && dashCooldownTracker <= 0f && (isGrounded || dashInAir) && isDashing && !nowDashing)
		{
			nowDashing = true;
            isDashing = false;
            dashTimer = Time.time;
            anim.SetInteger("PlayerAction", 1);
        }

        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, dashSpeed);

		currentVelocity = new Vector3(horizontalVelocity.x, (nowDashing ? 0 : currentVelocity.y), horizontalVelocity.z);
	}

    public bool MovementIsEnable()
    {
        return canMove && !nowLeft && !nowRight && !forcedInteraction;
    }

    private void Interact()
    {
        isInteracting = false;

        Collider[] hits;
        hits = Physics.OverlapBox(playerObj.position + boxCastSize.z / 2 * playerObj.forward, boxCastSize / 2, playerObj.rotation);

        foreach (Collider hit in hits)
        {
            if (hit.GetComponent<Interactable>() != null)
            {
                hit.GetComponent<Interactable>().Interact();
                break;
            }
        }
    }

    private bool InteractablePrompt()
    {
        InteractablePromptController controller = InteractablePromptController.instance;

        if (!forcedInteraction)
        {
            Collider[] hits;
            hits = Physics.OverlapBox(playerObj.position + boxCastSize.z / 2 * playerObj.forward, boxCastSize / 2, playerObj.rotation);

            foreach (Collider hit in hits)
            {
                if (hit.GetComponent<Interactable>() != null)
                {
                    controller.OpenPrompt(hit.GetComponent<Interactable>());
                    return true;
                }
            }
        }

        controller.DisableCanvas();
        return false;
    }

    private void UseItem(bool rightHand = true)
    {
        //get itemUseTime from item and according to the type of use
        float itemUseTime = 0.8f;

        if (isGrounded)
        {
            if (rightHand)
            {
                nowRight = true;
                usingRight = false;
                anim.SetInteger("PlayerAction", 3);
            }
            else
            {
                nowLeft = true;
                usingLeft = false;
                anim.SetInteger("PlayerAction", 4);
            }
        }
        else if (Physics.Raycast(playerObj.position, Vector3.down, out _, midAirUseDistance))
        { // modify to mid air use
            if (rightHand)
            {
                usingRight = false;
                anim.SetInteger("PlayerAction", 3);
            }
            else
            {
                usingLeft = false;
                anim.SetInteger("PlayerAction", 4);
            }
        }

        CancelInvoke("ResetUseLeftRight");
        Invoke("ResetUseLeftRight", itemUseTime);
    }

    private void ResetUseLeftRight()
    {
        nowLeft = nowRight = false;
        anim.SetInteger("PlayerAction", 0);

    }

    private void RunAnimTrance()
    {
        if (animTrance != AnimTrance.None && anim.GetInteger("PlayerAction") == 0)
        {
            switch (animTrance)
            {
                case AnimTrance.Dash:
                    isDashing = true;
                    break;
                case AnimTrance.Interact:
                    isInteracting = true;
                    break;
                case AnimTrance.LeftHand:
                    usingLeft = true;
                    break;
                case AnimTrance.RightHand:
                    usingRight = true;
                    break;
            }
            CancelInvoke("ResetAnimTrance");
            animTrance = AnimTrance.None;
        }
    }

    public void AnimInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if(anim.GetInteger("PlayerAction") != 0 && Enum.TryParse(ctx.action.name, out AnimTrance trance))
            {
                animTrance = trance;
                CancelInvoke("ResetAnimTrance");
                Invoke("ResetAnimTrance", animTranceDelay);
            }
        }
    }

    private void ResetAnimTrance()
    {
        animTrance = AnimTrance.None;
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

    public void InteractHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isInteracting = true;
        }
        else if (ctx.canceled)
        {
            isInteracting = false;
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

    public void RightHandHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            usingRight = true;
        }
        else if (ctx.canceled)
        {
            usingRight = false;
        }
    }

    public void LeftHandHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            usingLeft = true;
        }
        else if (ctx.canceled)
        {
            usingLeft = false;
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

        Gizmos.color = Color.white;
        Gizmos.DrawLine(playerObj.position, playerObj.position + Vector3.down * midAirUseDistance);
    }
}
