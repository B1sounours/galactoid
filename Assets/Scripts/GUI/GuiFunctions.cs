using UnityEngine;
using System.Collections;

//this class contains functions that are useful for all gui classes

public static class GuiFunctions
{
    public static void drawSlotTexture(Texture texture, float x, float y, float slotScale)
    {
        //this function knows how to clip a blockTexture for use as a slotTexture
        GuiFunctions.drawClippedTexture(texture, 48, 24, 16, 32, x, y, slotScale);
    }

    public static bool isMouseInGuiRect(Rect rect)
    {
        /* to be used only when rect is from GetLastRect()
         * 
         * the origin for Input.mousePosition is screen bottom left, but the origin
         * for GUILayoutUtility.GetLastRect() is screen top left. Also, GetLastRect uses
         * relative coordinates and mousePosition uses absolute. This handles that bullshit.
         */
        Vector2 newRectCoords = GUIUtility.GUIToScreenPoint(new Vector2(rect.xMin, rect.yMin));
        Rect realRect = new Rect(newRectCoords.x, newRectCoords.y, rect.width, rect.height);
        if (Input.mousePosition.x >= realRect.xMin && Input.mousePosition.x <= realRect.xMax)
        {
            float realMouseY = Screen.height - Input.mousePosition.y;
            if (realMouseY >= realRect.yMin && realMouseY <= realRect.yMax)
                return true;
        }
        return false;
    }

    public static void drawClippedTexture(Texture texture, float textureX, float textureY, float textureWidth,
    float textureHeight, float x, float y, float scale)
    {
        /*
         * the box containing the piece of texture we want to draw is:
         * (textureX,textureY) and (textureX+textureWidth,textureY+textureHeight)
         * 
         * x and y are screen coorindates
         * 
         * scale is how big to stretch the texture where 1.0 is no stretch
         */
        GUI.BeginGroup(new Rect(x, y, textureWidth * scale, textureHeight * scale));
        GUI.DrawTexture(new Rect(-textureX * scale, -textureY * scale, texture.width * scale, texture.height * scale),
            texture, ScaleMode.StretchToFill);
        GUI.EndGroup();
    }


    public static GUIStyle getLabelStyle(int fontSize)
    {
        GUIStyle newStyle = new GUIStyle();
        newStyle.font = Resources.Load(ResourcePaths.mainFont) as Font;
        newStyle.normal.textColor = Color.white;
        newStyle.alignment = TextAnchor.MiddleLeft;
        newStyle.fontSize = Screen.height * fontSize / 500;

        return newStyle;
    }

    private static GUIStyle titleStyle;
    public static GUIStyle getTitleStyle()
    {
        if (titleStyle == null)
        {
            titleStyle = getLabelStyle(48);
            titleStyle.alignment = TextAnchor.UpperCenter;
        }
        return titleStyle;
    }
    private static GUIStyle subtitleStyle;
    public static GUIStyle getSubtitleStyle()
    {
        if (subtitleStyle == null)
        {
            subtitleStyle = getLabelStyle(32);
            subtitleStyle.alignment = TextAnchor.UpperCenter;
        }
        return subtitleStyle;
    }
    private static GUIStyle buttonStyle;
    public static GUIStyle getButtonStyle()
    {
        if (buttonStyle == null)
        {
            buttonStyle = getLabelStyle(24);
            buttonStyle.alignment = TextAnchor.UpperLeft;
            buttonStyle.normal.textColor = new Color(230, 230, 255);
        }
        return buttonStyle;
    }
    private static GUIStyle normalStyle;
    public static GUIStyle getNormalStyle(Color color)
    {
        if (normalStyle == null)
        {
            normalStyle = getLabelStyle(24);
            normalStyle.alignment = TextAnchor.UpperCenter;
            normalStyle.normal.textColor = color;
        }
        return normalStyle;
    }
    private static GUIStyle tipStyle;
    public static GUIStyle getTipStyle()
    {
        if (tipStyle == null)
        {
            tipStyle = getLabelStyle(12);
            tipStyle.alignment = TextAnchor.UpperCenter;
            tipStyle.normal.textColor = Color.black;
        }
        return tipStyle;
    }
}
