using UnityEngine;
using System.Collections;

public class ViroidManager
{
	public ArrayList contagiousList = new ArrayList ();
	private BlobManager blobManager;
	
	public ViroidManager (BlobManager BlobManager)
	{		
		blobManager = BlobManager;
	}
	
	public void runHeartbeat ()
	{
		cleanCorruptList ();
		iterateCorruptList ();
	}
	
	public bool isQuarantined(){
		return contagiousList.Count==0;
	}
	
	public void addContagious (IntVector3 point)
	{
		contagiousList.Add (point);
	}
	
	private void cleanCorruptList ()
	{
		for (int index=0; index<contagiousList.Count; index++) {
			IntVector3 point = (IntVector3)contagiousList [index];
			if (! blobManager.isContagious (point)) {
				contagiousList.RemoveAt (index);
				index--;
			}
		}
	}
	
	private void iterateCorruptList ()
	{
		//print ("len clist "+contagiousList.Count );
		for (int index=0; index<contagiousList.Count; index++) {
			IntVector3 vector = (IntVector3)contagiousList [index];
			
			if (Random.Range (0, 8) == 0) {
				infectRandom (vector);
			}
		}
	}
	
	private void infectRandom (IntVector3 point)
	{
		if (! blobManager.isInsideBlob (point))
			return;
		if (! blobManager.isCorrupt (point))
			return;
		
		ArrayList candidateList = new ArrayList ();
		
		foreach (IntVector3 iterPoint in MDView.getNeighbors(blobManager.mapData,point)) {
			if (! blobManager.isCorrupt (iterPoint))
				candidateList.Add (iterPoint);
		}
		
		if (candidateList.Count > 0) {
			int index = Random.Range (0, candidateList.Count);
			IntVector3 voxelPoint = (IntVector3)candidateList [index];
		
			blobManager.setCorrupt (voxelPoint);
		}
	}
	
}
