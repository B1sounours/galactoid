using UnityEngine;
using System.Collections;

public static class PlatformGenerator
{

	public static int[,,] gen (VoxelTheme voxelTheme, IntVector3 size)
	{
		int [,,] mapData = new int[size.x, size.y, size.z];
		
		int maxPercent = 30;
		int engineCount = 0;
		int percent = Random.Range (0, maxPercent);
		int iSquare = size.x * percent / 100;
		int kSquare = size.z * (maxPercent - percent - 1) / 100;
		
		Square baseSquare = new Square (iSquare, kSquare, size.x - 2 * iSquare, size.z - 2 * kSquare);
		mapData = MDController.combine (mapData, genPlatformBase (voxelTheme, size.x, size.z, baseSquare), 0, 0, 0);
		mapData = MDController.combine (mapData, genPlatformPlate (voxelTheme, baseSquare.width, baseSquare.height), baseSquare.x, 1, baseSquare.y);
		
		int minFractalSpacing = 1;
		foreach (Square square in ZTools.getFractalSquares(new Square( 0,0,baseSquare.width ,baseSquare.height),minFractalSpacing)) {
			int i = square.x + baseSquare.x;
			int k = square.y + baseSquare.y;
			int j = 1 + MDView.getMapHeight (mapData, i + square.width / 2, k + square.height / 2);
			mapData = MDController.combine (mapData, GreebleGenerator.gen (voxelTheme, square.width, size.y - j, square.height), i, j, k);
			if (Random.Range (0, engineCount / 3 + 1) == 0) {
				if (mapData.GetLength (1) > j)
					mapData [i + square.width / 2, j, k + square.height / 2] = voxelTheme.getCode ("engine");
				engineCount++;
			}
		}
		
		return mapData;
	}

	private static int[,,] genPlatformBase (VoxelTheme voxelTheme, int width, int depth, Square square)
	{		
		int [,,] newData = new int[width, 1, depth];
		
		int gravityCode = voxelTheme.getCode ("gravity plate");
		int [,,] prismData = ShapeGenerator.genPrism (gravityCode, square.width, 1, square.height);
		MDController.combine (newData, prismData, square.x, 0, square.y);
		
		for (int side=0; side<2; side++) {
			int [,,] spikeData = genPlatformSpikes (voxelTheme.getCode ("hull"), square.width, square.y);
			int angle = (1 - side) * 180;
			int kOffset = side * (square.y + square.height);
			MDController.combine (newData, MDController.jRotate (spikeData, angle), square.x, 0, kOffset);
		}
		
		for (int side=0; side<2; side++) {
			int [,,] spikeData = genPlatformSpikes (voxelTheme.getCode ("hull"), square.height, square.x);
			int angle = (1 - side) * 180 + 90;
			int iOffset = side * (square.x + square.width);
			MDController.combine (newData, MDController.jRotate (spikeData, angle), iOffset, 0, square.y);
		}
		
		return newData;
	}
	
	private static int[,,] genPlatformPlate (VoxelTheme voxelTheme, int width, int depth)
	{
		//Debug.Log("genPlatformPlate "+width+" "+depth);
		int maxHeight = 3;
		int [,,] mapData = new int[width, maxHeight, depth];
		
		int temp = (int)Mathf.Sqrt (width * depth);
		int plateCount = Random .Range (temp / 2 + 1, temp * 2 + 1);
		
		for (int i=0; i<plateCount; i++) {
			int itemWidth = Random.Range (2, (int)Mathf.Sqrt (width) + 1);
			int itemHeight = Random.Range (1, maxHeight + 1);
			int itemDepth = Random.Range (2, (int)Mathf.Sqrt (depth) + 1);
			
			int iOffset = Random.Range (0, width - itemWidth);
			int kOffset = Random.Range (0, depth - itemDepth);
			
			int plateType = Random .Range (0, 7);
			
			if (plateType == 0)
				//prism
				mapData = MDController.combine (mapData, ShapeGenerator.genPrism (voxelTheme.getCode ("hull"), itemWidth, itemHeight, itemDepth), iOffset, 0, kOffset);
			if (plateType == 1) {
				//cylinder
				for (int h=0; h<itemHeight; h++)
					mapData = MDController.combine (mapData, ShapeGenerator.genCircle (voxelTheme.getCode ("hull"), itemWidth), iOffset, h, kOffset);
			}
			if (plateType == 2)
				//hollow square
				mapData = MDController.combine (mapData, GreebleGenerator.genCage (voxelTheme, itemWidth, 1, itemDepth), iOffset, 0, kOffset);
			if (plateType == 3) {
				//rounded prism
				mapData = MDController.combine (mapData, ShapeGenerator.genPrism (voxelTheme.getCode ("hull"), itemWidth, 1, itemDepth), iOffset, 0, kOffset);
				if (itemWidth > 2 && itemDepth > 2)
					mapData = MDController.combine (mapData, ShapeGenerator.genPrism (voxelTheme.getCode ("hull"), itemWidth - 2, 1, itemDepth - 2), iOffset + 1, 1, kOffset + 1);
			}
			if (plateType == 4)
				//perimeter fence
				mapData = MDController.combine (mapData, GreebleGenerator.genCage (voxelTheme, width, 1, depth), 0, 0, 0);
			if (plateType == 5 && itemWidth > 3) {
				//tunnel prism
				mapData = MDController.combine (mapData, ShapeGenerator.genPrism (voxelTheme.getCode ("hull"), itemWidth / 2, itemHeight, itemDepth), iOffset, 0, kOffset);
				mapData = MDController.combine (mapData, ShapeGenerator.genPrism (voxelTheme.getCode ("hull"), itemWidth / 2 - 1, itemHeight, itemDepth), iOffset + itemWidth / 2 + 1, 0, kOffset);
			}
			if (plateType == 6) {
				//partial perimeter fence
				int side = Random .Range (0, 2);
				if (Random .Range (0, 2) == 0) {
					mapData = MDController.combine (mapData, ShapeGenerator.genPrism (voxelTheme.getCode ("hull"), width, itemHeight * 2, 1), 0, 0, depth * side);
				} else {
					mapData = MDController.combine (mapData, ShapeGenerator.genPrism (voxelTheme.getCode ("hull"), 1, itemHeight * 2, depth), width * side, 0, 0);
				}
			}
		}
		
		return mapData;
	}
	
	private static int [,,] genPlatformSpikes (int voxelCode, int width, int depth)
	{
		//Debug.Log("genPlatformSpikes "+width+" "+depth );
		int [,,] mapData = new int[width, 1, depth];
		
		int lastLength = 0;
		int spikeLength = 0;
		for (int iOffset=0; iOffset<width; iOffset++) {
			
			int randomValue = Random.Range (0, 6);
			
			if (randomValue < 3)
				spikeLength = Random.Range (0, depth + 1);
			if (randomValue == 3)
				spikeLength = lastLength;
			if (randomValue == 4) {
				spikeLength = lastLength - 1;
				if (spikeLength < 0)
					spikeLength = 0;
			}
			if (randomValue == 5)
				spikeLength /= 2;
			
			lastLength = spikeLength;
			if (spikeLength > 0)
				MDController.combine (mapData, ShapeGenerator.genPrism (voxelCode, 1, 1, spikeLength), iOffset, 0, 0);
		}
		
		return mapData;
	}
}
