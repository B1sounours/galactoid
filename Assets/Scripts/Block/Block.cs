using UnityEngine;
using System.Collections;

//this is the only component for a block game object. it allows ship3DView to make visual changes to the block.

public class Block : MonoBehaviour
{
    public BlockData blockData;

    public void initialize(BlockData blockData)
    {
        setBlockData(blockData);
        setName();
    }

    public void setName()
    {
        this.name = "Block (" + blockData.position.x + "," + blockData.position.y + "," + blockData.position.z + ")";
    }

    private void setBlockData(BlockData blockData)
    {
        this.blockData = blockData;
        renderer.material.mainTexture = ResourceLookup.getBlockTexture(blockData.textureFilename);
    }
}

