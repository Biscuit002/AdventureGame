using System.Collections.Concurrent;
using UnityEngine;

public class BlockDestruction : MonoBehaviour
{
    private float mouseX;
    private float mouseY;
    public GameObject grassPrefab;
    private float blockInteractionDistance = 5f;
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButtonDown(0))
        {
            DestroyBlock();
        }
        if (Input.GetMouseButtonDown(1))
        {
            placeBlock();
        }
    }

    private void placeBlock()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider == null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Set z to 0 to place the block on the same plane

            mousePosition = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), -0.01f); // Round the position to snap to grid
            Instantiate(grassPrefab, mousePosition, Quaternion.identity);
        }

    }

    private void DestroyBlock()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.CompareTag("Block"))
        {
            // Check if the block is exposed before destroying it
            Block2 block2 = hit.collider.GetComponent<Block2>();
            if (block2.IsExposed() && Vector2.Distance(hit.point, transform.position) < blockInteractionDistance)
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
