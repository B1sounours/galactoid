using UnityEngine;
using System.Collections;

public class LeaveMapTeleporter : MonoBehaviour
{

	public VoxelManager lastFloorVoxel;
	private WorldManager worldManager;
	private CharacterMotor characterMotor;
	private GUIManager guiManager;
	private bool fallRecoveryMode;
	private float fallRecoveryTimer;
	private PlayerCounter playerCounter;

	void Start ()
	{
		worldManager = (WorldManager)FindObjectOfType (typeof(WorldManager));
		guiManager = (GUIManager)FindObjectOfType (typeof(GUIManager));
		
		characterMotor=worldManager.getPlayer().GetComponent<CharacterMotor>();
	}
	
	void Update ()
	{
		if (fallRecoveryMode) {
			fallRecoveryTimer -= Time.deltaTime * Time.timeScale;
			if (fallRecoveryTimer < 0) {
				guiManager.unfadeToBlack ();
				fallRecoveryTeleport ();
			}
		} else if (!worldManager.isPlayerInsideWorld ()) {
			fallRecoveryMode = true;
			guiManager.fadeToBlack (GameSettings.fadeToBlackTime);
			fallRecoveryTimer = GameSettings.fadeToBlackTime;
		}
	}
	
	private PlayerCounter getPlayerCounter ()
	{
		if (playerCounter == null)
			playerCounter = (PlayerCounter)FindObjectOfType (typeof(PlayerCounter));
		
		return playerCounter;
	}
	
	private void fallRecoveryTeleport ()
	{
		//this moves the player to the last voxel that was stood on
		fallRecoveryMode = false;
		characterMotor.SetVelocity (new Vector3 (0, 0, 0));
		
		getPlayerCounter().hasFallen=true;
		
		Vector3 newPosition;
		if (lastFloorVoxel == null || lastFloorVoxel.getBlobManagerOwner ().isDrifting) {
			//Debug.Log("warning: fallRecoveryTeleport got a null lastFloorVoxel.");
			newPosition = worldManager.getResetPosition ();
		} else {
			newPosition = new Vector3 (0, 0, 0);
			newPosition.x = lastFloorVoxel.transform.position.x;
			newPosition.y = lastFloorVoxel.transform.position.y + 2f;
			newPosition.z = lastFloorVoxel.transform.position.z;
			characterMotor.jumping.jumping = false;
		}
		characterMotor.tr.position = newPosition;
	}
}
