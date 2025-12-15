using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
	Rigidbody rb;
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // degrees per second
    public float crouchSpeedMultiplier = 0.5f;
    public float dashForce = 5f;
    public float dashDuration = 0.2f;
	public float jumpForce = 5f;
	
	[Header("Ladder Settings")]
	public bool onLadder = false;
	public float climbSpeed = 3f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    [HideInInspector]public bool isDashing;
    private float dashTimer;

	[Header("Effects")]
	public AudioSource aSource, stepASource;
	public AudioClip dashSound, jumpSound;
	

    // Movement input (set externally by PlayerInput or AI)
    private Vector2 moveInput;
    private float targetRotation;

    private void Awake()
    {
		rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
		if(onLadder)
		{
			HandleLadderMovement();
			return;
		}
		
        HandleDash();
        MoveCharacter();
		CheckGround();
    }

    // --- External Control Functions ---

    public void MoveXY(Vector2 movement)
    {
        moveInput = movement;
    }

    public void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;
        targetRotation = Quaternion.LookRotation(direction).eulerAngles.y;
    }
	
	public void Rotate(float rotation)
	{
		transform.Rotate(0, rotation, 0);
	}

    public void SetCrouch(bool crouch)
    {
        if (isDashing) return; // can't crouch mid-dash
        isCrouching = crouch;
		Vector3 scale = new Vector3(1f, isCrouching ? 0.8f : 1.0f, 1f); // simple height toggle
		transform.localScale = scale;
		
        //controller.height = isCrouching ? 1.0f : 2.0f; // simple height toggle
    }

    public bool Dash()
    {
        if (isDashing || isCrouching || moveInput.magnitude == 0f) return false;
		aSource.PlayOneShot(dashSound);
        dashTimer = dashDuration;
		isDashing = true;
		return true;
    }

	// --- Core Mechanics ---
	public float speedDampFactor;
    private void MoveCharacter()
    {
        if (isDashing) return; // handled separately
		
        Vector3 moveDir = transform.forward * moveInput.y + transform.right * moveInput.x;
		moveDir = Vector3.ClampMagnitude(moveDir, 1f);
		//moveDir.magnitude = Mathf.Clamp(moveDir.magnitude, -1f, 1f);
		float currentSpeed = moveSpeed * (isCrouching ? crouchSpeedMultiplier : 1f);

        Vector3 vel = rb.linearVelocity;
		Vector3 moveVelocity = (moveDir * currentSpeed) * 60f;
        float rbSpeed = vel.magnitude;
		speedDampFactor = 0f;

        if (moveVelocity.magnitude != 0f)
		{
            speedDampFactor = Mathf.Clamp01((moveVelocity.magnitude - Mathf.Abs(vel.magnitude)) / (moveVelocity.magnitude - (moveVelocity.magnitude * 0.7f)));
        }

		moveVelocity = (moveVelocity * speedDampFactor) + vel;
		rb.linearVelocity = new Vector3(moveVelocity.x, vel.y, moveVelocity.z);

		stepASource.enabled = moveVelocity.magnitude != 0f;
    }

    private float _rotationVelocity;

    private void HandleDash()
    {
        if (!isDashing) return;

        dashTimer -= Time.deltaTime;
		Vector3 moveDir = transform.forward * moveInput.y + transform.right * moveInput.x;
		moveDir = Vector3.ClampMagnitude(moveDir, 1f);
        Vector3 dashDir = moveDir * dashForce * 60f;
        //controller.Move(dashDir * Time.deltaTime);
		
		//rb.position += dashDir;
		Vector3 vel = rb.linearVelocity;
		rb.linearVelocity = new Vector3(dashDir.x, vel.y, dashDir.z);

        if (dashTimer <= 0)
        {
            isDashing = false;
        }
    }
	
	public void Jump()
	{
		if(!isGrounded) { print("Tried to jump, but char is not grounded."); return; }
		
		rb.AddForce(transform.up * jumpForce * (Time.fixedDeltaTime * 60f), ForceMode.Impulse);
        aSource.PlayOneShot(jumpSound);
        print("Jump!");
	}
	
	public void HandleLadderMovement()
	{
		if(!onLadder) return;
		
		rb.useGravity = false;
		
		Vector3 climbVelocity = new Vector3(moveInput.x, moveInput.y * climbSpeed, 0f);
		rb.linearVelocity = climbVelocity;
	}
	
	void CheckGround()
	{
		isGrounded = Physics.Raycast(transform.position, -transform.up, 1.2f);
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Ladder"))
		{
			onLadder = true;
			rb.useGravity = false;
			rb.linearVelocity = Vector3.zero;
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		if(other.gameObject.CompareTag("Ladder"))
		{
			onLadder = false;
			rb.useGravity = true;
		}
	}
}
