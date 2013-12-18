using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Directions {
	public enum Cardinal
	{
		unknown,
		north,
		south,
		east,
		west,
		up,
		down
	}
	
	public static IntVector3 getDirectionUnitVector (Directions.Cardinal cardinalDirection)
	{
		if (cardinalDirection == Directions.Cardinal.up)
			return new IntVector3 (0, 1, 0);
		if (cardinalDirection == Directions.Cardinal.down)
			return new IntVector3 (0, -1, 0);
		
		if (cardinalDirection == Directions.Cardinal.north)
			return new IntVector3 (0, 0, 1);
		if (cardinalDirection == Directions.Cardinal.south)
			return new IntVector3 (0, 0, -1);
		if (cardinalDirection == Directions.Cardinal.east)
			return new IntVector3 (1, 0, 0);
		if (cardinalDirection == Directions.Cardinal.west)
			return new IntVector3 (-1, 0, 0);
		
		Debug.Log ("warning: getDirectionUnitVector failed");
		return new IntVector3 (1, 0, 0);
	}
}
