using UnityEngine;
using System.Collections;

public class GameMenuManager : MonoBehaviour
{
    private bool escIsEnabled;
    private enum GameModes
    {
        gameplay,
        paused,
        toolSelect
    };
    private GameModes gameMode;
    private GameObject player;
    private MenuAction menuAction;
    private GameHUD gameHUD;
    private GameManager worldManager;
    private MenuTemplate pauseMenu;
    private MenuTemplate activeMenu;
    private ToolSelectMenu toolSelectMenu;

    public int lastMouseButton;

    void Start()
    {
        worldManager = (GameManager)FindObjectOfType(typeof(GameManager));
        player = worldManager.getPlayer();
        lastMouseButton = 0;

        escIsEnabled = true;
        setResume();

        getPauseMenu();
        getGameHUD();
        getToolSelectMenu();
    }

    private ToolSelectMenu getToolSelectMenu()
    {
        if (toolSelectMenu == null)
            toolSelectMenu = new ToolSelectMenu(this);
        return toolSelectMenu;
    }

    private GameHUD getGameHUD()
    {
        if (gameHUD == null)
            gameHUD = new GameHUD();
        return gameHUD;
    }

    private MenuTemplate getPauseMenu()
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

    private void setResume()
    {
        gameMode = GameModes.gameplay;
        setGameState(false);
    }

    private void setPause()
    {
        gameMode = GameModes.paused;
        setGameState(true);
    }

    private void setToolSelect(){
        gameMode = GameModes.toolSelect;
        mouseInputGoesToPlayer(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            lastMouseButton = 0;
        if (Input.GetMouseButtonDown(1))
            lastMouseButton = 1;

        if (!escIsEnabled && !Input.GetButton("Escape"))
            escIsEnabled = true;

        if (Input.GetButton("Escape") && escIsEnabled)
        {
            if (gameMode == GameModes.paused)
            {
                menuAction = new MenuAction("", "Resume");
            }
            else if (gameMode == GameModes.gameplay)
            {
                setPause();
            }
        }

        if (Input.GetButton("Tools"))
        {
            if (gameMode == GameModes.gameplay)
                setToolSelect();
        }
        else if (gameMode == GameModes.toolSelect)
        {
            setResume();
        }

        if (!Screen.showCursor)
            Screen.lockCursor = true;
    }

    void OnGUI()
    {
        if (gameMode == GameModes.gameplay)
        {
            menuAction = null;
            gameHUD.draw();
        }
        else if (gameMode == GameModes.paused)
        {
            menuAction = pauseMenu.draw();
        }
        else if (gameMode == GameModes.toolSelect) {
            menuAction = toolSelectMenu.draw();
        }

        if (menuAction != null)
            updateMenu();
    }

    private void mouseInputGoesToPlayer(bool flag)
    {
        //this toggles whether the mouse is hidden and locked onto the player controller
        Screen.showCursor = !flag;
        Screen.lockCursor = flag;
        player.GetComponent<PlayerController>().setPlayerInput(flag);
    }

    private void setGameState(bool isPaused)
    {
        Time.timeScale = isPaused ? 0 : 1f;
        player.GetComponent<PlayerController>().setWorldCanMovePlayer(!isPaused);
        mouseInputGoesToPlayer(!isPaused);
    }

    public void updateMenu()
    {
        //this is how other classes communicate menu events to the gui manager
        //Debug.Log (menuAction.menuName + ": " + menuAction.menuCommand);

        if (menuAction.menuCommand == "Resume")
        {
            setResume();
            return;
        }

        if (menuAction.menuName == "pause")
        {
            switch (menuAction.menuCommand)
            {
                case "Main Menu":
                    Application.LoadLevel("Start Menu");
                    break;
                case "Exit":
                    Application.Quit();
                    break;
            }
        }
    }
}
