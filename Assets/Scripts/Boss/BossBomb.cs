using UnityEngine;

public class BossBomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 3f;        // Radius in which blocks will be affected.
    public float explosionForce = 10f;        // Force applied to the blocks.
    public LayerMask blockLayer;              // Layer for destructible/affected blocks.
    public float fuseTime = 2f;               // Optional: fuse timer before explosion.

    private bool hasExploded = false;

    private void Start()
    {
        // If you want a timed explosion instead of relying solely on trigger collision, uncomment below.
        // Invoke("Explode", fuseTime);
    }

    // When the bomb's collider (set as trigger) hits an object tagged "Ground", trigger the explosion.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is tagged as "Ground" (adjust if needed) and if we haven't exploded yet.
        if (!hasExploded && collision.CompareTag("Ground"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        // Find all colliders within the explosionRadius on the specified blockLayer.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, blockLayer);
        foreach (Collider2D col in hitColliders)
        {
            // Attempt to get a Rigidbody2D from the hit object.
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                // If no rigidbody exists, add a dynamic one so that the block becomes affected by physics.
                rb = col.gameObject.AddComponent<Rigidbody2D>();
                // Optionally, set properties for the added rigidbody.
                rb.gravityScale = 1f;
                rb.mass = 1f;
            }

            // Calculate a direction and apply explosion force.
            Vector2 explosionDirection = (rb.transform.position - transform.position).normalized;
            rb.AddForce(explosionDirection * explosionForce, ForceMode2D.Impulse);
        }

        // Optionally, instantiate explosion visual or audio effects here.

        // Finally, destroy the bomb GameObject.
        Destroy(gameObject);
    }

    // Draw gizmos in the editor to visualize the explosion radius.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
