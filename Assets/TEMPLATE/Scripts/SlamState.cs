using UnityEngine;

public class SlamState : PlayerBaseState
{
    private float enterTime;
    private float slamForce = 15f; // Adjust this value to control slam speed
    private bool hasAppliedForce = false;

    public SlamState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        enterTime = Time.time;
        hasAppliedForce = false;
        
        // Play slam animation if available
        if (stateMachine.Animator != null)
            stateMachine.Animator.Play("SlamAnimation");
            
        Debug.Log($"[SlamState] Entering Slam State at {enterTime:F2}s");
    }

    public override void Tick(float deltaTime)
    {
        // Apply downward force once when entering the state
        if (!hasAppliedForce && stateMachine.RB != null)
        {
            // Cancel any upward velocity and apply slam force
            stateMachine.RB.linearVelocity = new Vector2(stateMachine.RB.linearVelocity.x, 0f);
            stateMachine.RB.AddForce(Vector2.down * slamForce, ForceMode2D.Impulse);
            hasAppliedForce = true;
        }

        // Allow horizontal movement during slam
        Vector2 moveInput = stateMachine.InputReader.GetMovementInput();
        float targetVelocityX = moveInput.x * stateMachine.MoveSpeed;
        stateMachine.RB.linearVelocity = new Vector2(targetVelocityX, stateMachine.RB.linearVelocity.y);

        // If grounded, transition to Idle/Walk/Run
        if (stateMachine.IsGrounded())
        {
            // Optional: Add screen shake or particle effect here
            stateMachine.JumpsRemaining = stateMachine.MaxJumps;
            if (moveInput == Vector2.zero)
                stateMachine.SwitchState(stateMachine.IdleState);
            else if (stateMachine.InputReader.IsRunPressed())
                stateMachine.SwitchState(stateMachine.RunState);
            else
                stateMachine.SwitchState(stateMachine.WalkState);
            return;
        }

        // Allow shooting during slam
        if (stateMachine.InputReader.IsShootPressed())
        {
            stateMachine.SwitchState(stateMachine.ShootState);
            return;
        }
    }

    public override void Exit()
    {
        Debug.Log($"[SlamState] Exiting Slam State after {Time.time - enterTime:F2}s");
    }
} 