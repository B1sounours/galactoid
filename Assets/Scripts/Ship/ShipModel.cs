using UnityEngine;
using System.Collections;

//This class stores all ship data. All data is abstract, it makes no references to gameobjects.

[System.Serializable]
public class ShipModel
{
	public BlockData[,,] blockDatas;
    public IntVector3 shipSize;
    private ShipInfo shipInfo;

    public ShipModel()
	{
        shipSize = new IntVector3(GameConstants.maxShipDimension, GameConstants.maxShipDimension, GameConstants.maxShipDimension);
        blockDatas = new BlockData[shipSize.x, shipSize.y, shipSize.z];
        shipInfo = new ShipInfo(this);
	}
	
	public void removeBlock (IntVector3 point)
	{
        blockDatas[point.x, point.y, point.z] = null;
	}
	
	public void createBlock (BlockData blockData, IntVector3 position)
	{
        blockData.position = new IntVector3(position.x, position.y, position.z) ;
        blockDatas[position.x, position.y, position.z] = blockData;
	}
	
}