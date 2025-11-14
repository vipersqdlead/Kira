using UnityEngine;

public class AIPatrolState : BaseAIState
{
    private Vector3 nextPatrolPoint;
    public Vector3[] patrolPoints;
    public int patrolIndex = 0;

    public override void OnStateStart(StateUser user, AIController userGO)
    {
        base.OnStateStart(user, userGO);
        AIUser.targetPosition = patrolPoints[patrolIndex];
        AIUser.movement.moveSpeed *= 0.3f;
    }

    public override void OnStateEnd()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateStay()
    {
        // If close to destination, pick a new one
        if (AIUser == null) { print("User is null!!"); return;  }

        if (Vector3.Distance(AIUser.transform.position, AIUser.targetPosition) <= AIUser.stopDistance)
        {
            if (patrolIndex < patrolPoints.Length - 1)
            {
                patrolIndex++;
            }
            else
            {
                patrolIndex = 0;
            }
            AIUser.targetPosition = patrolPoints[patrolIndex];
        }
    }
}
