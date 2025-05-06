using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Generation_Matrix : MonoBehaviour
{
    [Header("World Generation Settings")]
    public int rows = 128;
    public int cols = 128;
    public float fillProbability = 0.35f;
    public int smoothIterations = 3;
    public int carveDepth = 50;
    public int surfacePadding = 5;

    [Header("Prefab References")]
    public GameObject airPrefab;
    public GameObject grassPrefab;
    public GameObject dirtPrefab;
    public GameObject stonePrefab;
    public GameObject treeTrunkPrefab;
    public GameObject treeLeavesPrefab;
    public GameObject[] orePrefabs; // Index should match ore values

    [Header("Generation Settings")]
    public float blockSize = 1f;
    public Vector3 startPosition = Vector3.zero;

    private int[,] matrix;
    private bool[] mountainRangeMarker;
    private bool[] forestRangeMarker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        // Create the base matrix
        matrix = CreateMatrixWithPath(rows, cols);
        
        // Spawn the world based on the matrix
        SpawnWorld();
    }

    private int[,] CreateMatrixWithPath(int rows, int cols)
    {
        int[,] matrix = new int[rows, cols];
        
        // 1. Basic terrain generation
        int baseMin = 15;
        int baseMax = 25;
        int currentRow = UnityEngine.Random.Range(18, 22);

        // Generate basic terrain
        for (int col = 0; col < cols; col++)
        {
            int effectiveRow = Mathf.Clamp(currentRow, baseMin, baseMax);
            matrix[effectiveRow, col] = 1; // Grass

            if (effectiveRow + 1 < rows && UnityEngine.Random.value < 1f/3f)
            {
                matrix[effectiveRow + 1, col] = 1;
            }

            // Add dirt below grass
            for (int row = effectiveRow + 1; row < rows; row++)
            {
                if (matrix[row, col] == 0)
                {
                    matrix[row, col] = 2; // Dirt
                }
            }

            // Randomly adjust height for next column
            currentRow += UnityEngine.Random.Range(-1, 2);
        }

        // 2. Add stone layer
        AddStoneLayer(matrix, rows, cols);

        // 3. Generate caves
        GenerateCaves(matrix, rows, cols);

        // 4. Generate mountains and canyons
        GenerateMountainsAndCanyons(matrix, rows, cols);

        // 5. Generate ores
        GenerateOres(matrix, rows, cols);

        // 6. Generate trees
        GenerateTrees(matrix, rows, cols);

        return matrix;
    }

    private void SpawnWorld()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 position = new Vector3(
                    startPosition.x + col * blockSize,
                    startPosition.y - row * blockSize,
                    startPosition.z
                );

                GameObject prefabToSpawn = null;
                switch (matrix[row, col])
                {
                    case 0: // Air
                        prefabToSpawn = airPrefab;
                        break;
                    case 1: // Grass
                        prefabToSpawn = grassPrefab;
                        break;
                    case 2: // Dirt
                        prefabToSpawn = dirtPrefab;
                        break;
                    case 3: // Stone
                        prefabToSpawn = stonePrefab;
                        break;
                    case 4: // Tree Trunk
                        prefabToSpawn = treeTrunkPrefab;
                        break;
                    case 5: // Tree Leaves
                        prefabToSpawn = treeLeavesPrefab;
                        break;
                    default: // Ores
                        if (matrix[row, col] >= 6 && matrix[row, col] < 6 + orePrefabs.Length)
                        {
                            prefabToSpawn = orePrefabs[matrix[row, col] - 6];
                        }
                        break;
                }

                if (prefabToSpawn != null)
                {
                    Instantiate(prefabToSpawn, position, Quaternion.identity, transform);
                }
            }
        }
    }

    private void AddStoneLayer(int[,] matrix, int rows, int cols)
{
    int minStoneRow = 25; // Don’t generate stone above this row
    float[,] stoneNoise = new float[rows, cols];

    // Generate noise
    for (int row = 0; row < rows; row++)
    {
        for (int col = 0; col < cols; col++)
        {
            stoneNoise[row, col] = UnityEngine.Random.value;
        }
    }

    // Smooth the noise
    stoneNoise = SmoothArray(stoneNoise, 2);

    // Assign stone bottom-up
    for (int row = rows - 1; row >= minStoneRow; row--)
    {
        for (int col = 0; col < cols; col++)
        {
            float threshold = (row + 22f) / rows;
            if (stoneNoise[row, col] < threshold)
            {
                // Only replace dirt (don't overwrite grass or tree roots)
                if (matrix[row, col] == 2)
                {
                    matrix[row, col] = 3; // Stone
                }
            }
        }
    }

    // Add dirt patches inside stone for variety
    for (int col = 0; col < cols; col++)
    {
        for (int row = minStoneRow; row < rows; row++)
        {
            if (matrix[row, col] == 3 && UnityEngine.Random.value < Mathf.Max(0.02f, 1f - row / (float)rows))
            {
                matrix[row, col] = 2; // Dirt patch
            }
        }
    }
}


    private void GenerateCaves(int[,] matrix, int rows, int cols)
    {
        // Create initial cave noise
        bool[,] caveLayer = new bool[rows, cols];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                caveLayer[row, col] = UnityEngine.Random.value < fillProbability;
            }
        }

        // Smooth the cave layer
        for (int i = 0; i < smoothIterations; i++)
        {
            caveLayer = SmoothCaveStep(caveLayer, rows, cols);
        }

        // Carve out caves
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (row > carveDepth && caveLayer[row, col] && matrix[row, col] == 3)
                {
                    matrix[row, col] = 0; // Air
                }
            }
        }

        // Remove narrow cave structures
        RemoveNarrowCaveStructures(matrix, rows, cols);
    }

    private bool[,] SmoothCaveStep(bool[,] layer, int rows, int cols)
    {
        bool[,] newLayer = new bool[rows, cols];
        for (int row = 1; row < rows - 1; row++)
        {
            for (int col = 1; col < cols - 1; col++)
            {
                int neighbors = 0;
                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        if (dr == 0 && dc == 0) continue;
                        if (layer[row + dr, col + dc]) neighbors++;
                    }
                }
                newLayer[row, col] = neighbors >= 4;
            }
        }
        return newLayer;
    }

    private void RemoveNarrowCaveStructures(int[,] matrix, int rows, int cols)
    {
        bool[,] visited = new bool[rows, cols];
        Vector2Int[] directions = new Vector2Int[] {
            new Vector2Int(-1, 0), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(0, 1)
        };

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (matrix[row, col] == 0 && !visited[row, col])
                {
                    List<Vector2Int> region = new List<Vector2Int>();
                    Dictionary<int, List<int>> rowDict = new Dictionary<int, List<int>>();
                    Stack<Vector2Int> stack = new Stack<Vector2Int>();
                    
                    stack.Push(new Vector2Int(row, col));
                    visited[row, col] = true;

                    while (stack.Count > 0)
                    {
                        Vector2Int pos = stack.Pop();
                        region.Add(pos);
                        
                        if (!rowDict.ContainsKey(pos.x))
                            rowDict[pos.x] = new List<int>();
                        rowDict[pos.x].Add(pos.y);

                        foreach (Vector2Int dir in directions)
                        {
                            Vector2Int newPos = pos + dir;
                            if (newPos.x >= 0 && newPos.x < rows && 
                                newPos.y >= 0 && newPos.y < cols &&
                                !visited[newPos.x, newPos.y] && 
                                matrix[newPos.x, newPos.y] == 0)
                            {
                                visited[newPos.x, newPos.y] = true;
                                stack.Push(newPos);
                            }
                        }
                    }

                    // Check if region is too small
                    int maxContiguous = 0;
                    foreach (var kvp in rowDict)
                    {
                        kvp.Value.Sort();
                        int currentCount = 1;
                        int maxForRow = 1;
                        for (int i = 1; i < kvp.Value.Count; i++)
                        {
                            if (kvp.Value[i] == kvp.Value[i - 1] + 1)
                                currentCount++;
                            else
                            {
                                maxForRow = Mathf.Max(maxForRow, currentCount);
                                currentCount = 1;
                            }
                        }
                        maxForRow = Mathf.Max(maxForRow, currentCount);
                        maxContiguous = Mathf.Max(maxContiguous, maxForRow);
                    }

                    int verticalExtent = rowDict.Keys.Max() - rowDict.Keys.Min() + 1;
                    int regionArea = region.Count;

                    if (maxContiguous < 3 || verticalExtent < 3 || regionArea < 10)
                    {
                        foreach (Vector2Int pos in region)
                        {
                            matrix[pos.x, pos.y] = 2; // Fill with dirt
                        }
                    }
                }
            }
        }
    }

    private float[,] SmoothArray(float[,] arr, int iterations)
    {
        float[,] res = (float[,])arr.Clone();
        for (int i = 0; i < iterations; i++)
        {
            float[,] padded = PadArray(res, 1);
            float[,] smoothed = new float[arr.GetLength(0), arr.GetLength(1)];
            
            for (int row = 0; row < arr.GetLength(0); row++)
            {
                for (int col = 0; col < arr.GetLength(1); col++)
                {
                    float sum = 0;
                    for (int dr = 0; dr < 3; dr++)
                    {
                        for (int dc = 0; dc < 3; dc++)
                        {
                            sum += padded[row + dr, col + dc];
                        }
                    }
                    smoothed[row, col] = sum / 9f;
                }
            }
            res = smoothed;
        }
        return res;
    }

    private float[,] PadArray(float[,] arr, int padding)
    {
        int rows = arr.GetLength(0);
        int cols = arr.GetLength(1);
        float[,] padded = new float[rows + 2 * padding, cols + 2 * padding];
        
        // Copy center
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                padded[row + padding, col + padding] = arr[row, col];
            }
        }
        
        // Pad edges
        for (int row = 0; row < rows + 2 * padding; row++)
        {
            for (int col = 0; col < cols + 2 * padding; col++)
            {
                if (row < padding || row >= rows + padding || 
                    col < padding || col >= cols + padding)
                {
                    int r = Mathf.Clamp(row - padding, 0, rows - 1);
                    int c = Mathf.Clamp(col - padding, 0, cols - 1);
                    padded[row, col] = arr[r, c];
                }
            }
        }
        
        return padded;
    }

    private void GenerateOres(int[,] matrix, int rows, int cols)
    {
        // Ore definitions
        var oreDefs = new List<OreDefinition>
        {
            new OreDefinition { name = "copper", value = 12, depth = 1, rarity = 2 },
            new OreDefinition { name = "silver", value = 7, depth = 2, rarity = 3 },
            new OreDefinition { name = "gold", value = 8, depth = 3, rarity = 4 },
            new OreDefinition { name = "iron", value = 10, depth = 2, rarity = 2 },
            new OreDefinition { name = "platinum", value = 9, depth = 4, rarity = 5 },
            new OreDefinition { name = "titanium", value = 11, depth = 4, rarity = 4 },
            new OreDefinition { name = "diamond", value = 6, depth = 5, rarity = 5 }
        };

        float baseDiamond = 0.05f / 3f;
        float RarityMultiplier(int r) => 2.0f - 0.3f * (r - 1);

        // Calculate thresholds and probabilities
        foreach (var ore in oreDefs)
        {
            float computed = 7 + (ore.depth - 1) * 18.25f - 12;
            ore.threshold = Mathf.Max(0, computed) / rows;
            ore.baseProbability = baseDiamond * RarityMultiplier(ore.rarity);
            ore.gamma = 1 + (6 - ore.depth) / 2.0f;
        }

        // Sort by threshold (highest first)
        oreDefs.Sort((a, b) => b.threshold.CompareTo(a.threshold));

        // Generate ores
        for (int row = 0; row < rows; row++)
        {
            float normDepth = row / (float)rows;
            for (int col = 0; col < cols; col++)
            {
                if (matrix[row, col] == 3) // Only in stone
                {
                    foreach (var ore in oreDefs)
                    {
                        if (normDepth >= ore.threshold)
                        {
                            float depthFactor = Mathf.Pow(
                                (normDepth - ore.threshold) / (1 - ore.threshold),
                                ore.gamma
                            );
                            float chance = ore.baseProbability * depthFactor;

                            if (UnityEngine.Random.value < chance)
                            {
                                matrix[row, col] = ore.value;
                                
                                // Add ore clusters
                                for (int dx = -1; dx <= 1; dx++)
                                {
                                    for (int dy = -1; dy <= 1; dy++)
                                    {
                                        if (dx == 0 && dy == 0) continue;
                                        
                                        int newRow = row + dx;
                                        int newCol = col + dy;
                                        
                                        if (newRow >= 0 && newRow < rows && 
                                            newCol >= 0 && newCol < cols &&
                                            matrix[newRow, newCol] == 3 && 
                                            UnityEngine.Random.value < 0.5f)
                                        {
                                            matrix[newRow, newCol] = ore.value;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void GenerateTrees(int[,] matrix, int rows, int cols)
    {
        int lastTreeCol = -3;
        bool[] mountainRangeMarker = new bool[cols]; // You'll need to implement this
        bool[] forestRangeMarker = new bool[cols];   // You'll need to implement this

        // Regular trees
        for (int col = 0; col < cols; col++)
        {
            if (mountainRangeMarker[col]) continue;
            if (forestRangeMarker[col]) continue;

            // Find grass surface
            int grassTop = -1;
            for (int row = 0; row < rows; row++)
            {
                if (matrix[row, col] == 1)
                {
                    grassTop = row;
                    break;
                }
            }

            if (grassTop == -1 || grassTop < 22) continue;
            if (col - lastTreeCol < 2) continue;

            if (UnityEngine.Random.value < 0.3f)
            {
                int trunkHeight = UnityEngine.Random.Range(4, 8);
                List<int> trunkPositions = new List<int>();
                int trunkTop = Mathf.Max(0, grassTop - trunkHeight + 1);

                // Create trunk
                for (int row = trunkTop; row <= grassTop; row++)
                {
                    matrix[row, col] = 4; // Tree trunk
                    trunkPositions.Add(row);

                    if (UnityEngine.Random.value < 0.2f)
                    {
                        int side = UnityEngine.Random.Range(0, 2) * 2 - 1; // -1 or 1
                        if (col + side >= 0 && col + side < cols && 
                            matrix[row, col + side] == 0)
                        {
                            matrix[row, col + side] = 4;
                        }
                    }
                }

                lastTreeCol = col;

                // Add leaves
                foreach (int row in trunkPositions)
                {
                    if (grassTop - row >= 2)
                    {
                        int radius = Mathf.Max(1, 2 - (grassTop - row - 2));
                        AddLeaves(matrix, row, col, radius, rows, cols);
                    }
                }
            }
        }

        // Forest trees
        for (int col = 0; col < cols; col++)
        {
            if (!forestRangeMarker[col]) continue;

            // Find grass surface
            int grassTop = -1;
            for (int row = 0; row < rows; row++)
            {
                if (matrix[row, col] == 1)
                {
                    grassTop = row;
                    break;
                }
            }

            if (grassTop == -1) continue;

            if (UnityEngine.Random.value < 0.8f)
            {
                int trunkHeight = UnityEngine.Random.Range(6, 11);
                List<int> trunkPositions = new List<int>();
                int trunkTop = Mathf.Max(0, grassTop - trunkHeight + 1);

                // Create trunk
                for (int row = trunkTop; row <= grassTop; row++)
                {
                    matrix[row, col] = 4;
                    trunkPositions.Add(row);

                    if (UnityEngine.Random.value < 0.4f)
                    {
                        for (int side = -1; side <= 1; side += 2)
                        {
                            if (col + side >= 0 && col + side < cols && 
                                matrix[row, col + side] == 0)
                            {
                                matrix[row, col + side] = 4;
                            }
                        }
                    }
                }

                // Add leaves
                foreach (int row in trunkPositions)
                {
                    if (grassTop - row >= 2)
                    {
                        int radius = Mathf.Max(1, 3 - Mathf.Max(0, (grassTop - row - 2)));
                        AddLeaves(matrix, row, col, radius, rows, cols);
                    }
                }
            }
        }
    }

    private void AddLeaves(int[,] matrix, int row, int col, int radius, int rows, int cols)
    {
        for (int dx = -radius; dx <= radius; dx++)
        {
            if (dx == 0) continue;
            int newCol = col + dx;
            if (newCol >= 0 && newCol < cols)
            {
                if (matrix[row, newCol] == 0 || matrix[row, newCol] == 2)
                {
                    matrix[row, newCol] = 5; // Leaves
                }
                if (row - 1 >= 0 && (matrix[row - 1, newCol] == 0 || matrix[row - 1, newCol] == 2))
                {
                    matrix[row - 1, newCol] = 5;
                }
            }
        }
    }

    private void GenerateMountainsAndCanyons(int[,] matrix, int rows, int cols)
    {
        // Generate mountain ranges
        bool[] mountainRangeMarker = new bool[cols];
        bool[] forestRangeMarker = new bool[cols];
        
        // Create noise for mountain ranges
        float[] mountainNoise = new float[cols];
        for (int col = 0; col < cols; col++)
        {
            mountainNoise[col] = UnityEngine.Random.value;
        }
        
        // Smooth mountain noise
        mountainNoise = Smooth1DArray(mountainNoise, 3);
        
        // Identify mountain ranges
        for (int col = 0; col < cols; col++)
        {
            if (mountainNoise[col] > 0.7f)
            {
                mountainRangeMarker[col] = true;
                
                // Create mountain
                int baseHeight = UnityEngine.Random.Range(15, 20);
                int peakHeight = UnityEngine.Random.Range(5, 10);
                
                for (int row = 0; row < rows; row++)
                {
                    if (row < baseHeight)
                    {
                        matrix[row, col] = 3; // Stone
                    }
                    else if (row < baseHeight + peakHeight)
                    {
                        float slope = (row - baseHeight) / (float)peakHeight;
                        if (UnityEngine.Random.value < slope)
                        {
                            matrix[row, col] = 3; // Stone
                        }
                    }
                }
                
                // Add forest markers near mountains
                if (col > 0) forestRangeMarker[col - 1] = true;
                if (col < cols - 1) forestRangeMarker[col + 1] = true;
            }
        }
        
        // Generate canyons
        float[] canyonNoise = new float[cols];
        for (int col = 0; col < cols; col++)
        {
            canyonNoise[col] = UnityEngine.Random.value;
        }
        
        // Smooth canyon noise
        canyonNoise = Smooth1DArray(canyonNoise, 2);
        
        // Create canyons
        for (int col = 0; col < cols; col++)
        {
            if (canyonNoise[col] > 0.8f && !mountainRangeMarker[col])
            {
                int canyonWidth = UnityEngine.Random.Range(3, 7);
                int canyonDepth = UnityEngine.Random.Range(5, 10);
                
                for (int w = 0; w < canyonWidth; w++)
                {
                    int currentCol = col + w;
                    if (currentCol >= cols) break;
                    
                    // Find surface
                    int surfaceRow = -1;
                    for (int row = 0; row < rows; row++)
                    {
                        if (matrix[row, currentCol] == 1)
                        {
                            surfaceRow = row;
                            break;
                        }
                    }
                    
                    if (surfaceRow != -1)
                    {
                        // Carve canyon
                        for (int row = surfaceRow; row < surfaceRow + canyonDepth; row++)
                        {
                            if (row < rows)
                            {
                                matrix[row, currentCol] = 0; // Air
                            }
                        }
                        
                        // Add canyon walls
                        for (int row = surfaceRow + canyonDepth; row < rows; row++)
                        {
                            if (row < rows)
                            {
                                matrix[row, currentCol] = 3; // Stone
                            }
                        }
                    }
                }
            }
        }
        
        // Update the markers for tree generation
        this.mountainRangeMarker = mountainRangeMarker;
        this.forestRangeMarker = forestRangeMarker;
    }

    private float[] Smooth1DArray(float[] arr, int iterations)
    {
        float[] res = (float[])arr.Clone();
        for (int i = 0; i < iterations; i++)
        {
            float[] padded = new float[arr.Length + 2];
            Array.Copy(arr, 0, padded, 1, arr.Length);
            padded[0] = arr[0];
            padded[padded.Length - 1] = arr[arr.Length - 1];
            
            for (int j = 1; j < padded.Length - 1; j++)
            {
                res[j - 1] = (padded[j - 1] + padded[j] + padded[j + 1]) / 3f;
            }
        }
        return res;
    }

    private class OreDefinition
    {
        public string name;
        public int value;
        public int depth;
        public int rarity;
        public float threshold;
        public float baseProbability;
        public float gamma;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
