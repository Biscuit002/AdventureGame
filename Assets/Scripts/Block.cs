using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Block : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isFalling = false;

    void Start()
    {
        // Start with physics disabled
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void EnablePhysics()
    {
        if (!isFalling)
        {
            isFalling = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 1;
        }
    }

    public void DisablePhysics()
    {
        if (isFalling)
        {
            isFalling = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;
        }
    }
}