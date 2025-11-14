using UnityEngine;

public class TestMannequin : MonoBehaviour
{
    public SwordController sword;
	public float timeToAttack;
	float attackTimer;
    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
		
		if(attackTimer > timeToAttack)
		{
			sword.StartAttack();
			attackTimer = 0f;
		}
    }
}
