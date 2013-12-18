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
        return !sm.blocks[point.x, point.y, point.z] == null;
    }

    public bool isInsideArray(IntVector3 point)
    {
        return ((point.x >= 0 && point.x < sm.shipSize.x) &&
            (point.x >= 0 && point.y < sm.shipSize.y) &&
            (point.x >= 0 && point.z < sm.shipSize.z));
    }

}