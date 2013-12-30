using UnityEngine;
using System.Collections;

//this class stores all block information that can change during gameplay.

public class BlockStatus {
    public IntVector3 position;

    public BlockStatus()
    {
        this.position = new IntVector3(0,0,0);
    }

    public BlockStatus(IntVector3 position)
    {
        this.position = position;
    }
}
