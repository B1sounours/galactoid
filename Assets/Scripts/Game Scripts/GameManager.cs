using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private GameObject player;
    private GameObject blockPrefab;

    private Camera backgroundCamera;
    private Block blockManager;

    void Awake()
    {
        blockManager = gameObject.AddComponent<Block>();
        gameObject.AddComponent<GameMenuManager>();
        getPlayer();

        //genSpaceMakers ();
    }
    public GameObject getBlockPrefab()
    {
        if (blockPrefab == null)
            blockPrefab = ResourceLookup.getBlockPrefab();

        return blockPrefab;
    }
    void Update()
    {
    }

    public Block getBlockManager()
    {
        return blockManager;
    }

    public GameObject getPlayer()
    {
        if (player == null)
            player = PlayerGenerator.genPlayer();

        return player;
    }

}
