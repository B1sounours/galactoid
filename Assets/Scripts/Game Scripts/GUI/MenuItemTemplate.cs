using UnityEngine;
using System.Collections;

public class MenuItemTemplate
{
	public string text;
	public string itemType;
	public GUIStyle style;
		
	public MenuItemTemplate (string text, string itemType, GUIStyle style)
	{
		this.text = text;
		this.itemType = itemType;
		this.style = style;
	}
}
