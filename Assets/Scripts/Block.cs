using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Block : MonoBehaviour
{
    [Header("Block Properties")]

    [Tooltip("Set this to match the tag from your BlockDefinitionManager.")]
    public string blockTag;

    [Tooltip("Delay (in seconds) before starting stability checks (to allow terrain generation).")]
    public float startDelay = 0.5f;

    [Tooltip("Interval (in seconds) between each stability check.")]
    public float stabilityCheckInterval = 1.0f;

    [Tooltip("Layer mask used for detecting block colliders (assign only the layer that your blocks are on).")]
    public LayerMask blockLayerMask;

    [HideInInspector]
    public float weight;
    [HideInInspector]
    public float maxSupport;

    private Rigidbody2D rb;
    [HideInInspector]
    public bool isFalling = false;

    IEnumerator Start()
    {
        // Wait for terrain generation to complete
        yield return new WaitForSeconds(startDelay);

        // Retrieve block properties from the BlockDefinitionManager
        if (BlockDefinitionManager.Instance != null)
        {
            BlockDefinition def = BlockDefinitionManager.Instance.GetBlockDefinition(blockTag);
            if (def != null)
            {
                weight = def.weight;
                maxSupport = def.maxSupport;
                Debug.Log($"[{blockTag}] Properties set: weight = {weight}, maxSupport = {maxSupport}");
            }
            else
            {
                Debug.LogError($"BlockDefinition for tag '{blockTag}' was not found!");
            }
        }
        else
        {
            Debug.LogError("BlockDefinitionManager instance not found in the scene.");
        }

        // Begin checking stability at the defined interval.
        InvokeRepeating("CheckStability", 0f, stabilityCheckInterval);
    }

    /// <summary>
    /// Checks whether this block has immediate support and if the cumulative weight below exceeds its support value.
    /// </summary>
    void CheckStability()
    {
        if (isFalling)
            return;

        // Use an overlap check to see if there is a collider immediately below.
        // Calculate the bottom position of the block. Assuming your blocks are 1 unit tall,
        // the bottom is at (transform.position.y - 0.5). We add a small epsilon offset.
        float epsilon = 0.05f;  // Adjust this tolerance based on your collider dimensions
        Vector2 bottomPos = new Vector2(transform.position.x, transform.position.y - 0.5f + epsilon);
        Collider2D immediateCollider = Physics2D.OverlapCircle(bottomPos, epsilon, blockLayerMask);
        if (immediateCollider == null)
        {
            Debug.Log($"[{blockTag}] No immediate support detected at {bottomPos}. Falling.");
            Fall();
            return;
        }

        // Otherwise, calculate the total weight of blocks below.
        float totalWeightBelow = CalculateWeightBelow();
        Debug.Log($"[{blockTag}] Total weight below: {totalWeightBelow} (maxSupport = {maxSupport}) at {transform.position}");

        if (totalWeightBelow > maxSupport)
        {
            Fall();
        }
    }

    /// <summary>
    /// Recursively calculates the total weight of blocks below this block using raycasts.
    /// A safety counter prevents infinite loops if blocks are missing.
    /// </summary>
    float CalculateWeightBelow()
    {
        float totalWeight = 0f;
        // Start just below this block.
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.5f;

        int safetyCounter = 0;
        int maxChecks = 50;  // Maximum iterations as a safeguard.
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1f, blockLayerMask);

        while (hit.collider != null && safetyCounter < maxChecks)
        {
            Block blockBelow = hit.collider.GetComponent<Block>();
            if (blockBelow != null)
            {
                totalWeight += blockBelow.weight;
                Debug.Log($"-- Detected block [{blockBelow.blockTag}] at {hit.collider.transform.position} with weight {blockBelow.weight}");
            }

            // Advance the origin to just below the hit block.
            origin = (Vector2)hit.collider.transform.position + Vector2.down * 0.5f;
            hit = Physics2D.Raycast(origin, Vector2.down, 1f, blockLayerMask);
            safetyCounter++;
        }

        if (safetyCounter >= maxChecks)
        {
            Debug.LogWarning("CalculateWeightBelow safety counter reached maxChecks.");
        }

        return totalWeight;
    }

    /// <summary>
    /// Initiates falling physics on this block and cascades the falling effect to connected blocks below.
    /// </summary>
    public void Fall()
    {
        if (isFalling)
            return;

        isFalling = true;
        Debug.Log($"[{blockTag}] Falling triggered at {transform.position}");

        // Add a Rigidbody2D (or use the existing one) so physics take over.
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 1;

        // Stop further stability checks.
        CancelInvoke("CheckStability");

        // Cascade the fall to any block directly below.
        CascadeFall();
    }

    /// <summary>
    /// Checks for a block directly beneath and triggers its Fall() if not already falling.
    /// </summary>
    void CascadeFall()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1f, blockLayerMask);
        if (hit.collider != null)
        {
            Block belowBlock = hit.collider.GetComponent<Block>();
            if (belowBlock != null && !belowBlock.isFalling)
            {
                Debug.Log($"-- Cascading fall to block [{belowBlock.blockTag}] at {hit.collider.transform.position}");
                belowBlock.Fall();
            }
        }
    }
}
