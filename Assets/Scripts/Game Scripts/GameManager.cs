using UnityEngine;
using System.Collections;


//used for binary saving
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/*
 * the GameManager authorizes all in game actions and events. All player input must be routed here 
 * in order to make change or save the game world.
 */

public class GameManager : MonoBehaviour
{
    private GameObject player;
    private GameObject blockPrefab;
    private GuiManager guiManager;
    private GameHud gameHud;

    private Camera backgroundCamera;
    public ShipController shipController;
    public ShipInfo shipInfo;

    void Awake()
    {
        if (GameOptions.loadSavedGame)
        {
            loadGame();
        }
        else
        {
            shipController = new ShipController(new ShipModel());
            setDebugPlatform();
        }

        centerPlayer();
        shipInfo = shipController.shipInfo;
        guiManager = gameObject.AddComponent<GuiManager>();
        gameObject.AddComponent<SkyboxManager>();
        getPlayer();
    }

    GameHud getGameHud()
    {
        return guiManager.getGameHUD();
    }

    public void centerPlayer()
    {
        getPlayer().transform.position = new Vector3(5, 10, 5);
    }

    public void saveGame()
    {
        BinaryFormatter b = new BinaryFormatter();
        FileStream f = File.Create(GameOptions.getSaveLoadPath());
        b.Serialize(f, shipController.shipModel);
        f.Close();
    }

    public void loadGame()
    {
        if (File.Exists(GameOptions.getSaveLoadPath()))
        {
            BinaryFormatter b = new BinaryFormatter();
            FileStream f = File.Open(GameOptions.getSaveLoadPath(), FileMode.Open);
            ShipModel sm = (ShipModel)b.Deserialize(f);
            shipController = new ShipController(sm);
        }
    }

    public void requestPlaceBlock(IntVector3 position)
    {
        if (getGameHud().selectedBlockStack == null)
        {
            Debug.Log("warning: requestPlaceBlock but selectedBlock is null");
            return;
        }
        shipController.createBlock(getGameHud().selectedBlockStack.blockData.blockCode,
            new IntVector3(position.x, position.y, position.z));
    }

    public void requestHarvestBlock(IntVector3 position)
    {
        shipController.removeBlock(position);
    }

    private void setDebugPlatform()
    {
        int bSize = 40;
        int blockCode = UnityEngine.Random.Range(1, 100);
        for (int i = 0; i < bSize; i++)
            for (int j = 0; j < bSize; j++)
                shipController.createBlock(blockCode + j / 10, new IntVector3(i, 0, j));
        

        for (int i = 1; i < 3; i++)
            for (int j = 0; j < 112; j++)
                shipController.createBlock(j, new IntVector3((j % 20) * 2, i, (j / 10)));

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
