using UnityEngine;
using System.Collections;
using System;

public class BlockSelectMenu
{
    private ShipInfo shipInfo;
    private Texture background;
    private GUIStyle normalStyle;
    private int blockDataIndex = 0;

    private BlockStack mouseDownBlockStack;

    private int slotPositions = 9;
    private float slotWidth;
    private float slotHeight;

    private Rect backgroundRect;
    private GUILayoutOption[] slotGuiOptions;

    public BlockStack[] selectedBlockStacks;

    public BlockSelectMenu(ShipInfo shipInfo)
    {
        this.shipInfo = shipInfo;
        background = Resources.Load(ResourcePaths.toolSelectBackground) as Texture;
        normalStyle = GuiFunctions.getNormalStyle(Color.black);

        slotWidth = Screen.width / 12;
        slotHeight = slotWidth * 2;

        float ySpace = Screen.height / 2 - (float)(slotHeight * 1.2);
        backgroundRect = new Rect(0, ySpace, Screen.width, Screen.height - ySpace * 2);

        slotGuiOptions = new GUILayoutOption[4] { GUILayout.Width(slotWidth), 
            GUILayout.Height(slotHeight),GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true) };
        selectedBlockStacks = new BlockStack[slotPositions];
    }

    public MenuActions draw()
    {
        MenuActions menuAction = MenuActions.none;
        GUI.DrawTexture(backgroundRect, background, ScaleMode.ScaleAndCrop, false, 0);

        GUILayout.BeginArea(backgroundRect);
        GUILayout.BeginVertical();
        GUILayout.Label("Assign blocks to keys 1-9", normalStyle);

        float slotScale = slotWidth / 16;
        ArrayList blockStacks = getBlockStacksForSlots();
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(ResourceLookup.getSideButtonTexture(0), slotGuiOptions))
                scrollLeft();

            if (blockStacks.Count == slotPositions)
                GUILayout.FlexibleSpace();

            BlockStack blockStack;
            for (int i = 0; i < blockStacks.Count; i++)
            {
                GUILayout.Box("", slotGuiOptions);
                if (blockStacks[i] == null)
                    break;
                blockStack = (BlockStack)blockStacks[i];
                Rect rect = GUILayoutUtility.GetLastRect();
                GuiFunctions.drawSlotTexture(blockStack.blockTexture, rect.xMin, rect.yMin, slotScale);

                if (Input.GetMouseButtonDown(0) && GuiFunctions.isMouseInGuiRect(rect))
                    mouseDownBlockStack = blockStack;

            }


            GUILayout.FlexibleSpace();

            if (GUILayout.Button(ResourceLookup.getSideButtonTexture(1), slotGuiOptions))
                scrollRight();
            GUILayout.EndHorizontal();
        }

        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            for (int i = 0; i < slotPositions; i++)
            {
                GUILayout.Box((i + 1).ToString(), slotGuiOptions);
                Rect rect = GUILayoutUtility.GetLastRect();
                if (selectedBlockStacks[i] != null)
                {
                    BlockStack blockStack = (BlockStack)selectedBlockStacks[i];
                    GuiFunctions.drawSlotTexture(blockStack.blockTexture, rect.xMin, rect.yMin, slotScale);
                }
                if (Input.GetMouseButtonUp(0) && mouseDownBlockStack != null && GuiFunctions.isMouseInGuiRect(rect))
                {
                    selectedBlockStacks[i] = mouseDownBlockStack;
                    mouseDownBlockStack = null;
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();

        if (mouseDownBlockStack != null)
        {
            GuiFunctions.drawSlotTexture(mouseDownBlockStack.blockTexture, Input.mousePosition.x - slotWidth / 2, Screen.height - Input.mousePosition.y - slotHeight / 2, slotScale);
            if (!Input.GetMouseButton(0) && Event.current.type == EventType.repaint)
                mouseDownBlockStack = null;
        }

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
        ArrayList inventory = shipInfo.getBlockInventory();
        if (blockDataIndex < 0)
            blockDataIndex = inventory.Count - 1;
        if (blockDataIndex >= inventory.Count)
            blockDataIndex = 0;
    }

    private ArrayList getBlockStacksForSlots()
    {
        ArrayList inventory = shipInfo.getBlockInventory();
        int count = inventory.Count - blockDataIndex;
        count = count > slotPositions ? slotPositions : count;
        return inventory.GetRange(blockDataIndex, count);
    }
}

