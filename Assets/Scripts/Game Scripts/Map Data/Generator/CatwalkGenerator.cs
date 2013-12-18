using UnityEngine;
using System.Collections;

public static class CatwalkGenerator
{
	
	public static int[,,] genDebug (VoxelTheme voxelTheme, int width, int height, int depth)
	{
		/*
		a series of unit tests for CatwalkGenerator
		*/
		
		int[,,] mapData = new int[width, height, depth];
		int voxelCode = voxelTheme.getCode ("default");
		mapData = MDController.combine (mapData, ShapeGenerator.genPrism (voxelCode, width, 1, depth));
		
		IntVector3 destination = new IntVector3 (0, 0, 0);
		IntVector3 offset = new IntVector3 (0, 0, 0);
		int jOffset=1;
		
		/*
		destination=new IntVector3(5,2,0);
		offset=new IntVector3(5,jOffset,5);
		mapData=MDController.combine(mapData,genBasic(voxelTheme,destination,3),offset);
		
		destination=new IntVector3(-5,2,0);
		offset=new IntVector3(15,jOffset,5);
		mapData=MDController.combine(mapData,genBasic(voxelTheme,destination,3),offset);
		
		destination=new IntVector3(0,2,5);
		offset=new IntVector3(25,jOffset,5);
		mapData=MDController.combine(mapData,genBasic(voxelTheme,destination,3),offset);
		*/
		
		/*
		destination = new IntVector3 (8, 5, 0);
		offset = new IntVector3 (5, jOffset, 5);
		mapData = MDController.combine (mapData, genPillarSteps (voxelTheme, destination,3), offset);
		
		destination = new IntVector3 (14, -8, 0);
		offset = new IntVector3 (5, jOffset, 10);
		mapData = MDController.combine (mapData, genPillarSteps (voxelTheme, destination, 3), offset);
		
		destination = new IntVector3 (0, 2, 5);
		offset = new IntVector3 (5, jOffset, 15);
		mapData = MDController.combine (mapData, genPillarSteps (voxelTheme, destination, 3), offset);
		*/
		
		/*
		destination = new IntVector3 (8, 5, 0);
		offset = new IntVector3 (5, jOffset, 5);
		mapData = MDController.combine (mapData, genLilypads (voxelTheme, destination, 1), offset);
		
		destination = new IntVector3 (14, -4, 0);
		offset = new IntVector3 (5, jOffset, 10);
		mapData = MDController.combine (mapData, genLilypads (voxelTheme, destination, 2), offset);
		
		destination = new IntVector3 (0, 2, 5);
		offset = new IntVector3 (5, jOffset, 15);
		mapData = MDController.combine (mapData, genLilypads (voxelTheme, destination, 3), offset);
		*/

		
		destination = new IntVector3 (8, 5, 0);
		offset = new IntVector3 (5, jOffset, 5);
		mapData = MDController.combine (mapData, genFootballPosts (voxelTheme, destination,5,2), offset);
		
		destination = new IntVector3 (14, -4, 0);
		offset = new IntVector3 (5, jOffset, 10);
		mapData = MDController.combine (mapData, genFootballPosts (voxelTheme, destination, 6,3), offset);
		
		destination = new IntVector3 (0, 2, 5);
		offset = new IntVector3 (5, jOffset, 15);
		mapData = MDController.combine (mapData, genFootballPosts (voxelTheme, destination, 9,1), offset);
		
		
		return mapData;
	}
	
	private static int[,,] genBasic (VoxelTheme voxelTheme, IntVector3 destination, int width)
	{
		return genBasic (voxelTheme, destination, width, true);
	}
	
	private static int[,,] genBasic (VoxelTheme voxelTheme, IntVector3 destination, int width, bool rotate)
	{
		//generally used when size requirements are not met but a catwalk must be made.		
		int[,,] newData = genMapDataFromPoint (destination, width);
		int voxelCode = voxelTheme.getCode ("block");
		
		Prism prism = new Prism (0, 0, 0, 1, newData.GetLength (1), newData.GetLength (2));
		newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelCode, prism));
		
		prism = new Prism (0, 0, 0, newData.GetLength (0), 1, newData.GetLength (2));
		IntVector3 offset = new IntVector3 (0, 0, 0);
		newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelCode, prism), offset);
		
		if (rotate)
			return rotateMapData (newData, destination);
		return newData;
	}
	
	public static int[,,] genFootballPosts (VoxelTheme voxelTheme, IntVector3 destination, int width, int addedHeight)
	{	
		int cageHeight=5;
		int[,,] newData = genMapDataFromPoint (destination, width,addedHeight+cageHeight);
		
		int iSize = newData.GetLength (0);
		int jSize = newData.GetLength (1);
		if (iSize < 3 || width<3 || jSize/iSize>1)
			return genBasic (voxelTheme, destination, width);
		
		IntVector3 lastHopPoint = new IntVector3 (0, 0, -1);
		int stepSpacing=5;
		foreach (IntVector3 hopPoint in getHopArray(iSize,jSize-addedHeight,stepSpacing)) {
			IntVector3 offset = new IntVector3 (hopPoint.x, hopPoint.y+addedHeight-1, 0);
			int[,,] cageData=new int[1,cageHeight-1,width];
			cageData=MDController.combine(cageData,GreebleGenerator.genCage(voxelTheme,1,cageHeight,width));
			newData = MDController.combine (newData, cageData, offset);	
			if (lastHopPoint.z != -1) {
				offset = new IntVector3 (lastHopPoint.x,0,width/2);
				int underhangAddedHeight=Mathf.Min (hopPoint.y,lastHopPoint.y)+addedHeight;
				int[,,] underhangData=ShapeGenerator.genUnderhang(voxelTheme, hopPoint,lastHopPoint,underhangAddedHeight);
				newData = MDController.combine (newData, underhangData, offset);
			}
			lastHopPoint = hopPoint;
		}
		
		return rotateMapData (newData, destination);
	}
	
	public static int[,,] genRings (VoxelTheme voxelTheme, IntVector3 destination, int width, int addedHeight)
	{	
		int cageHeight=5;
		int[,,] newData = genMapDataFromPoint (destination, width,addedHeight+cageHeight);
		
		int iSize = newData.GetLength (0);
		int jSize = newData.GetLength (1);
		if (iSize < 3 || width<3 || jSize/iSize>1)
			return genBasic (voxelTheme, destination, width);
		
		IntVector3 lastHopPoint = new IntVector3 (0, 0, -1);
		int stepSpacing=5;
		foreach (IntVector3 hopPoint in getHopArray(iSize,jSize-addedHeight,stepSpacing)) {
			IntVector3 offset = new IntVector3 (hopPoint.x, hopPoint.y+addedHeight-1, 0);
			newData = MDController.combine (newData, GreebleGenerator.genCage(voxelTheme,1,cageHeight,width), offset);	
			if (lastHopPoint.z != -1) {
				offset = new IntVector3 (lastHopPoint.x,0,width/2);
				int underhangAddedHeight=Mathf.Min (hopPoint.y,lastHopPoint.y)+addedHeight;
				int[,,] underhangData=ShapeGenerator.genUnderhang(voxelTheme, hopPoint,lastHopPoint,underhangAddedHeight);
				newData = MDController.combine (newData, underhangData, offset);
			}
			lastHopPoint = hopPoint;
		}
		
		return rotateMapData (newData, destination);
	}
	
	public static int[,,] genLilypads (VoxelTheme voxelTheme, IntVector3 destination, int addedHeight)
	{	
		int kSize = 3;
		int[,,] newData = genMapDataFromPoint (destination, kSize,addedHeight);
		
		int iSize = newData.GetLength (0);
		int jSize = newData.GetLength (1);
		if (iSize < 6 || jSize < 2)
			return genBasic (voxelTheme, destination, 1);
		
		int padCode = voxelTheme.getCode ("plate");
		
		IntVector3 lastHopPoint = new IntVector3 (0, 0, -1);
		int stepSpacing=5;
		foreach (IntVector3 hopPoint in getHopArray(iSize,jSize-addedHeight,stepSpacing)) {
			IntVector3 offset = new IntVector3 (hopPoint.x - 1, hopPoint.y+addedHeight-1, 0);
			newData = MDController.combine (newData, ShapeGenerator.genPrism (padCode, 3, 1, 3), offset);	
			if (lastHopPoint.z != -1) {
				offset = new IntVector3 (lastHopPoint.x,0,kSize/2);
				int underhangAddedHeight=Mathf.Min (hopPoint.y,lastHopPoint.y)+addedHeight;
				int[,,] underhangData=ShapeGenerator.genUnderhang(voxelTheme, hopPoint,lastHopPoint,underhangAddedHeight);
				newData = MDController.combine (newData, underhangData, offset);
			}
			lastHopPoint = hopPoint;
		}
		
		return rotateMapData (newData, destination);
	}
	
	public static int[,,] genPillarSteps (VoxelTheme voxelTheme, IntVector3 destination, int width)
	{	
		int[,,] newData = genBasic (voxelTheme, destination, width, false);
		
		int iSize = newData.GetLength (0);
		int jSize = newData.GetLength (1);
		if (iSize < 4)
			return genBasic (voxelTheme, destination, width);
		
		int stepCode = voxelTheme.getCode ("up");
		int kOffset = width / 2;
		
		int stepSpacing=3;
		foreach (IntVector3 hopPoint in getHopArray(iSize,jSize,stepSpacing)) {
			int stepHeight = hopPoint.y+1;
			IntVector3 offset = new IntVector3 (hopPoint.x, 0, kOffset);
			newData = MDController.combine (newData, ShapeGenerator.genPrism (stepCode, 1, stepHeight, 1), offset);	
		}
		
		return rotateMapData (newData, destination);
	}
	
	private static IntVector3[] getHopArray (int width, int height, int stepSpacing)
	{
		/*
		returns a list of points that are appropriate platforms to jump to within
		the provided parameters
		*/
		int stepTotal = width/stepSpacing;
		
		IntVector3[] hopArray = new IntVector3[stepTotal+1];
		
		for (int i=0; i<hopArray.Length; i++) {
			int stepHeight = height * Mathf.Abs (i - stepTotal) / (stepTotal);
			hopArray [i] = new IntVector3 (i * stepSpacing, stepHeight, 0);
		}
		hopArray [hopArray.Length-1] = new IntVector3 (width-1, 0, 0);
		return hopArray;
	}
	
	private static int[,,] rotateMapData (int[,,] mapData, IntVector3 destination)
	{
		/*
		this changes mapData
		
		all catwalk generators assume you move high to low, and move in the positive i direction.
		this isn't always true, and the mapData they generate may need to be rotated according
		to the original destination we were given.
		*/
		
		int angle = 0;
		if (destination.y > 0)
			angle += 180;
		
		if (destination.x == 0)
			angle += destination.z > 0 ? 270 : 90;
		if (destination.z == 0 && destination.x < 0)
			angle += 180;
		
		mapData = MDController.jRotate (mapData, angle);
		return mapData;
	}
	
	private static int[,,] genMapDataFromPoint (IntVector3 destination, int width)
	{		
		return genMapDataFromPoint (destination, width, 0);
	}
	
	private static int[,,] genMapDataFromPoint (IntVector3 destination, int width, int addedHeight)
	{
		/*
		makes a suitably sized mapData for CatwalkGenerator functions. the intention is to rotate
		the mapdata later, for now the first dimension is the longest of destination.i and destination.k
		*/
		IntVector3 arraySize = new IntVector3 (Mathf.Abs (destination.x) + 1, Mathf.Abs (destination.y) + 1, Mathf.Abs (destination.z) + 1);
		if (arraySize.x != 1 && arraySize.z != 1)
			Debug.Log ("warning: CatwalkGenerator got a weird point.");
		
		if (arraySize.x < arraySize.z)
			arraySize.x = arraySize.z;
		arraySize.z = width;
		
		return new int[arraySize.x, arraySize.y + addedHeight, arraySize.z];
	}
}
