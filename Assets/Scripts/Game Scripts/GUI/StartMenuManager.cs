using UnityEngine;
using System.Collections;

public class StartMenuManager : MonoBehaviour
{	
	private bool escIsEnabled;
	private MenuAction menuAction;
	private MenuTemplate mainMenu;
	private MenuTemplate creditsMenu;
	private MenuTemplate activeMenu;
	private MenuTemplate startGameMenu;
	private Hashtable levelNameDic;
	
	void Awake ()
	{
		escIsEnabled = true;
		menuAction = null;
		levelNameDic=new Hashtable();
		
		startMenuMusic ();
		
		mainMenu = getMainMenu ();
		creditsMenu = getCreditsMenu ();
		startGameMenu = getStartGameMenu ();
		
		activeMenu = mainMenu;
	}
	
	private void startMenuMusic ()
	{
		GameObject musicManager = Resources.Load ("Prefabs/Music Managers/main menu") as GameObject;
		Instantiate (musicManager);
	}
	
	private MenuTemplate getMainMenu ()
	{
		MenuTemplate menu = new MenuTemplate ("main");
		menu.setTitle ();
		menu.addButton ("Start Game");
		menu.addButton ("Sandbox");
		menu.addButton ("Credits");
		menu.addButton ("Exit");
		
		return menu;
	}
	
	private MenuTemplate getCreditsMenu ()
	{
		MenuTemplate menu = new MenuTemplate ("credits");
		menu.addText ("Credits:", MenuTemplate.MenuFonts.large);
		menu.addText ("Stuart Spence", MenuTemplate.MenuFonts.medium);
		menu.addText ("Programmer", MenuTemplate.MenuFonts.small);
		menu.addText ("Kyler Kelly", MenuTemplate.MenuFonts.medium);
		menu.addText ("Artist", MenuTemplate.MenuFonts.small);
		menu.addText ("Philip Storey", MenuTemplate.MenuFonts.medium);
		menu.addText ("Project Starter", MenuTemplate.MenuFonts.small);

		menu.addButton ("Back");
		
		return menu;
	}
	
	private MenuTemplate getStartGameMenu ()
	{
		MenuTemplate menu = new MenuTemplate ("levels");
		menu.addText ("Levels", MenuTemplate.MenuFonts.large);
		
		foreach (Object obj in Resources.LoadAll("Prefabs/Levels", typeof(LevelData))) {
		//foreach (GameObject obj in Resources.LoadAll("Prefabs/Levels", typeof(LevelData))) {
			if (obj.name!="default" && obj.name!="sandbox"){
				string levelName=((LevelData)obj).levelName;
				levelNameDic.Add(levelName,obj.name);
				menu.addButton (levelName);
			}
		}
		
		menu.addButton ("Back");
		return menu;
	}
	
	void Update ()
	{	
		if (!Screen.showCursor)
			Screen.lockCursor = true;
		
		if (!escIsEnabled && !Input.GetKeyDown (KeyCode.Escape))
			escIsEnabled = true;
		
		if (escIsEnabled && Input.GetKeyDown (KeyCode.Escape)) {
			string command = activeMenu == mainMenu ? "Exit" : "Back";
			menuAction = new MenuAction (activeMenu.menuName, command);
		}
		
		if (menuAction != null) {
			updateMenu ();
			menuAction = null;
		}
	}
	
	void OnGUI ()
	{
		menuAction = activeMenu.draw ();
	}
	
	private void updateMenu ()
	{
		//Debug.Log (menuAction.menuName + ": " + menuAction.menuCommand);
		
		if (menuAction.menuCommand == "Back") {
			activeMenu = mainMenu;
			return;
		}
		
		if (menuAction.menuName == "main") {
		
			switch (menuAction.menuCommand) {
			
			case "Start Game":
				activeMenu = startGameMenu;
				break;
			case "Sandbox":
				GameSettings.sandboxEnabled = true;
				GameSettings.setLevelPath ("sandbox");
				Application.LoadLevel ("Game World");
				break;
			case "Credits":
				activeMenu = creditsMenu;
				break;
			case "Exit":
				Application.Quit ();
				break;
			}
		} else if (menuAction.menuName == "levels") {
			string levelName=(string)levelNameDic[menuAction.menuCommand];
			GameSettings.setLevelPath (levelName);
			GameSettings.sandboxEnabled = false;
			Application.LoadLevel ("Game World");
		}
	}
	
}
