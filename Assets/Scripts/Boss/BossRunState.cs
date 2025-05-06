using UnityEngine;

public class BossRunState : BossBaseState
{
    private float runEndTime;
    private float runDuration;
    private float stompTimer;
    private float stompInterval = 0.5f;
    private bool isStomping = false;
    public GameObject stompDamageBox;

    public BossRunState(BossStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        runDuration = Random.Range(2f, 4f);
        runEndTime = Time.time + runDuration;
        stompTimer = 0f;

        // Play run animation
        if (stateMachine.Animator != null)
            stateMachine.Animator.Play("BossRun");
    }

    public override void Tick(float deltaTime)
    {
        // Move towards player
        Vector2 direction = stateMachine.GetDirectionToPlayer();
        stateMachine.RB.linearVelocity = new Vector2(direction.x * stateMachine.MoveSpeed, stateMachine.RB.linearVelocity.y);

        // Handle stomping
        stompTimer += deltaTime;
        if (stompTimer >= stompInterval)
        {
            stompTimer = 0f;
            ToggleStomp();
        }

        // Check for state transitions
        if (Time.time >= runEndTime)
        {
            float random = Random.value;
            if (random < 0.3f)
                stateMachine.SwitchState(stateMachine.IdleState);
            else if (random < 0.6f)
                stateMachine.SwitchState(stateMachine.JumpState);
            else
                stateMachine.SwitchState(stateMachine.ThrowState);
        }
    }

    private void ToggleStomp()
    {
        isStomping = !isStomping;
        stompDamageBox.SetActive(isStomping);
    }

    public override void Exit()
    {
        stompDamageBox.SetActive(false);
    }
} 