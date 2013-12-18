using UnityEngine;
using System.Collections;

public class JunkGenerator : MonoBehaviour
{
	private static VoxelTheme junkTheme;
	private static IntVector3 size;

	public static int[,,] gen (VoxelTheme voxelTheme, IntVector3 junkSize)
	{
		size=junkSize;
		
		junkTheme = voxelTheme;
		
		int [,,] mapData = new int[size.x, size.y, size.z];
		
		int junkMax = (int)Mathf.Sqrt (Mathf.Min (size.x, size.y, size.z)) + 1;
		for (int i=0; i<junkMax; i++)
			addRandomJunk (mapData);
		
		return MDController.trim (mapData);
	}
	
	private static void addRandomJunk (int[,,] mapData)
	{
		int junkType = Random.Range (0, 4);
		
		if (junkType == 0)
			addTwisty (mapData);
		if (junkType == 1)
			addPlane (mapData);
		if (junkType == 2)
			addDonutFragment (mapData);
		if (junkType == 3)
			addGreeble (mapData);
	}
	
	private static void connectPoints (int[,,] mapData, int voxelCode, IntVector3 point, IntVector3 targetPoint)
	{
		/*
		fills voxels to connect point to targetPoint. can be used to guarentee that junk is one 
		contiguous piece
		
		chooses a random solution
		*/
		
		ArrayList directions = new ArrayList ();
		directions.Add (0);
		directions.Add (1);
		directions.Add (2);
		ZTools.shuffleArraylist (directions);
		
		foreach (int direction in directions) {
			
			if (direction == 0) {
				while (point.x!=targetPoint.x) {
					if (MDView.isInsideMapData (mapData, point))
						mapData [point.x, point.y, point.z] = voxelCode;
					point.x += point.x < targetPoint.x ? 1 : -1;
					continue;
				}
			}
			
			if (direction == 1) {
				while (point.y!=targetPoint.y) {
					if (MDView.isInsideMapData (mapData, point))
						mapData [point.x, point.y, point.z] = voxelCode;
					point.y += point.y < targetPoint.y ? 1 : -1;
					continue;
				}
			}
			
			if (direction == 2) {
				while (point.z!=targetPoint.z) {
					if (MDView.isInsideMapData (mapData, point))
						mapData [point.x, point.y, point.z] = voxelCode;
					point.z += point.z < targetPoint.z ? 1 : -1;
					continue;
				}
			}
			
		}
		if (MDView.isInsideMapData (mapData, targetPoint))
			mapData [targetPoint.x, targetPoint.y, targetPoint.z] = voxelCode;
	}
	
	private static void addPiece (int[,,] mapData, int[,,] pieceData, IntVector3 targetPoint)
	{
		/*
		adds a generic piece of junk to mapData using random orientation and position
		*/
		pieceData = MDController.jRotate (pieceData, Random.Range (0, 4) * 90);
		if (Random.Range (0, 2) == 1) {
			if (pieceData.GetLength (0) > pieceData.GetLength (2))			
				pieceData = MDController.jRotate (pieceData, 90);
			pieceData = MDController.iRotate (pieceData, Random.Range (0, 4) * 90);
		}
		
		IntVector3 offset = new IntVector3 (0, 0, 0);
		if (pieceData.GetLength (0) == 1)
			offset.x = Random.Range (0, size.x - pieceData.GetLength (0));
		if (pieceData.GetLength (1) == 1)
			offset.y = Random.Range (0, size.y - pieceData.GetLength (1));
		if (pieceData.GetLength (2) == 1)
			offset.z = Random.Range (0, size.z - pieceData.GetLength (2));
		
		mapData = MDController.combine (mapData, pieceData, offset);
		
		//now ensure that the new data connects to targetPoint
		
		int voxelCode = junkTheme.getCode ("block");
		bool breakFlag = false;
		for (int i=0; i<pieceData.GetLength(0); i++) {
			for (int j=0; j<pieceData.GetLength(1); j++) {
				for (int k=0; k<pieceData.GetLength(2); k++) {
					if (pieceData [i, j, k] > 0) {
						IntVector3 point = new IntVector3 (i + offset.x, j + offset.y, k + offset.z);
						connectPoints (mapData, voxelCode, point, targetPoint);
						breakFlag = true;
						break;
					}
				}
				if (breakFlag)
					break;
			}
			if (breakFlag)
				break;
		}
			
	}
	
	private static void addTwisty (int[,,] mapData)
	{
		IntVector3 point = ZTools.getRandomPointFromFaces (new Prism (0, 0, 0, size.x, size.y, size.z));
		IntVector3 targetPoint = new IntVector3 (size.x / 2, size.y / 2, size.z / 2);
		int voxelCode = junkTheme.getRandomCode ();
		connectPoints (mapData, voxelCode, point, targetPoint);
	}
	
	private static void addPlane (int[,,] mapData)
	{
		int voxelCode = junkTheme.getRandomCode ();
		
		ArrayList sizes = new ArrayList ();
		sizes.Add (size.x);
		sizes.Add (size.y);
		sizes.Add (size.z);
		ZTools.shuffleArraylist (sizes);
		
		Prism prism = new Prism (0, 0, 0, Random.Range (1, (int)sizes [0]), 1, Random.Range (1, (int)sizes [1]));
		int[,,] planeData = ShapeGenerator.genPrism (voxelCode, prism);
		
		//create random gaps in the plane
		Prism negativePrism;
		IntVector3 offset;
		int iterMax = (int)Mathf.Sqrt (Mathf.Min (size.x, size.y, size.z));
		for (int i=0; i<Random.Range(1,iterMax); i++) {
			negativePrism = new Prism (0, 0, 0, Random.Range (1, prism.width), 1, Random.Range (1, prism.depth));
			offset = new IntVector3 (Random.Range (0, prism.width), 0, Random.Range (0, prism.depth));
			planeData = MDController.subtract (planeData, ShapeGenerator.genPrism (voxelCode, negativePrism), offset);
		}
		//ensure planeData is never completely blank
		planeData [0, 0, 0] = voxelCode;
		
		IntVector3 targetPoint = new IntVector3 (size.x / 2, size.y / 2, size.z / 2);
		addPiece (mapData, planeData, targetPoint);
	}
	
	private static void addDonutFragment (int[,,] mapData)
	{
		int voxelCode = junkTheme.getRandomCode ();
		int diameter = Random.Range (5, Mathf.Min (size.x, size.y, size.z));
		
		Prism negativePrism = new Prism (0, 0, 0, Random.Range (1, diameter), 1, Random.Range (1, diameter));
		IntVector3 negativeOffset = new IntVector3 (Random.Range (0, diameter), 0, Random.Range (0, diameter));
		
		int[,,] donutData = ShapeGenerator.genDonut (voxelCode, diameter);
		donutData = MDController.subtract (donutData, ShapeGenerator.genPrism (1, negativePrism), negativeOffset);
		IntVector3 targetPoint = new IntVector3 (size.x / 2, size.y / 2, size.z / 2);
		addPiece (mapData, donutData, targetPoint);
	}
	
	private static void addGreeble (int[,,] mapData)
	{
		int minDimension = 4;
		IntVector3 greebleSize = new IntVector3 (Random.Range (minDimension, size.x), Random.Range (minDimension, size.y), Random.Range (minDimension, size.z));
		int[,,] greebleData = GreebleGenerator.gen (junkTheme, greebleSize);
		
		IntVector3 targetPoint = new IntVector3 (size.x / 2, size.y / 2, size.z / 2);
		addPiece (mapData, greebleData, targetPoint);
	}
}





