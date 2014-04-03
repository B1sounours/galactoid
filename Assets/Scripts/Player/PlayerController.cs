using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	
	FPSInputController fpsi;
	CharacterController cc;
	MouseLook ml;
    PlayerClicker pc;
	
	void Awake(){
		fpsi=(FPSInputController)FindObjectOfType(typeof(FPSInputController));
		cc=(CharacterController)FindObjectOfType(typeof(CharacterController));
		ml=(MouseLook)FindObjectOfType(typeof(MouseLook));
        pc = (PlayerClicker)FindObjectOfType(typeof(PlayerClicker));
	}

    public void setWorldCanMovePlayer(bool isEnabled)
    {
        cc.enabled = isEnabled;
    }

	public void setPlayerInput(bool isEnabled){
		fpsi.enabled=isEnabled;
		ml.enabled=isEnabled;
        fpsi.motor.input.direction = Vector3.zero;
        pc.enabled = isEnabled;
	}
}
