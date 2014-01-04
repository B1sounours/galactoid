using UnityEngine;
using System.Collections;

public static class BlockDataLookup
{
    private static Hashtable blockDatas;

    static private void setBlockDatas(){
        blockDatas=new Hashtable();

        int blockCode = 0;
        //optimization: this resource load really only needs the paths of the texture
        foreach (Texture2D texture in Resources.LoadAll(ResourcePaths.blockTextures,typeof(Texture2D)))
        {
            BlockData blockData=new BlockData(texture.name,blockCode,IntVector3.zero);
            blockDatas.Add(blockCode, blockData);
            blockCode++;
        }
    }

    static public BlockData getBlockDataByCode(int blockCode)
    {
        //warning: make copies of the blockdata returned here if you want to make changes.
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
