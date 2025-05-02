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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                Block2 hitBlock = hit.collider.GetComponent<Block2>();
                if (hit.collider.CompareTag("Block") && hitBlock != null && hitBlock.isExposesd)
                {
                    Destroy(hit.collider.gameObject);
                }
            }
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

        Instantiate(grassPrefab, hitPoint, Quaternion.identity);

    }
}
