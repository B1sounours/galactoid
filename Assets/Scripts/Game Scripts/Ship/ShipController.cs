using UnityEngine;
using System.Collections;

//This class contains the "business logic" functions and updates shipModel and ship3DView appropriately

public class ShipController
{
    public ShipModel shipModel;
    public Ship3DView ship3DView;
    public ShipInfo shipInfo;

    public ShipController()
    {
        shipModel = new ShipModel();
        shipInfo = new ShipInfo(shipModel);

        GameObject go = new GameObject("Ship3DView");
        ship3DView = go.AddComponent<Ship3DView>();

        ship3DView.shipModel = shipModel;
    }


    public void removeBlock(IntVector3 point)
    {
        if (!shipInfo.isBlockOccupied(point))
        {
            Debug.Log("removeBlock got unoccupied request " + ZDebug.toString(point));
            return;
        }

        ship3DView.removeBlock(point);
        shipModel.removeBlock(point);
    }

    public void createBlock(int blockCode, IntVector3 position)
    {
        if (!shipInfo.isInsideArray(position))
        {
            Debug.Log("aborted createBlock. bad coordinates: " + ZDebug.toString(position));
            return;
        }
        if (shipInfo.isBlockOccupied(position))
        {
            Debug.Log("aborted createBlock. occupied coordinates: " + ZDebug.toString(position));
            return;
        }

        BlockData blockData = BlockDataLookup.getBlockDataByCode(blockCode);
        Block block = ship3DView.createBlock(blockData, position);
        shipModel.createBlock(block, blockData, position);
    }


}
