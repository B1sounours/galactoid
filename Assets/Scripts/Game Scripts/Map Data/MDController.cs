using UnityEngine;
using System.Collections;
using System.Linq;

public class MDController
{

	public static int[,,] iRotate (int[,,]mapData, int angle)
	{
		/*
		rotates mapData counter-clockwise along the i axis by multiples of 90 degrees. 
		shapes lying flat will stand up
		*/
		if (angle % 90 != 0) {
			Debug.Log ("iRotate failed. invalid angle: " + angle);
			return mapData;
		}
		
		int[,,] lastData = mapData;
		
		while (angle>0) {
			angle -= 90;
			int[,,] newData = new int[lastData.GetLength (0), lastData.GetLength (2), lastData .GetLength (1)];
			
			int iLength = lastData .GetLength (0);
			int jLength = lastData .GetLength (1);
			int kLength = lastData .GetLength (2);
			
			for (int i=0; i<iLength; i++) {
				for (int j=0; j<jLength; j++) {
					for (int k=0; k<kLength; k++) {
						newData [i, k, jLength - j - 1] = lastData [i, j, k];
					}
				}
			}
			lastData = newData;
		}
		return lastData;
	}
	
	public static int[,,] jRotate (int[,,]mapData, int angle)
	{
		//rotates mapData clockwise along the j axis by multiples of 90 degrees
		if (angle % 90 != 0) {
			Debug.Log ("jRotate failed. invalid angle: " + angle);
			return mapData;
		}
		
		int[,,] lastData = mapData;
		
		while (angle>0) {
			angle -= 90;
			int[,,] newData = new int[lastData.GetLength (2), lastData.GetLength (1), lastData .GetLength (0)];
			
			int iLength = lastData .GetLength (0);
			int jLength = lastData .GetLength (1);
			int kLength = lastData .GetLength (2);
			
			for (int i=0; i<iLength; i++) {
				for (int j=0; j<jLength; j++) {
					for (int k=0; k<kLength; k++) {
						newData [k, j, iLength - i - 1] = lastData [i, j, k];
					}
				}
			}
			lastData = newData;
		}
		return lastData;
	}
	
	public static int[,,] combine (int[,,]mapData, int[,,]movingData)
	{
		return merge (1, mapData, movingData, 0, 0, 0);
	}
	
	public static int[,,] combine (int[,,]mapData, int[,,]movingData, int iOffset, int jOffset, int kOffset)
	{
		return merge (1, mapData, movingData, iOffset, jOffset, kOffset);
	}
	
	public static int[,,] combine (int[,,]mapData, int[,,]movingData, IntVector3 offset)
	{
		return merge (1, mapData, movingData, offset.x, offset.y, offset.z);
	}
	
	public static int[,,] subtract (int[,,]mapData, int[,,]movingData)
	{
		return merge (2, mapData, movingData, 0, 0, 0);
	}
	
	public static int[,,] subtract (int[,,]mapData, int[,,]movingData, int iOffset, int jOffset, int kOffset)
	{
		return merge (2, mapData, movingData, iOffset, jOffset, kOffset);
	}
	
	public static int[,,] subtract (int[,,]mapData, int[,,]movingData, IntVector3 offset)
	{
		return merge (2, mapData, movingData, offset.x, offset.y, offset.z);
	}
	
	private static int[,,] merge (int combineMode, int[,,]mapData, int[,,]movingData, int iOffset, int jOffset, int kOffset)
	{
		/*
		combineMode=1
		adds mapData and movingData using offsets.
		if movingData[0,0,0] is 2, mapData[iOffset,jOffset,kOffset] becomes 2
		
		combineMode=2
		subtracts movingData from mapData using offsets.
		if movingData[0,0,0] is greater than 0, mapData[iOffset,jOffset,kOffset] becomes 0
		*/
		int width = movingData.GetLength (0);
		int height = movingData.GetLength (1);
		int depth = movingData.GetLength (2);
		
		int[,,] newData = mapData;
		
		for (int i=0; i<width; i++) {
			for (int j=0; j<height; j++) {
				for (int k=0; k<depth; k++) {
					int voxelCode = movingData [i, j, k];
					
					if (voxelCode > 0) {
						
						IntVector3 iterPoint=new IntVector3(i + iOffset,
						j + jOffset,
						k + kOffset);
						
						if (MDView.isInsideMapData (mapData, iterPoint)) {
							int newVoxelCode = 0;
							if (combineMode == 1)
								newVoxelCode = voxelCode;
						
							if (combineMode == 2)
								newVoxelCode = 0;
						
							newData [iterPoint.x,iterPoint.y,iterPoint.z] = newVoxelCode;
						} else {
							//print ("combineMapData got a bad voxel: " + newi + " " + newj + " " + newk);
						}
					}
				}
			}
		}
		
		return newData;
	}
	
	public static int[,,] trim (int[,,] mapData)
	{
		/*
		trims empty space on any side of the mapData prism, returning the smallest possible mapData
		without losing any voxelCodes
		*/
		IntVector3[] directions = Angles.getDirections ();
		
		IntVector3 minOffset = new IntVector3 (0, 0, 0);
		IntVector3 maxOffset = new IntVector3 (0, 0, 0);
		
		foreach (IntVector3 direction in directions) {
			int[,] collisionArray = MDCollision.getCollisionArray (mapData, direction);
			int offset = getMin (collisionArray);
			if (direction.x + direction.y + direction.z < 0) {
				minOffset += direction * offset * -1;
			} else {
				maxOffset += direction * offset;
			}
		}
		
		int width = mapData.GetLength (0) - minOffset.x - maxOffset.x;
		int height = mapData.GetLength (1) - minOffset.y - maxOffset.y;
		int depth = mapData.GetLength (2) - minOffset.z - maxOffset.z;
		
		int [,,] newData = new int[width, height, depth];
		
		for (int i=0; i<width; i++) {
			for (int j=0; j<height; j++) {
				for (int k=0; k<depth; k++) {
					newData [i, j, k] = mapData [i + minOffset.x, j + minOffset.y, k + minOffset.z];
				}
			}
		}
		
		return newData;
	}
	
	private static int getMin (int[,] array)
	{
		/*
		for some damn reason array.Cast<int>() and other solutions don't compile in c# unity,
		only pure c#? this finds min int of a 2D array
		*/
		int minimum = -1;
		for (int x=0; x<array.GetLength(0); x++) {
			for (int y=0; y<array.GetLength(1); y++) {
				int val = array [x, y];
				if (val == -1)
					continue;
				
				if (val < minimum || minimum == -1)
					minimum = val;
			}
		}
		
		return minimum;
	}
				
}

