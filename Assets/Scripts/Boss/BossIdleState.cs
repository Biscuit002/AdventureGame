using UnityEngine;

public class BossIdleState : BossBaseState
{
    private float idleEndTime;
    private float idleDuration;

    public BossIdleState(BossStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        // Set random idle duration between 1-3 seconds
        idleDuration = Random.Range(1f, 3f);
        idleEndTime = Time.time + idleDuration;

        // Play idle animation
        if (stateMachine.Animator != null)
            stateMachine.Animator.Play("BossIdle");
    }

    public override void Tick(float deltaTime)
    {
        if (Time.time >= idleEndTime)
        {
            stateMachine.SwitchState(stateMachine.RunState);
            return;
        }
    }

    public override void Exit()
    {
        // Clean up if needed
    }
} 