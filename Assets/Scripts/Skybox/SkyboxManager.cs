using UnityEngine;
using System.Collections;

public class SkyboxManager : MonoBehaviour
{
    public void Awake(){
        genSkyboxes();
    }

    private void genSkyboxes()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        GameObject skyboxParent = new GameObject("Skyboxes");
        foreach (GameObject skybox in getSkyboxes())
        {
            //Debug.Log("skybox: " + skybox.name);
            GameObject go = (GameObject)Instantiate(skybox, pos, transform.rotation);
            go.name = "Skybox: " + skybox.gameObject.name;
            go.transform.parent = skyboxParent.transform;
        }
    }

    private ArrayList getSkyboxes()
    {
        return ResourceLookup.getSkyboxPrefabs();
    }
}