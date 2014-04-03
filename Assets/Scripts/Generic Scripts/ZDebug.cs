using UnityEngine;
using System.Collections;

public static class ZDebug
{
	
	public static string toString (IntVector3 vec)
	{
		return " x: " + vec.x + " y: " + vec.y + " z: " + vec.z;
	}
	
	public static string toString (IntVector2 vec)
	{
		return " x: " + vec.x + " y: " + vec.y;
	}
	
	public static void show (Prism prism)
	{
		Debug.Log (" x: " + prism.x + " y: " + prism.y + " z: " + prism.z + " w: " + prism.width + " h: " + prism.height + " d: " + prism.depth);
	}
	
	public static void show (Vector2 vec)
	{
		Debug.Log (" x: " + vec.x + " y: " + vec.y);
	}

    public static void show(Vector3 vec)
    {
        Debug.Log(" x: " + vec.x + " y: " + vec.y + " z: " + vec.z);
    }

	public static void show (IntVector2 vec)
	{
		Debug.Log (toString (vec));
	}
	
	public static void show (IntVector3 vec)
	{
		Debug.Log (toString (vec));
	}
	
	public static void show (Square square)
	{
		Debug.Log (" x: " + square.x + " y: " + square.y + " width: " + square.width + " height: " + square.height);
	}

    public static void show(Rect rect)
    {
        Debug.Log(" x: " + rect.x + " y: " + rect.y + " width: " + rect.width + " height: " + rect.height);
    }
}








