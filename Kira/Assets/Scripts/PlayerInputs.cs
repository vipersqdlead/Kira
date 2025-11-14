using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
	public CharacterMovement movement;
	public SwordController sword;

	public KeyCode crouchKey, jumpKey, attackKey, blockKey;
	
	[Header("Dash Settings")]
    public float doubleTapTime = 0.25f; // max delay between taps to count as double tap
    private float lastTapW; // Track timing for each key
	
	private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // lock cursor in the center
        Cursor.visible = false;
    }
	
    void Update()
    {
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movement.MoveXY(move);
		
		if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastTapW < doubleTapTime)
            {
                movement.Dash(); // Dash forward
				print("Dash");
            }
            lastTapW = Time.time;
        }
		
		if(Input.GetKeyDown(jumpKey))
		{
			movement.Jump();
		}
		
		if(Input.GetKeyDown(attackKey))
		{
			sword.StartAttack();
		}
		
		if(Input.GetKeyDown(blockKey))
		{
			sword.OnBlockPressed();
		}
		if(Input.GetKeyUp(blockKey))
		{
			sword.OnBlockReleased();
		}
		
		movement.SetCrouch(Input.GetKey(crouchKey));
		/*
        if (Input.GetKeyDown(KeyCode.C))
            movement.SetCrouch(true);
        if (Input.GetKeyUp(KeyCode.C))
            movement.SetCrouch(false); */


    }
	
	void FixedUpdate()
	{
		HandleMouseLook();

        // Rotate toward camera direction (for FPS)
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        movement.RotateTowards(cameraForward);
	}

    [Header("Sensitivity Settings")]
    public float mouseSensitivity = 150f;
    public float verticalLookLimit = 89f; // max degrees up/down

    private float xRotation = 0f; // current up/down rotation (in degrees)

    private void HandleMouseLook()
    {
        // Get mouse movement (already frame rate–independent)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 0.02f;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 0.02f;

        // Rotate player left/right
        movement.Rotate(mouseX);

        // Rotate camera up/down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
