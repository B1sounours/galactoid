using UnityEngine;
using System.Collections;

//This class stores all ship data

public class ShipModel
{
	public Block[,,] blocks;
    public IntVector3 shipSize;
    public ShipInfo shipInfo;

    public ShipModel()
	{
        shipSize = new IntVector3(GameConstants.maxShipDimension, GameConstants.maxShipDimension, GameConstants.maxShipDimension);
        blocks=new Block[shipSize.x,shipSize.y,shipSize.z];
        shipInfo = new ShipInfo(this);
	}
	
	public void removeBlock (IntVector3 point)
	{
		Block block = (Block)blocks [point.x, point.y, point.z];
		blocks [point.x, point.y, point.z] = null;
	}
	
	public void createBlock (Block block, BlockData blockData,IntVector3 position)
	{
        block.initialize(blockData, new BlockStatus(position));
        blocks[position.x, position.y, position.z] = block;
	}
	
}