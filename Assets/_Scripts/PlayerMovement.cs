using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour{
	[Header("Required References")]
	[SerializeField] private CharacterController characterController;
	[SerializeField] private Transform playerOrientation;
	[SerializeField] private Transform groundCheckTransform;
	[SerializeField] private LayerMask groundLayers;	

	[Header("Base Movement Variables")]
	[SerializeField] private float movementSpeed;
	[SerializeField] private float slopeRayStandingDetectDistance = 1.05f;

	[Header("Jumping Variables")]
	[SerializeField] private float maxJumpHeight = 2f;
	[SerializeField] private float jumpCooldown = 0.15f;
	[SerializeField] private float jumpBufferWindow = 0.25f;
	[SerializeField] private float coyoteTimeWindow = 0.15f;

	[Header("Gravity Variables")]
	[SerializeField] private float gravity = -9.31f;
	[SerializeField] private float upwardJumpGravity;
	[SerializeField] private float downwardJumpGravity;
	[SerializeField] private float groundedRadius = 0.5f;

	[Header("Debug Variables")]
	[SerializeField] private bool drawGizmos = false;

	[HideInInspector] public Vector3 ExternalMovement = new Vector3();

	public const int MOVEMENT_ENERGY_CONSUMPTION_RATE = 5;

	public EventHandler<PlayerMovementDirectionChangedEventArgs> OnPlayerMovementDirectionChanged;
	public class PlayerMovementDirectionChangedEventArgs : EventArgs{
		public Vector3 rawDirection;
		public PlayerMovementDirectionChangedEventArgs(Vector3 _rawDirection){
			rawDirection = _rawDirection;
		}
	}

	public EventHandler OnPlayerMovementStopped;

	private const float terminalVelocity = -53f;

	private float xInput;
	private float yInput;
	private float verticalVelocity;

	private bool grounded;
	private bool hasJumped = false;

	private Vector2 previousMoveInput;
	private Vector2 playerMoveInput;
	private Vector3 moveDirection;

	private IEnumerator currentJumpCooldown;
	private IEnumerator currentCoyoteTimeWindow;
	private IEnumerator currentJumpBufferWindow;

	private bool previousGroundCheck = false;

	private void OnDestroy() {
		StopAllCoroutines();
	}

	private void Update() {
		Move();
		GroundCheck();
        Gravity();
		ExternalForceCalc();
	}

	private void ExternalForceCalc(){
		//consumes the energy each frame:
		if(ExternalMovement == Vector3.zero) return;
		ExternalMovement = Vector3.Lerp(ExternalMovement, Vector3.zero, MOVEMENT_ENERGY_CONSUMPTION_RATE * Time.deltaTime);
		if(ExternalMovement.magnitude > 0.5f) ExternalMovement = Vector3.zero;
	}

    public void MoveInput(InputAction.CallbackContext context){
		xInput = context.ReadValue<Vector2>().x;
		yInput = context.ReadValue<Vector2>().y;

		playerMoveInput.x = xInput;
		playerMoveInput.y = yInput;
	}

	public void JumpInput(InputAction.CallbackContext context){
        if(currentJumpCooldown != null) return;
		if(context.phase != InputActionPhase.Performed) return;

        if (!grounded && currentCoyoteTimeWindow == null){
            StopJumpBufferWindow();
			currentJumpBufferWindow = JumpBufferCoroutine();
			StartCoroutine(currentJumpBufferWindow);
			return;
        }

        if (!grounded && currentCoyoteTimeWindow != null){
            StopCoyoteTimeWindow();
        }

		if(currentJumpBufferWindow != null){
			Debug.Log(jumpBufferWindow + " isn't null?");
			return;
		} 

        StartJump();
    }

    private void StartJump(){
		hasJumped = true;

        float height = maxJumpHeight;

		verticalVelocity = Mathf.Sqrt(height * -2f * upwardJumpGravity);

        currentJumpCooldown = JumpCooldownCoroutine();
        StartCoroutine(currentJumpCooldown);
    }

	public Vector3 GetMoveDirection(){
		return moveDirection;
	}

	public bool IsMoving(){
		return moveDirection != Vector3.zero;
	}

	public bool IsGrounded(){
		return grounded;
	}

	public Vector3 GetGroundCheckPos(){
		return groundCheckTransform.position;
	}

	public LayerMask GetGroundLayerMask(){
		return groundLayers;
	}

    private void GroundCheck(){
		grounded = Physics.CheckSphere(groundCheckTransform.position, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
		if(previousGroundCheck && !grounded && !hasJumped){
			//left the ground start the coyote time window
			currentCoyoteTimeWindow = CoyoteTimeWindowCoroutine();
			StartCoroutine(CoyoteTimeWindowCoroutine());
		}

		if(!previousGroundCheck && grounded && currentCoyoteTimeWindow != null){
			//We are grounded stop the coyote time window
			StopCoyoteTimeWindow();
		}

		if(grounded && hasJumped) hasJumped = false;

		previousGroundCheck = grounded;
    }

	private void Gravity(){
        if(grounded){
			verticalVelocity = 0f;
			return;
		}

		if(verticalVelocity > terminalVelocity){
			float currentGravity = gravity;
			if(hasJumped) currentGravity = downwardJumpGravity;
			verticalVelocity += currentGravity * Time.deltaTime;
		}
    }

	public void ResetVerticalVelocity(){
		verticalVelocity = 0f;
	}

	public void IncreaseDownwardGravity(){
		hasJumped = true;
	}

	public void DecreaseDownwardGravity(){
		hasJumped = false;
	}

    private void Move(){
        moveDirection = playerOrientation.forward * yInput + playerOrientation.right * xInput;
		if(moveDirection == Vector3.zero){
			OnPlayerMovementStopped?.Invoke(this, EventArgs.Empty);
		}

		if(playerMoveInput != previousMoveInput){
			OnPlayerMovementDirectionChanged?.Invoke(this, new PlayerMovementDirectionChangedEventArgs(playerMoveInput));
		}

		Vector3 velocity = movementSpeed * (moveDirection.normalized + ExternalMovement);

		velocity = AdjustMovementVectorToSlope(velocity);
		velocity.y += verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
		previousMoveInput = playerMoveInput;
    }

    private void StopJumpBufferWindow(){
		if(currentJumpBufferWindow != null){
			StopCoroutine(currentJumpBufferWindow); 
			currentJumpBufferWindow = null;
		}
	}

	private void StopCoyoteTimeWindow(){
		if(currentCoyoteTimeWindow != null){
			StopCoroutine(currentCoyoteTimeWindow);
			currentCoyoteTimeWindow = null;
		}
	}

	private Vector3 AdjustMovementVectorToSlope(Vector3 velocity){
		Ray slopeDetectionRay = new Ray(transform.position, Vector3.down);

		float slopeDetectionDistance = slopeRayStandingDetectDistance;

		if(!Physics.Raycast(slopeDetectionRay, out RaycastHit hitInfo, slopeDetectionDistance, groundLayers)) return velocity;

		Vector3 adjustedVelocity = Vector3.ProjectOnPlane(velocity, hitInfo.normal);

		if(adjustedVelocity.y < 0) return adjustedVelocity;
		return velocity;
	}

	private IEnumerator JumpBufferCoroutine(){
		float currentCheckTime = 0;

		while(currentCheckTime <= jumpBufferWindow){
			if(grounded){
				StartJump();
				StopJumpBufferWindow();
				yield break;
			}
			currentCheckTime += Time.deltaTime;
			yield return null;
		}

		StopJumpBufferWindow();
	}

	private IEnumerator CoyoteTimeWindowCoroutine(){
		yield return new WaitForSeconds(coyoteTimeWindow);
		currentCoyoteTimeWindow = null;
	}

	private IEnumerator JumpCooldownCoroutine(){
		yield return new WaitForSeconds(jumpCooldown);
		currentJumpCooldown = null;
	}

	private void OnDrawGizmosSelected() {
		if(!drawGizmos) return;

		float slopeDetectionDistance = slopeRayStandingDetectDistance;

		Gizmos.DrawLine(transform.position, transform.position + Vector3.down * slopeDetectionDistance);

		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		Gizmos.DrawSphere(groundCheckTransform.position, groundedRadius);
	}
}
