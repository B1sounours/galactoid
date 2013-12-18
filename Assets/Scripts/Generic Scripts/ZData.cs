
public struct IntVector2
{
	public int x;
	public int y;
		
	public IntVector2 (int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	
	public static bool operator == (IntVector2 vec1, IntVector2 vec2)
	{
		return vec1.x==vec2.x && vec1.y==vec2.y;
	}
	
		public static bool operator != (IntVector2 vec1, IntVector2 vec2)
	{
		return !(vec1==vec2);
	}
}

[System.Serializable]
public struct IntVector3
{
	public int x;
	public int y;
	public int z;
		
	public IntVector3 (int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}
	
	public bool isOrthogonalUnitVector ()
	{
		//checks that vector3 is an orthogonal unit vector		
		if (x * x + y * y + z * z != 1)
			return false;
		
		return ((x == 0 && y == 0) || (x == 0 && z == 0) || (y == 0 && z == 0));
	}
	
	public static IntVector3 operator * (IntVector3 a, int d)
	{
		return new IntVector3 (a.x * d, a.y * d, a.z * d);
	}
	
	public static IntVector3 operator * (IntVector3 a, float d)
	{
		return new IntVector3 ((int)(a.x * d), (int)(a.y * d), (int)(a.z * d));
	}
	
	public static IntVector3 operator + (IntVector3 vec1, IntVector3 vec2)
	{
		return new IntVector3 (vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z);
	}
	
}

public struct Square
{
	public int x;
	public int y;
	public int width;
	public int height;
	
	public Square (int x, int y, int width, int depth)
	{
		this.x = x;
		this.y = y;
		
		this.width = width;
		this.height = depth;
	}
	
	public IntVector3 getPoint ()
	{
		return new IntVector3 (x, 0, y);
	}
	
	public int getArea(){
		return width*height;
	}
}

public class Prism
{
	public int x;
	public int y;
	public int z;
	public int width;
	public int height;
	public int depth;
	public IntVector3 position;
	public int volume;
	public int baseArea;
	
	public Prism (int x, int y, int z, int width, int height, int depth)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		
		position = new IntVector3 (x, y, z);
		
		this.width = width;
		this.height = height;
		this.depth = depth;
		
		volume = width * height * depth;
		baseArea = width * depth;
	}
	
	public bool isPrismEqual (Prism prism)
	{
		if (x != prism.x || y != prism.y || z != prism.z)
			return false;
		
		if (width != prism.width || height != prism.height || depth != prism.depth)
			return false;
		
		return true;
	}
	
	public IntVector3 getPosition ()
	{
		return new IntVector3 (x, y, z);
	}
	
	public IntVector3 getLowestPoint ()
	{
		return new IntVector3 (x, y, z);
	}
	
	public IntVector3 getHighestPoint ()
	{
		return new IntVector3 (x + width, y + height, z + depth);
	}
}
