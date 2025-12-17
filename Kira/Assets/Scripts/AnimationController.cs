using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public CharacterMovement characterMove;
    public Animator animationEnemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isMoving = characterMove.moveInput.magnitude != 0;
        
            animationEnemy.SetBool("Walking", isMoving);
       
    }

    public void StartAttack()
    {
        animationEnemy.SetBool("IsAttacking", true);
    }
    public void EndAttack()
    {
        animationEnemy.SetBool("IsAttacking", false);
    }

    public void IsHitted()
    {
        animationEnemy.SetBool("IsHitted", true);
    }
    public void EndHitted()
    {
        animationEnemy.SetBool("IsHitted", false);
    }
    public void IsDead()
    {
        EndHitted();
        EndAttack();
        animationEnemy.SetBool("Walking", false);
        animationEnemy.SetBool("IsDead", true);
    }
}
