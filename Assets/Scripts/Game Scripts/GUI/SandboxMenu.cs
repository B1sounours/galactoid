using UnityEngine;
using System.Collections;

public class SandboxMenu
{
	
	private Texture mainBackground;
	private GUIStyle titleStyle;
	//private GUIStyle createStyle;
	private GUIStyle normalStyle;
	private GUIStyle toggleStyle;
	private int sliderWidth;
	private int sliderSpace;
	private int moveDirection;
	private int generatorType;
	private Hashtable directions;
	private Hashtable generatorTypes;
	public string menuCommand = "Create";
	public string menuName = "sandbox";
	private BlobData blobData;
	
	public SandboxMenu ()
	{
		mainBackground = Resources.Load ("Textures/SandboxGUI") as Texture;
		titleStyle = MenuTemplate.getLabelStyle (20, TextAnchor.UpperLeft, Color.black);
		//createStyle = MenuTemplate.getLabelStyle (25, TextAnchor.UpperCenter, Color.black);
		normalStyle = MenuTemplate.getLabelStyle (40, TextAnchor.UpperRight, Color.black);
		
		sliderWidth = Screen.width / 3;
		sliderSpace = Screen.width / 10;
		blobData = BlobData.getBlobData ("sandbox");
		
		setupEnumSliders ();
	}
	
	private void setupEnumSliders ()
	{
		moveDirection = (int)blobData.moveDirection;
		generatorType = (int)blobData.generatorType;
		
		directions = new Hashtable ();
		int counter = 0;
		foreach (string direction in System.Enum.GetNames(typeof(Directions.Cardinal))) {
			directions [counter] = direction;
			counter++;
		}
		
		generatorTypes = new Hashtable ();
		counter = 0;
		foreach (string genType in System.Enum.GetNames(typeof(BlobData.GeneratorTypes))) {
			generatorTypes [counter] = genType;
			counter++;
		}
	}
	
	public BlobData getNewBlobData ()
	{
		blobData.setMoveDirection ((Directions.Cardinal)moveDirection);
		blobData.generatorType = (BlobData.GeneratorTypes)generatorType;
		blobData.genMapData ();
		return blobData;
	}
	
	public MenuAction draw ()
	{
		MenuAction menuAction = null;
		Rect rect = new Rect (0, 0, Screen.width, Screen.height);
		GUI.DrawTexture (rect, mainBackground, ScaleMode.ScaleAndCrop, false, 0);
		
		GUILayout.BeginArea (new Rect (Screen.width / 12, Screen.height / 12, Screen.width * 10 / 12, Screen.height * 10 / 12));
		GUILayout.BeginVertical ();
		GUILayout.Label ("Sandbox", titleStyle);
		
		drawIntegerSlider ("Width", ref blobData.size.width, 1, 100);
		drawIntegerSlider ("Height", ref blobData.size.height, 1, 100);
		drawIntegerSlider ("Depth", ref blobData.size.depth, 1, 100);
		drawFloatSlider ("Speed", ref blobData.speed, 0.5f, 20f);
		drawIntegerSlider ("xOffset", ref blobData.offset.x, -200, 200);
		drawIntegerSlider ("yOffset", ref blobData.offset.y, -200, 200);
		
		drawBoolCheckbox ("Guarentee Hit", ref blobData.offset.guarenteeHit);
		drawFloatSlider ("Viroid Ratio", ref blobData.viroid.exposure, 0f, 1f);
		drawFloatSlider ("Viroid Height Pref", ref blobData.viroid.heightPreference, 0f, 1f);
		drawHashtableSlider ("Move Direction", ref moveDirection, directions);
		drawHashtableSlider ("Generator Type", ref generatorType, generatorTypes);
		
		GUILayout.BeginHorizontal ();
		GUILayout.Space (Screen.width / 3);		
		if (GUILayout.Button (menuCommand)) 
			menuAction = new MenuAction (menuName, menuCommand);
		GUILayout.Space (Screen.width / 3);
		GUILayout.EndHorizontal ();
		
		GUILayout.EndVertical ();
		
		GUILayout.EndArea ();
		
		return menuAction;
	}
	
	public GUIStyle getToggleStyle ()
	{
		if (toggleStyle == null) {
			toggleStyle = GUI.skin.toggle;
			toggleStyle.font = normalStyle.font;
			toggleStyle.fontSize = normalStyle.fontSize;
			toggleStyle.normal.textColor = normalStyle.normal.textColor;
			toggleStyle.onNormal.textColor = normalStyle.normal.textColor;
		}
		return toggleStyle;
	}
	
	public void drawBoolCheckbox (string text, ref bool checkValue)
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Space (Screen.width / 2);
		checkValue = (bool)GUILayout.Toggle (checkValue, text, getToggleStyle ());
		GUILayout.EndHorizontal ();
	}
	
	public void drawHashtableSlider (string text, ref int sliderValue, Hashtable hashtable)
	{
		int max = hashtable.Count - 1;
		GUILayout.BeginHorizontal ();
		GUILayout.Label (text + ": " + hashtable [sliderValue], normalStyle);
		GUILayout.Space (sliderSpace);
		sliderValue = (int)GUILayout.HorizontalSlider (sliderValue, 0, max, GUILayout.Width (sliderWidth));
		GUILayout.EndHorizontal ();
	}
			
	public void drawIntegerSlider (string text, ref int sliderValue, int min, int max)
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Label (text + ": " + sliderValue, normalStyle);
		GUILayout.Space (sliderSpace);
		sliderValue = (int)GUILayout.HorizontalSlider (sliderValue, min, max, GUILayout.Width (sliderWidth));
		GUILayout.EndHorizontal ();
	}
	
	public void drawFloatSlider (string text, ref float sliderValue, float min, float max)
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Label (text + ": " + sliderValue, normalStyle);
		GUILayout.Space (sliderSpace);
		sliderValue = GUILayout.HorizontalSlider (sliderValue, min, max, GUILayout.Width (sliderWidth));
		GUILayout.EndHorizontal ();
	}
	
}

