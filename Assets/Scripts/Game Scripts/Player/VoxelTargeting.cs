using UnityEngine;
using System.Collections;

public class VoxelTargeting : MonoBehaviour
{
	private struct TargetingTool
	{
		public float damagePerSecond;
		public float range;
		
		public TargetingTool (float dps, float range)
		{
			damagePerSecond = dps;
			this.range = range;
		}
	}
	
	private Transform cameraTransform;
	//public AudioClip[] metalBreakSounds;
	//private AudioSource audioSource;
	
	private TargetingTool tool1;
	private TargetingTool tool2;
	private PlayerCounter playerCounter;
	
	void Start ()
	{
		//audioSource = gameObject.GetComponent<AudioSource> ();
		tool1 = new TargetingTool (4, 3);
		tool2 = new TargetingTool (30, 100);
		cameraTransform=gameObject.GetComponent<Camera>().camera.transform;
	}
	
	private PlayerCounter getPlayerCounter ()
	{
		if (playerCounter == null)
			playerCounter = (PlayerCounter)FindObjectOfType (typeof(PlayerCounter));
		
		return playerCounter;
	}
	
	void Update ()
	{
		if (Input.GetMouseButton (0)||Input.GetMouseButton (1)) {
			
			if (Input.GetMouseButton (1))
				getPlayerCounter().usedLaser=true;
			
			TargetingTool tool=Input.GetMouseButton (0)?tool1:tool2;
			
			Vector3 pos = cameraTransform.position;
			RaycastHit hitInfo = new RaycastHit ();
			if (Physics.Linecast (pos, pos + cameraTransform.forward * tool.range, out hitInfo, 1)) {
				GameObject voxelObject = hitInfo.transform.gameObject;
				voxelObject.GetComponent<VoxelManager> ().takeDamage (tool.damagePerSecond * Time.deltaTime);
			}
		}
	}
	
}
