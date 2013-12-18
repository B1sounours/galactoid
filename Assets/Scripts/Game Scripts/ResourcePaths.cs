using UnityEngine;
using System.Collections;

/*
place store all "Assets/Resources" paths used by Unity
*/

public class ResourcePaths
{
    public static string blockDataFolder;
    public static string blockPrefab;
    public static string mainSplash;
    public static string mainFont;

    static ResourcePaths()
	{
        blockDataFolder = "Prefabs/Block Datas";
        blockPrefab = "Prefabs/Block";
        mainSplash = "Textures/main_splash";
        mainFont = "Fonts/startrek";
	}
	
}
