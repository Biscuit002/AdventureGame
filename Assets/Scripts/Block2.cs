using NUnit.Framework;
using UnityEngine;

public class Block2 : MonoBehaviour
{
    float rayDistance = 0.1f;
    float offset = 0.6f;

    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.up * rayDistance);
        Gizmos.DrawRay(transform.position, Vector2.down * rayDistance);
        Gizmos.DrawRay(transform.position, Vector2.left * rayDistance);
        Gizmos.DrawRay(transform.position, Vector2.right * rayDistance);
    }
    public bool IsExposed()
    {
        RaycastHit2D hit;

        // Cast ray upwards
        hit = Physics2D.Raycast(transform.position + new Vector3(0, offset, 0), Vector2.up, rayDistance);
        if (hit.collider == null || !hit.collider.CompareTag("Block") && Vector2.Distance(hit.point, transform.position) > 0.1f)
        {
            return true;
        }

        // Cast ray downwards
        hit = Physics2D.Raycast(transform.position + new Vector3(0, -offset, 0), Vector2.down, rayDistance);
        if (hit.collider == null || !hit.collider.CompareTag("Block"))
        {
            return true;
        }

        // Cast ray to the left
        hit = Physics2D.Raycast(transform.position + new Vector3(-offset, 0, 0), Vector2.left, rayDistance);
        if (hit.collider == null || !hit.collider.CompareTag("Block"))
        {
            return true;
        }

        // Cast ray to the right
        hit = Physics2D.Raycast(transform.position + new Vector3(offset, 0, 0), Vector2.right, rayDistance);
        if (hit.collider == null || !hit.collider.CompareTag("Block"))
        {
            return true;
        }
        return false;
    }
}
