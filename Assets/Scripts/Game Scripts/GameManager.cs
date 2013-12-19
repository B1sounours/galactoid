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
        sm.createBlock(0, position);
    }

    private void debug1()
    {
        getPlayer().GetComponent<CharacterMotor>().tr.position = new Vector3(2, 2, 2);

        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                sm.createBlock(0, new IntVector3(i, 0, j));

        sm.createBlock(0, new IntVector3(5, 1, 5));
        sm.createBlock(0, new IntVector3(5, 2, 5));
        sm.createBlock(0, new IntVector3(5, 3, 5));
        sm.createBlock(0, new IntVector3(5, 3, 6));
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
