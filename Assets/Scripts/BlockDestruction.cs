using System.Collections.Concurrent;
using UnityEngine;

public class BlockDestruction : MonoBehaviour
{
    private float mouseX;
    private float mouseY;
    public GameObject grassPrefab;
    public GameObject dirtPrefab;
    public GameObject stonePrefab;
    public GameObject woodPrefab;
    public GameObject leafPrefab;
    public GameObject copperPrefab;
    public GameObject diamondPrefab;
    public GameObject goldPrefab;
    public GameObject ironPrefab;
    public GameObject platinumPrefab;
    public GameObject silverPrefab;
    public GameObject titaniumPrefab;
    private float blockInteractionDistance = 5f;
    public Inventory inventory;
    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }
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
            mousePosition.z = 0;

            mousePosition = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), -0.01f);
            Instantiate(grassPrefab, mousePosition, Quaternion.identity);
        }
    }

    private void DestroyBlock()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.CompareTag("Block"))
        {
            Block2 block2 = hit.collider.GetComponent<Block2>();
            if (block2.IsExposed() && Vector2.Distance(hit.point, transform.position) < blockInteractionDistance)
            {
                Destroy(hit.collider.gameObject);
                if(block2.blockType == "Grass")
                {
                    inventory.grassAmount++;
                }
                else if(block2.blockType == "Dirt")
                {
                    inventory.dirtAmount++;
                }
                else if(block2.blockType == "Stone")
                {
                    inventory.stoneAmount++;
                }
                else if(block2.blockType == "Wood")
                {
                    inventory.woodAmount++;
                }
                else if(block2.blockType == "Leaf")
                {
                    inventory.leafAmount++;
                }
                else if(block2.blockType == "Copper")
                {
                    inventory.copperAmount++;
                }
                else if(block2.blockType == "Diamond")
                {
                    inventory.diamondAmount++;
                }
                else if(block2.blockType == "Gold")
                {
                    inventory.goldAmount++;
                }
                else if(block2.blockType == "Iron")
                {
                    inventory.ironAmount++;
                }
                else if(block2.blockType == "Platinum")
                {
                    inventory.platinumAmount++;
                }
                else if(block2.blockType == "Silver")
                {
                    inventory.silverAmount++;
                }
                else if(block2.blockType == "Titanium")
                {
                    inventory.titaniumAmount++;
                }
            }
        }
    }
}
