using UnityEngine;
using System.Collections;

//this class creates and updates the 3d representation of a shipmodel

public class Ship3DView : MonoBehaviour
{
    private GameObject blockContainer;
    private GameObject[, ,] blockGameObjects;
    public ShipModel shipModel;

    void Awake()
    {
        blockGameObjects = new GameObject[GameConstants.maxShipDimension, GameConstants.maxShipDimension, GameConstants.maxShipDimension];
        blockContainer = new GameObject("Blocks");
    }

    public void initializeAllBlocks()
    {
        //used when no 3d blocks have been instantiated but a shipModel already exists
        foreach (BlockData blockData in shipModel.blockDatas)
        {
            if (blockData == null)
                continue;

            if (blockData.position.x == 10)
            {
                Debug.Log(blockData.textureFilename);
                blockData.position.show();
            }

            createBlock(blockData, blockData.position);
        }
    }

    public void removeBlock(IntVector3 position)
    {
        GameObject go = blockGameObjects[position.x, position.y, position.z];
        Destroy(go);
        blockGameObjects[position.x, position.y, position.z] = null;
    }

    public Block createBlock(BlockData blockData, IntVector3 position)
    {
        GameObject blockGO = Instantiate(ResourceLookup.getBlockPrefab(), position.getVector3(), Quaternion.identity) as GameObject;
        blockGO.transform.parent = blockContainer.transform;
        Block block = blockGO.GetComponent<Block>();
        blockData.position = position;
        block.initialize(blockData);

        if (blockData.isRotationRandom)
        {
            blockGO.transform.eulerAngles = Angles.getRandom();
        }
        else
        {
            blockGO.transform.eulerAngles = Angles.getFlat();
        }
        //blockGO.transform.parent = transform;
        blockGameObjects[position.x, position.y, position.z] = blockGO;

        return block;
    }

}
