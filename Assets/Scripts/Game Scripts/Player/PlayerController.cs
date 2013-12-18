using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	
	FPSInputController fpsi;
	CharacterController cc;
	MouseLook ml;
	
	void Awake(){
		fpsi=(FPSInputController)FindObjectOfType(typeof(FPSInputController));
		cc=(CharacterController)FindObjectOfType(typeof(CharacterController));
		ml=(MouseLook)FindObjectOfType(typeof(MouseLook));
	}
	
	
	public void setInput(bool isEnabled){
		fpsi.enabled=isEnabled;
		cc.enabled=isEnabled;
		ml.enabled=isEnabled;
	}
}
