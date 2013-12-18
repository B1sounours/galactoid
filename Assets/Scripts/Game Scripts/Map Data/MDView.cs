using UnityEngine;
using System.Collections;

public class MDView
{
		
	public static int getMapHeight (int[,,] mapData, int i, int k)
	{
		//returns the height of the highest voxel at i,?,k
		if (mapData.GetLength(1)==0)
			return 0;
		
		for (int j=mapData.GetLength(1)-1; j>=0; j--) {
			if (mapData [i, j, k] > 0)
				return j;
		}
		return 0;
	}
	
	public static ArrayList getNeighbors (int [,,] mapData, IntVector3 point)
	{
		//print ("getNeighbors: i: " + i + " j: " + j + " k: " + k);
		ArrayList neighbors = new ArrayList ();
		
		neighbors.Add (new IntVector3 (point.x- 1, point.y,  point.z));
		neighbors.Add (new IntVector3 (point.x+ 1, point.y,  point.z));
		neighbors.Add (new IntVector3 (point.x, point.y - 1,  point.z));
		neighbors.Add (new IntVector3 (point.x, point.y + 1,  point.z));
		neighbors.Add (new IntVector3 (point.x, point.y,  point.z - 1));
		neighbors.Add (new IntVector3 (point.x, point.y,  point.z + 1));
			
		for (int index=0; index<neighbors.Count; index++) {
			IntVector3 neighborPoint = (IntVector3)neighbors [index];
			if (! isVoxelOccupied (mapData, neighborPoint)) {
				neighbors.Remove (neighborPoint);
				index--;
			}
		}
		
		return neighbors;
	}
	
	public static bool isInsideMapData (int[,,]mapData, IntVector3 point)
	{
		return (point.x >= 0 && point.y >= 0 && point.z >= 0 && 
			mapData.GetLength (0) > point.x && 
			mapData.GetLength (1) > point.y && 
			mapData.GetLength (2) > point.z);
	}
	
	public static bool isVoxelOccupied (int [,,] mapData, IntVector3 point)
	{
		if (!isInsideMapData (mapData, point))
			return false;
		
		return mapData [point.x, point.y, point.z] > 0;
	}
	
}

