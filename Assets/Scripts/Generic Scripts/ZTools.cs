using UnityEngine;
using System.Collections;

public static class ZTools
{
	
	public static string toString (IntVector3 vec)
	{
		return " x: " + vec.x + " y: " + vec.y + " z: " + vec.z;
	}
	
	public static string toString (IntVector2 vec)
	{
		return " x: " + vec.x + " y: " + vec.y;
	}
	
	public static void show (Prism prism)
	{
		Debug.Log (" x: " + prism.x + " y: " + prism.y + " z: " + prism.z + " w: " + prism.width + " h: " + prism.height + " d: " + prism.depth);
	}
	
	public static void show (Vector2 vec)
	{
		Debug.Log (" x: " + vec.x + " y: " + vec.y);
	}
	
	public static void show (IntVector2 vec)
	{
		Debug.Log (toString (vec));
	}
	
	public static void show (IntVector3 vec)
	{
		Debug.Log (toString (vec));
	}
	
	public static void show (Square square)
	{
		Debug.Log (" x: " + square.x + " z: " + square.y + " width: " + square.width + " depth: " + square.height);
	}
	
	private static ArrayList getSplitSquares (int iOffset, int kOffset, int width, int depth, bool isUp, int minStepSize, int spacingSize)
	{
		/*
		returns a list of squares where the region has been split with either vertical or horizontal
		cuts.
		*/
		ArrayList squares = new ArrayList ();
		
		int divideLength = isUp ? depth : width;
		
		int maxSplits = divideLength / (minStepSize + spacingSize);
		int splitCount = Random.Range (1, maxSplits + 1);
		int stepSize = divideLength / splitCount;
		int squareSize = stepSize - spacingSize;
		
		if (maxSplits > 0) {
			int squareWidth = isUp ? width : squareSize;
			int squareDepth = isUp ? squareSize : depth;
			int offsetInit = (divideLength % splitCount) / 2;
			
			for (int offset=offsetInit; offset+squareSize<divideLength; offset+=stepSize) {
				int i = isUp ? iOffset : iOffset + offset;
				int k = isUp ? kOffset + offset : kOffset;
				//Debug.Log ("getSplitSquares square " + i + " " + k + " " + squareWidth + " " + squareDepth);
				squares.Add (new Square (i, k, squareWidth, squareDepth));
			}
		}
		
		return squares;
	}
	
	public static ArrayList getFractalSquares (Square square, int minFractalSpacing)
	{
		/*
		shatters the given space into a list of non-overlapping 2D squares, suitable for 
		greeble generation. leaves spacingSize space between squares so greeble isn't glued together.
		*/
		
		ArrayList squares = new ArrayList ();
		int minFractalSize = 5; 
		
		if (square.width < minFractalSize || square.height < minFractalSize) {
			//Debug.Log ("width or depth less than minStepSize");
			return squares;
		}
		
		float ratio = (float)square.width / square.height;		
		bool isUp;
		if (ratio < 0.5) {
			isUp = true;
		} else if (ratio > 2) {
			isUp = false;
		} else {			
			if (square.width + square.height < 35 && Random.Range (0, square.width + square.height) < minFractalSize * 3) {
				squares.Add (new Square (square.x, square.y, square.width, square.height));
				return squares;
			}
			
			isUp = Random.Range (0, square.width + square.height) > square.width;
		}
		
		foreach (Square iterSquare in getSplitSquares(square.x,square.y,square.width,square.height,isUp,minFractalSize,minFractalSpacing)) {
			//Debug.Log ("running fractals recursively");
			squares.AddRange (getFractalSquares (iterSquare, minFractalSpacing));
		}
		
		return squares;
	}
	
	public static Prism getPrismFromPoints (ArrayList points)
	{
		return getPrismFromPoints (points.ToArray (typeof(IntVector3)) as IntVector3[]);
	}
	
	public static Prism getPrismFromPoints (params IntVector3[] points)
	{
		//returns the smallest prism containing all points
		IntVector3 lowest = new IntVector3 (0, 0, 0);
		IntVector3 highest = new IntVector3 (0, 0, 0);
		
		bool firstRun = true;
		foreach (IntVector3 point in points) {
			if (firstRun) {
				firstRun = false;
				lowest = point;
				highest = point;	
				continue;
			}
			
			if (point.x < lowest.x)
				lowest.x = point.x;
			if (point.y < lowest.y)
				lowest.y = point.y;
			if (point.z < lowest.z)
				lowest.z = point.z;
			
			if (point.x > highest.x)
				highest.x = point.x;
			if (point.y > highest.y)
				highest.y = point.y;
			if (point.z > highest.z)
				highest.z = point.z;
		}
		
		int width = Mathf.Abs (highest.x - lowest.x) + 1;
		int height = Mathf.Abs (highest.y - lowest.y) + 1;
		int depth = Mathf.Abs (highest.z - lowest.z) + 1;
		
		Prism prism = new Prism (lowest.x, lowest.y, lowest.z, width, height, depth);
		return prism;
	}
	
	public static IntVector3 getRandomPointFromFaces (Prism prism)
	{
		//given a prism, returns a point chosen randomly from all points on the prism faces
		
		IntVector3 direction = Angles.getRandomDirection ();
		
		int i = Random.Range (0, prism.width);
		int j = Random.Range (0, prism.height);
		int k = Random.Range (0, prism.depth);
		
		if (direction.x != 0)
			i = direction.x == 1 ? 0 : prism.width - 1;
		if (direction.y != 0)
			j = direction.y == 1 ? 0 : prism.height - 1;
		if (direction.z != 0)
			k = direction.z == 1 ? 0 : prism.depth - 1;
		
		return new IntVector3 (prism.x + i, prism.y + j, prism.z + k);
		
	}
	
	public static void shuffleArraylist (ArrayList alist)
	{
		for (int i=0; i<alist.Count; i++) {
			var temp = alist [i];
			int randomIndex = Random.Range (0, alist.Count);
			alist [i] = alist [randomIndex];
			alist [randomIndex] = temp;
		}
	}
	
	public static float evaluateProbabilityCurve (AnimationCurve curve, int sliceCount)
	{
		/*
		allows you to use animationCurves as probability curves.
		
		uses domain 0-1
		any range
		
		slices the domain into sliceCount pieces. the odds of returning the x value from a slice
		is linearly proportional to the y value on the curve
		*/
		float total = 0;
		float stepSize = 1 / (float)sliceCount;
		for (float x=0; x<=1; x+=stepSize) {
			total += curve.Evaluate (x);
		}
		
		float rand = ((float)Random.Range (0, total * 1000)) / 1000;
		for (float x=0; x<=1; x+=stepSize) {
			float y = curve.Evaluate (x);
			if (y > 0)
				rand -= y;
			
			if (rand < 0)
				return x;
		}
		
		Debug.Log ("warning: evaluateProbabilityCurve never evaluated. returning 1");
		return 1f;
	}
	
	public static float truncate (float val, float min, float max)
	{
		if (val < min)
			return min;
		if (val > max)
			return max;
		
		return val;
			
	}
	
	public static ArrayList getGridSquares (IntVector2 areaSize, IntVector2 cellSize, int count, int spacing)
	{
		/*
		where area is at 0,0
		where cell is the size of each cell
		
		returns an array of points that are evenly distributed over the surface of the area square.
		handles cases where there aren't enough cells to fill area, or there are too many.
		the points returned are coordinates for all the cell squares.
		*/
		
		int xCells = areaSize.x / cellSize.x;
		int yCells = areaSize.y / cellSize.y;
		
		if (xCells * yCells < count)
			return getGridOversizeSquares (areaSize, cellSize, count, spacing);
		return getGridUndersizeSquares (areaSize, cellSize, count);
		
	}
	
	private static ArrayList getGridOversizeSquares (IntVector2 areaSize, IntVector2 cellSize, int count, int spacing)
	{
		/*
		when not all cells will fit inside area. returns a tightly packed list of points
		centered on the area square.
		
		spacing can add padding separating each cell
		*/
		
		IntVector2 realCellSize=new IntVector2 (cellSize.x+spacing,cellSize.y+spacing);
		
		//calculate how many rows and columns we want to use
		Vector2 cellCount = new Vector2 (areaSize.x / realCellSize.x,
		areaSize.y / realCellSize.y);
		bool increaseX = true;
		while (cellCount.x*cellCount.y<count) {
			if (increaseX) {
				cellCount.x++;
			} else {
				cellCount.y++;
			}
			increaseX = !increaseX;
		}
		
		int xOffset = -(int)(realCellSize.x * cellCount.x - areaSize.x) / 2;
		int yOffset = -(int)(realCellSize.y * cellCount.y - areaSize.y) / 2;
		IntVector2 offset = new IntVector2 (xOffset, yOffset);
		
		//randomly select which cells will be filled, out of all possible cell locations
		ArrayList cellPoints = getRandomIntegerPoints (cellCount, count);
		
		ArrayList points = new ArrayList ();
		foreach (IntVector2 cellPoint in cellPoints) {
			int x = cellPoint.x * realCellSize.x + offset.x;
			int y = cellPoint.y * realCellSize.y + offset.y;
			points.Add (new IntVector2 (x, y));
		}
		
		return points;
	}
	
	private static ArrayList getGridUndersizeSquares (IntVector2 areaSize, IntVector2 cellSize, int count)
	{
		/*
		when count number of cells will fit inside area. returns a list of intvector2 points 
		for all cell coordinates.
		*/
		
		//calculate how many rows and columns we want to use
		Vector2 iterVec = new Vector2 (areaSize.x / cellSize.x,
		areaSize.y / cellSize.y);
		Vector2 cellCount = iterVec;
		bool decreaseX = true;
		while (iterVec.x*iterVec.y>=count) {
			cellCount = iterVec;
			if (decreaseX) {
				iterVec.x--;
			} else {
				iterVec.y--;
			}
			decreaseX = !decreaseX;
		}
		
		//randomly select which cells will be filled, out of all possible cell locations
		ArrayList cellPoints = getRandomIntegerPoints (cellCount, count);
		
		//fill returned arraylist
		ArrayList points = new ArrayList ();
		Vector2 offset = new Vector2 (Mathf.CeilToInt (areaSize.x / (cellCount.x + 1)),
			Mathf.CeilToInt (areaSize.y / (cellCount.y + 1)));
		foreach (IntVector2 cellPoint in cellPoints) {				
			int x = (cellPoint.x + 1) * (int)offset.x;
			int y = (cellPoint.y + 1) * (int)offset.y;
			points.Add (new IntVector2 (x, y));
		}
		
		return points;
	}
	
	private static ArrayList getRandomIntegerPoints (Vector2 cellCount, int count)
	{
		/*
		returns a random collection of unique points of size count:
		with integer coordinates
		with domain [0,cellCount.x[
		with range [0,cellCount.y[
		*/
		
		ArrayList cellPoints = new ArrayList ();
		for (int x=0; x<cellCount.x; x++) {
			for (int y=0; y<cellCount.y; y++) {
				cellPoints.Add (new IntVector2 (x, y));
			}
		}
		if (cellPoints.Count > count) {
			//ZTools.shuffleArraylist (cellPoints);
			cellPoints.RemoveRange (count, cellPoints.Count - count);
		}
		
		return cellPoints;
	}
	
}








