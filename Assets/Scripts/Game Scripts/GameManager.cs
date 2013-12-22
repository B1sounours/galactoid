using UnityEngine;
using System.Collections;

/*
 * the GameManager authorizes all in game actions and events. All player input must be routed here 
 * in order to make changes to the game world.
 */

public class GameManager : MonoBehaviour
{
    private GameObject player;
    private GameObject blockPrefab;

    private Camera backgroundCamera;
    public ShipModel shipModel;

    void Awake()
    {
        shipModel = gameObject.AddComponent<ShipModel>();
        gameObject.AddComponent<GameMenuManager>();
        gameObject.AddComponent<SkyboxManager>();
        getPlayer();

        debug1();
    }

    void Update()
    {
    }

    public void requestPlaceBlock(IntVector3 position)
    {
        shipModel.createBlock(3, position);
    }

    public void requestHarvestBlock(IntVector3 position)
    {
        shipModel.removeBlock(position);
    }

    private void debug1()
    {
        getPlayer().GetComponent<CharacterMotor>().tr.position = new Vector3(2, 2, 2);
        getPlayer().transform.position = new Vector3(5, 10, 5);

        int blockCode = Random.Range(1, 100);
        for (int i = 0; i < 40; i++)
            for (int j = 0; j < 40; j++)
                shipModel.createBlock(blockCode+j/10, new IntVector3(i, 0, j));

        for (int i = 1; i < 3; i++)
            for (int j = 0; j < 100; j++)
                shipModel.createBlock(j, new IntVector3((j%20)*2, i, (j/10)));

    }

    public GameObject getBlockPrefab()
    {
        if (blockPrefab == null)
            blockPrefab = ResourceLookup.getBlockPrefab();

        return blockPrefab;
    }

    public GameObject getPlayer()
    {
        if (player == null)
            player = PlayerGenerator.genPlayer();

        return player;
    }

}
