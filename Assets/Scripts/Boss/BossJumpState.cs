using UnityEngine;

public class BossJumpState : BossBaseState
{
    private int jumpsRemaining;
    private float jumpCooldown = 1f;
    private float nextJumpTime;
    public GameObject slamDamageBox;

    public BossJumpState(BossStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        jumpsRemaining = Random.Range(3, 7);
        nextJumpTime = Time.time;

        // Play jump animation
        if (stateMachine.Animator != null)
            stateMachine.Animator.Play("BossJump");
    }

    public override void Tick(float deltaTime)
    {
        if (Time.time >= nextJumpTime && jumpsRemaining > 0)
        {
            // Perform jump
            stateMachine.RB.AddForce(Vector2.up * stateMachine.JumpForce, ForceMode2D.Impulse);
            jumpsRemaining--;
            nextJumpTime = Time.time + jumpCooldown;

            // Activate slam damage box when landing
            if (stateMachine.IsGrounded())
            {
                slamDamageBox.SetActive(true);
                // Deactivate after a short duration
                Invoke(nameof(DeactivateSlamBox), 0.2f);
            }
        }
        else if (jumpsRemaining <= 0 && stateMachine.IsGrounded())
        {
            stateMachine.SwitchState(stateMachine.IdleState);
        }
    }

    private void DeactivateSlamBox()
    {
        slamDamageBox.SetActive(false);
    }

    public override void Exit()
    {
        slamDamageBox.SetActive(false);
    }
} 