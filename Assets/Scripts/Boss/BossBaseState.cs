using UnityEngine;

public abstract class BossBaseState
{
    protected BossStateMachine stateMachine;

    public BossBaseState(BossStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Tick(float deltaTime);
    public abstract void Exit();
} 