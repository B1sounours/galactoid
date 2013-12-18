using UnityEngine;
using System.Collections;

public class ClickBlock : MonoBehaviour
{
	private Transform cameraTransform;
	
	void Start ()
	{
		//audioSource = gameObject.GetComponent<AudioSource> ();
		cameraTransform=gameObject.GetComponent<Camera>().camera.transform;
	}
	
	void Update ()
	{
		if (Input.GetMouseButton (0)) {
            float range = 100;
			Vector3 pos = cameraTransform.position;
			RaycastHit hitInfo = new RaycastHit ();
			if (Physics.Linecast (pos, pos + cameraTransform.forward * range, out hitInfo, 1)) {
				GameObject blockObject = hitInfo.transform.gameObject;
				Debug.Log("left clicked: "+blockObject.GetComponent<Block> ().name);
			}
		}
	}
	
}
