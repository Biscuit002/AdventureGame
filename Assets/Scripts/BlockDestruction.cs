using UnityEngine;

public class BlockDestruction : MonoBehaviour
{
    private float mouseX;
    private float mouseY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
                if (hit.collider.CompareTag("Block"))
                {
                    // Destroy the block
                    Destroy(hit.collider.gameObject);
                    Debug.Log("Destroyed: " + hit.collider.gameObject.name);
                }
            }
        }
    }
}
