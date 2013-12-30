using UnityEngine;
using System.Collections;

//this class stores all block information that does not change during gameplay.

public class BlockData
{
    public string name;
	public int blockCode = 0;
	public Texture texture;

    public bool isRotationRandom = false;

    public BlockData(string name, int blockCode, Texture texture)
    {
        this.name = name;
        this.blockCode = blockCode;
        this.texture = texture;
    }
}
