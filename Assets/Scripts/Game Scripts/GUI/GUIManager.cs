using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour
{
	private bool escIsEnabled;
	private enum GameModes
	{
		gameplay,
		paused,
		sandbox,
		endlevel
	};
	private GameModes gameMode;
	private GameObject player;
	private MenuAction menuAction;
	private GameHUD gameHUD;
	private WorldManager worldManager;
	private MenuTemplate pauseMenu;
	private SandboxMenu sandboxMenu;
	private DialogueMenu endLevelMenu;
	private MenuTemplate activeMenu;
	
	void Start ()
	{
		//player = GameObject.FindGameObjectWithTag ("Player");
		worldManager = (WorldManager)FindObjectOfType (typeof(WorldManager));
		player = worldManager.getPlayer ();
		
		escIsEnabled = true;
		setResume ();		
		
		pauseMenu = getPauseMenu ();
		sandboxMenu = getSandboxMenu ();
		endLevelMenu = getEndLevelMenu ();
		gameHUD = getGameHUD ();
	}
	
	private GameHUD getGameHUD ()
	{
		PlayerCounter playerCounter = (PlayerCounter)FindObjectOfType (typeof(PlayerCounter));
		gameHUD = new GameHUD (playerCounter, worldManager.getLevelData ().useTutorial);
		gameHUD.showSandboxTip=GameSettings.sandboxEnabled;
		return gameHUD;
	}
	
	private DialogueMenu getEndLevelMenu ()
	{
		DialogueMenu dmenu = new DialogueMenu ("game over", "Done!");		
		return dmenu;
	}
	
	private SandboxMenu getSandboxMenu ()
	{
		return new SandboxMenu ();
	}
	
	private MenuTemplate getPauseMenu ()
	{
		MenuTemplate menu = new MenuTemplate ("pause");
		menu.setTitle ();
		menu.addButton ("Resume");
		menu.addButton ("Main Menu");
		menu.addButton ("Exit");
		
		return menu;
	}
	
	private void setResume ()
	{
		gameMode = GameModes.gameplay;
		setGameState (false);
	}
	
	private void setPause ()
	{
		gameMode = GameModes.paused;
		setGameState (true);
	}
	
	private void setSandbox ()
	{
		gameMode = GameModes.sandbox;
		gameHUD.showSandboxTip=false;
		setGameState (true);
	}
	
	public void unfadeToBlack ()
	{
		gameHUD.unfadeToBlack ();
	}
	
	public void endLevel ()
	{
		gameMode = GameModes.endlevel;
		setGameState (true);
		PlayerCounter pc=(PlayerCounter)FindObjectOfType(typeof(PlayerCounter));
		ScoreCalculator.setScoreDialogue(endLevelMenu,pc,worldManager.getFinalVoxelCount());
	}
	
	public void fadeToBlack (int fadeTime)
	{
		gameHUD.fadeToBlack (fadeTime);
	}
	
	void Update ()
	{
		if (!escIsEnabled && !Input.GetButton ("Escape"))
			escIsEnabled = true;
			
		if (Input.GetButton ("Escape") && escIsEnabled) {
			if (gameMode == GameModes.paused) {
				menuAction = new MenuAction ("", "Resume");
			} else if (gameMode == GameModes.gameplay) {
				setPause ();
			}
		}
		
		if (Input.GetButton ("Sandbox") && GameSettings.sandboxEnabled && gameMode != GameModes.sandbox)
			setSandbox ();
		
		if (gameMode == GameModes.sandbox && !Input.GetButton ("Sandbox"))
			setResume ();
		
		if (!Screen.showCursor)
			Screen.lockCursor = true;
	}
	
	void OnGUI ()
	{
		if (gameMode == GameModes.gameplay) {
			menuAction = null;
			gameHUD.draw ();
		} else if (gameMode == GameModes.paused) {
			menuAction = pauseMenu.draw ();
		} else if (gameMode == GameModes.sandbox) {
			menuAction = sandboxMenu.draw ();
		} else if (gameMode == GameModes.endlevel) {
			menuAction = endLevelMenu.draw ();
		}
		if (menuAction != null)
			updateMenu ();
	}
	
	private void setGameState (bool isPaused)
	{		
		Screen.showCursor = isPaused;
		Screen.lockCursor = !isPaused;
		Time.timeScale = isPaused ? 0 : 1f;
		
		player.GetComponent<PlayerController> ().setInput (!isPaused);
	}
	
	public void updateMenu ()
	{
		//this is how other classes communicate menu events to the gui manager
		//Debug.Log (menuAction.menuName + ": " + menuAction.menuCommand);
		
		if (menuAction.menuCommand == "Resume") {
			setResume ();
			return;
		}
		
		if (menuAction.menuName == "game over" && menuAction.menuCommand == "Done")
			Application.LoadLevel ("Start Menu");
		
		if (menuAction.menuName == "pause") {
			switch (menuAction.menuCommand) {
			case "Main Menu":
				Application.LoadLevel ("Start Menu");
				break;
			case "Exit":
				Application.Quit ();
				break;
			}
		}
		if (menuAction.menuName == sandboxMenu.menuName) {
			if (menuAction.menuCommand == sandboxMenu.menuCommand) {
				worldManager.genBlob (sandboxMenu.getNewBlobData ());
			}
		}
	}	
}
