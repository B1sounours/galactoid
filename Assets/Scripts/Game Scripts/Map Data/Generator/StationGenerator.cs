using UnityEngine;
using System.Collections;

public static class StationGenerator {
	
	public static int[,,] genDebug (VoxelTheme voxelTheme)
	{
		int width=60;
		int height=30;
		int depth=60;
		int[,,] mapData = new int[width, height, depth];
		int voxelCode = voxelTheme.getCode ("default");
		mapData = MDController.combine (mapData, ShapeGenerator.genPrism (voxelCode, width, 1, depth));
		
		mapData=MDController.combine(mapData,genFloor(voxelTheme,60),0,1,0);
		
		return mapData;
	}
	
	private static int[,,] genFloor (VoxelTheme voxelTheme, int diameter)
	{	
		int floorHeight=diameter/4;
		int floorCode=voxelTheme.getCode("hull");
		int[,,] newData = new int[diameter,floorHeight,diameter];
		
		int catwalkWidth=3;
		int offset=diameter/2-catwalkWidth/2;
		newData=MDController.combine(newData,ShapeGenerator.genPrism(floorCode,diameter,1,catwalkWidth),0,0,offset);
		newData=MDController.combine(newData,ShapeGenerator.genPrism(floorCode,catwalkWidth,1,diameter),offset,0,0);
		
		int innerDiameter=diameter/4;
		offset=diameter/2-innerDiameter/2;
		newData=MDController.subtract(newData,ShapeGenerator.genCircle(floorCode,innerDiameter),offset,0,offset);
		newData=MDController.combine(newData,ShapeGenerator.genDonut(floorCode,innerDiameter),offset,0,offset);
		
		
		return newData;
	}
	
}
