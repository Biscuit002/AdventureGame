using UnityEngine;
using System.Collections;

public class BossBomb : MonoBehaviour
{
    public CircleCollider2D triggerArea; // Assign the trigger area in the Inspector
    public Collider2D bombCollider; // Assign the non-trigger collider
    public string targetTag; // The tag that triggers the explosion
    public float destructionDelay = 1f; // Time delay before explosion

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Only trigger explosion if collided object has the specified tag
        if (collision.gameObject.CompareTag(targetTag))
        {
            StartCoroutine(StartDestructionTimer());
        }
    }

    IEnumerator StartDestructionTimer()
    {
        yield return new WaitForSeconds(destructionDelay); // Wait before explosion

        DestroyAllTargetsInTrigger(); // Destroy objects within the trigger area
        Destroy(gameObject); // Destroy itself after explosion
    }

    void DestroyAllTargetsInTrigger()
    {
        // Find all colliders within the trigger area
        Collider2D[] colliders = Physics2D.OverlapCircleAll(triggerArea.transform.position, triggerArea.radius);

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag(targetTag))
            {
                Destroy(col.gameObject); // Destroy only objects matching the tag
            }
        }
    }
}
