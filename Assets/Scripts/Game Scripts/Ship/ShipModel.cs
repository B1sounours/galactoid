using UnityEngine;
using System.Collections;

//This class stores and manipulates all ship data

public class ShipModel : MonoBehaviour
{
	public string blobName;
	public Block[,,] blocks;
    public IntVector3 shipSize;
    private ShipView sv;
	
	void Awake ()
	{
        shipSize = new IntVector3(100, 100, 100);
        blocks=new Block[shipSize.x,shipSize.y,shipSize.z];
        sv = new ShipView(this);
	}
	
	void Update ()
	{
	}
	
	public void removeBlock (IntVector3 point)
	{
		if (! sv.isBlockOccupied (point)) {
			Debug.Log ("removeBlock got unoccupied request " + ZDebug.toString (point));
			return;
		}

		Block block = (Block)blocks [point.x, point.y, point.z];
        block.remove();
		blocks [point.x, point.y, point.z] = null;
	}
	
	public void createBlock (int blockCode, IntVector3 point)
	{
		//Debug.Log ("create block: i: " + i + " j: " + j + " k: " + k);
		if (! sv.isInsideArray (point)) {
			Debug.Log ("aborted createBlock. bad coordinates: " + ZDebug.toString (point));
			return;
		}
        if (sv.isBlockOccupied(point))
        {
            Debug.Log("aborted createBlock. occupied coordinates: " + ZDebug.toString(point));
            return;
        }


        GameObject blockGO = Instantiate(ResourceLookup.getBlockPrefab(), point.getVector3(), Quaternion.identity) as GameObject;
		BlockData blockData = ResourceLookup.getBlockDataByCode (blockCode);
        Block block = blockGO.GetComponent<Block>();
        block.initialize (blockData, blockGO,point);
		
		if (blockData.isRotationRandom) {
			blockGO.transform.eulerAngles = Angles.getRandom ();
		} else {
			blockGO.transform.eulerAngles = Angles.getFlat ();
		}
		blockGO.transform.parent = transform;
		blocks [point.x, point.y, point.z] = block;
	}
	
}