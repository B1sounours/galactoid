using UnityEngine;
using System.Collections;

public static class RaisedPlatformGenerator
{
	public static int[,,] gen (VoxelTheme voxelTheme, IntVector3 size)
	{
		int [,,] mapData = new int[size.x, size.y, size.z];
		int minFractalSpacing = 1;
		
		IntVector3 lastPoint = new IntVector3 (-1, -1, -1);
		
		ArrayList squares = ZTools.getFractalSquares (new Square (0, 0, size.x, size.z), minFractalSpacing);
		int emptySquareCounter = 0;
		foreach (Square square in squares) {
			//a chance that the square is empty
			if (emptySquareCounter < squares.Count / 8 && Random.Range (0, 5) == 0)
				continue;
			
			int platformHeight = Random.Range (3, size.y > 7 ? 7 : size.y - 1);
			
			IntVector3 offset = new IntVector3 (square.x, platformHeight, square.y);
			
			//platform base
			int[,,] newData = ShapeGenerator.genPrism (voxelTheme.getCode ("hull"), square.width, 1, square.height);
			mapData = MDController.combine (mapData, newData, offset);
			
			//platform rim
			newData = GreebleGenerator.genCage (voxelTheme, square.width, 1, square.height);
			mapData = MDController.combine (mapData, newData, offset);
			
			//greeble
			newData = GreebleGenerator.gen (voxelTheme, square.width, size.y - platformHeight, square.height);
			mapData = MDController.combine (mapData, newData, offset);
			
			//gravity plate
			int gravityWidth = Random.Range (3, square.width);
			int gravityDepth = Random.Range (3, square.height);
			IntVector3 gravityOffset = new IntVector3 (square.x + square.width / 2 - gravityWidth / 2, 1, square.y + square.height / 2 - gravityDepth / 2);
			newData = ShapeGenerator.genPrism (voxelTheme.getCode ("gravity plate"), gravityWidth, 1, gravityDepth);
			mapData = MDController.combine (mapData, newData, gravityOffset);
			
			//guarentee gravity plate is at least attached to lilypad directly above it
			IntVector3 basePoint = new IntVector3 (square.x + square.width / 2, 1, square.y + square.height / 2);
			newData = ShapeGenerator.genPrism (voxelTheme.getCode ("block"), 1, platformHeight, 1);
			mapData = MDController.combine (mapData, newData, basePoint);
			
			//attach lilypads with underhang
			IntVector3 point = basePoint;
			point.y = platformHeight - 1;
			if (lastPoint.x != -1) {
				Prism prism = ZTools.getPrismFromPoints (point, lastPoint);
				IntVector3 offsetPoint = prism.getPosition ();
				offsetPoint.y = 0;
				
				int addedHeight = point.y < lastPoint.y ? point.y + 1 : lastPoint.y + 1;
				newData = ShapeGenerator.genUnderhang (voxelTheme, point, lastPoint, addedHeight);
				mapData = MDController.combine (mapData, newData, offsetPoint);
			}
			lastPoint = point;
		}
		
		//mapData=StationGenerator.genDebug(voxelTheme);
		
		return mapData;
	}
	
	private static int[,,] genDebug (VoxelTheme voxelTheme, int width, int height, int depth)
	{
		int [,,] mapData = new int[width, height, depth];
		int voxelCode = voxelTheme.getCode ("hull");
		mapData = MDController.combine (mapData, ShapeGenerator.genPrism (voxelCode, width, 1, depth), 0, 0, 0);
		
		IntVector3 point1 = new IntVector3 (1, 3, 1);
		IntVector3 point2 = new IntVector3 (5, 6, 10);
		int addedHeight = 4;
		mapData = MDController.combine (mapData, ShapeGenerator.genUnderhang (voxelTheme, point1, point2, addedHeight), 2, 2, 2);
		
		return mapData;
	}
}
