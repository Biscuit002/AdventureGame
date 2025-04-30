using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BlockDefinition
{
    [Tooltip("The unique tag used to identify this block type.")]
    public string tag;

    [Tooltip("The weight of this block. This value contributes to the weight load on blocks above.")]
    public float weight = 1f;

    [Tooltip("How much cumulative weight this block can support above it before collapsing.")]
    public float maxSupport = 5f;  
}

public class BlockDefinitionManager : MonoBehaviour
{
    public static BlockDefinitionManager Instance { get; private set; }

    [Tooltip("List of block definitions. Add or modify definitions here in the editor.")]
    public List<BlockDefinition> blockDefinitions = new List<BlockDefinition>();

    private Dictionary<string, BlockDefinition> blockDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeDictionary()
    {
        blockDict = new Dictionary<string, BlockDefinition>();
        foreach (BlockDefinition def in blockDefinitions)
        {
            if (!blockDict.ContainsKey(def.tag))
                blockDict.Add(def.tag, def);
            else
                Debug.LogWarning("Duplicate block tag detected: " + def.tag);
        }
    }

    /// <summary>
    /// Retrieves the block definition for the given tag.
    /// </summary>
    public BlockDefinition GetBlockDefinition(string tag)
    {
        if (blockDict != null && blockDict.ContainsKey(tag))
        {
            return blockDict[tag];
        }
        else
        {
            Debug.LogError("BlockDefinition for tag '" + tag + "' not found!");
            return null;
        }
    }
}
