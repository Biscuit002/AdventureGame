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

    [HideInInspector]
    public float weight;
    [HideInInspector]
    public float maxSupport;

    private Rigidbody2D rb;
    [HideInInspector]
    public bool isFalling = false;

    // Use a coroutine for Start() to allow a delay before initiating stability checks.
    IEnumerator Start()
    {
        // Wait for the specified delay. This delay helps ensure that the terrain generator has instantiated the blocks.
        yield return new WaitForSeconds(startDelay);
        
        // Retrieve the block properties from the manager based on blockTag.
        if (BlockDefinitionManager.Instance != null)
        {
            BlockDefinition def = BlockDefinitionManager.Instance.GetBlockDefinition(blockTag);
            if (def != null)
            {
                weight = def.weight;
                maxSupport = def.maxSupport;
            }
            else
            {
                Debug.LogError("BlockDefinition for tag '" + blockTag + "' was not found!");
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
    /// Checks if the total weight of blocks below exceeds this block's maxSupport.
    /// </summary>
    void CheckStability()
    {
        if (isFalling)
            return;

        float totalWeightBelow = CalculateWeightBelow();

        if (totalWeightBelow > maxSupport)
        {
            Fall();
        }
    }

    /// <summary>
    /// Recursively calculates the total weight of blocks below this block.
    /// It uses a safety counter (maxChecks) to avoid endless loops if blocks haven't been generated.
    /// </summary>
    float CalculateWeightBelow()
    {
        float totalWeight = 0f;
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.5f; // Start just below the block.

        int safetyCounter = 0;
        int maxChecks = 50;  // Maximum iterations to prevent infinite loops in case of missing blocks.
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1f);

        while (hit.collider != null && safetyCounter < maxChecks)
        {
            Block blockBelow = hit.collider.GetComponent<Block>();
            if (blockBelow != null)
            {
                totalWeight += blockBelow.weight;
            }

            // Advance the origin to just below the block that was hit.
            origin = (Vector2)hit.collider.transform.position + Vector2.down * 0.5f;
            hit = Physics2D.Raycast(origin, Vector2.down, 1f);
            safetyCounter++;
        }

        return totalWeight;
    }

    /// <summary>
    /// Triggers falling physics on this block and cascades the effect to connected blocks below.
    /// </summary>
    public void Fall()
    {
        if (isFalling)
            return;

        isFalling = true;

        // Add a Rigidbody2D component if one isn't already attached, so Unity's physics take over.
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 1;

        // Stop further stability checks since this block is now falling.
        CancelInvoke("CheckStability");

        // Trigger the fall cascade on any block immediately below.
        CascadeFall();
    }

    /// <summary>
    /// Checks the block directly below and triggers its Fall() if it isn’t already falling.
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
