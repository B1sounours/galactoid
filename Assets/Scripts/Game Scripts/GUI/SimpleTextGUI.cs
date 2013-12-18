using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleTextGUI {
	private Rect rect;
	private string text;
	private GUIStyle style;
	
	public SimpleTextGUI(string text){
		this.rect = new Rect (0, Screen.height * 4 / 5, Screen.width, 50);
		this.text=text;
		style=MenuTemplate.getLabelStyle(30);
		style.alignment=TextAnchor.MiddleCenter;
	}
	
	public void draw(){
		GUI.Label (rect, text, style);
	}
}
