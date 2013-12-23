using UnityEngine;
using System.Collections;

//this class contains functions that are useful for all gui classes

public static class GuiFunctions {

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
         * scale is how big to stretch the texture
         */
        GUI.BeginGroup(new Rect(x, y, textureWidth * scale, textureHeight * scale));
        GUI.DrawTexture(new Rect(-textureX * scale, -textureY * scale, texture.width * scale, texture.height * scale),
            texture, ScaleMode.StretchToFill);
        GUI.EndGroup();
    }
}
