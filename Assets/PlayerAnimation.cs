using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D playerRigidbody;
    public SpriteRenderer spriteRenderer;
    public float speedMultiplier = 0.2f;
    public float idleThreshold = 0.05f;
    public float flipThreshold = 0.1f;
    public float lerpSpeed = 0.1f; // Speed of frame transition

    private float previousXSpeed = 0f;
    private bool isGrounded = true;
    private float targetFrame = 0f;

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Mathf.Approximately(playerRigidbody.linearVelocity.y, 0f);

        // Get current speed
        float xSpeed = playerRigidbody.linearVelocity.x;

        // Handle flipping based on speed changes
        if (Mathf.Abs(xSpeed - previousXSpeed) > flipThreshold)
        {
            spriteRenderer.flipX = xSpeed < 0;
            previousXSpeed = xSpeed;
        }

        // Determine target animation frame
        if (!isGrounded)
        {
            // Airborne: Lerp towards frame 3 or 7
            targetFrame = GetCurrentAnimationFrame() < 3 ? 3f : 7f;
        }
        else if (Mathf.Abs(xSpeed) < idleThreshold)
        {
            // Grounded and slowing down: Lerp towards frame 1 or 5
            targetFrame = Mathf.Abs(GetCurrentAnimationFrame() - 1) < Mathf.Abs(GetCurrentAnimationFrame() - 5) ? 1f : 5f;
        }

        // Smoothly lerp to target frame
        float currentFrame = Mathf.Lerp(GetCurrentAnimationFrame(), targetFrame, lerpSpeed);
        animator.Play("AnimationName", 0, currentFrame / GetTotalFrames());

        // Adjust animation speed dynamically when moving
        if (isGrounded && Mathf.Abs(xSpeed) >= idleThreshold)
        {
            animator.speed = Mathf.Clamp(Mathf.Abs(xSpeed) * speedMultiplier, 0.5f, 3f);
        }
        else
        {
            animator.speed = 0; // Pause animation when transitioning frames
        }
    }

    int GetCurrentAnimationFrame()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return Mathf.RoundToInt(stateInfo.normalizedTime * GetTotalFrames());
    }

    int GetTotalFrames()
    {
        return 10; // Adjust this value based on your actual animation frame count
    }
}
