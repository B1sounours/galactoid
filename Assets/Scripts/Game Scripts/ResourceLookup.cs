using UnityEngine;
using System.Collections;

//stores all Resources.LoadAll results for later use to avoid multiple initialization.

public class ResourceLookup : MonoBehaviour
{
    private static ArrayList blockDatas;
    private static ArrayList skyboxes;
    private static GameObject blockPrefab;
    private static GameObject planePrefab;

    static private ArrayList getBlockDatas()
    {
        if (blockDatas != null)
            return blockDatas;

        blockDatas = new ArrayList();
        foreach (GameObject go in Resources.LoadAll(ResourcePaths.blockDataFolder))
        {
            blockDatas.Add(go.GetComponent<BlockData>());
        }

        return blockDatas;
    }

    static public GameObject getBlockPrefab()
    {
        if (blockPrefab == null)
            blockPrefab = Resources.Load(ResourcePaths.blockPrefab) as GameObject;

        return blockPrefab;
    }

    static public GameObject getPlanePrefab()
    {
        if (planePrefab == null)
            planePrefab = Resources.Load(ResourcePaths.planePrefab) as GameObject;

        return planePrefab;
    }

    static public ArrayList getSkyboxPrefabs()
    {
        if (skyboxes != null)
            return skyboxes;

        skyboxes = new ArrayList();
        foreach (GameObject go in Resources.LoadAll(ResourcePaths.skyboxFolder))
        {
            skyboxes.Add(go);
        }

        return skyboxes;
    }

    static public BlockData getBlockByName(string blockName)
    {
        //optimization: this should be a hashtable
        foreach (BlockData blockData in getBlockDatas())
        {
            if (blockData.name == blockName)
                return blockData;
        }

        if (blockName == "default")
        {
            Debug.Log("Ah! failed to find default block data.");
            return null;
        }
        else
        {
            Debug.Log("warning: failed to find blockData with name: " + blockName);
            return getBlockByName("default");
        }
    }

    static public BlockData getBlockByCode(int blockCode)
    {
        foreach (BlockData blockData in getBlockDatas())
        {
            if (blockData.blockCode == blockCode)
                return blockData;
        }

        Debug.Log("warning: failed to find blockData with code: " + blockCode);
        return getBlockByName("default");
    }
}
