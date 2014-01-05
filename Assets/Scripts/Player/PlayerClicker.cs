using UnityEngine;
using System.Collections;

public class PlayerClicker : MonoBehaviour
{
    private Transform cameraTransform;
    private float timeSinceLastClick = 0;
    private GameManager gameManager;

    void Start()
    {
        //audioSource = gameObject.GetComponent<AudioSource> ();
        cameraTransform = gameObject.GetComponent<Camera>().camera.transform;
        gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
    }

    void Update()
    {
        timeSinceLastClick += Time.deltaTime;

        for (int mouseButton = 0; mouseButton < 2; mouseButton++)
        {
            if (Input.GetMouseButtonDown(mouseButton) || (Input.GetMouseButton(mouseButton) && timeSinceLastClick > 0.3))
            {
                timeSinceLastClick = 0;
                float range = 10;
                Vector3 pos = cameraTransform.position;
                RaycastHit rayCastHit = new RaycastHit();
                if (Physics.Linecast(pos, pos + cameraTransform.forward * range, out rayCastHit, 1))
                {
                    GameObject hitObject = rayCastHit.transform.gameObject;
                    IntVector3 clicked = new IntVector3(hitObject.transform.position);
                    if (GameOptions.mouseTool[mouseButton] == GameOptions.toolModes.placeBlock)
                    {
                        //IntVector3 adjacent = getAdjacentCoordFromClick(hitObject, rayCastHit.point);
                        IntVector3 adjacent = getAdjacentCoordFromRayHit(rayCastHit);
                        gameManager.requestPlaceBlock(adjacent);
                    }
                    else if (GameOptions.mouseTool[mouseButton] == GameOptions.toolModes.removeBlock)
                    {
                        gameManager.requestHarvestBlock(clicked);
                    }
                    else if (GameOptions.mouseTool[mouseButton] == GameOptions.toolModes.repairBlock)
                    {
                        Debug.Log("repair block is undefined");
                    }
                    else if (GameOptions.mouseTool[mouseButton] == GameOptions.toolModes.scanBlock)
                    {
                        Debug.Log("scan block is undefined");
                    }
                }
            }
        }
    }

    private IntVector3 getAdjacentCoordFromRayHit(RaycastHit rayCastHit)
    {
        /*
        Vector3 objectOrigin= rayCastHit.transform.parent.transform.position;
        Debug.Log("objectOrigin: " + objectOrigin);
        Vector3 relativePoint = rayCastHit.point - objectOrigin;

        IntVector3 adj = new IntVector3(Mathf.RoundToInt(relativePoint.x), Mathf.RoundToInt(relativePoint.y), Mathf.RoundToInt(relativePoint.z));
         */
        IntVector3 adj = new IntVector3(Mathf.FloorToInt(rayCastHit.point.x + 0.45f + rayCastHit.normal.x),
            Mathf.FloorToInt(rayCastHit.point.y + 0.45f + rayCastHit.normal.y),
            Mathf.FloorToInt(rayCastHit.point.z + 0.45f + rayCastHit.normal.z));
        if (rayCastHit.normal.x < -0.5)
            adj.x++;
        if (rayCastHit.normal.y < -0.5)
            adj.y++;
        if (rayCastHit.normal.z < -0.5)
            adj.z++;

        return adj;
    }

    private IntVector3 getAdjacentCoordFromBlockClick(Vector3 blockPosition, Vector3 clickPoint)
    {
        //for a block at position blockPosition which was just clicked at clickPoint, returns the blockPosition of the
        //new block that should be placed next to the original block.

        //note: after a raycast on a cube at (0,0,0), clickPoint x y z will be within [-0.5,0.5]
        Vector3 originClick = new Vector3(clickPoint.x - blockPosition.x,
            clickPoint.y - blockPosition.y,
            clickPoint.z - blockPosition.z);

        if (originClick.y < 0.55 && originClick.y > 0.45)
            return new IntVector3(blockPosition.x, blockPosition.y + 1, blockPosition.z);
        if (originClick.y > -0.55 && originClick.y < -0.45)
            return new IntVector3(blockPosition.x, blockPosition.y - 1, blockPosition.z);

        if (originClick.x < 0.55 && originClick.x > 0.45)
            return new IntVector3(blockPosition.x + 1, blockPosition.y, blockPosition.z);
        if (originClick.x > -0.55 && originClick.x < -0.45)
            return new IntVector3(blockPosition.x - 1, blockPosition.y, blockPosition.z);

        if (originClick.z < 0.55 && originClick.z > 0.45)
            return new IntVector3(blockPosition.x, blockPosition.y, blockPosition.z + 1);
        if (originClick.z > -0.55 && originClick.z < -0.45)
            return new IntVector3(blockPosition.x, blockPosition.y, blockPosition.z - 1);

        Debug.Log("warning! getAdjacentCoordFromClick failed for some reason");
        return new IntVector3();
    }

}
