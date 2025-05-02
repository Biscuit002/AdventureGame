using UnityEngine;

public class JumpState : PlayerBaseState
{
    private float wallJumpHorizontalForce = 5f;
    private float enterTime;
    private bool hasJumped = false;
    private bool isWallJump = false;
    private float jumpForce = 7f; // Initial jump force
    private float jumpGravityScale = 1.5f; // Gravity when falling
    private float normalGravityScale = 1f; // Normal gravity
    private bool isHoldingJump = false;
    private float jumpStartTime;
    private float maxJumpDuration = 0.2f; // Maximum time to hold jump for max height
    private float minJumpDuration = 0.05f; // Minimum time to hold for minimum height

    public JumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        enterTime = Time.time;
        jumpStartTime = Time.time;
        hasJumped = false;
        isWallJump = false;
        isHoldingJump = true;

        // Set initial gravity scale
        if (stateMachine.RB != null)
        {
            stateMachine.RB.gravityScale = normalGravityScale;
        }

        // --- Start coyote time (grounded grace period) ---
        stateMachine.GetType().GetField("jumpGroundedGraceTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(stateMachine, 0.10f);

        // Play jump animation
        if (stateMachine.Animator != null)
            stateMachine.Animator.Play("JumpAnimation");
        Debug.Log($"[JumpState] Entering Jump State at {enterTime:F2}s");

        // If grounded, set jumps to MaxJumps - 1 (so the ground jump counts as the first jump)
        bool isWallJumpNow = stateMachine.IsTouchingWall() && !stateMachine.IsGrounded();

        if (isWallJumpNow)
        {
            // Only allow wall jump if JumpsRemaining > 0
            if (stateMachine.JumpsRemaining > 0)
            {
                Vector2 wallJumpDir = GetWallJumpDirection();
                if (stateMachine.RB != null)
                {
                    stateMachine.RB.linearVelocity = Vector2.zero;
                    stateMachine.RB.AddForce(new Vector2(wallJumpDir.x * wallJumpHorizontalForce, stateMachine.WallJumpForce), ForceMode2D.Impulse);
                    hasJumped = true;
                    isWallJump = true;
                    stateMachine.JumpsRemaining = Mathf.Max(0, stateMachine.JumpsRemaining - 1);
                    Debug.Log("[JumpState] Wall Jump performed. Jumps left: " + stateMachine.JumpsRemaining);
                }
            }
        }
        else if (stateMachine.IsGrounded())
        {
            // Ground jump: do not decrement JumpsRemaining (already set to MaxJumps-1 on landing)
            if (stateMachine.RB != null)
            {
                stateMachine.RB.linearVelocity = new Vector2(stateMachine.RB.linearVelocity.x, 0f);
                // Apply initial jump force
                stateMachine.RB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                hasJumped = true;
                Debug.Log("[JumpState] Ground Jump performed. Jumps left: " + stateMachine.JumpsRemaining);
            }
        }
        else if (stateMachine.JumpsRemaining > 0)
        {
            // Air jump: decrement JumpsRemaining
            stateMachine.JumpsRemaining = Mathf.Max(0, stateMachine.JumpsRemaining - 1);
            if (stateMachine.RB != null)
            {
                stateMachine.RB.linearVelocity = new Vector2(stateMachine.RB.linearVelocity.x, 0f);
                // Apply initial jump force
                stateMachine.RB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                hasJumped = true;
                Debug.Log("[JumpState] Double Jump performed. Jumps left: " + stateMachine.JumpsRemaining);
            }
        }
    }

    public override void Tick(float deltaTime)
    {
        // Check if jump key is being held
        isHoldingJump = stateMachine.InputReader.IsJumpHeld();

        // Apply variable jump height
        if (stateMachine.RB != null)
        {
            // Calculate how long we've been holding the jump
            float holdTime = Time.time - jumpStartTime;
            
            // If we're still holding jump and haven't reached max duration
            if (isHoldingJump && holdTime < maxJumpDuration)
            {
                // Apply reduced gravity while holding jump
                stateMachine.RB.gravityScale = normalGravityScale;
            }
            else
            {
                // Apply increased gravity when not holding or after max duration
                stateMachine.RB.gravityScale = jumpGravityScale;
            }
        }

        // Check for Shoot input first
        if (stateMachine.InputReader.IsShootPressed())
        {
            stateMachine.SwitchState(stateMachine.ShootState);
            return;
        }

        // Check for slam input
        if (stateMachine.InputReader.IsSlamPressed())
        {
            stateMachine.SwitchState(stateMachine.SlamState);
            return;
        }

        // Apply horizontal movement input while airborne
        Vector2 moveInputAir = stateMachine.InputReader.GetMovementInput();
        float targetVelocityX = moveInputAir.x * stateMachine.MoveSpeed;
        stateMachine.RB.linearVelocity = new Vector2(targetVelocityX, stateMachine.RB.linearVelocity.y);

        // If grounded, reset jumps and transition to Idle/Walk/Run
        if (stateMachine.IsGrounded())
        {
            stateMachine.JumpsRemaining = stateMachine.MaxJumps;
            Vector2 moveInput = stateMachine.InputReader.GetMovementInput();
            if (moveInput == Vector2.zero)
                stateMachine.SwitchState(stateMachine.IdleState);
            else if (stateMachine.InputReader.IsRunPressed())
                stateMachine.SwitchState(stateMachine.RunState);
            else
                stateMachine.SwitchState(stateMachine.WalkState);
            return;
        }

        // Check if falling against a wall -> transition to Wall Cling
        if (stateMachine.IsTouchingWall() && stateMachine.RB.linearVelocity.y <= 0)
        {
            stateMachine.SwitchState(stateMachine.WallClingState);
            return;
        }

        // If not grounded and not touching wall, transition to FallState
        if (!stateMachine.IsGrounded() && !stateMachine.IsTouchingWall())
        {
            stateMachine.SwitchState(stateMachine.FallState);
            return;
        }
    }

    public override void Exit()
    {
        // Reset gravity scale when exiting jump state
        if (stateMachine.RB != null)
        {
            stateMachine.RB.gravityScale = normalGravityScale;
        }
        Debug.Log($"[JumpState] Exiting Jump State after {Time.time - enterTime:F2}s");
    }

    private Vector2 GetWallJumpDirection()
    {
        float facing = stateMachine.transform.localScale.x;
        return facing > 0 ? Vector2.left : Vector2.right;
    }
}