using System.Collections;
using UnityEngine;

public class AISuspisciousState : BaseAIState
{
    private bool arrived = false;

    public override void OnStateStart(StateUser user, AIController userGO)
    {
        base.OnStateStart(user, userGO);
        AIUser.targetPosition = AIUser.lastKnownPlayerPosition;
    }

    public override void OnStateEnd()
    {
        AIUser.StopAllCoroutines();
		return;
    }

    public override void OnStateStay()
    {
        // If close to destination, pick a new one
        if (AIUser == null) { print("User is null!!"); return;  }

        if(AIUser.CanSeePlayer())
		{
			if(GetComponent<AIFightingState>() == null)
			{
				AIUser.SetState(gameObject.AddComponent<AIFightingState>());
			}
			else
			{
				AIUser.SetState(gameObject.GetComponent<AIFightingState>());
			}
			return;
		}
		
		float distance = Vector3.Distance(AIUser.transform.position, AIUser.targetPosition);
		
		if(!arrived && distance < AIUser.stopDistance)
		{
			arrived = true;
			
			AIUser.StartCoroutine(InvestigateAndReturn(AIUser));
		}
    }
	
	private IEnumerator InvestigateAndReturn(AIController AIUser)
	{
		yield return new WaitForSeconds(3f);
		
		if(!AIUser.CanSeePlayer())
		{
			AIUser.SetState(AIUser.GetComponent<AIPatrolState>());
		}
		yield return null;
	}
}
