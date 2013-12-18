using UnityEngine;
using System.Collections;

public class CollisionManager
{
	/*
	rebuilds mapData, voxelGrid, viroidManager.contagiousList, and re-calculates blob offsets.		
	*/
	
	public bool isDone;
	private bool isStarted;
	public bool isBlob2InsideBlob1;
	public int[,,]newMapData;
	private GameObject[,,]newVoxelGrid;
	private ArrayList newContagiousList;
	private Prism iterPrism;
	private Prism combinedPrism;
	private IntVector3 iterOffset;
	private string blobName1;
	private string blobName2;
	public BlobManager blob1;
	public BlobManager blob2;
	private BlobManager newBlobManager;
	private WorldManager worldManager;
	private int subtaskCounter;
	private ArrayList blobs;
	
	public CollisionManager (WorldManager worldManager, string blobName2, BlobManager blobManager)
	{
		//make blobManager null when we just want to put blob2 contents into blob1
		this.worldManager = worldManager;
		
		blobName1 = "main blob";
		this.blobName2 = blobName2;
		this.newBlobManager = blobManager;
		isBlob2InsideBlob1 = newBlobManager == null;
		
		isDone = false;
		isStarted = false;
		subtaskCounter = 0;
	}
	
	private void setStart ()
	{
		//only one collisionManager should be in progress at the same time for the whole scene
		isStarted = true;
		
		blobs = new ArrayList ();
		
		if (blobName1 == "main blob")
			blobName1 = worldManager.getMainBlobName ();
		blob1 = (BlobManager)worldManager.blobs [blobName1];
		blob2 = (BlobManager)worldManager.blobs [blobName2];
		
		blob1.setName (true);
		blob2.setName (true);
		if (newBlobManager != null)
			newBlobManager.setName (true);
		
		blobs.Add (blob2);
		
		if (isBlob2InsideBlob1) {
			//case: when blob2 collided with blob1, it was completely within blob1 bounds
			//Debug.Log ("inside collision");
			newBlobManager = blob1;
			
			iterPrism = blob2.genPrism ();
			combinedPrism = blob1.getPrism ();
			
			iterOffset = new IntVector3 (iterPrism.x - combinedPrism.x, iterPrism.y - combinedPrism.y, iterPrism.z - combinedPrism.z);			
			
			//ZTools.show ("combinedPrism ", combinedPrism);
			newMapData = blob1.mapData;
			newVoxelGrid = blob1.voxels;
			newContagiousList = blob1.getContagiousList ();
		} else {
			//case: blob2 hit the outside of blob1. newblob bounds must become larger
			//Debug.Log ("outside collision");
			blobs.Add (blob1);
			iterPrism = getPrismFromBlobs (blob1, blob2);
			iterOffset = new IntVector3 (0, 0, 0);
			
			combinedPrism = iterPrism;
			newMapData = new int[iterPrism.width, iterPrism.height, iterPrism.depth];
			newVoxelGrid = new GameObject[iterPrism.width, iterPrism.height, iterPrism.depth];
			newContagiousList = new ArrayList ();
			newBlobManager.gameObject.transform.position = new Vector3 (iterPrism.x, iterPrism.y, iterPrism.z);
		}
		//ZTools.show("combined prism",combinedPrism);
	}
	
	public BlobManager getBlobManager ()
	{
		blob1.setName (false);
		blob2.setName (false);
		newBlobManager.setName (false);
		
		newBlobManager.setContagiousList (newContagiousList);
		newBlobManager.setVoxelGrid (newVoxelGrid, false);
		newBlobManager.setMapData (newMapData);
		
		if (!isBlob2InsideBlob1)
			newBlobManager.applyActions (blob1);
		newBlobManager.applyActions (blob2);
		newBlobManager.isColliding = false;
		
		return newBlobManager;
	}
	
	public void runSubtask ()
	{
		if (! isStarted)
			setStart ();
		
		int stepStart = subtaskCounter * GameSettings.subtaskStepsize;
		for (int index=stepStart; index< stepStart+ GameSettings.subtaskStepsize; index++) {
			int i = index / (iterPrism.height * iterPrism.depth);
			
			int index2 = index - i * (iterPrism.height * iterPrism.depth);
			int j = index2 / iterPrism.depth;
			int k = index - i * (iterPrism.height * iterPrism.depth) - j * iterPrism.depth;
			
			if (i >= iterPrism.width) {
				isDone = true;
				break;
			}
			//future optimization: instead of iterating over all points, iterate over all voxels in blob
			//and get points
			
			//Debug.Log ("k: " + k + " iterOffset.k: " + iterOffset.k);
			IntVector3 point = new IntVector3 (i + iterOffset.x, j + iterOffset.y, k + iterOffset.z);
			setPoint (point);
		}	
		subtaskCounter++;
	}
	
	private void setPoint (IntVector3 point)
	{
		//Debug.Log ("setPoint for " + i + " " + j + " " + k);
		bool alreadySet;
		foreach (BlobManager blob in blobs) {
			alreadySet = false;
			Prism prism = blob.getPrism ();
			
			IntVector3 blobPoint = new IntVector3 (point.x - prism.x + combinedPrism.x,
			point.y - prism.y + combinedPrism.y,
			point.z - prism.z + combinedPrism.z);
			//Debug.Log ("kBlob: "+kBlob+" k: "+k+" p.k: "+prism.k+" iterP.k: "+iterPrism.k+" combP.k: "+combinedPrism.k+" iterO.k: "+iterOffset.k);
			if (!blob.isInsideBlob (blobPoint))
				continue;
				
			if (alreadySet) {
				Debug.Log ("warning: setPoint found two voxels at same spot. destroying one.");
				blob.setDestroyed (blobPoint);
			} else {
				if (MDView.isVoxelOccupied(blob.mapData,blobPoint)){
					alreadySet = true;
					GameObject voxelManager = blob.voxels [blobPoint.x,blobPoint.y,blobPoint.z];
					newVoxelGrid [point.x,point.y,point.z] = voxelManager;
						
					if (voxelManager != null) {
						voxelManager.GetComponent<VoxelManager> ().reassign (newBlobManager, point);
						newMapData [point.x,point.y,point.z] = blob.mapData [blobPoint.x,blobPoint.y,blobPoint.z];
						if (blob.isCorrupt (blobPoint))
							newContagiousList.Add (point);
					}
				}
			}
			
		}
	}
	
	public static Prism getPrismFromBlobs (BlobManager blob1, BlobManager blob2)
	{
		/*
		returns a Prism that just barely contains all of blob1 and blob2
		*/
		Prism p1 = blob1.genPrism ();
		Prism p2 = blob2.genPrism ();
		
		int i = Mathf.Min (p1.x, p2.x);
		int j = Mathf.Min (p1.y, p2.y);
		int k = Mathf.Min (p1.z, p2.z);
		
		int iCorner = Mathf.Max (p1.x + p1.width, p2.x + p2.width);
		int jCorner = Mathf.Max (p1.y + p1.height, p2.y + p2.height);
		int kCorner = Mathf.Max (p1.z + p1.depth, p2.z + p2.depth);
		
		int width = iCorner - i;
		int height = jCorner - j;
		int depth = kCorner - k;
		
		return new Prism (i, j, k, width, height, depth);
	}	
	
}