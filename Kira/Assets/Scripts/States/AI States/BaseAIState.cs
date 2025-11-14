using UnityEngine;

public class BaseAIState : StateBase
{
    public AIController AIUser;

    public virtual void OnStateStart(StateUser user, AIController userGO)
    {
        base.OnStateStart(user);
        AIUser = userGO;
    }

    public override void OnStateEnd()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateStay()
    {
        throw new System.NotImplementedException();
    }
}
