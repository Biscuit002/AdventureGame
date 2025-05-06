using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Block2 : MonoBehaviour
{
    public bool isExposesd;

    void Update()
    {
        isExposesd = false; // Reset to false every frame

        RaycastHit hit;
        float rayDistance = 0.5f;

        // Cast rays in 4 directions
        if (!Physics.Raycast(transform.position, Vector3.up, out hit, rayDistance) || !hit.collider.CompareTag("Block"))
        {
            isExposesd = true;
        }
        if (!Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance) || !hit.collider.CompareTag("Block"))
        {
            isExposesd = true;
        }
        if (!Physics.Raycast(transform.position, Vector3.left, out hit, rayDistance) || !hit.collider.CompareTag("Block"))
        {
            isExposesd = true;
        }
        if (!Physics.Raycast(transform.position, Vector3.right, out hit, rayDistance) || !hit.collider.CompareTag("Block"))
        {
            isExposesd = true;
        }
    }
}
