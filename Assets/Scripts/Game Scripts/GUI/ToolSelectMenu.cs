using UnityEngine;
using System.Collections;
using System;

public class ToolSelectMenu
{
    private Texture background;
    private GUIStyle normalStyle;
    private GUIStyle tipStyle;

    private Rect backgroundRect;
    private Rect guiRect;
    private GUILayoutOption[] toolGuiOptions;
    private GUILayoutOption[] mouseGuiOptions;
    private float toolIconSpacing;

    private GuiManager gmm;

    public ToolSelectMenu(GuiManager gameMenuManager)
    {
        background = Resources.Load(ResourcePaths.toolSelectBackground) as Texture;
        normalStyle = MenuTemplate.getLabelStyle(40, TextAnchor.UpperCenter, Color.black);
        tipStyle = MenuTemplate.getLabelStyle(60, TextAnchor.UpperCenter, Color.black);
        gmm = gameMenuManager;

        int xSpace = Screen.width / 6;
        int ySpace = Screen.height / 6;
        backgroundRect = new Rect(xSpace, ySpace, Screen.width - xSpace * 2, Screen.height - ySpace * 2);

        guiRect = new Rect(backgroundRect.xMin + Screen.width / 40, backgroundRect.yMin + Screen.height / 40,
            backgroundRect.width - Screen.width / 20, backgroundRect.height - Screen.height / 20);

        float toolIconSize = guiRect.width / 5;
        float mouseIconSize = toolIconSize / 2;
        toolIconSpacing = toolIconSize / 4;
        toolGuiOptions = new GUILayoutOption[2] { GUILayout.Width(toolIconSize), GUILayout.Height(toolIconSize) };
        mouseGuiOptions = new GUILayoutOption[2] { GUILayout.Width(mouseIconSize), GUILayout.Height(mouseIconSize) };
    }

    public MenuAction draw()
    {
        MenuAction menuAction = null;
        GUI.DrawTexture(backgroundRect, background, ScaleMode.ScaleAndCrop, false, 0);

        GUILayout.BeginArea(guiRect);
        GUILayout.BeginVertical();
        {
            GUILayout.Label("Set left and right mouse buttons", normalStyle);

            GUILayout.BeginHorizontal();
            foreach (GameOptions.toolModes toolMode in Enum.GetValues(typeof(GameOptions.toolModes)))
            {
                if (GUILayout.Button(ResourceLookup.getToolModeTexture(toolMode), toolGuiOptions))
                    setMouseButton(toolMode);
                GUILayout.Space(toolIconSpacing);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(toolIconSpacing);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            drawMouseIcon(0);
            GUILayout.FlexibleSpace();
            drawMouseIcon(1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();

        return menuAction;
    }

    private void drawMouseIcon(int mouseCode)
    {
        GUILayout.BeginVertical();
        if (mouseCode == 0)
            GUILayout.Label("Left click", tipStyle);

        if (mouseCode == 1)
            GUILayout.Label("Right click", tipStyle);

        GUILayout.Box(ResourceLookup.getToolModeTexture(GameOptions.mouseTool[mouseCode]), mouseGuiOptions);
        
        GUILayout.EndVertical();
    }

    private void setMouseButton(GameOptions.toolModes toolMode)
    {
        Debug.Log("setMouseButton for " + toolMode);
        if (gmm.lastMouseButton < 0 || gmm.lastMouseButton > 1)
        {
            Debug.Log("Error. ran setMouseButton but left and right mouse weren't clicking?");
        }
        else
        {
            GameOptions.mouseTool[gmm.lastMouseButton] = toolMode;
        }
    }

}

