using UnityEngine;
using System.Collections;

public static class GreebleGenerator
{
	public static int[,,] gen (VoxelTheme voxelTheme, IntVector3 size){
		return gen (voxelTheme,size.x,size.y,size.z);
	}
	
	public static int[,,] gen (VoxelTheme voxelTheme, int width, int height, int depth)
	{
		//the culmination of this entire module, packaged into one glorious function!
		int greebleTypeCount = 11;
		int smallestSize = width;
		int biggestSize = width;
		if (width < depth) {
			biggestSize = depth;
		} else {
			smallestSize = depth;
		}
		
		//Debug.Log("soft code:"+softCode+" upCode:" +upCode);
		
		int maxAttempts = 100;
		
		for (int attemptCount=0; attemptCount<maxAttempts; attemptCount++) {
			int greebleIndex = Random.Range (0, greebleTypeCount);
			if (greebleIndex == 0) {
				if (biggestSize > 4)
					continue;
				
				int greebleHeight = Random.Range (smallestSize, smallestSize * 2);
				if (greebleHeight > height)
					greebleHeight = height;
				return genBattery (voxelTheme.getCode ("up"), voxelTheme.getCode ("plate"), width, greebleHeight, depth);
			}
			
			if (greebleIndex == 1) {
				if (smallestSize < 4)
					continue;
				
				int greebleHeight = Random.Range (smallestSize * 2, smallestSize * 5);
				if (greebleHeight > height)
					greebleHeight = height;
				return genCageAntenna (voxelTheme, width, greebleHeight, depth);
			}
			
			if (greebleIndex == 2) {
				if (biggestSize > 4)
					continue;
				
				int greebleHeight = Random.Range (smallestSize, smallestSize * 2);
				if (greebleHeight > height)
					greebleHeight = height;
				return genCapacitor (voxelTheme.getCode ("up"), voxelTheme.getCode ("plate"), width, greebleHeight, depth);
			}
			
			if (greebleIndex == 3) {
				if (smallestSize < 3)
					continue;
				
				int greebleHeight = Random.Range (smallestSize * 2, smallestSize * 5);
				if (greebleHeight > height)
					greebleHeight = height;
				return genCoilTower (voxelTheme, width, greebleHeight, depth);
			}
			
			if (greebleIndex == 4) {
				if (smallestSize < 6)
					continue;
				
				int greebleHeight = Random.Range (smallestSize / 3, smallestSize * 3 / 2);
				if (greebleHeight < 3)
					greebleHeight = 3;
				if (greebleHeight > height)
					greebleHeight = height;
				return genMushroom (voxelTheme.getCode ("plate"), voxelTheme.getCode ("block"), voxelTheme.getCode ("soft"), width, greebleHeight, depth);
			}
			
			if (greebleIndex == 5) {				
				int greebleHeight = Random.Range (biggestSize / 2, biggestSize * 3);
				if (greebleHeight > height)
					greebleHeight = height;
				return genRingAntenna (voxelTheme, width, greebleHeight, depth);
			}
			
			if (greebleIndex == 6) {
				int greebleHeight = Random.Range (biggestSize / 2, biggestSize * 5);
				if (greebleHeight > height)
					greebleHeight = height;
				return genTwisty (voxelTheme, width, greebleHeight, depth);
			}
			
			if (greebleIndex == 7) {
				if (smallestSize < 3 || height < 3)
					continue;
				
				int greebleHeight = Random.Range (biggestSize, biggestSize * 3);
				if (greebleHeight > height)
					greebleHeight = height;
				return genWillow (voxelTheme, width, greebleHeight, depth);
			}
			
			if (greebleIndex == 8) {
				int greebleHeight = Random.Range (smallestSize / 2, smallestSize * 2);
				if (greebleHeight > height)
					greebleHeight = height;
				return genComb (voxelTheme.getCode ("block"), width, greebleHeight, depth);
			}
			
			if (greebleIndex == 9) {
				int greebleHeight = Random.Range (smallestSize / 2, smallestSize * 2);
				if (greebleHeight > height)
					greebleHeight = height;
				return genCircleComb (voxelTheme.getCode ("block"), width, greebleHeight, depth);
			}
			
			if (greebleIndex == 10) {
				int greebleHeight = Random.Range (smallestSize * 3, smallestSize * 9);
				if (greebleHeight > height)
					greebleHeight = height;
				return genSatellite (voxelTheme.getCode ("up"), voxelTheme.getCode ("soft"), width, greebleHeight, depth);
			}
			
		}
		
		Debug.Log ("genGreeble failed after " + maxAttempts + " attempts.");
		return new int[width, height, depth];
	}
	
	public static int[,,] genBattery (int needleCode, int bodyCode, int width, int height, int depth)
	{
		int[,,] newData = new int[width, height, depth];
		
		int needleHeight = Random .Range (height / 3 + 1, height);
		
		int diameter;
		int iCenter = width / 2;
		int kCenter = depth / 2;
		if (width >= depth) {
			diameter = depth;
			iCenter = diameter / 2;
		} else {
			diameter = width;
			kCenter = diameter / 2;
		}
		
		newData = MDController.combine (newData, ShapeGenerator.genPrism (needleCode, 1, needleHeight, 1), iCenter, needleHeight, kCenter);
		
		for (int kOffset=0; kOffset<needleHeight; kOffset++) {
			newData = MDController.combine (newData, ShapeGenerator.genCircle (bodyCode, diameter), 0, kOffset, 0);
		}
		
		return newData;
	}
	
	public static int[,,] genCage (VoxelTheme voxelTheme, int width, int height, int depth)
	{
		//Debug.Log ("genCage " + width + " " + height + " " + depth);
		int[,,] newData = ShapeGenerator.genPrism (voxelTheme.getCode ("side"), width, height, depth);
		
		if (height > 2)
			newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelTheme.getCode ("up"), width, height - 2, depth), 0, 1, 0);
		
		for (int iOffset=0; iOffset<2; iOffset++) {
			for (int jOffset=0; jOffset<2; jOffset++) {
				for (int kOffset=0; kOffset<2; kOffset++) {
					IntVector3 point=new IntVector3(iOffset * (width - 1),
					jOffset * (height - 1),
					kOffset * (depth - 1));
					
					if (MDView.isInsideMapData (newData, point))
						newData [point.x,point.y,point.z] = voxelTheme.getCode ("block");
				}
			}
		}
		
		if (width > 2 && height > 2)
			newData = MDController.subtract (newData, ShapeGenerator.genPrism (1, width - 2, height - 2, depth), 1, 1, 0);
		
		if (width > 2 && depth > 2)
			newData = MDController.subtract (newData, ShapeGenerator.genPrism (1, width - 2, height, depth - 2), 1, 0, 1);
		
		if (height > 2 && depth > 2)
			newData = MDController.subtract (newData, ShapeGenerator.genPrism (1, width, height - 2, depth - 2), 0, 1, 1);
		
		return newData;
	}
	
	public static int[,,] genCageAntenna (VoxelTheme voxelTheme, int width, int height, int depth)
	{
		int[,,] newData = new int[width, height, depth];
		newData = MDController.combine (newData, genCage (voxelTheme, width, height + 1, depth));
		
		int stepMax = (int)Mathf.Sqrt (height);
		int jOffset = 0;
		int bodyHeight = Random.Range (height / 2, height * 5 / 6);
		
		while (jOffset<bodyHeight) {
			int cageHeight = Random.Range (3, stepMax + 3);
			newData = MDController.combine (newData, genCage (voxelTheme, width, cageHeight, depth), 0, jOffset, 0);
			jOffset += cageHeight - 1;
		}
		
		return newData;
	}
	
	public static int[,,] genCapacitor (int trunkCode, int bodyCode, int width, int height, int depth)
	{
		int[,,] newData = new int[width, height, depth];
		
		int trunkHeight = Random .Range (1, height / 2 + 1);
		
		int diameter;
		int iCenter = width / 2;
		int kCenter = depth / 2;
		if (width >= depth) {
			diameter = depth;
			iCenter = diameter / 2;
		} else {
			diameter = width;
			kCenter = diameter / 2;
		}
		
		newData = MDController.combine (newData, ShapeGenerator.genPrism (trunkCode, 1, trunkHeight, 1), iCenter, 0, kCenter);
		
		for (int h=trunkHeight; h<height; h++) {
			newData = MDController.combine (newData, ShapeGenerator.genCircle (bodyCode, diameter), 0, h, 0);
		}
		
		return newData;
	}
	
	public static int[,,] genCircleComb (int voxelCode, int width, int height, int depth)
	{
		//Debug.Log("genCircleComb "+width+" "+height+" "+depth);
		int[,,] newData = new int[width, height, depth];
		
		int diameter = height < depth ? height : depth;
		int stepSize = Random.Range (2, width / 10);
		stepSize = stepSize < 2 ? 2 : stepSize;
		for (int i=0; i<width; i+=stepSize) {
			int[,,] circleData = ShapeGenerator.genCircle (voxelCode, diameter);
			circleData = MDController.iRotate (circleData, 90);
			circleData = MDController.jRotate (circleData, 90);
			newData = MDController.combine (newData, circleData, i, -diameter / 2, 0);
		}
		
		//ensure all combs are connected
		Prism prism=new Prism(0,0,0,width,1,1);
		newData=MDController.combine(newData,ShapeGenerator.genPrism (voxelCode,prism));
		
		return newData;
	}
	
	public static int[,,] genCoilTower (VoxelTheme voxelTheme, int width, int height, int depth)
	{
		//Debug.Log("genCoilTower "+width+" "+height+" "+depth);
		int[,,] newData = new int[width, height, depth];
		
		int diameter = width >= depth ? depth : width;
		
		int stemWidth = Mathf.CeilToInt ((float)diameter / 2);
		int center = diameter / 2 - stemWidth / 2;
		int stemOffset = 0;
		if (diameter % 2 == 0 && stemWidth % 2 == 1)
			stemOffset = 1;
		
		if ((diameter % 2 == 1) ^ (stemWidth % 2 == 1)) {
			//stemWidth++;
		}
		
		int stepSize = Random.Range (2 + height / 10, (int)Mathf.Sqrt (height) + 1);
		stepSize = stepSize < 2 + height / 10 ? 2 + height / 10 : stepSize;
		
		for (int kOffset=stepSize; kOffset<height; kOffset+=stepSize) {
			newData = MDController.combine (newData, ShapeGenerator.genDonut (voxelTheme.getCode ("plate"), diameter, stemWidth), 0, kOffset, 0);
		}
		newData = MDController.combine (newData, genCage (voxelTheme, stemWidth - stemOffset, height + 2, stemWidth - stemOffset), center, -1, center);
		
		return newData;
	}
	
	public static int[,,] genComb (int voxelCode, int width, int height, int depth)
	{
		//Debug.Log("genComb "+width+" "+height+" "+depth);
		int[,,] newData = new int[width, height, depth];
		
		int stepSize = Random.Range (2, width / 10);
		stepSize = stepSize < 2 ? 2 : stepSize;
		for (int i=0; i<width; i+=stepSize) {
			int[,,] triangleData = ShapeGenerator.genTriangle (voxelCode, width, height);
			triangleData = MDController.iRotate (triangleData, 90);
			triangleData = MDController.jRotate (triangleData, 90);
			newData = MDController.combine (newData, triangleData, i, 0, 0);
		}
		
		//ensure all combs are connected
		Prism prism=new Prism(0,0,0,width,1,1);
		newData=MDController.combine(newData,ShapeGenerator.genPrism (voxelCode,prism));
		
		return newData;
	}
	
	public static int[,,] genMushroom (int stemCode, int shellCode, int softCode, int width, int height, int depth)
	{
		//Debug.Log("genMushroom "+width+" "+height+" "+depth);
		int[,,] newData = new int[width, height, depth];
		
		int stemHeight = Random.Range (height / 5 + 1, height / 3);
		int stemWidth = (int)Mathf .Sqrt (width + depth) / 4 + 1;
		
		int diameter;
		if (width >= depth) {
			diameter = depth;
		} else {
			diameter = width;
		}
		int center = diameter / 2 - stemWidth / 2;
		
		newData = MDController.combine (newData, ShapeGenerator.genPrism (stemCode, stemWidth, height, stemWidth), center, 0, center);
		newData = MDController.combine (newData, ShapeGenerator.genPrism (softCode, stemWidth, height / 3, stemWidth), center, 0, center);
		newData = MDController.combine (newData, ShapeGenerator.genCircle (shellCode, diameter - 2), 1, height - 1, 1);
		
		for (int jOffset=stemHeight; jOffset <height-1; jOffset ++) {
			newData = MDController.combine (newData, ShapeGenerator.genDonut (shellCode, diameter), 0, jOffset, 0);
		}
		
		return newData;
	}

	public static int[,,] genRingAntenna (VoxelTheme voxelTheme, int width, int height, int depth)
	{
		//Debug.Log("genRingAntenna "+width+" "+height+" "+depth);
		int[,,] newData = new int[width, height, depth];
		
		int ringWidth = Random.Range (1, (int)Mathf .Sqrt (width + depth));
		int stemWidth = ringWidth < 3 ? ringWidth : 2;
		
		for (int offset=0; offset<ringWidth; offset++) {
			newData = MDController.combine (newData, genCage (voxelTheme, width - offset * 2, 1, depth - offset * 2), offset, height - 1, offset);
		}
		newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelTheme.getCode ("block"), stemWidth, height - 1, stemWidth), width / 2 - stemWidth / 2, 0, 0);
		newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelTheme.getCode ("techno"), stemWidth, 1, stemWidth), width / 2 - stemWidth / 2, height / 3, 0);
		newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelTheme.getCode ("techno"), stemWidth, 1, stemWidth), width / 2 - stemWidth / 2, height * 2 / 3, 0);
		
		if (width == depth) {
			newData = MDController .jRotate (newData, Random .Range (0, 4) * 90);
		} else {
			newData = MDController .jRotate (newData, Random .Range (0, 2) * 180);
		}
		
		return newData;
	}
	
	public static int[,,] genSatellite (int voxelCode, int delicateCode, int width, int height, int depth)
	{
		//Debug.Log("genSatellite "+width+" "+height+" "+depth);
		int[,,] newData = new int[width, height, depth];
		
		int diameter = width > depth ? depth : width;
		
		int center = diameter / 2 - 1;
		center = center > 0 ? center : 1;
		
		int satHeight = height / 3 + 1;
		newData = MDController.combine (newData, ShapeGenerator.genCircle (delicateCode, diameter), 0, satHeight, 0);
		newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelCode, 2, satHeight, 2), center, 0, center);
		newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelCode, 1, height - satHeight, 1), center + 1, satHeight, center + 1);
		
		return newData;
	}
	
	public static int[,,] genTwisty (VoxelTheme voxelTheme, int width, int height, int depth)
	{
		//Debug.Log("genTwisty "+width+" "+height+" "+depth);
		int[,,] newData = new int[width, height, depth];
		if (width==0||height==0||depth==0)
			return newData;
		
		int maxStep = Random.Range (3, (int)Mathf.Sqrt (height) + 3);
		
		IntVector3 point = new IntVector3 (width / 2, 0, depth / 2);
		IntVector3 lastPoint = new IntVector3 (0, 0, 0);
		
		int iterCount = 0;
		int side = 0;
		int directionCount = 0;
		int randomCase = 10;
		int voxelCode = voxelTheme.getCode ("block");
		
		newData [point.x, point.y, point.z] = voxelCode;
		while (iterCount<200) {
			iterCount ++;
			directionCount ++;
			
			//newData [point.i, point.j, point.k] = voxelCode;
			
			if (randomCase < 2) {
				point.x += side;
				voxelCode = voxelTheme.getCode ("side");
			}
			if (randomCase >= 2 && randomCase < 4) {
				point.z += side;
				voxelCode = voxelTheme.getCode ("side");
			}
			if (randomCase == 4) {
				point.y -= 1;
				voxelCode = voxelTheme.getCode ("soft");
				;
			}
			if (randomCase > 4) {
				point.y += 1;
				voxelCode = voxelTheme.getCode ("up");
			}
			
			if (directionCount > maxStep || Random.Range (0, 10) > directionCount) {
				randomCase = Random.Range (0, 10);
				directionCount = 0;
				side = Random.Range (0, 2) * 2 - 1;
			}
			
			if (MDView.isInsideMapData (newData, point)) {
				newData [point.x, point.y, point.z] = voxelCode;
				lastPoint = point;
			} else {
				if (point.y >= height - 1)
					break;
				point = lastPoint;
				directionCount = 100;
			}
		}
		//Debug.Log (iterCount);
		return newData;
	}
	
	public static int[,,] genWillow (VoxelTheme voxelTheme, int width, int height, int depth)
	{
		int[,,] newData = new int[width, height, depth];
		
		int iCenter = width / 2;
		int kCenter = depth / 2;
		
		int trunkWidth = (int)Mathf .Sqrt (width + height) / 4 + 1;
		
		int canopyWidth = width > 3 ? width - 2 : 1;
		int canopyDepth = depth > 3 ? depth - 2 : 1;
		
		newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelTheme.getCode ("block"), trunkWidth, height, trunkWidth), iCenter, 0, kCenter);
		newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelTheme.getCode ("soft"), trunkWidth, height / 5 + 1, trunkWidth), iCenter, height * 4 / 5 - 1, kCenter);
		newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelTheme.getCode ("plate"), canopyWidth, 1, canopyDepth), 1, height - 1, 1);
		
		int branchHeight = Random.Range (height / 3, height * 3 / 4);
		
		for (int swap=0; swap<2; swap++) {
			for (int full=0; full<2; full++) {
				float iRatio = full;
				float kRatio = 1f / 2f;
				
				if (swap == 1) {
					float temp = iRatio;
					iRatio = kRatio;
					kRatio = temp;
				}
				
				int i = Mathf.FloorToInt ((width-1) * iRatio);
				int k = Mathf.FloorToInt ((depth-1) * kRatio);
				
				newData = MDController.combine (newData, ShapeGenerator.genPrism (voxelTheme.getCode ("block"), 1, branchHeight, 1), i, height - branchHeight, k);
			}
		}
		
		return newData;
	}
}
