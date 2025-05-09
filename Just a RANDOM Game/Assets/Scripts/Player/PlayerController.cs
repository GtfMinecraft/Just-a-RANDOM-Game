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

	[Header("Movement")]
    [SerializeField] private float groundAcceleration; // acceleration while grounded
	[SerializeField] private float airAcceleration; // acceleration while not grounded
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
    private bool tranceLeftRight = false;

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
    private bool firstJump = true;
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
        isGrounded = playerCharacterController.isGrounded || Physics.Raycast(playerObj.position, Vector3.down, 1.1f);
        int playerAction = anim.GetInteger("PlayerAction");

        UpdateVelocity();

        // handle movement
        playerCharacterController.Move(currentVelocity * Time.deltaTime);

        anim.SetBool("Walking", currentMovement != Vector2.zero && !forcedInteraction);

        List<Interactable> canInteract = InteractablePrompt();

        if (canControl)
        {
            PlayerItemController itemController = PlayerItemController.instance;

            // interact
            if (isInteracting && canInteract != null)
            {
                if ((nowRight || nowLeft) && canInteract[0].GetType() != typeof(ItemInteractable))
                {
                    if (itemController.isAiming)
                        itemController.StopAiming(true);
                    if (itemController.isFishing)
                        itemController.StopFishing();
                    if (itemController.isEating)
                        itemController.StopEating();
                    if(itemController.torchCoroutine != null)
                        StopCoroutine(itemController.torchCoroutine);
                    itemController.CancelInvoke("ResetAnim");
                    itemController.ResetAnim();
                    itemController.ResetAnim();
                }
                if (canInteract[0] is ItemInteractable)
                    InteractablePromptController.instance.ResetPickedDrops();
                isInteracting = false;
                for (int i = 0; i < canInteract.Count; i++)
                    canInteract[i].Interact();
            }

            // possibly implement double-wielding
            if (usingRight && (itemController.isFishing || playerAction == 0 || playerAction == 4 || playerAction == 2 && canInteract[0] is ItemInteractable))
            {
                UseItem(true);
            }

            if (usingLeft && (itemController.isFishing || playerAction == 0 || playerAction == 3))
            {
                UseItem(false);
            }
        }

        RunAnimTrance();
    }

    private void UpdateVelocity()
	{
        if (!MovementIsEnable())
        {
            isDashing = false;
            isJumping = false;
            isRunning = false;
            currentMovement = Vector2.zero;
        }

        float speedMultiplier = isRunning ? runMultiplier : 1f;
        float maxSpeed = GetComponent<PlayerEntity>().baseSpeed * GetComponent<PlayerEntity>().speedMultiplier;

        anim.SetBool("Running", isRunning);

        if(!anim.GetBool("Jump") && !isGrounded && firstJump)
        {
            firstJump = false;
            anim.SetBool("Jump", true);
        }
        else if (anim.GetBool("Jump") && currentVelocity.y <= 0 && Physics.Raycast(playerObj.position, Vector3.down, midAirUseDistance))
        {
            anim.SetBool("Jump", false);
        }
        else if(!anim.GetBool("Jump") && isGrounded && !firstJump)
        {
            firstJump = true;
        }

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
                anim.SetInteger("PlayerAction", 0);
                anim.SetBool("Dash", false);
                dashCooldownTracker = dashCooldown;
            }
        }

        if (dashCooldownTracker > 0f)
		{
            dashCooldownTracker -= Time.deltaTime;
        }
        else if (anim.GetInteger("PlayerAction") == 0 && dashCooldownTracker <= 0f && (isGrounded || dashInAir) && isDashing && !nowDashing)
		{
			nowDashing = true;
            isDashing = false;
            dashTimer = Time.time;
            anim.SetInteger("PlayerAction", 1);
            anim.SetBool("Dash", true);
        }

        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, dashSpeed);

		currentVelocity = new Vector3(horizontalVelocity.x, (nowDashing ? 0 : currentVelocity.y), horizontalVelocity.z);
	}

    public bool MovementIsEnable()
    {
        return canMove && !forcedInteraction;
    }

    private List<Interactable> InteractablePrompt()
    {
        InteractablePromptController controller = InteractablePromptController.instance;

        if (!forcedInteraction && canControl)
        {
            Collider[] hits;
            hits = Physics.OverlapBox(playerObj.position + boxCastSize.z / 2 * playerObj.forward, boxCastSize / 2, playerObj.rotation);

            List<Interactable> interactions = new List<Interactable>();
            foreach (Collider hit in hits)
            {
                Interactable interactable = hit.GetComponent<Interactable>();
                if (interactable != null && interactable.enabled)
                {
                    if (interactions.Count == 0)
                    {
                        controller.OpenPrompt(interactable);
                        interactions.Add(hit.GetComponent<Interactable>());
                        if (interactable.GetType() != typeof(ItemInteractable))
                            return interactions;
                    }
                    else if (interactable.GetType() == typeof(ItemInteractable))
                    {
                        interactions.Add(hit.GetComponent<Interactable>());
                    }
                }
            }

            if (interactions.Count != 0)
                return interactions;
        }

        controller.DisablePrompt();
        return null;
    }

    private void UseItem(bool rightHand = true)
    {
        if (isGrounded || Physics.Raycast(playerObj.position, Vector3.down, midAirUseDistance))
        {
            if (rightHand)
            {
                nowRight = true;
                usingRight = false;
                anim.SetInteger("PlayerAction", 3);
                PlayerItemController.instance.UseItem();
                if (tranceLeftRight)
                {
                    tranceLeftRight = false;
                    PlayerItemController.instance.Release();
                }
            }
            else
            {
                nowLeft = true;
                usingLeft = false;
                anim.SetInteger("PlayerAction", 4);
                PlayerItemController.instance.UseItem(false);
                if (tranceLeftRight)
                {
                    tranceLeftRight = false;
                    PlayerItemController.instance.Release(false);
                }
            }
        }
    }

    public void ResetUseLeftRight(bool isRight)
    {
        if (isRight && nowRight)
        {
            nowRight = false;
            anim.SetInteger("PlayerAction", 0);
        }
        else if(!isRight && nowLeft)
        {
            nowLeft = false;
            anim.SetInteger("PlayerAction", 0);
        }
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
                    tranceLeftRight = true;
                    usingLeft = true;
                    break;
                case AnimTrance.RightHand:
                    tranceLeftRight = true;
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
            if(Enum.TryParse(ctx.action.name, out AnimTrance trance) && (anim.GetInteger("PlayerAction") != 0 || trance == AnimTrance.Dash && dashCooldownTracker > 0))
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
            anim.SetBool("RightHand", true);
        }
        else if (ctx.canceled)
        {
            usingRight = false;
            anim.SetBool("RightHand", false);
            if (nowRight)
            {
                PlayerItemController.instance.Release();
            }
        }
    }

    public void LeftHandHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            usingLeft = true;
            anim.SetBool("LeftHand", true);
        }
        else if (ctx.canceled)
        {
            usingLeft = false;
            anim.SetBool("LeftHand", false);
            if (nowLeft)
            {
                PlayerItemController.instance.Release(false);
            }
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
