using UnityEngine;
using System.Collections;

//this is a container for all non-unity data associated to one block (no gameobjects)

[System.Serializable]
public class BlockData
{
    public string textureFilename;
	public int blockCode = 0;
    public IntVector3 position;

    public bool isRotationRandom = false;

    public BlockData(string textureFilename, int blockCode, IntVector3 position)
    {
        this.textureFilename = textureFilename;
        this.blockCode = blockCode;
        this.position = position;
    }

    public BlockData getCopy()
    {
        return new BlockData(textureFilename, blockCode, new IntVector3(position.x,position.y,position.z));
    }
}
