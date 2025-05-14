using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockDestruction : MonoBehaviour
{
    private float mouseX;
    private float mouseY;
    public GameObject[] blockPrefabs;
    private float blockInteractionDistance = 5f;
    public Inventory inventory;
    public int blockIndex;
    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();

    }
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            blockIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            blockIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            blockIndex = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            blockIndex = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            blockIndex = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            blockIndex = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            blockIndex = 6;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            blockIndex = 7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            blockIndex = 8;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            blockIndex = 9;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            blockIndex = 10;
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            blockIndex = 11;
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            blockIndex = 12;
        }

        if (Input.GetMouseButtonDown(0))
        {
            DestroyBlock();
        }
        if (Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < blockPrefabs.Length; i++)
            {
                if (i == blockIndex && inventory.currentBlockAmount > 0)
                {
                    placeBlock(blockPrefabs[i]);
                }
            }
            if (blockIndex == 0 && inventory.grassAmount > 0)
            {
                inventory.grassAmount--;
            }
            else if (blockIndex == 1 && inventory.dirtAmount > 0)
            {
                inventory.dirtAmount--;
            }
            else if (blockIndex == 2 && inventory.stoneAmount > 0)
            {
                inventory.stoneAmount--;
            }
            else if (blockIndex == 3 && inventory.woodAmount > 0)
            {
                inventory.woodAmount--;
            }
            else if (blockIndex == 4 && inventory.leafAmount > 0)
            {
                inventory.leafAmount--;
            }
            else if (blockIndex == 5 && inventory.copperAmount > 0)
            {
                inventory.copperAmount--;
            }
            else if (blockIndex == 6 && inventory.diamondAmount > 0)
            {
                inventory.diamondAmount--;
            }
            else if (blockIndex == 7 && inventory.goldAmount > 0)
            {
                inventory.goldAmount--;
            }
            else if (blockIndex == 8 && inventory.ironAmount > 0)
            {
                inventory.ironAmount--;
            }
            else if (blockIndex == 9 && inventory.platinumAmount > 0)
            {
                inventory.platinumAmount--;
            }
            else if (blockIndex == 10 && inventory.silverAmount > 0)
            {
                inventory.silverAmount--;
            }
            else if (blockIndex == 11 && inventory.titaniumAmount > 0)
            {
                inventory.titaniumAmount--;
            }
        }
    }

    private void placeBlock(GameObject blockType)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider == null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            mousePosition = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), -0.01f);
            Instantiate(blockType, mousePosition, Quaternion.identity);
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
