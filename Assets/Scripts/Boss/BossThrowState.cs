using UnityEngine; 
 
public class BossThrowState : BossBaseState 
{ 
    private int throwsRemaining; 
    private float throwCooldown = 0.5f; 
    private float nextThrowTime; 
 
    public BossThrowState(BossStateMachine stateMachine) : base(stateMachine) { } 
 
    public override void Enter() 
    { 
        throwsRemaining = Random.Range(3, 7); 
        nextThrowTime = Time.time; 
 
        // Play throw animation 
        if (stateMachine.Animator != null) 
            stateMachine.Animator.Play("BossThrow"); 
    } 
 
    public override void Tick(float deltaTime) 
    { 
        if (Time.time >= nextThrowTime && throwsRemaining > 0) 
        { 
            ThrowBomb(); 
            throwsRemaining--; 
            nextThrowTime = Time.time + throwCooldown; 
        } 
        else if (throwsRemaining <= 0) 
        { 
            stateMachine.SwitchState(stateMachine.IdleState); 
        } 
    } 
 
    private void ThrowBomb() 
    { 
        if (stateMachine.bombPrefab == null) 
        { 
            Debug.LogError("Bomb prefab is not assigned in the BossStateMachine."); 
            return; 
        } 
 
        // Instantiate bomb and play sound 
        GameObject bomb = Object.Instantiate(stateMachine.bombPrefab, stateMachine.transform.position, Quaternion.identity); 
        if (stateMachine.AudioSource != null && stateMachine.throwSound != null) 
        { 
            stateMachine.AudioSource.PlayOneShot(stateMachine.throwSound); 
        } 
    } 
 
    public override void Exit() 
    { 
        // Cleanup or reset logic when exiting the throw state 
    } 
} 