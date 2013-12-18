using UnityEngine;
using System.Collections;

public class GameHUD
{	
	float fadeTimeElapsed;
	float fadeTimeMax;
	bool isFadingToBlack;
	public bool showSandboxTip=false;
	Texture fadeTexture;
	TutorialGUI tutorialGUI;
	private GUIStyle normalStyle;
	private SimpleTextGUI sandboxText;
	
	public GameHUD (PlayerCounter playerCounter,bool useTutorial)
	{
		sandboxText=new SimpleTextGUI("Press and hold \"G\" for sandbox options.");
		normalStyle = MenuTemplate.getLabelStyle (30, TextAnchor.MiddleCenter, Color.white);
		isFadingToBlack = false;
		fadeTimeMax = 5;
		fadeTexture = Resources.Load ("Textures/FadeToBlack") as Texture;
		tutorialGUI=new TutorialGUI(playerCounter,useTutorial);
	}
	
	public void fadeToBlack (int fadeTime)
	{
		isFadingToBlack = true;
		fadeTimeElapsed = 0;
	}
	
	public void unfadeToBlack ()
	{
		isFadingToBlack = false;
	}
	
	public bool isFadeFinished ()
	{
		return fadeTimeElapsed > fadeTimeMax;
	}
	
	public void draw ()
	{
		if (showSandboxTip){
			sandboxText.draw ();
		}
		
		if (GameSettings.guiFPS) {
			int fps = (int)(1f / Time.deltaTime * Time.timeScale);
			GUILayout.Box ("FPS " + fps);
		}
		
		if (GameSettings.guiReticule) {
			Vector2 size = GUI.skin.label.CalcSize (new GUIContent ("+"));
			Rect rect = new Rect (0, 0, size.x, size.y);
			rect.center = new Vector2 (Screen.width, Screen.height) / 2f;
			GUI.Label (rect, "+");
		}
		
		if (isFadingToBlack) {
			fadeTimeElapsed += Time.deltaTime * Time.timeScale;
			
			Color lastColor = GUI.color;
			float alpha = fadeTimeElapsed / fadeTimeMax;
			GUI.color = new Color (lastColor.r, lastColor.g, lastColor.b, alpha);
			Rect rect = new Rect (0, 0, Screen.width, Screen.height);
			GUI.DrawTexture (rect, fadeTexture, ScaleMode.StretchToFill);
			GUI.color = lastColor;
		}
		
		tutorialGUI.activate();
		
	}
	
	
}
