using UnityEngine;
using System.Collections;

public class MDCollision
{
	
	public static int getCollisionDistance (int[,,]movingData, int[,,]mapData, IntVector3 offset)
	{
		/*
		given two mapDatas and offset, returns the distance the movingData must move in order to
		collide with mapData
		*/
		IntVector3 direction = getCollisionDirection (movingData, mapData, offset);
		if (! direction.isOrthogonalUnitVector ()) {
			//Debug.Log("warning: getCollisionDistance returned -1");
			return -1;
		}
		int [,] movingCollisionArray = getCollisionArray (movingData, direction);
		
		//invert direction vector so the face of the other side is used for stationary mapData
		IntVector3 invertDirection = new IntVector3 (direction.x * -1, direction.y * -1, direction.z * -1);
		int [,] mapCollisionArray = getCollisionArray (mapData, invertDirection);
		
		/*
		use direction vector to find out what x,y offsets should be for adjusting collision array 
		placement
		*/
		int xOffset = 0;
		int yOffset = 0;
		
		int originOffset = 0;
		int lengthIndex = 0;
		if (direction.x != 0) {
			xOffset = offset.z;
			yOffset = offset.y;
			
			originOffset = offset.x;
			lengthIndex = 0;
		}
		if (direction.y != 0) {
			xOffset = offset.x;
			yOffset = offset.z;
			
			originOffset = offset.y;
			lengthIndex = 1;
		}
		if (direction.z != 0) {
			xOffset = offset.x;
			yOffset = offset.y;
			
			originOffset = offset.z;
			lengthIndex = 2;
		}
		
		int minimumDistance = 0;
		if (direction.x == 1 || direction.y == 1 || direction.z == 1) {
			minimumDistance = Mathf.Abs (originOffset) - movingData.GetLength (lengthIndex);
		} else {
			minimumDistance = Mathf.Abs (originOffset) - mapData.GetLength (lengthIndex);
		}
		
		/*
		possible optimization: only iterate through the smaller of the two areas
		
		int movingArea=movingCollisionArray.GetLength(0)*movingCollisionArray.GetLength(1);
		int mapArea=mapCollisionArray.GetLength(0)*mapCollisionArray.GetLength(1);
		*/
		
		int xSize1 = mapCollisionArray.GetLength (0);
		int ySize1 = mapCollisionArray.GetLength (1);
		int xSize2 = movingCollisionArray.GetLength (0);
		int ySize2 = movingCollisionArray.GetLength (1);
		
		//Debug.Log ("xOffset: " + xOffset + " yOffset: " + yOffset+" minimumDistance: "+minimumDistance);
		
		int smallest = -1;
		for (int x=0; x<xSize2 && x+xOffset<xSize1; x++) {
			for (int y=0; y<ySize2 && y+yOffset<ySize1; y++) {
				int xMap = x + xOffset;
				int yMap = y + yOffset;
				if (xMap < 0 || yMap < 0)
					continue;
				
				int distance1 = mapCollisionArray [xMap, yMap];
				int distance2 = movingCollisionArray [x, y];
				
				if (distance1 == -1 || distance2 == -1)
					continue;
				
				//Debug.Log ("x: " + x + " y: " + y + " d1: " + distance1 + " d2: " + distance2);
				if (smallest > distance1 + distance2 || smallest == -1)
					smallest = distance1 + distance2;
			}
		}
		
		if (smallest == -1) {
			//Debug.Log ("warning: getCollisionDistance found no collision");
			return -1;
		}
		
		int collisionDistance = smallest + minimumDistance;
		return collisionDistance;
		
	}
	
	public static IntVector3 getCollisionDirection (int[,,]movingData, int[,,]mapData, IntVector3 offset)
	{
		/*
		where movingData starts at offsets, find the direction it must move to collide with mapData.
		mapData starts at 0,0,0
		
		returns a unit vector, iff an orthogonal collision is possible.
		*/
		IntVector3 direction = new IntVector3 (0, 0, 0);
		
		Prism movingPrism = new Prism (offset.x, offset.y, offset.z, movingData.GetLength (0), movingData.GetLength (1), movingData.GetLength (2));
		Prism mapPrism = new Prism (0, 0, 0, mapData.GetLength (0), mapData.GetLength (1), mapData.GetLength (2));
		
		bool iInside = isRangeInside (movingPrism.x, movingPrism.width, mapPrism.x, mapPrism.width);
		bool jInside = isRangeInside (movingPrism.y, movingPrism.height, mapPrism.y, mapPrism.height);
		bool kInside = isRangeInside (movingPrism.z, movingPrism.depth, mapPrism.z, mapPrism.depth);
		
		int insideCount = 0;
		if (iInside)
			insideCount++;
		if (jInside)
			insideCount++;
		if (kInside)
			insideCount++;
		
		IntVector3 emptyDirection = new IntVector3 (0, 0, 0);
		if (insideCount < 2) {
			//Debug.Log ("getCollisionDirection called with no possible orthogonal collision. insideCount: " + insideCount);
			return emptyDirection;
		} else if (insideCount == 3) {
			Debug.Log ("getCollisionDirection called when movingData is inside mapData.");
			return emptyDirection;
		}
		
		if (! iInside)
			direction.x = movingPrism.x < mapPrism.x ? 1 : -1;
		if (! jInside)
			direction.y = movingPrism.y < mapPrism.y ? 1 : -1;
		if (! kInside)
			direction.z = movingPrism.z < mapPrism.z ? 1 : -1;
		
		//Debug.Log ("getCollisionDirection i: " + direction.i + " j: " + direction.j + " k: " + direction.k);
		return direction;
	}
	
	private static bool isRangeInside (int start1, int size1, int start2, int size2)
	{
		/*
		on a one dimensional strip, we have two ranges. one at start1 with width size1, and the other, 2.
		returns whether range 1 is within range 2
		*/
		
		if (start1 + size1 >= start2 && start1 < start2 + size2)
			return true;
		if (start2 + size2 >= start1 && start2 < start1 + size1)
			return true;
		
		return false;
	}
	
	public static IntVector2 getCollisionArraySize (int[,,]mapData, Directions.Cardinal direction)
	{
		//future optimization: this function is used a fair bit, but it's overloaded retardedly
		IntVector3 mapDataSize=new IntVector3(mapData.GetLength(0),mapData.GetLength(1),mapData.GetLength(2));
		return getCollisionArraySize(mapDataSize,Directions.getDirectionUnitVector(direction));
	}
	
	public static IntVector2 getCollisionArraySize (int[,,]mapData, IntVector3 direction)
	{
		IntVector3 mapDataSize=new IntVector3(mapData.GetLength(0),mapData.GetLength(1),mapData.GetLength(2));
		return getCollisionArraySize(mapDataSize,direction);
	}
	
	public static IntVector2 getCollisionArraySize (IntVector3 mapDataSize, IntVector3 direction)
	{
		/*
		returns dimensions of the face of mapData that would collide if moving
		in the given direction		
		*/
		IntVector2 arraySize = new IntVector2 (0, 0);
		
		if (direction.x != 0) {
			arraySize.x = mapDataSize.z;
			arraySize.y = mapDataSize.y;
		}
		if (direction.y != 0) {
			arraySize.x = mapDataSize.x;
			arraySize.y = mapDataSize.z;
		}
		if (direction.z != 0) {
			arraySize.x = mapDataSize.x;
			arraySize.y = mapDataSize.y;
		}
		
		return arraySize;
	}
	
	public static int[,] getCollisionArray (int[,,]mapData, IntVector3 direction)
	{
		/*
		where direction is a unit vector, pointing in the direction mapData is travelling
		
		the direction vector decides what the collision plane will be. for example the collision plane
		of a moving car is vertical, like the surface of a brick wall, in front of the car.
		
		the returned collisionArray is the heights of the nearest voxel lining up to each (x,y) of that 
		plane.
		*/
		
		//Debug.Log ("getCollisionArray " + direction.i + " " + direction.j + " " + direction.k);
		
		if (! direction.isOrthogonalUnitVector ())
			Debug.Log ("warning: getCollisionArray didn't get a unit vector.");
			
		IntVector2 arraySize = getCollisionArraySize (mapData, direction);		

		int [,] collisionArray = new int [arraySize.x, arraySize.y];
		
		for (int xOffset=0; xOffset<arraySize.x; xOffset++) {
			for (int yOffset=0; yOffset<arraySize.y; yOffset++) {
				int dist = getClosestVoxelDistance (mapData, xOffset, yOffset, direction);
				collisionArray [xOffset, yOffset] = dist;
				if (dist != -1) {
					//Debug.Log ("xoffset: " + xOffset + " yoffset: " + yOffset + " dist: " + dist);
				}
			}
		}
		
		return collisionArray;
	}
	
	public static int getClosestVoxelDistance (int [,,] mapData, int xOffset, int yOffset, IntVector3 direction)
	{
		/*
		for point (xOffset,yOffset) on the collision plane as described in getCollisionArray,
		returns the distance to that plane.
		
		returns -1 if there is no voxel in that row
		*/
		
		int distance = -1;
		int loopSize = 0;
		int loopDirection = 0;
		
		if (direction.x != 0) {
			loopSize = mapData.GetLength (0);
			loopDirection = direction.x;
		}
		if (direction.y != 0) {
			loopSize = mapData.GetLength (1);
			loopDirection = direction.y;
		}
		if (direction.z != 0) {
			loopSize = mapData.GetLength (2);
			loopDirection = direction.z;
		}
		int offsetStart = loopDirection < 0 ? 0 : loopSize - 1;
		
		for (int offset=offsetStart; offset<loopSize && offset>=0; offset+=loopDirection*-1) {
			int voxelCode = 0;
			if (direction.x != 0)
				voxelCode = mapData [offset, yOffset, xOffset];
			if (direction.y != 0)
				voxelCode = mapData [xOffset, offset, yOffset];
			if (direction.z != 0)
				voxelCode = mapData [xOffset, yOffset, offset];
			
			if (voxelCode > 0) {
				distance = offsetStart == 0 ? offset : offsetStart - offset;
				break;
			}
		}
		
		return distance;
	}
	
	public static IntVector2 getCollisionPoint (int[,,] mapData, IntVector2 center, IntVector3 moveDirection)
	{
		/*
		returns a point (x,y) whose row is closest possible to point (x,y) of the 
		blob such that the row contains a voxel
		*/
		
		int[,] collisionArray = MDCollision.getCollisionArray (mapData, moveDirection);
		
		int xSize = collisionArray.GetLength (0);
		int ySize = collisionArray.GetLength (1);
		
		int x = center.x;
		int y = center.y;
		int dx = 0;
		int dy = -1;
		
		int iMax = (int)Mathf.Pow (Mathf.Max (20, xSize, ySize), 2);
		bool foundResult = false;
		for (int i=0; i<iMax; i++) {
			if (x >= 0 && x < xSize && y >= 0 && y < ySize) {
				if (collisionArray [x, y] >= 0) {
					foundResult = true;
					break;
				}
			}
			
			int adjustedX = x - center.x;
			int adjustedY = y - center.y;
			
			if (adjustedX == adjustedY ||
				(x < center.x && adjustedX == -adjustedY) ||
				(x > center.x && adjustedX == 1 - adjustedY)) {
				int temp = dx;
				dx = -dy;
				dy = temp;
			}
			
			x += dx;
			y += dy;
		}
		
		//if no result found, retry with a point definitely within mapData
		IntVector2 result;
		if (foundResult) {
			result = new IntVector2 (x, y);
		} else {
			int newX = center.x;
			if (newX < 0)
				newX = 0;
			if (newX > xSize)
				newX = xSize;
			
			int newY = center.y;
			if (newY < 0)
				newY = 0;
			if (newY > ySize)
				newY = ySize;
			
			IntVector2 newCenter = new IntVector2 (newX, newY);
			if (newCenter == center) {
				Debug.Log ("warning: getCollisionPoint failed. returned center of mapData");
				return center;
			} else {
				Debug.Log ("note: retrying getCollisionPoint with point: "+newX+" "+newY);
				return getCollisionPoint(mapData,newCenter,moveDirection);
			}
		}
		return result;
	}
	
	public static IntVector2 getCenterCollisionPoint (int[,,] mapData, IntVector3 moveDirection)
	{
		/*
		returns a point (x,y) whose row is closest possible to the center of the 
		blob such that the row contains a voxel
		*/
		
		IntVector2 arraySize = getCollisionArraySize (mapData, moveDirection);
		IntVector2 centerPoint = new IntVector2 (arraySize.x / 2, arraySize.y / 2);
		return getCollisionPoint (mapData, centerPoint, moveDirection);
	}
	
}

