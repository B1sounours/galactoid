using UnityEngine;
using System.Collections;

public class Starport3DView : MonoBehaviour
{
    GameObject floorPrefab;
    GameObject wallPrefab;

    void Awake()
    {
        floorPrefab = Resources.Load(ResourcePaths.starportFloorPrefab) as GameObject;
        wallPrefab = Resources.Load(ResourcePaths.starportWallPrefab) as GameObject;

    }

    private GameObject createPiece(GameObject prefab, Vector3 position, GameObject containerGameobject)
    {
        GameObject go = Instantiate(prefab) as GameObject;
        go.transform.position = position;
        go.transform.parent = containerGameobject.transform;
        go.name = prefab.name + " " + position.ToString();
        return go;
    }

    public void createStartport(int dimension, GameObject containerGameobject)
    {
        int iters = (int)Mathf.Ceil((float)dimension / 8);
        Vector3 position;
        for (int i = 0; i < iters; i++)
        {
            for (int j = 0; j < iters; j++)
            {
                position = new Vector3(0, 0, 0);
                //floor
                createPiece(floorPrefab, new Vector3(i * 8, -1, j * 8), containerGameobject);

                //ceiling
                createPiece(floorPrefab, new Vector3(i * 8, dimension, j * 8), containerGameobject);

                //side walls
                createPiece(wallPrefab, new Vector3(-1, i * 8, j * 8), containerGameobject);

                createPiece(wallPrefab, new Vector3(dimension, i * 8, j * 8), containerGameobject);

                //back wall
                GameObject go =createPiece(wallPrefab, new Vector3(i * 8, j * 8, dimension), containerGameobject);
                go.transform.Rotate(new Vector3(0, 90, 0));
            }
        }

    }
}
