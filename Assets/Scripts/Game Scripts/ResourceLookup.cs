using UnityEngine;
using System.Collections;

//stores all Resources.LoadAll results for later use to avoid multiple initialization.

public class ResourceLookup : MonoBehaviour
{
    private static ArrayList blockDatas;
    private static ArrayList skyboxes;
    private static GameObject blockPrefab;
    private static GameObject planePrefab;

    private static Hashtable toolModeTextures;

    static public Texture getToolModeTexture(GameOptions.toolModes toolMode)
    {
        if (toolModeTextures == null)
        {
            toolModeTextures = new Hashtable();

            Texture texture = Resources.Load(ResourcePaths.toolModePlace) as Texture;
            toolModeTextures.Add(GameOptions.toolModes.placeBlock, texture);

            texture = Resources.Load(ResourcePaths.toolModeRemove) as Texture;
            toolModeTextures.Add(GameOptions.toolModes.removeBlock, texture);

            texture = Resources.Load(ResourcePaths.toolModeRepair) as Texture;
            toolModeTextures.Add(GameOptions.toolModes.repairBlock, texture);

            texture = Resources.Load(ResourcePaths.toolModeScan) as Texture;
            toolModeTextures.Add(GameOptions.toolModes.scanBlock, texture);
        }

        return (Texture)toolModeTextures[toolMode];
    }

    static private ArrayList getBlockDatas()
    {
        if (blockDatas != null)
            return blockDatas;

        blockDatas = new ArrayList();
        foreach (GameObject go in Resources.LoadAll(ResourcePaths.blockDataFolder))
        {
            blockDatas.Add(go.GetComponent<BlockData>());
        }

        /*
        int counter=0;
        foreach (Texture texture in Resources.LoadAll<Texture2D>(ResourcePaths.blockTextures))
        {
            GameObject go = new GameObject("automated blockdata");
            BlockData bd = go.AddComponent<BlockData>();
            Debug.Log(texture);
            bd.textures = new Texture[1];
            bd.textures.SetValue(texture, counter);
            bd.blockCode = counter + 1;
            counter++;
            blockDatas.Add(bd);
        }
         */

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

    static public BlockData getBlockDataByName(string blockName)
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
            return getBlockDataByName("default");
        }
    }

    static public BlockData getBlockDataByCode(int blockCode)
    {
        foreach (BlockData blockData in getBlockDatas())
        {
            if (blockData.blockCode == blockCode)
                return blockData;
        }

        Debug.Log("warning: failed to find blockData with code: " + blockCode);
        return getBlockDataByName("default");
    }
}
