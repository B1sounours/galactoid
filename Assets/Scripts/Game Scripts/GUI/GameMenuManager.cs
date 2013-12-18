using UnityEngine;
using System.Collections;

public class GameMenuManager : MonoBehaviour
{
	private bool escIsEnabled;
	private enum GameModes
	{
		gameplay,
		paused
	};
	private GameModes gameMode;
	private GameObject player;
	private MenuAction menuAction;
	private GameHUD gameHUD;
	private GameManager worldManager;
	private MenuTemplate pauseMenu;
	private MenuTemplate activeMenu;
	
	void Start ()
	{
		worldManager = (GameManager)FindObjectOfType (typeof(GameManager));
		player = worldManager.getPlayer ();
		
		escIsEnabled = true;
		setResume ();		
		
		pauseMenu = getPauseMenu ();
		gameHUD = getGameHUD ();
	}
	
	private GameHUD getGameHUD ()
	{
        if (gameHUD==null)
		    gameHUD = new GameHUD ();
		return gameHUD;
	}
    	
	private MenuTemplate getPauseMenu ()
	{
        if (pauseMenu != null)
            return pauseMenu;

        pauseMenu = new MenuTemplate("pause");
        pauseMenu.setTitle();
        pauseMenu.addButton("Resume");
        pauseMenu.addButton("Main Menu");
        pauseMenu.addButton("Exit");

        return pauseMenu;
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
	}	
}
