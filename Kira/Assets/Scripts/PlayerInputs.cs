using TMPro;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
	public CharacterMovement movement;
	public SwordController sword;
    public HealthController hp;

	public KeyCode crouchKey, jumpKey, attackKey, blockKey, dashKey;
	public float dashCooldown = 0.5f;

    public Animator animations,camaraAnimation;
	bool isBlocking;
    public int changeAttack = 1, changeAttackCamera =1;
	private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // lock cursor in the center
        Cursor.visible = false;
    }
	
    void Update()
    {
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movement.MoveXY(move);
		if(move.y != 0 || move.x != 0)
        {
            animations.SetBool("Walking",true);
            camaraAnimation.SetBool("Dash",false);
        }
        else
        {
           animations.SetBool("Walking",false); 
        }
		if(Input.GetKeyDown(jumpKey))
		{
			movement.Jump();
		}
		if(Input.GetKeyDown(dashKey) && hp.stamina >= .3f)
		{
            
			movement.Dash();
            hp.stamina -= 0.3f;
            camaraAnimation.SetBool("Dash",true);
		}
		
		if(Input.GetKeyDown(attackKey) && isBlocking ==false)
		{
            changeAttack++;
            changeAttackCamera++;
            if(changeAttack > 2)
            {
                changeAttack = 1;
            }
            if(changeAttackCamera > 2)
            {
                changeAttackCamera = 1;
            }
			sword.StartAttack();
            animations.SetBool("Attaking",true); 
            animations.SetInteger("Attacked", changeAttack);
            camaraAnimation.SetBool("Attaking",true);
		}
		else
        {
            animations.SetBool("Attaking",false); 
            camaraAnimation.SetBool("Attaking",false);
        }

		if(Input.GetKeyDown(blockKey)&& !Input.GetKeyDown(attackKey))
		{
			sword.OnBlockPressed();
            isBlocking =true;
            animations.SetBool("Blocking",true);
		}
		if(Input.GetKeyUp(blockKey))
		{
            isBlocking =false;
			sword.OnBlockReleased();
            animations.SetBool("Blocking",false);
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

    void LateUpdate()
    {

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
