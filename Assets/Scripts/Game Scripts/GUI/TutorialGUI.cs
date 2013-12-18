using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialGUI
{
	enum TutorialPhases
	{
		wasd,
		jump,
		leftclick,
		rightclick,
		grav,
		viroidtip,
		done
	}
	TutorialPhases tutorialPhase;
	private float viroidTipStart = 0;
	private SimpleTextGUI viroidTipGui;
	
	public TutorialGUI (PlayerCounter playerCounter, bool isEnabled)
	{
		viroidTipGui = new SimpleTextGUI ("Stop red viroids from spreading!");
		this.isEnabled = isEnabled;
		tutorialPhase = TutorialPhases.wasd;
		this.playerCounter = playerCounter;
		setTutorialGraphics ();
	}
	
	private class TutorialGraphic
	{
		public Texture upTexture;
		public Texture downTexture;
		public Rect rect;
		public bool enabled;
		public bool isUp;
		public KeyCode disableKeyCode;
		
		public TutorialGraphic (Texture upTexture, Texture downTexture, Rect rect, KeyCode disableKeyCode)
		{
			this.upTexture = upTexture;
			this.downTexture = downTexture;
			this.rect = rect;
			this.disableKeyCode = disableKeyCode;
			enabled = false;
			isUp = true;

		}
		
		public void activate ()
		{
			if (!enabled)
				return;
			
			if (Input.GetKey (disableKeyCode))
				enabled = false;
			
			draw ();
		}
		
		public void draw ()
		{
			if (isUp)
				GUI.DrawTexture (rect, upTexture, ScaleMode.StretchToFill);
			else
				GUI.DrawTexture (rect, downTexture, ScaleMode.StretchToFill);
		}
		
	}
	TutorialGraphic[] tutorialGraphics;
	bool isEnabled = false;
	float tutorialLastSwapTime = 0;
	PlayerCounter playerCounter;
	
	private void setTutorialGraphics ()
	{
		tutorialGraphics = new TutorialGraphic[8];
		
		IntVector2 buttonSize = new IntVector2 (Screen.width / 10, Screen.width / 10);
		Rect buttonRect = new Rect (Screen.width / 9, Screen.height * 9 / 10 - buttonSize.y, buttonSize.x, buttonSize.y);
		int buttonSpacing = Screen.width / 100;
		
		Texture upTexture = Resources.Load ("Textures/GUI/a up") as Texture;
		Texture downTexture = Resources.Load ("Textures/GUI/a down") as Texture;
		tutorialGraphics [0] = new TutorialGraphic (upTexture, downTexture, buttonRect, KeyCode.A);
		
		buttonRect.x += buttonSize.x + buttonSpacing;
		upTexture = Resources.Load ("Textures/GUI/s up") as Texture;
		downTexture = Resources.Load ("Textures/GUI/s down") as Texture;
		tutorialGraphics [1] = new TutorialGraphic (upTexture, downTexture, buttonRect, KeyCode.S);
		
		buttonRect.y -= buttonSize.y + buttonSpacing;
		upTexture = Resources.Load ("Textures/GUI/w up") as Texture;
		downTexture = Resources.Load ("Textures/GUI/w down") as Texture;
		tutorialGraphics [2] = new TutorialGraphic (upTexture, downTexture, buttonRect, KeyCode.W);
		
		buttonRect.x += buttonSize.x + buttonSpacing;
		buttonRect.y += buttonSize.y + buttonSpacing;
		upTexture = Resources.Load ("Textures/GUI/d up") as Texture;
		downTexture = Resources.Load ("Textures/GUI/d down") as Texture;
		tutorialGraphics [3] = new TutorialGraphic (upTexture, downTexture, buttonRect, KeyCode.D);
		
		for (int i=0; i<4; i++)
			tutorialGraphics [i].enabled = true;
		
		upTexture = Resources.Load ("Textures/GUI/space up") as Texture;
		downTexture = Resources.Load ("Textures/GUI/space down") as Texture;
		int width = Screen.width / 3;
		int height = width * 120 / 1024;
		buttonRect = new Rect (Screen.width / 9, Screen.height * 9 / 10 - height, width, height);
		
		tutorialGraphics [4] = new TutorialGraphic (upTexture, downTexture, buttonRect, KeyCode.Space);
		
		upTexture = Resources.Load ("Textures/GUI/left up") as Texture;
		downTexture = Resources.Load ("Textures/GUI/left down") as Texture;
		width = Screen.width / 8;
		height = width * 1024 / 691;
		buttonRect = new Rect (Screen.width / 9, Screen.height * 9 / 10 - height, width, height);
		
		tutorialGraphics [5] = new TutorialGraphic (upTexture, downTexture, buttonRect, KeyCode.Exclaim);
		
		upTexture = Resources.Load ("Textures/GUI/right up") as Texture;
		downTexture = Resources.Load ("Textures/GUI/right down") as Texture;
		width = Screen.width / 8;
		height = width * 1024 / 691;
		buttonRect = new Rect (Screen.width / 9, Screen.height * 9 / 10 - height, width, height);
		
		tutorialGraphics [6] = new TutorialGraphic (upTexture, downTexture, buttonRect, KeyCode.Exclaim);
		
		upTexture = Resources.Load ("Textures/GUI/shift up") as Texture;
		downTexture = Resources.Load ("Textures/GUI/shift down") as Texture;
		width = Screen.width / 2;
		height = width * 120 / 1400;
		buttonRect = new Rect (Screen.width / 9, Screen.height * 9 / 10 - height, width, height);
		
		tutorialGraphics [7] = new TutorialGraphic (upTexture, downTexture, buttonRect, KeyCode.Exclaim);
		
	}
	
	public void activate ()
	{
		if (!isEnabled)
			return;
		
		bool swapIcon = false;
		if (Time.timeSinceLevelLoad - tutorialLastSwapTime > 1) {
			tutorialLastSwapTime = Time.timeSinceLevelLoad;
			swapIcon = true;
		}
		foreach (TutorialGraphic tg in tutorialGraphics) {
			if (tg == null)
				continue;
			
			if (swapIcon)
				tg.isUp = !tg.isUp;
			
			tg.activate ();
		}
		
		if (tutorialPhase == TutorialPhases.wasd) {
			for (int i=0; i<4; i++) {				
				if (tutorialGraphics [i].enabled)
					break;
				if (i == 3) {
					tutorialPhase = TutorialPhases.jump;
					tutorialGraphics [4].enabled = true;
				}
			}
		}
		
		if (tutorialPhase == TutorialPhases.jump && !tutorialGraphics [4].enabled) {
			tutorialPhase = TutorialPhases.leftclick;
			tutorialGraphics [5].enabled = true;
		}
		
		if (tutorialPhase == TutorialPhases.leftclick) {
			if (playerCounter.breakCount > 0) {
				tutorialPhase = TutorialPhases.rightclick;
				tutorialGraphics [5].enabled = false;
				tutorialGraphics [6].enabled = true;
			}
		}
		
		if (tutorialPhase == TutorialPhases.rightclick) {
			if (playerCounter.laserBreakCount > 0) {
				tutorialPhase = TutorialPhases.grav;
				tutorialGraphics [6].enabled = false;
				tutorialGraphics [7].enabled = true;
			}
		}
		
		if (tutorialPhase == TutorialPhases.grav) {
			if (playerCounter.usedFly) {
				tutorialPhase = TutorialPhases.viroidtip;
				viroidTipStart = Time.timeSinceLevelLoad;
				tutorialGraphics [7].enabled = false;
			}
		}
		
		if (tutorialPhase == TutorialPhases.viroidtip) {
			viroidTipGui.draw ();
			
			if (Time.timeSinceLevelLoad - viroidTipStart > 10)
				tutorialPhase = TutorialPhases.done;
		}
		
	}
}
