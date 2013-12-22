using UnityEngine;
using System.Collections;
using System;

public class BlockSelectMenu
{
    private ShipView shipView;
    private Texture background;
    private GUIStyle normalStyle;
    private int blockDataIndex = 0;
    private float slotWidth;

    private Rect backgroundRect;
    private GUILayoutOption[] slotGuiOptions;

    public BlockSelectMenu(ShipView shipView)
    {
        this.shipView = shipView;
        background = Resources.Load(ResourcePaths.toolSelectBackground) as Texture;
        normalStyle = MenuTemplate.getLabelStyle(60, TextAnchor.UpperCenter, Color.black);

        slotWidth = Screen.width / 12;
        float slotHeight = slotWidth * 2;

        float xSpace = slotWidth / 2;
        float ySpace = Screen.height / 2 - (float)(slotHeight * 1.2);
        backgroundRect = new Rect(xSpace, ySpace, Screen.width - xSpace * 2, Screen.height - ySpace * 2);

        slotGuiOptions = new GUILayoutOption[2] { GUILayout.Width(slotWidth), GUILayout.Height(slotHeight) };
    }

    private void drawClippedTexture(Texture texture, float textureX, float textureY, float textureWidth,
        float textureHeight, float x, float y, float scale)
    {
        /*
         * the box containing the piece of texture we want to draw is:
         * (textureX,textureY) and (textureX+textureWidth,textureY+textureHeight)
         * 
         * x and y are screen coorindates
         * 
         * scale is how big to stretch the texture
         * 
         * Far from optimized, but it works.
         */
        GUI.BeginGroup(new Rect(x, y, textureWidth * scale, textureHeight * scale));
        GUI.DrawTexture(new Rect(-textureX * scale, -textureY * scale, texture.width * scale, texture.height * scale), 
            texture, ScaleMode.StretchToFill);
        GUI.EndGroup();
    }

    public MenuAction draw()
    {
        MenuAction menuAction = null;
        GUI.DrawTexture(backgroundRect, background, ScaleMode.ScaleAndCrop, false, 0);

        GUILayout.BeginArea(backgroundRect);
        GUILayout.BeginVertical();
        GUILayout.Label("Assign blocks to keys 1-9", normalStyle);
        GUILayout.FlexibleSpace();

        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<", slotGuiOptions))
                scrollLeft();
            GUILayout.FlexibleSpace();

            Rect rect = GUILayoutUtility.GetLastRect();
            float scale=slotWidth/16;
            foreach (BlockStack blockStack in getBlockStacksForSlots())
            {
                drawClippedTexture(blockStack.blockData.texture,48,24,16,32,rect.xMin,rect.yMin,scale);
                rect.xMin += slotWidth;
            }

            if (GUILayout.Button(">", slotGuiOptions))
                scrollRight();

            GUILayout.EndHorizontal();
        }
        

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();

        return menuAction;
    }

    private void scrollLeft()
    {
        blockDataIndex--;
        checkBlockDataIndex();
    }

    private void scrollRight()
    {
        blockDataIndex++;
        checkBlockDataIndex();
    }

    private void checkBlockDataIndex()
    {
        ArrayList inventory = shipView.getBlockInventory();
        if (blockDataIndex < 0)
            blockDataIndex = inventory.Count - 1;
        if (blockDataIndex >= inventory.Count)
            blockDataIndex = 0;
    }

    private ArrayList getBlockStacksForSlots()
    {
        ArrayList inventory = shipView.getBlockInventory();
        int count = inventory.Count - blockDataIndex;
        count = count > 9 ? 9 : count;
        return inventory.GetRange(blockDataIndex, count);
    }
}

