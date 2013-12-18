using UnityEngine;
using System.Collections;

public class Angles
{
	public static Vector3 getRandom ()
	{
		return new Vector3 (Random.Range (0, 4) * 90,
			Random.Range (0, 4) * 90,
			Random.Range (0, 4) * 90);
	}
	
	public static Vector3 getFlat ()
	{
		return new Vector3 (0,
							Random.Range (0, 4) * 90,
							0);
	}
	
	public static IntVector3[] getDirections(){
		return new IntVector3[]{
			new IntVector3 (1, 0, 0),
			new IntVector3 (-1, 0, 0),
			new IntVector3 (0, 1, 0),
			new IntVector3 (0, -1, 0),
			new IntVector3 (0, 0, 1),
			new IntVector3 (0, 0, -1)};
	}
	
	public static IntVector3 getRandomDirection(){
		return getDirections()[Random.Range(0,6)];
	}
}
