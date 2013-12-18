using UnityEngine;
using System.Collections;

public class SpacePlane : MonoBehaviour
{
	
	void Start ()
	{
		
		//setColor (Color.red);	
	}
	
	/* Goofy Rotation Possibility
	void Update()
	{
		//transform.Rotate(0,1,0);	
	}
	*/
	
	
	public void setColor (Color colorMix)
	{
		renderer.material.color = colorMix;
	
	}
	
	public void setTexture (Texture2D texture)
	{
		renderer.material.mainTexture = texture;	
	}
	
	public void setDistance (float distance)
	{
		transform.position = new Vector3 (0, 0, distance);
	}
}
