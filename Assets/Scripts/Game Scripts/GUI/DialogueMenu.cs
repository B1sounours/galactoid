using UnityEngine;
using System.Collections;

public class DialogueMenu
{
	private class TableItem{
		public string itemName;
		public string itemValue;
		public TableItem(string itemName,string itemValue){
			this.itemName=itemName;
			this.itemValue=itemValue;
		}
	}
	
	private Texture background;
	private GUIStyle titleStyle;
	private GUIStyle normalStyle;
	private GUIStyle mainTextStyle;
	private ArrayList tableItems;
	private string title;
	private string mainText;
	private string menuCommand = "Done";
	private string menuName;
	
	public DialogueMenu (string menuName,string title)
	{
		this.menuName=menuName;
		this.title=title;
		this.mainText="";
		tableItems=new ArrayList();
		background = Resources.Load (ResourcePaths.mainSplash) as Texture;
		titleStyle = MenuTemplate.getLabelStyle (25, TextAnchor.UpperCenter, Color.black);
		mainTextStyle = MenuTemplate.getLabelStyle (35, TextAnchor.UpperCenter, Color.black);
		normalStyle = MenuTemplate.getLabelStyle (46, TextAnchor.UpperLeft, Color.black);
	}
	
	public void setMainText(string mainText){
		this.mainText=mainText;
	}
	
	public void addItem(string itemName, int itemValue){
		addItem(itemName,itemValue.ToString());
	}
	
	public void addItem(string itemName, bool itemValue){
		addItem(itemName,itemValue?"Yep!":"Nope");
	}
	
	public void addItem(string itemName, float itemValue){
		addItem(itemName,itemValue.ToString());
	}
	
	public void addItem(string itemName, string itemValue){
		tableItems.Add(new TableItem(itemName,itemValue));
	}
	
	public MenuAction draw ()
	{
		MenuAction menuAction = null;
		
		int width=Screen.width*2/3;
		int height=width*6/8;
		
		Rect rect = new Rect ((Screen.width-width)/2, (Screen.height-height)/2,
			width, height);
		GUI.DrawTexture (rect, background, ScaleMode.StretchToFill,true, 0);
		
		rect.x+=rect.width/20;
		rect.y+=rect.height/20;
		rect.width-=rect.width/10;
		rect.height-=rect.height/10;
		
		GUILayout.BeginArea (rect);
		GUILayout.BeginVertical ();
		GUILayout.Label (title, titleStyle);
		if (mainText!=""){
			GUILayout.Label (mainText, mainTextStyle);
			GUILayout.Label ("", normalStyle);
		}
		
		foreach(TableItem ti in tableItems){
			GUILayout.BeginHorizontal();
			GUILayout.Label(ti.itemName,normalStyle);
			GUILayout.FlexibleSpace();
			GUILayout.Label(ti.itemValue,normalStyle);
			GUILayout.EndHorizontal();
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal ();
		GUILayout.Space (rect.width / 3);		
		if (GUILayout.Button (menuCommand)) 
			menuAction = new MenuAction (menuName, menuCommand);
		GUILayout.Space (rect.width / 3);
		GUILayout.EndHorizontal ();
		
		GUILayout.EndVertical ();
		
		GUILayout.EndArea ();
		
		return menuAction;
	}
	
}

