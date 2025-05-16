using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator; // Reference to the Animator
    public Rigidbody2D playerRigidbody; // Reference to the Boss's Rigidbody2D
    public SpriteRenderer spriteRenderer; // Reference to the Sprite Renderer
    public float speedMultiplier = 0.2f; // Adjusts animation speed sensitivity
    public float idleThreshold = 0.05f; // Minimum speed before animation starts
    public float flipThreshold = 0.1f; // Minimum speed change required to flip

    private float previousXSpeed = 0f; // Tracks the previous X speed

    void Update()
    {
        // Get current X-axis movement speed
        float xSpeed = playerRigidbody.linearVelocity.x;

        // Only flip if the speed change exceeds the flip threshold
        if (Mathf.Abs(xSpeed - previousXSpeed) > flipThreshold)
        {
            if (xSpeed > 0)
            {
                spriteRenderer.flipX = false; // Facing right
            }
            else if (xSpeed < 0)
            {
                spriteRenderer.flipX = true; // Facing left
            }

            // Update previous speed only if change is significant
            previousXSpeed = xSpeed;
        }

        // Adjust animation speed based on movement
        if (Mathf.Abs(xSpeed) < idleThreshold)
        {
            animator.speed = 0; // Stop animation when still
        }
        else
        {
            animator.speed = Mathf.Clamp(Mathf.Abs(xSpeed) * speedMultiplier, 0.5f, 3f);
        }
    }
}
