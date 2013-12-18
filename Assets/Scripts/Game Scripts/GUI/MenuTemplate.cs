using UnityEngine;
using System.Collections;

public class MenuTemplate
{	
	private ArrayList itemTemplates;
	private Texture mainBackground;
	private string clickedItem;
	private Hashtable styles;
	public string menuName;
	
	public enum MenuFonts
	{
		title,
		large,
		medium,
		small
	}
	
	public MenuTemplate (string name)
	{
		menuName = name;
		itemTemplates = new ArrayList ();
		mainBackground = Resources.Load ("Textures/MainBackground") as Texture;
		setStyles ();
	}
	
	public static GUIStyle getLabelStyle (int fontDivisor)
	{
		//if fontDivisor is large, the fontSize will be smaller, linearly proportional to screen width
		GUIStyle newStyle = new GUIStyle ();
		newStyle.font = Resources.Load ("Fonts/PasseroOne-Regular") as Font;
		newStyle.normal.textColor = Color.white;
		newStyle.alignment = TextAnchor.UpperRight;
		newStyle.fontSize = Screen.width / fontDivisor;
		
		return newStyle;
	}
	
	public static GUIStyle getLabelStyle (int fontDivisor, TextAnchor textAnchor, Color color)
	{
		GUIStyle newStyle = getLabelStyle (fontDivisor);
		newStyle.alignment = textAnchor;
		newStyle.normal.textColor = color;
		
		return newStyle;
	}
	
	public void setTitle ()
	{
		addText ("Viroid", MenuTemplate.MenuFonts.title);
		//addText ("Greeble Madness", MenuTemplate.MenuFonts.small);
	}
	
	private void setStyles ()
	{	
		styles = new Hashtable ();
		styles [MenuFonts.title] = getLabelStyle (10);
		styles [MenuFonts.large] = getLabelStyle (16);
		styles [MenuFonts.medium] = getLabelStyle (28);
		styles [MenuFonts.small] = getLabelStyle (40);
	}
	
	public void addButton (string text)
	{
		GUIStyle style = (GUIStyle)styles [MenuFonts.medium];
		itemTemplates.Add (new MenuItemTemplate (text, "button", style));
	}
	
	public void addText (string text, MenuFonts menuFont)
	{
		GUIStyle style = (GUIStyle)styles [menuFont];
		itemTemplates.Add (new MenuItemTemplate (text, "text", style));
	}
	
	public MenuAction draw ()
	{
		MenuAction menuAction = null;
		Rect rect = new Rect (0, 0, Screen.width, Screen.height);
		GUI.DrawTexture (rect, mainBackground, ScaleMode.ScaleAndCrop, false, 0);
		
		GUILayout.BeginArea (new Rect (0, 0, Screen.width * 5 / 6, Screen.height * 19 / 20));
		GUILayout.BeginVertical ();
		
		GUILayout.FlexibleSpace ();
		
		foreach (MenuItemTemplate item in itemTemplates) {
			if (item.itemType == "button") {
				if (GUILayout.Button (item.text, item.style))
					menuAction = new MenuAction (menuName, item.text);
			}
			if (item.itemType == "text")
				GUILayout.Label (item.text, item.style);
			
		}
		
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
		
		return menuAction;
	}
}
