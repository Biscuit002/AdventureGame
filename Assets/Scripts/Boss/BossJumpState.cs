using UnityEngine; 

public class BossJumpState : BossBaseState 
{ 
    private bool hasLanded = false; 
    private GameObject slamDamageBox; 

    public BossJumpState(BossStateMachine stateMachine, GameObject slamDamageBoxRef) : base(stateMachine) 
    { 
        slamDamageBox = slamDamageBoxRef; 
    } 

    public override void Enter() 
    { 
        hasLanded = false; 

        // Add vertical force for jump 
        stateMachine.RB.linearVelocity = new Vector2(stateMachine.RB.linearVelocity.x, stateMachine.JumpForce); 

        // Play jump sound 
        if (stateMachine.AudioSource != null && stateMachine.jumpSound != null) 
        { 
            stateMachine.AudioSource.PlayOneShot(stateMachine.jumpSound); 
        } 
    } 

    public override void Tick(float deltaTime) 
    { 
        // Check if the boss has landed 
        if (!hasLanded && stateMachine.RB.linearVelocity.y == 0) 
        { 
            hasLanded = true; 

            // Play ground hit sound 
            if (stateMachine.AudioSource != null && stateMachine.groundHitSound != null) 
            { 
                stateMachine.AudioSource.PlayOneShot(stateMachine.groundHitSound); 
            } 
        } 
    } 

    public override void Exit() 
    { 
        // Cleanup or reset logic if needed 
    } 
} 