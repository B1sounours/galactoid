using UnityEngine;
using System.Collections;

/*
this class stores the "options" set by the player during gameplay. saved between scenes.

it also contains constants that are almost never changed.
*/

public class GameOptions
{
	public static bool guiFPS;
	public static bool guiReticule;
	
	static GameOptions ()
	{
		setDefaults ();
	}
	
	
	static void setDefaults ()
	{
		//which elements on the gameplay gui to show
		guiReticule = true;
		guiFPS = true;
	}
	
}
