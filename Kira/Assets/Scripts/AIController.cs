using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class AIController : MonoBehaviour, StateUser
{
    [Header("References")]
    public CharacterMovement movement;       // moves the character
    public SwordController swordController;      // used for attacking
    public Transform player;                 // usually your player reference

    [Header("AI Settings")]
    public float stopDistance = 2.5f;        // how close to stop before target
    public float speed = 0.3f;                 // Speed multiplier - How fast is it trying to go?
    public float turnSpeed = 5f;             // rotation speed toward target

    public BaseAIState currentState;
    [HideInInspector] public Vector3 targetPosition;

    private void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        swordController = GetComponent<SwordController>();
    }

    private void Start()
    {
        ExecuteStateOnStart();
    }

    private void Update()
    {
        ExecuteStateOnUpdate();

        MoveTowardTarget();
    }

    public void SetState(BaseAIState newState)
    {
        if (currentState != null)
            currentState.OnStateEnd();

        currentState = newState;

        if (currentState != null)
            currentState.OnStateStart(this, this);
    }

    Vector3 moveInput, newMoveInput;
    private void MoveTowardTarget()
    {
        Vector3 dir = (targetPosition - transform.position);
        dir.y = 0f; // flatten

        float distance = dir.magnitude;
        stopDistance = Random.Range(1.5f, 2f);
        if (distance < stopDistance)
        {
            movement.MoveXY(Vector2.zero);
            return;
        }

        dir.Normalize();

        // Convert world direction to local X/Z (for CharacterMovement)
        Vector3 localDir = transform.InverseTransformDirection(dir);
        moveInput = new Vector2(localDir.x, localDir.z);
        newMoveInput = new Vector2(Mathf.Lerp(moveInput.x, newMoveInput.x, Time.deltaTime), Mathf.Lerp(moveInput.x, newMoveInput.x, Time.deltaTime));

        movement.MoveXY(moveInput);

        // Smoothly rotate toward target
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
        }
    }

    public void ChangeState(StateBase newState)
    {
        throw new System.NotImplementedException();
    }

    public void ExecuteStateOnUpdate()
    {
        if (currentState != null)
            currentState.OnStateStay();
    }

    public void ExecuteStateOnStart()
    {
        // We'll assign a default state later (like Patrol)
        if (currentState == null)
            SetState(new AILookOutState());
        
        currentState.OnStateStart(this, this);
        
    }
}
