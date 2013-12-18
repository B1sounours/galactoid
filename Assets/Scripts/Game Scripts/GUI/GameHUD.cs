using UnityEngine;
using System.Collections;

public class GameHUD
{	
	private GUIStyle normalStyle;
	
	public GameHUD ()
	{
		normalStyle = MenuTemplate.getLabelStyle (30, TextAnchor.MiddleCenter, Color.white);
	}
		
	public void draw ()
	{		
		if (GameOptions.guiFPS) {
			int fps = (int)(1f / Time.deltaTime * Time.timeScale);
			GUILayout.Box ("FPS " + fps);
		}
		
		if (GameOptions.guiReticule) {
			Vector2 size = GUI.skin.label.CalcSize (new GUIContent ("+"));
			Rect rect = new Rect (0, 0, size.x, size.y);
			rect.center = new Vector2 (Screen.width, Screen.height) / 2f;
			GUI.Label (rect, "+");
		}		
	}
	
	
}
