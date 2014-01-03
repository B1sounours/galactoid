using UnityEngine;
using System.Collections;

//this class is for player inventory. it stores a quantity of blocks of the same blockdata and blockstatus

public class BlockStack {

    public BlockData blockData;
    public float quantity;
    public Texture blockTexture;

    public BlockStack(BlockData blockData, float quantity)
    {
        this.blockData = blockData;
        this.quantity = quantity;
        blockTexture = ResourceLookup.getBlockTexture(blockData.textureFilename);
    }
}
