using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    // States
    public BossIdleState IdleState { get; private set; }
    public BossRunState RunState { get; private set; }
    public BossJumpState JumpState { get; private set; }
    public BossThrowState ThrowState { get; private set; }

    // Components
    public Rigidbody2D RB { get; private set; }
    public Animator Animator { get; private set; }
    public Transform Player { get; private set; }

    // Settings
    public float MoveSpeed = 5f;
    public float JumpForce = 15f;
    public float ThrowForce = 10f;
    public float StompDamage = 20f;
    public float SlamDamage = 30f;
    public float ExplosionRadius = 3f;

    private BossBaseState currentState;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize states
        IdleState = new BossIdleState(this);
        RunState = new BossRunState(this);
        JumpState = new BossJumpState(this);
        ThrowState = new BossThrowState(this);
    }

    private void Start()
    {
        SwitchState(IdleState);
    }

    private void Update()
    {
        currentState?.Tick(Time.deltaTime);
    }

    public void SwitchState(BossBaseState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
    }

    public float GetDistanceToPlayer()
    {
        return Vector2.Distance(transform.position, Player.position);
    }

    public Vector2 GetDirectionToPlayer()
    {
        return (Player.position - transform.position).normalized;
    }
} 