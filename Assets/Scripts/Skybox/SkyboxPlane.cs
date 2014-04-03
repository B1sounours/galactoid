using UnityEngine;
using System.Collections;

public class SkyboxPlane : MonoBehaviour
{

    void Start()
    {
    }

    public void setColor(Color colorMix)
    {
        renderer.material.color = colorMix;

    }

    public void setTexture(Texture2D texture)
    {
        renderer.material.mainTexture = texture;
    }

    public void setDistance(float distance)
    {
        transform.position = new Vector3(0, 0, distance);
    }
}
