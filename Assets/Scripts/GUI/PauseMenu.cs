using UnityEngine;
using System.Collections;

public class PauseMenu
{
    private Texture mainBackground;

    private Rect splashArea;
    private Rect guiArea;
    private float buttonSpace;

    public PauseMenu()
    {
        mainBackground = Resources.Load(ResourcePaths.mainSplash) as Texture;

        buttonSpace = Screen.width / 10;
        float border = 0.05f;
        guiArea = new Rect(Screen.width * border, Screen.height * border, Screen.width * (1 - border * 2), Screen.height * (1 - border * 2));
        splashArea = new Rect(0, 0, Screen.width, Screen.height);
    }

    public MenuActions draw()
    {
        MenuActions menuAction = MenuActions.none;
        GUI.DrawTexture(splashArea, mainBackground, ScaleMode.ScaleAndCrop, false, 0);

        GUILayout.BeginArea(guiArea);
        GUILayout.BeginVertical();
        {
            GUILayout.Label("Galactoid", GuiFunctions.getTitleStyle());
            GUILayout.Label("Pause", GuiFunctions.getSubtitleStyle());

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(buttonSpace);
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Resume", GuiFunctions.getButtonStyle()))
                    menuAction = MenuActions.resumeGame;
                if (GUILayout.Button("Save & Exit", GuiFunctions.getButtonStyle()))
                    menuAction = MenuActions.gotoMainMenu;
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();

        return menuAction;
    }
}
