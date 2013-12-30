using UnityEngine;
using System.Collections;

//this class creates and updates the 3d representation of a shipmodel

public class Ship3DView : MonoBehaviour {
    private GameObject[,,] blockGameObjects;
    public ShipModel shipModel;

    void Awake()
    {
        blockGameObjects=new GameObject[GameConstants.maxShipDimension,GameConstants.maxShipDimension,GameConstants.maxShipDimension];
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
        Block block = blockGO.GetComponent<Block>();

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
