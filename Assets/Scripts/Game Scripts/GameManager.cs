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
    private ShipModel sm;

    void Awake()
    {
        sm = gameObject.AddComponent<ShipModel>();
        gameObject.AddComponent<GameMenuManager>();
        gameObject.AddComponent<SkyboxManager>();
        getPlayer();

        debug1();
    }

    void Update()
    {
    }

    public void requestPlaceBlock(IntVector3 position){
        sm.createBlock(3, position);
    }

    public void requestHarvestBlock(IntVector3 position)
    {
        sm.removeBlock(position);
    }

    private void debug1()
    {
        getPlayer().GetComponent<CharacterMotor>().tr.position = new Vector3(2, 2, 2);

        for (int i = 0; i < 40; i++)
            for (int j = 0; j < 40; j++)
                sm.createBlock(0, new IntVector3(i, j/10, j));

        for (int i = 0; i < 30; i++)
        {
            sm.createBlock(1, new IntVector3(20, i, 20));
            sm.createBlock(2, new IntVector3(40, i, 70));
            sm.createBlock(3, new IntVector3(60, i, 20));
        }
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
