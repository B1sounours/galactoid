using UnityEngine;
using System.Collections;

public static class PlayerGenerator
{
	private static GameObject player;
	
	public static GameObject genPlayer ()
	{
		player = new GameObject ("Player");

		setMainCamera ();
		setSpaceCamera ();
		setCharacterController ();
		setCharacterMotor ();
		setPlayerComponents ();
		
		return player;
	}
	
	private static void setMainCamera ()
	{
		Camera mainCamera = player.AddComponent<Camera> ();
		mainCamera.cullingMask = ~(1 << 9);
		mainCamera.depth = 2;
		mainCamera.clearFlags = CameraClearFlags.Nothing;
		//mainCameraGO.AddComponent<GUILayer> ();
	}
	
	private static void setSpaceCamera ()
	{
		GameObject spaceCameraGO = new GameObject ("Space Camera");
		Camera spaceCamera = spaceCameraGO.AddComponent<Camera> ();
		spaceCamera.cullingMask = (1 << 9);
		spaceCamera.depth = 1;
		spaceCamera.clearFlags = CameraClearFlags.Skybox;
		spaceCamera.backgroundColor = Color.black;
		spaceCameraGO.transform.parent = player.transform;
		spaceCamera.nearClipPlane = 1f;
		spaceCamera.farClipPlane = 500f;
	}
	
	private static void setCharacterController ()
	{
		CharacterController cc = player.AddComponent<CharacterController> ();
		cc.radius = 0.4f;
		cc.height = 1.8f;
		cc.stepOffset = 0.4f;
	}

	private static void setCharacterMotor ()
	{
		CharacterMotor cm = player.AddComponent<CharacterMotor> ();
		cm.movement.regularGravity = 9.81f;
		cm.jumping.minHeight = 2f;
		cm.jumping.maxHeight = 2f;
	}

	private static void setPlayerComponents ()
	{
		MouseLook mouseLook= player.AddComponent<MouseLook> ();
		mouseLook.minimumY=-80f;
		mouseLook.maximumY=80f;
		
		player.AddComponent<FPSInputController> ();
		player.AddComponent<AudioListener> ();
		player.AddComponent<PlayerController>();
		player.AddComponent<PlayerClicker> ();
	}

}
