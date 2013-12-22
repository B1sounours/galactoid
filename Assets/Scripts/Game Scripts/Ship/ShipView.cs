using UnityEngine;
using System.Collections;

//This class views data in a shipModel

public class ShipView
{
    ShipModel sm;

    public ShipView(ShipModel sm)
    {
        this.sm = sm;
    }

    public bool isBlockOccupied(IntVector3 point)
    {
        return !(sm.blocks[point.x, point.y, point.z] == null);
    }

    public bool isInsideArray(IntVector3 point)
    {
        return ((point.x >= 0 && point.x < sm.shipSize.x) &&
            (point.x >= 0 && point.y < sm.shipSize.y) &&
            (point.x >= 0 && point.z < sm.shipSize.z));
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