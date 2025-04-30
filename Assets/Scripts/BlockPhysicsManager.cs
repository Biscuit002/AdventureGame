using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BlockPhysicsManager : MonoBehaviour
{
    public static BlockPhysicsManager Instance { get; private set; }
    
    [Header("Settings")]
    public float physicsRadius = 32f;
    public float updateInterval = 1f;
    public int chunkSize = 16;
    
    private Transform player;
    private Dictionary<Vector2Int, List<Block>> chunkMap = new Dictionary<Vector2Int, List<Block>>();
    private List<Block> activeBlocks = new List<Block>();
    private float nextUpdateTime;
    private Vector2Int currentChunk;
    private bool isUpdating = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextUpdateTime = Time.time;
        currentChunk = GetChunkPosition(player.position);
        
        // Initialize chunk map with existing blocks
        Block[] allBlocks = FindObjectsOfType<Block>();
        foreach (Block block in allBlocks)
        {
            Vector2Int chunkPos = GetChunkPosition(block.transform.position);
            if (!chunkMap.ContainsKey(chunkPos))
            {
                chunkMap[chunkPos] = new List<Block>();
            }
            chunkMap[chunkPos].Add(block);
        }
    }

    void Update()
    {
        if (Time.time >= nextUpdateTime && !isUpdating)
        {
            StartCoroutine(UpdatePhysicsInChunks());
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    System.Collections.IEnumerator UpdatePhysicsInChunks()
    {
        isUpdating = true;
        
        Vector2Int playerChunk = GetChunkPosition(player.position);
        
        if (playerChunk != currentChunk)
        {
            currentChunk = playerChunk;
            
            // Get all chunks within physics radius
            HashSet<Vector2Int> chunksToUpdate = GetChunksInRadius(playerChunk);
            
            foreach (Vector2Int chunkPos in chunksToUpdate)
            {
                UpdateChunk(chunkPos);
                yield return null; // Wait one frame between chunks
            }
        }
        else
        {
            UpdateChunk(playerChunk);
        }
        
        isUpdating = false;
    }

    HashSet<Vector2Int> GetChunksInRadius(Vector2Int centerChunk)
    {
        HashSet<Vector2Int> chunks = new HashSet<Vector2Int>();
        int radiusInChunks = Mathf.CeilToInt(physicsRadius / chunkSize);
        
        for (int x = -radiusInChunks; x <= radiusInChunks; x++)
        {
            for (int y = -radiusInChunks; y <= radiusInChunks; y++)
            {
                Vector2Int chunkPos = new Vector2Int(
                    centerChunk.x + x,
                    centerChunk.y + y
                );
                
                if (Vector2.Distance(
                    new Vector2(chunkPos.x * chunkSize, chunkPos.y * chunkSize),
                    new Vector2(centerChunk.x * chunkSize, centerChunk.y * chunkSize)
                ) <= physicsRadius)
                {
                    chunks.Add(chunkPos);
                }
            }
        }
        
        return chunks;
    }

    void UpdateChunk(Vector2Int chunkPos)
    {
        if (!chunkMap.ContainsKey(chunkPos)) return;
        
        Vector2 chunkCenter = new Vector2(
            chunkPos.x * chunkSize + chunkSize / 2f,
            chunkPos.y * chunkSize + chunkSize / 2f
        );
        
        foreach (Block block in chunkMap[chunkPos])
        {
            float distance = Vector2.Distance(block.transform.position, player.position);
            
            if (distance <= physicsRadius)
            {
                if (!activeBlocks.Contains(block))
                {
                    activeBlocks.Add(block);
                    block.EnablePhysics();
                }
            }
            else
            {
                if (activeBlocks.Contains(block))
                {
                    activeBlocks.Remove(block);
                    block.DisablePhysics();
                }
            }
        }
    }

    Vector2Int GetChunkPosition(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / chunkSize),
            Mathf.FloorToInt(position.y / chunkSize)
        );
    }

    // Call this when a block is created or destroyed
    public void UpdateBlockInChunks(Block block, Vector2Int oldChunk)
    {
        Vector2Int newChunk = GetChunkPosition(block.transform.position);
        
        if (chunkMap.ContainsKey(oldChunk))
        {
            chunkMap[oldChunk].Remove(block);
        }
        
        if (!chunkMap.ContainsKey(newChunk))
        {
            chunkMap[newChunk] = new List<Block>();
        }
        
        chunkMap[newChunk].Add(block);
    }

    void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, physicsRadius);
            
            Gizmos.color = Color.cyan;
            Vector2Int chunk = GetChunkPosition(player.position);
            Vector3 chunkCenter = new Vector3(
                chunk.x * chunkSize + chunkSize / 2f,
                chunk.y * chunkSize + chunkSize / 2f,
                0
            );
            Gizmos.DrawWireCube(chunkCenter, new Vector3(chunkSize, chunkSize, 0));
        }
    }
} 