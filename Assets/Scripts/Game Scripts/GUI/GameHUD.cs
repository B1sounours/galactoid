using UnityEngine;
using System.Collections;

public class GameHud
{
    private GUIStyle normalStyle;
    private BlockSelectMenu blockSelectMenu;

    public BlockStack selectedBlockStack;
    private float selectedBlockScale;
    private Vector2 selectedBlockPosition;

    public GameHud(BlockSelectMenu blockSelectMenu)
    {
        normalStyle = MenuTemplate.getLabelStyle(30, TextAnchor.MiddleCenter, Color.white);
        this.blockSelectMenu = blockSelectMenu;

        float slotWidth = Screen.width / 20;
        selectedBlockScale = slotWidth / 16;
        selectedBlockPosition = new Vector2(Screen.width - (float)(slotWidth * 1.5),
            Screen.height - (float)(slotWidth * 2.5));
    }

    public int getBlockSelectButton()
    {
        //returns 1-9, or 0 if no block select key is being pressed.
        for (int i = 1; i <= 9; i++)
            if (Input.GetButton("Block " + i.ToString()))
                return i;
        return 0;
    }

    public void draw()
    {
        if (GameOptions.guiFPS)
        {
            int fps = (int)(1f / Time.deltaTime * Time.timeScale);
            GUILayout.Box("FPS " + fps);
        }

        if (GameOptions.guiReticule)
        {
            Vector2 size = GUI.skin.label.CalcSize(new GUIContent("+"));
            Rect rect = new Rect(0, 0, size.x, size.y);
            rect.center = new Vector2(Screen.width, Screen.height) / 2f;
            GUI.Label(rect, "+");
        }

        drawSelectedBlock();
    }

    public void drawSelectedBlock()
    {
        int blockSelectKey = getBlockSelectButton();

        if (blockSelectKey > 0 && blockSelectMenu.selectedBlockStacks[blockSelectKey-1] != null)
            selectedBlockStack = (BlockStack)blockSelectMenu.selectedBlockStacks[blockSelectKey - 1];

        if (selectedBlockStack != null)
            GuiFunctions.drawSlotTexture(selectedBlockStack.blockData.texture, selectedBlockPosition.x,
                selectedBlockPosition.y, selectedBlockScale);
    }

}
