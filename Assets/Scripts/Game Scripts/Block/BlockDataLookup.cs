using UnityEngine;
using System.Collections;

public static class BlockDataLookup
{
    private static Hashtable blockDatas;

    static private void setBlockDatas(){
        blockDatas=new Hashtable();

        int blockCode = 0;
        foreach (Texture2D texture in Resources.LoadAll(ResourcePaths.blockTextures,typeof(Texture2D)))
        {
            BlockData blockData=new BlockData(texture.name,blockCode,texture);

            blockDatas.Add(blockCode, blockData);
            blockCode++;
        }
    }

    static public BlockData getBlockDataByCode(int blockCode)
    {
        if (blockDatas == null)
            setBlockDatas();

        if (blockDatas.ContainsKey(blockCode))
            return (BlockData)blockDatas[blockCode];

        Debug.Log("warning: failed to find blockData with code: " + blockCode);
        if (blockCode!=0)
            return getBlockDataByCode(0);
        return null;
    }
}
