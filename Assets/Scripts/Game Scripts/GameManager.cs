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
    private GuiManager guiManager;
    private GameHud gameHud;

    private Camera backgroundCamera;
    public ShipModel shipModel;

    void Awake()
    {
        shipModel = gameObject.AddComponent<ShipModel>();
        guiManager = gameObject.AddComponent<GuiManager>();
        gameObject.AddComponent<SkyboxManager>();
        getPlayer();

        debug1();
    }

    GameHud getGameHud()
    {
        return guiManager.getGameHUD();
    }

    public void requestPlaceBlock(IntVector3 position)
    {
        if (getGameHud().selectedBlockStack == null)
        {
            Debug.Log("warning: requestPlaceBlock but selectedBlock is null");
            return;
        }
        shipModel.createBlock(getGameHud().selectedBlockStack.blockData.blockCode, 
            new IntVector3(position.x,position.y,position.z));
    }

    public void requestHarvestBlock(IntVector3 position)
    {
        shipModel.removeBlock(position);
    }

    private void debug1()
    {
        getPlayer().GetComponent<CharacterMotor>().tr.position = new Vector3(2, 2, 2);
        getPlayer().transform.position = new Vector3(5, 10, 5);

        int bSize = 40;
        int blockCode = Random.Range(1, 100);
        for (int i = 0; i < bSize; i++)
            for (int j = 0; j < bSize; j++)
                shipModel.createBlock(blockCode+j/10, new IntVector3(i, 0, j));
        
        for (int i = 1; i < 3; i++)
            for (int j = 0; j < 120; j++)
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
