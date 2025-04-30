using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Block : MonoBehaviour
{
    [Header("Block Properties")]

    [Tooltip("Set this to match the tag from your BlockDefinitionManager.")]
    public string blockTag;

    [HideInInspector]
    public float weight;
    [HideInInspector]
    public float maxSupport;

    private Rigidbody2D rb;
    [HideInInspector]
    public bool isFalling = false;

    void Start()
    {
        // Retrieve and apply properties based on blockTag.
        if (BlockDefinitionManager.Instance != null)
        {
            BlockDefinition def = BlockDefinitionManager.Instance.GetBlockDefinition(blockTag);
            if (def != null)
            {
                weight = def.weight;
                maxSupport = def.maxSupport;
            }
        }
        else
        {
            Debug.LogError("BlockDefinitionManager instance not found in the scene.");
        }

        // Begin checking stability every 0.5 seconds.
        InvokeRepeating("CheckStability", 0f, 0.5f);
    }

    /// <summary>
    /// Determines if this block is overloaded by the blocks beneath it.
    /// </summary>
    void CheckStability()
    {
        if (isFalling)
            return;

        float totalWeightBelow = CalculateWeightBelow();

        // If the cumulative weight below exceeds what this block can support, trigger a fall.
        if (totalWeightBelow > maxSupport)
        {
            Fall();
        }
    }

    /// <summary>
    /// Recursively calculates the total weight of all blocks directly below this one.
    /// Assumes each block occupies 1 unit of vertical space.
    /// </summary>
    float CalculateWeightBelow()
    {
        float totalWeight = 0f;
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.5f; // start just below center

        // Cast a ray downward with a distance equal to the block's height (assuming 1 unit)
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1f);
        while (hit.collider != null)
        {
            Block blockBelow = hit.collider.GetComponent<Block>();
            if (blockBelow != null)
            {
                totalWeight += blockBelow.weight;
                // Move the origin to just below the found block and check further down.
                origin = (Vector2)blockBelow.transform.position + Vector2.down * 0.5f;
                hit = Physics2D.Raycast(origin, Vector2.down, 1f);
            }
            else
            {
                break;
            }
        }
        return totalWeight;
    }

    /// <summary>
    /// Initiates falling physics on this block and cascades the fall to connected blocks.
    /// </summary>
    public void Fall()
    {
        if (isFalling)
            return;

        isFalling = true;

        // Add a Rigidbody2D component if not already present
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 1;

        // Stop checking stability since the block is now falling.
        CancelInvoke("CheckStability");

        // Trigger cascade: force any block directly below to fall as well.
        CascadeFall();
    }

    /// <summary>
    /// Casts a ray downward to identify and trigger a fall in any connected block beneath.
    /// </summary>
    void CascadeFall()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1f);
        if (hit.collider != null)
        {
            Block belowBlock = hit.collider.GetComponent<Block>();
            if (belowBlock != null && !belowBlock.isFalling)
            {
                belowBlock.Fall();
            }
        }
    }
}
