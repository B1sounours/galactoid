using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    private MenuActions menuAction;

    private enum Menus
    {
        main,
        newGame,
        loadGame
    }
    private Menus currentMenu;
    private string currentSubtitle;

    private string newShipName = "Enterprise";

    //these are precalculated constants to be used for OnGui
    private Rect screenArea;
    private Rect guiArea;
    //left space between the screen and the start of a button
    private float buttonSpace;
    private Texture splashBackground;

    void Awake()
    {
        setCurrentMenu(Menus.main);

        buttonSpace = Screen.width / 10;
        float border = 0.05f;
        guiArea = new Rect(Screen.width * border, Screen.height * border, Screen.width * (1 - border * 2), Screen.height * (1 - border * 2));
        screenArea = new Rect(0, 0, Screen.width, Screen.height);
        splashBackground = Resources.Load(ResourcePaths.mainSplash) as Texture;
    }

    private void setCurrentMenu(Menus currentMenu)
    {
        this.currentMenu = currentMenu;
        if (currentMenu == Menus.main)
            currentSubtitle = "";
        if (currentMenu == Menus.loadGame)
            currentSubtitle = "Load Game";
        if (currentMenu == Menus.newGame)
            currentSubtitle = "New Game";
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (currentMenu == Menus.main)
            {
                menuAction = MenuActions.exitGame;
            }
            else
            {
                menuAction = MenuActions.gotoMainMenu;
            }
        }

        processMenuAction();
    }

    void OnGUI()
    {
        GUI.DrawTexture(screenArea, splashBackground, ScaleMode.ScaleAndCrop, false, 0);

        GUILayout.BeginArea(guiArea);
        GUILayout.BeginVertical();
        GUILayout.Label("Galactoid", GuiFunctions.getTitleStyle());
        GUILayout.Label(currentSubtitle, GuiFunctions.getSubtitleStyle());
        GUILayout.EndVertical();

        if (currentMenu == Menus.main)
            drawMain();
        if (currentMenu == Menus.loadGame)
            drawLoadGame();
        if (currentMenu == Menus.newGame)
            drawNewGame();

        GUILayout.EndArea();
    }

    private void drawMain()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(buttonSpace);
        GUILayout.BeginVertical();

        if (GUILayout.Button("New Ship", GuiFunctions.getButtonStyle()))
            menuAction = MenuActions.newGame;
        if (GUILayout.Button("Continue", GuiFunctions.getButtonStyle()))
            menuAction = MenuActions.loadGame;
        if (GUILayout.Button("Exit", GuiFunctions.getButtonStyle()))
            menuAction = MenuActions.exitGame;

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    private void drawLoadGame()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(buttonSpace);
        GUILayout.BeginVertical();


        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    private void drawNewGame()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(buttonSpace);
        GUILayout.BeginVertical();

        GUILayout.Label("Ship Name:", GuiFunctions.getButtonStyle());
        newShipName=GUILayout.TextField(newShipName);

        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Begin", GuiFunctions.getButtonStyle()))
                menuAction = MenuActions.submitNewGame;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        GUILayout.Space(buttonSpace);
        GUILayout.EndHorizontal();
    }

    private void processMenuAction()
    {
        if (menuAction == MenuActions.none)
            return;

        if (menuAction == MenuActions.exitGame)
            Application.Quit();
        if (menuAction == MenuActions.gotoMainMenu)
            setCurrentMenu(Menus.main);
        if (menuAction == MenuActions.loadGame)
        {
            GameOptions.loadSavedGame = true;
            Application.LoadLevel("Starport");
        }
        if (menuAction == MenuActions.newGame)
        {
            menuAction = MenuActions.submitNewGame;
            //setCurrentMenu(Menus.newGame);
        }
        if (menuAction == MenuActions.submitNewGame)
        {
            GameOptions.loadSavedGame = false;
            Application.LoadLevel("Starport");
        }

        menuAction = MenuActions.none;
    }

}
