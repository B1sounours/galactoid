using UnityEngine;
using System.Collections;

//stores all Resources.LoadAll results for later use to avoid multiple initialization.

public class ResourceLookup : MonoBehaviour
{
    public static ArrayList blockDatas;
    private static ArrayList skyboxes;
    private static GameObject blockPrefab;
    private static GameObject planePrefab;

    private static Hashtable toolModeTextures;
    private static Texture[] sideButtonTextures;

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

}
