using UnityEngine;
using System.Collections;

//this class is for player inventory. it stores a quantity of blocks of the same blockdata and blockstatus

public class BlockStack {

    public BlockData blockData;
    public BlockStatus blockStatus;
    public float quantity;

    public BlockStack(BlockData blockData, BlockStatus blockStatus, float quantity)
    {
        this.blockData = blockData;
        this.blockStatus = blockStatus;
        this.quantity = quantity;
    }
}
