using UnityEngine;
using System.Collections;

/*
this class is used to communicate between scenes.

it also contains constants that are almost never changed.
*/

public class GameSettings
{
	public static bool guiFPS;
	public static bool guiReticule;
	public static bool musicEnabled;
	public static bool sandboxEnabled;
	public static int detachDistance;
	public static float detachSpeed;
	public static float redInterval;
	public static int subtaskStepsize;
	public static int blobSpawnDistance;
	public static int leavingMapDistance;
	public static int fadeToBlackTime;
	public static float collisionWait;
	private static string levelPath;
	
	static GameSettings ()
	{
		setDefaults ();
	}
	
	public static void setLevelPath (string levelName)
	{
		levelPath = "Prefabs/Levels/"+levelName;
	}
	
	public static GameObject getLevelDataPrefab ()
	{
		GameObject go = Resources.Load (levelPath) as GameObject;
		
		if (go == null){
			Debug.Log ("warning: getLevelDataPrefab returned default for levelpath: "+levelPath);
			setLevelPath("default");
			return getLevelDataPrefab();
		}
		
		return go;
	}
	
	static void setDefaults ()
	{
		sandboxEnabled = false;
		setLevelPath("default");
		
		musicEnabled=true;
		
		//which elements on the gameplay gui to show
		guiReticule = true;
		guiFPS = true;
		
		//how far blobs drift when detached before being destroyed by the game
		detachDistance = 50;
		//how fast detached blobs drift up
		detachSpeed = 0.5f;
		//how far away blobs spawn
		blobSpawnDistance = 100;
		//how far outside map domain player must travel to register as having left the map
		leavingMapDistance = 10;
		//minimum amount of time that must pass between collisions on the same side
		collisionWait=15;
		
		//seconds between each red spread attempt
		redInterval = 3;
		//how long it takes to fade to black for player leaving map event
		fadeToBlackTime = 5;
		
		//the max number of operations that will be performed per frame to rebuild the map upon collision
		//future optimization: replace with as many processes that can be done in 1/60 sec
		subtaskStepsize = 300;
	}
	
}
