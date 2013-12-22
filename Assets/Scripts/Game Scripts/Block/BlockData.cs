using UnityEngine;
using System.Collections;

//this class stores block information that will never change during gameplay.

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
