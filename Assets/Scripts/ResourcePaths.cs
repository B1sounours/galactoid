using UnityEngine;
using System.Collections;

/*
place store all "Assets/Resources" paths used by Unity
*/

public class ResourcePaths
{
    public static string blockDataFolder;

    public static string blockTextures;
    public static string[] sideButtons;

    public static string toolSelectBackground;
    public static string toolModePlace, toolModeRemove, toolModeRepair, toolModeScan;

    public static string skyboxFolder;
    public static string mainSplash;
    public static string mainFont;

    public static string blockPrefab;
    public static string planePrefab;

    public static string starportWallPrefab, starportFloorPrefab;

    static ResourcePaths()
    {
        //prefabs
        blockDataFolder = "Prefabs/blockdata";
        skyboxFolder = "Prefabs/skybox";
        planePrefab = "Prefabs/skyboxPlane";
        blockPrefab = "Prefabs/block";
        starportFloorPrefab = "Prefabs/Starport/floor";
        starportWallPrefab = "Prefabs/Starport/wall";

        //textures
        mainSplash = "Textures/main_splash";
        sideButtons = new string[2] { "Textures/gui/left_arrow", "Textures/gui/right_arrow" };

        blockTextures = "Textures/block";

        toolSelectBackground = "Textures/gui/background1";

        toolModePlace = "Textures/gui/gear-hammer";
        toolModeRemove = "Textures/gui/cogsplosion";
        toolModeRepair = "Textures/gui/auto-repair";
        toolModeScan = "Textures/gui/magnifying-glass";

        //fonts
        mainFont = "Fonts/startrek";
    }

}
