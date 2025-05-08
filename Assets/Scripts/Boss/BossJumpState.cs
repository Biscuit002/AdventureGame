using System.Collections;
using UnityEngine;

    public class BossJumpState : BossBaseState
    {
        private int jumpsRemaining;
        private float jumpCooldown = 2.5f;
        private float nextJumpTime;
        public GameObject slamDamageBox;

        // Constructor requires a reference to the BossStateMachine and the slam damage box.
        public BossJumpState(BossStateMachine stateMachine, GameObject slamDamageBoxRef) : base(stateMachine)
        {
            slamDamageBox = slamDamageBoxRef;
        }

        public override void Enter()
        {
            // If slamDamageBox is not already assigned, try to find it in the scene.
            if (slamDamageBox == null)
            {
                Debug.LogError("slamDamageBox not assigned in BossJumpState. Attempting to find object in the hierarchy.");
                slamDamageBox = GameObject.Find("SlamDamageBox");
                if (slamDamageBox == null)
                    Debug.LogError("SlamDamageBox not found. Please check the name or assign the reference manually.");
            }

            jumpsRemaining = Random.Range(3, 7);
            nextJumpTime = Time.time;

            // Play the jump animation if available.
            if (stateMachine.Animator != null)
                stateMachine.Animator.Play("BossJump");
        }

        public override void Tick(float deltaTime)
        {
            Vector2 direction = stateMachine.GetDirectionToPlayer();
            stateMachine.RB.linearVelocity = new Vector2(direction.x * stateMachine.MoveSpeed * 0.5f, stateMachine.RB.linearVelocity.y);

            if (Time.time >= nextJumpTime && jumpsRemaining > 0)
            {
                stateMachine.RB.AddForce(Vector2.up * stateMachine.JumpForce, ForceMode2D.Impulse);
                jumpsRemaining--;
                nextJumpTime = Time.time + jumpCooldown;

                if (stateMachine.IsGrounded())
                {
                    slamDamageBox.SetActive(true);
                    stateMachine.StartCoroutine(DeactivateSlamBoxAfterDelay());
                }
            }
            else if (jumpsRemaining <= 0 && stateMachine.IsGrounded())
            {
                stateMachine.SwitchState(stateMachine.IdleState);
            }
        }

        private IEnumerator DeactivateSlamBoxAfterDelay()
        {
            yield return new WaitForSeconds(0.2f);
            if (slamDamageBox != null)
                slamDamageBox.SetActive(false);
        }

        public override void Exit()
        {
            if (slamDamageBox != null)
                slamDamageBox.SetActive(false);
        }
    }
