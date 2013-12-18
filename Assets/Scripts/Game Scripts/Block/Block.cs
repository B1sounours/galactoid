using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{
	public BlockData blockData;
	private int textureIndex;
    private IntVector3 position;
    private GameObject blockPrefab;

    public void initialize(BlockData blockData, IntVector3 position)
	{
        this.position = position;
        textureIndex = 0;
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
        this.name = "Block (" + position.x + "," + position.y + "," + position.z + ")";
	}
	
	private void setBlockData (BlockData blockData)
	{
		this.blockData = blockData;
        renderer.material.mainTexture = blockData.textures[textureIndex];
	}

    public void remove()
    {
    }
}

