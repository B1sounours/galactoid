using UnityEngine;
using System.Collections;

public class VoxelManager : MonoBehaviour
{
	public class VoxelStatus
	{
		public ViroidSpecies viroidSpecies;
		public bool corrupt = false;
		public float infection;
		public float damage = 0;
	}
	public VoxelStatus status;
	public VoxelData voxelData;
	public GameObject tumor;
	private int textureIndex;
	private IntVector3 startPosition;
	private IntVector3 lastStartPosition;
	private BlobManager blobOwner;
	private BlobManager lastBlobOwner;
	
	public void initialize (VoxelData voxelData, BlobManager blobOwner, IntVector3 startPosition)
	{
		this.blobOwner = blobOwner;
		this.startPosition = startPosition;
		textureIndex = 0;
		status = new VoxelStatus ();
		setVoxelData (voxelData);
		setName ();
	}
	
	public void setName ()
	{
		this.name = "Voxel (" + startPosition.x + "," + startPosition.y + "," + startPosition.z + ")";
	}
	
	public void reassign (BlobManager newBlobOwner, IntVector3 newStartPosition)
	{
		lastBlobOwner = blobOwner;
		lastStartPosition = startPosition;
		
		blobOwner = newBlobOwner;
		startPosition = newStartPosition;
		setName ();
		transform.parent = blobOwner.transform;
	}
	
	public void setStartPosition (IntVector3 StartPosition)
	{
		startPosition = StartPosition;
	}
	
	public void setCorrupt ()
	{
		//Debug.Log("normal voxel setCorrupt "+startPosition.i+" "+startPosition.j+" "+startPosition.k);
		status.corrupt = true;
		//renderer.material.color = Color.red;
		GameObject myTumor = Instantiate (tumor, transform.position, Quaternion.identity) as GameObject;
		myTumor.transform.eulerAngles = Angles.getRandom ();
		myTumor.transform.parent = transform;
		//myTumor.transform.localPosition = new Vector3(0f, -0.05f, 0.1f);
	}
	
	public bool isCorrupt ()
	{
		return status.corrupt;
	}
	
	private int getNewTextureIndex ()
	{
		int count = voxelData.textures.GetLength (0);
		float stepSize = voxelData.strength / count;
		int index = Mathf.FloorToInt (status.damage / stepSize);
		if (index >= count)
			index = count - 1;
		
		return index;
	}
	
	public void takeDamage (float amount)
	{
		if (status.damage < voxelData.strength) {
			status.damage += amount;
			int newIndex = getNewTextureIndex ();
			if (newIndex != textureIndex) {
				textureIndex = newIndex;
				renderer.material.mainTexture = voxelData.textures [textureIndex];
			}
		} else {
			setBroken ();
		}
	}
	
	public BlobManager getBlobManagerOwner ()
	{
		return blobOwner;
	}
	
	public void setBroken ()
	{
		if (blobOwner.isInitialized) {
			blobOwner.setBroken (startPosition);
		} else {
			lastBlobOwner.setBroken (lastStartPosition);
		}
	}
	
	private void setVoxelData (VoxelData voxelData)
	{
		this.voxelData = voxelData;
		textureIndex = getNewTextureIndex ();
		renderer.material.mainTexture = voxelData.textures [textureIndex];
	}
}

