using UnityEngine;

public class BlockDestruction : MonoBehaviour
{
    private float mouseX;
    private float mouseY;
    public GameObject grassPrefab;
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        Vector3 hitPoint = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), -0.01f); 

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
            if (block2.IsExposed())
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
