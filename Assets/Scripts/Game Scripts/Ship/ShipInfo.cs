using UnityEngine;
using System.Collections;

//This class provides information and analysis about data found in a shipmodel

public class ShipInfo
{
    ShipModel shipModel;

    public ShipInfo(ShipModel shipModel)
    {
        this.shipModel = shipModel;
    }

    public bool isBlockOccupied(IntVector3 point)
    {
        return !(shipModel.blocks[point.x, point.y, point.z] == null);
    }

    public bool isInsideArray(IntVector3 point)
    {
        return ((point.x >= 0 && point.x < shipModel.shipSize.x) &&
            (point.x >= 0 && point.y < shipModel.shipSize.y) &&
            (point.x >= 0 && point.z < shipModel.shipSize.z));
    }

    public ArrayList getBlockInventory()
    {
        //returns an arraylist of all blockstacks available for the player to place
        ArrayList inventory = new ArrayList();
        for (int i = 0; i < 100; i++){
            BlockData bd = BlockDataLookup.getBlockDataByCode(i);
            BlockStatus bs = new BlockStatus();
            inventory.Add(new BlockStack(bd,bs,10));
        }

        return inventory;
    }

}