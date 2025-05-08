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

    // References to damage boxes
    public GameObject stompDamageBox;
    public GameObject slamDamageBox;
    public GameObject bombPrefab;

    private BossBaseState currentState;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize states
        IdleState = new BossIdleState(this);
        RunState  = new BossRunState(this);
        JumpState = new BossJumpState(this, slamDamageBox);
        ThrowState = new BossThrowState(this);

        // Set up damage boxes
        if (stompDamageBox == null)
        {
            Debug.LogError("StompDamageBox not assigned in BossStateMachine!");
        }
        if (slamDamageBox == null)
        {
            Debug.LogError("SlamDamageBox not assigned in BossStateMachine!");
        }
        if (bombPrefab == null)
        {
            Debug.LogError("BombPrefab not assigned in BossStateMachine!");
        }

        // Initialize damage boxes to inactive
        if (stompDamageBox != null) stompDamageBox.SetActive(false);
        if (slamDamageBox != null) slamDamageBox.SetActive(false);
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

    // NEW: Choose the next state after the run state based on probabilities.
    // 0-2: Idle (30%), 3-5: Jump (30%), 6-9: Throw (40%)
    public void ChooseNextStateAfterRun()
    {
        int nextAction = Random.Range(0, 10); // Generates a number between 0 and 9.
        if (nextAction < 3)
        {
            SwitchState(IdleState);
            Debug.Log("Next state chosen: IdleState");
        }
        else if (nextAction < 6)
        {
            SwitchState(JumpState);
            Debug.Log("Next state chosen: JumpState");
        }
        else
        {
            SwitchState(ThrowState);
            Debug.Log("Next state chosen: ThrowState");
        }
    }
}
