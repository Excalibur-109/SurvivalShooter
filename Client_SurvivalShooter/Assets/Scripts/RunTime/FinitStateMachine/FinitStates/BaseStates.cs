using Excalibur;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class IdleState : BaseFiniteState
{
    IsMoveComponent isMove;

    public override AnimName animName => AnimName.idle;

    public override void SetComponent(IComponent componentData)
    {
        isMove = componentData as IsMoveComponent;
    }

    public override void OnEnter()
    {
    }

    public override void OnExecute()
    {
        if (isMove.isMoving)
        {
            _fsm.TransitionState(FinitState.Run);
        }
    }

    public override void OnExit()
    {
    }
}

public class RunState : BaseFiniteState
{
    IsMoveComponent isMove;

    public override AnimName animName => AnimName.run;

    public override void SetComponent(IComponent componentData)
    {
        isMove = componentData as IsMoveComponent;
    }

    public override void OnEnter()
    {
    }

    public override void OnExecute()
    {
        if (!isMove.isMoving)
        {
            _fsm.TransitionState(FinitState.Idle);
        }
    }

    public override void OnExit()
    {
    }
}

public class ChaseState : BaseFiniteState
{
    private PositionComponent target;
    public override AnimName animName => AnimName.run;

    public override void SetComponent(IComponent componentData)
    {
        target = componentData as PositionComponent;
    }

    public override void OnEnter()
    {
    }

    public override void OnExecute()
    {
    }

    public override void OnExit()
    {
    }
}

public class AttackState : BaseFiniteState
{
    public override AnimName animName => AnimName.fire;

    public override void OnEnter()
    {
    }

    public override void OnExecute()
    {
    }

    public override void OnExit()
    {
    }
}


public class DeadState : BaseFiniteState
{
    public override AnimName animName => throw new System.NotImplementedException();

    public override void OnEnter()
    {
    }

    public override void OnExecute()
    {
    }

    public override void OnExit()
    {
    }
}
