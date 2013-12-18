using UnityEngine;
using System.Collections;

public static class ShapeGenerator
{
	public static int[,,] genDonut (int voxelCode, int bigDiameter)
	{
		return genDonut (voxelCode, bigDiameter, bigDiameter - 4);
	}
	
	public static int[,,] genCircle (int voxelCode, int diameter)
	{
		//Debug.Log("genCircle diameter: "+diameter);
		if (diameter < 2)
			return ShapeGenerator.genPrism (voxelCode, 1, 1, 1);
		
		int[,,] newData = new int[diameter, 1, diameter];
		
		float center = (float)diameter / 2;
		int maxRadius = Mathf.CeilToInt (center);
		
		for (int i=0; i<=maxRadius; i++) {
			for (int k=0; k<=maxRadius; k++) {
				float distance = Mathf.Sqrt (Mathf.Pow ((float)i - center, 2) + Mathf.Pow ((float)k - center, 2));
				
				if (distance <= maxRadius)
					newData [i, 0, k] = voxelCode;
			}
		}
		
		newData = MDController.combine (newData, MDController.jRotate (newData, 90));
		newData = MDController.combine (newData, MDController.jRotate (newData, 180));
		
		return newData;
	}
	
	public static int[,,] genDonut (int voxelCode, int bigDiameter, int smallDiameter)
	{
		int[,,] newData = new int[bigDiameter, 1, bigDiameter];
		
		newData = MDController.combine (newData, genCircle (voxelCode, bigDiameter));
		
		if (bigDiameter - smallDiameter < 4 || smallDiameter < 0) {
			//Debug.Log ("warning: genDonut made a circle. got weird donut parameters: " + bigDiameter + " " + smallDiameter);
			return newData;
		}
		
		int offset = (bigDiameter - smallDiameter) / 2;
		newData = MDController.subtract (newData, genCircle (voxelCode, smallDiameter), offset, 0, offset);
		
		return newData;
	}
	
	public static int[,,] genPrism (int voxelCode, Prism prism)
	{
		return genPrism (voxelCode, prism.width, prism.height, prism.depth);
	}
	
	public static int[,,] genPrism (int voxelCode, int width, int height, int depth)
	{	
		if (width < 1 || height < 1 || depth < 1) {
			Debug.Log ("genPrism failed " + width + " " + height + " " + depth);
			return new int[1, 1, 1];
		}
		
		int[,,] newData = new int[width, height, depth];
		
		for (int i=0; i<width; i++) {
			for (int j=0; j<height; j++) {
				for (int k=0; k<depth; k++) {
					newData [i, j, k] = voxelCode;
					
				}
			}
		}
		
		return newData;
	}
	
	public static int[,,] genUnderhang (VoxelTheme voxelTheme, IntVector3 point1, IntVector3 point2, int addedHeight)
	{		
		Prism prism = ZTools.getPrismFromPoints (point1, point2);
		prism.height += addedHeight;
		int[,,] newData = new int[prism.width, prism.height, prism.depth];
		
		IntVector3 offset = new IntVector3 (point1.x - prism.x, 0, point1.z - prism.z);
		newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelTheme.getCode ("block"), 1, point1.y-prism.y+addedHeight, 1), offset);
		offset = new IntVector3 (point2.x - prism.x, 0, point2.z - prism.z);
		newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelTheme.getCode ("block"), 1, point2.y-prism.y+addedHeight, 1), offset);
		
		newData = MDController.combine (newData, GreebleGenerator.genCage (voxelTheme, prism.width, 1, prism.depth));
		
		return newData;
	}
	
	public static int[,,] genTriangle (int voxelCode, int width, int depth)
	{
		int[,,] newData = new int[width, 1, depth];
		
		float slope = (float)width / (float)depth;
		
		for (int k=0; k<depth; k++) {
			int newSize = (int)(width - k * slope);
			if (newSize > 0)
				newData = MDController.combine (newData, genPrism (voxelCode, newSize, 1, 1), 0, 0, k);
		}
		
		return newData;
	}
}
