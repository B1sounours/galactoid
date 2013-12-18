using UnityEngine;
using System.Collections;

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
        getPlayer();

        debug1();
        //genSpaceMakers ();
    }

    private void makeDebugLight(Vector3 rotation)
    {
        GameObject go = new GameObject("light");
        go.transform.rotation.Set(rotation.x, rotation.y,rotation.z,0);
        Light light = go.AddComponent<Light>();
        light.intensity = 5;
        light.type = LightType.Directional;
    }

    private void makeDebugPlatform()
    {
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                sm.createBlock(0, new IntVector3(i, 0, j));
    }

    private void debug1()
    {
        getPlayer().GetComponent<CharacterMotor>().tr.position = new Vector3(2, 2, 2);
        makeDebugPlatform();
        makeDebugLight(new Vector3(0, 300, 0));
        makeDebugLight(new Vector3(50, 50, 0));
        makeDebugLight(new Vector3(100, 180, 50));
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

    public GameObject getPlayer()
    {
        if (player == null)
            player = PlayerGenerator.genPlayer();

        return player;
    }

}
