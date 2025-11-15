using UnityEngine;

public class AIFightingState : BaseAIState
{
    public override void OnStateStart(StateUser user, AIController userGO)
    {
        base.OnStateStart(user, userGO);
    }

    public override void OnStateEnd()
    {
        return;
    }

    public override void OnStateStay()
    {
		AIUser.targetPosition = AIUser.player.position;
		
		if(!AIUser.CanSeePlayer())
		{
			if(GetComponent<AISuspisciousState>() == null)
			{
				AIUser.SetState(gameObject.AddComponent<AISuspisciousState>());
			}
			else
			{
				AIUser.SetState(gameObject.GetComponent<AISuspisciousState>());
			}
			
			return;
		}
		
		float dist = Vector3.Distance(AIUser.transform.position, AIUser.player.position);
		
		if(dist <= AIUser.stopDistance + 0.5f)
		{
			AIUser.movement.MoveXY(Vector2.zero);
			AIUser.swordController.StartAttack();
		}
    }
}
