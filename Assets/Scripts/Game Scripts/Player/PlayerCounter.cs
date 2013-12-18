using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCounter : MonoBehaviour {
	public int breakCount=0;
	public int laserBreakCount=0;
	public int corruptBreakCount=0;
	
	public bool usedFly=false;
	public bool usedLaser=false;
	public bool usedJump=false;
	public bool hasFallen=false;
}
