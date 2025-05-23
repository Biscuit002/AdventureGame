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
    public AudioSource AudioSource { get; private set; } // AudioSource for playing sounds 
    private AudioSource cameraAudioSource; // AudioSource on the main camera 
 
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
 
    // Sound effects 
    public AudioClip jumpSound; 
    public AudioClip throwSound; 
    public AudioClip groundHitSound; 
    public AudioClip bombExplosionSound; 
 
    private BossBaseState currentState; 
 
    private void Awake() 
    { 
        RB = GetComponent<Rigidbody2D>(); 
        Animator = GetComponent<Animator>(); 
        Player = GameObject.FindGameObjectWithTag("Player").transform; 
        AudioSource = GetComponent<AudioSource>(); 
        if (AudioSource == null) 
        { 
            Debug.LogError("No AudioSource found on the Boss. Please add one."); 
        } 
        // Find the main camera's AudioSource 
        Camera mainCamera = Camera.main; 
        if (mainCamera != null) 
        { 
            cameraAudioSource = mainCamera.GetComponent<AudioSource>(); 
            if (cameraAudioSource == null) 
            { 
                Debug.LogError("No AudioSource found on the main camera. Please add one."); 
            } 
        } 
        else 
        { 
            Debug.LogError("Main camera not found in the scene."); 
        } 
 
        // Initialize states 
        IdleState = new BossIdleState(this); 
        RunState = new BossRunState(this); 
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
 
    public void PlaySound(AudioClip clip) 
    { 
        if (cameraAudioSource != null && clip != null) 
        { 
            cameraAudioSource.pitch = Random.Range(0.9f, 1.1f); // Random pitch variation 
            cameraAudioSource.PlayOneShot(clip); 
        } 
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
            PlaySound(jumpSound); // Play jump sound 
        } 
        else 
        { 
            SwitchState(ThrowState); 
            Debug.Log("Next state chosen: ThrowState"); 
            PlaySound(throwSound); // Play throw sound 
        } 
    } 
 
    public void OnGroundHit() 
    { 
        PlaySound(groundHitSound); // Play ground hit sound 
    } 
 
    public void OnBombExplosion() 
    { 
        PlaySound(bombExplosionSound); // Play bomb explosion sound 
    } 
} 