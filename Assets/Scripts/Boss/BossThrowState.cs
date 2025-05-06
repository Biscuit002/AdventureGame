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
            Debug.LogError("Bomb prefab not assigned in BossStateMachine!");
            return;
        }

        // Calculate trajectory to player
        Vector2 targetPos = stateMachine.Player.position;
        Vector2 currentPos = stateMachine.transform.position;
        Vector2 direction = (targetPos - currentPos).normalized;

        // Spawn bomb
        GameObject bomb = Object.Instantiate(stateMachine.bombPrefab, stateMachine.transform.position, Quaternion.identity);
        Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();
        
        // Apply force with slight randomness
        Vector2 randomOffset = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        );
        bombRb.AddForce((direction + randomOffset) * stateMachine.ThrowForce, ForceMode2D.Impulse);
    }

    public override void Exit()
    {
        // Clean up if needed
    }
} 