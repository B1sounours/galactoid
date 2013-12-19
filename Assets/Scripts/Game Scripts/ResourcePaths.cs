using UnityEngine;
using System.Collections;

/*
place store all "Assets/Resources" paths used by Unity
*/

public class ResourcePaths
{
    public static string blockDataFolder;
    public static string skyboxFolder;
    public static string blockPrefab;
    public static string planePrefab;
    public static string mainSplash;
    public static string mainFont;

    static ResourcePaths()
	{
        blockDataFolder = "Prefabs/blockdata";
        skyboxFolder = "Prefabs/skybox";
        planePrefab = "Prefabs/skyboxPlane";
        blockPrefab = "Prefabs/block";
        mainSplash = "Textures/main_splash";
        mainFont = "Fonts/startrek";
	}
	
}
