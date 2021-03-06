using UnityEngine;
using System.Collections;

/*
this class stores the options set by the player while the game is running. saved between scenes.

it also contains constants that are almost never changed.
*/

public class GameOptions
{
	public static bool guiFPS, guiReticule;
    public static int[] placeBlockSlots;
    public static int targetFramerate=60;

    //save and load data
    public static string gameSaveLoadKey="game1";
    public static bool loadSavedGame=false;

    public enum toolModes
    {
        placeBlock,
        removeBlock,
        repairBlock,
        scanBlock
    }
    public static toolModes[] mouseTool;

	static GameOptions ()
	{
		setDefaults ();
	}

    public static string getSaveLoadPath()
    {
        return Application.persistentDataPath + "/" + gameSaveLoadKey;
    }
	
	static void setDefaults ()
	{
		//which elements on the gameplay gui to show
		guiReticule = true;
		guiFPS = true;

        //which mouse buttons do what
        mouseTool = new toolModes[2];
        mouseTool[0] = toolModes.removeBlock;
        mouseTool[1] = toolModes.placeBlock;

        //slots 1-9 and 0 for place block
        placeBlockSlots = new int[10];
	}
	
}
