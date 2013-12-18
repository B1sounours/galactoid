using UnityEngine;
using System.Collections;

/*

BlobData is meant to be exactly what WorldManager.genBlob needs to generate a blob.
No more, no less.

*/
public class BlobData : MonoBehaviour
{

	//the direction the blob will be moving in order to hit the main blob
	public Directions.Cardinal moveDirection = Directions.Cardinal.north;
	public float speed = 2;
	
	public enum GeneratorTypes
	{
		platform,
		lilypad,
		junk
	}
	public GeneratorTypes generatorType = GeneratorTypes.lilypad;
	
	[System.Serializable]
	public class BlobOffset
	{
		public int x = 0;
		public int y = 0;
		public bool guarenteeHit = false;
	}
	public BlobOffset offset = new BlobOffset ();
	
	
	[System.Serializable]
	public class BlobSize
	{
		public int width = 20;
		public int height = 20;
		public int depth = 20;
	}
	public BlobSize size = new BlobSize ();
	
	//public IntVector3 size;
	
	[System.Serializable]
	public class ViroidData
	{
		//what percent of voxels start infected  0 to 1
		public float exposure = 0;
		//0 to 1. redistributes exposure to be more likely at this height percentage. 1 is top.
		public float heightPreference = 1;
		//does infection spread while platform has no collided yet
		public bool spreadWhileArriving = false;
	}
	public ViroidData viroid;
	private int[,,] mapData;
	
	public Directions.Cardinal getMoveDirection ()
	{
		return moveDirection;
	}
	
	public float getDriftTime ()
	{
		//the length of time the blob expects to be drifting before its collision
		//presuming it collides
		return (float)GameSettings.blobSpawnDistance / (float)speed;
	}
	
	public bool viroidInfectRoll (int height)
	{
		//rolls a dice whether it should infect that block.
		float heightRatio = (float)height / (float)getSizeVector ().y;
		float multiplier = 5 - Mathf.Abs (heightRatio - viroid.heightPreference) * 10;
		if (multiplier < 0)
			return false;
		float divisor = viroid.exposure * multiplier;
		return Random.Range (0, (int)(1 / divisor)) == 0;
	}
	
	public void setOffset(IntVector2 newOffset){
		offset.x=newOffset.x;
		offset.y=newOffset.y;
	}
	
	public void setMoveDirection (Directions.Cardinal newDirection)
	{
		moveDirection = newDirection;
	}
	
	public IntVector3 getSizeVector ()
	{
		return new IntVector3 (size.width,size.height,size.depth);
		//return new IntVector3 (getMapData ().GetLength (0), getMapData ().GetLength (1), getMapData ().GetLength (2));
	}
	
	public int[,,] getMapData ()
	{
		if (mapData != null)
			return mapData;
		mapData = genMapData ();
		return mapData;
	}
	
	private VoxelTheme getVoxelTheme ()
	{
		VoxelDataManager vdm = (VoxelDataManager)FindObjectOfType (typeof(VoxelDataManager));
		return vdm.getRandomTheme ();
	}
	
	public IntVector3 getSize ()
	{
		return new IntVector3 (size.width, size.height, size.depth);
	}
	
	public void setSize (IntVector3 newSize)
	{
		size.width = newSize.x;
		size.height = newSize.y;
		size.depth = newSize.z;
	}
	
	public int[,,] genMapData ()
	{
		if (generatorType == GeneratorTypes.platform)
			return PlatformGenerator.gen (getVoxelTheme (), getSize ());
		
		if (generatorType == GeneratorTypes.lilypad) 	
			return RaisedPlatformGenerator.gen (getVoxelTheme (), getSize ());
		
		if (generatorType == GeneratorTypes.junk)
			return JunkGenerator.gen (getVoxelTheme (), getSize ());
		
		return JunkGenerator.gen (getVoxelTheme (), getSize ());
	}
	
	public IntVector3 getMoveDirectionUnitVector ()
	{
		return Directions.getDirectionUnitVector (moveDirection);
	}
	
	public Vector3 getVelocity ()
	{
		IntVector3 vec = getMoveDirectionUnitVector ();
		float i = vec.x * speed;
		float j = vec.y * speed;
		float k = vec.z * speed;
		Vector3 velocity = new Vector3 (i, j, k);
		
		return velocity;
	}

	public static BlobData getDefaultBlobData ()
	{
		return getBlobData ("default");
	}
	
	public static BlobData getBlobData (string blobFileName)
	{
		GameObject prefab = Resources.Load ("Prefabs/Blobs/" + blobFileName) as GameObject;
		
		if (prefab == null && blobFileName != "default") {
			Debug.Log ("warning: default BlobData. failed to find " + blobFileName);
			return getBlobData ("default");	
		}
		
		WorldManager worldManager = (WorldManager)FindObjectOfType (typeof(WorldManager));
		return worldManager.getBlobDataFromPrefab (prefab);
	}
	
}
