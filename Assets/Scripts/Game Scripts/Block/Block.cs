using UnityEngine;
using System.Collections;

//this class changes its parent block gameobject. it is its only component.

public class Block : MonoBehaviour
{
	public BlockData blockData;
    public BlockStatus blockStatus;
    private GameObject blockPrefab;
    private GameObject blockGameObject;

    public void initialize(BlockData blockData, BlockStatus blockStatus)
	{
        this.blockStatus = blockStatus;
		setBlockData (blockData);
		setName ();
	}

    public GameObject getBlockPrefab()
    {
        if (blockPrefab == null)
            blockPrefab = Resources.Load(ResourcePaths.blockPrefab) as GameObject;

        return blockPrefab;
    }

	public void setName ()
	{
        this.name = "Block (" + blockStatus.position.x + "," + blockStatus.position.y + "," + blockStatus.position.z + ")";
	}
	
	private void setBlockData (BlockData blockData)
	{
		this.blockData = blockData;
        renderer.material.mainTexture = blockData.texture;
	}
}

