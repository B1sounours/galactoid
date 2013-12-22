using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{
	public BlockData blockData;
    private IntVector3 position;
    private GameObject blockPrefab;
    private GameObject blockGameObject;

    public void initialize(BlockData blockData, GameObject blockGameObject, IntVector3 position)
	{
        this.position = position;
		setBlockData (blockData);
        this.blockGameObject = blockGameObject;
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
        this.name = "Block (" + position.x + "," + position.y + "," + position.z + ")";
	}
	
	private void setBlockData (BlockData blockData)
	{
		this.blockData = blockData;
        renderer.material.mainTexture = blockData.texture;
	}

    public void remove()
    {
        Destroy(blockGameObject);
    }
}

