using UnityEngine;
using System.Collections;

public class LevelData : MonoBehaviour
{
	public string levelName="no name";
	public bool useTutorial=false;
	public GameObject firstBlobPrefab;
	public WaveData[] waveDatas;
	public GameObject[] spaceMakers;
	public GameObject musicManager;
	
	
	public BlobData getFirstBlob ()
	{
		if (firstBlobPrefab == null) {
			firstBlobPrefab = Resources.Load ("Prefabs/Blobs/default") as GameObject;
			Debug.Log ("incomplete levelData: using default blob for first blob");
		}
		
		WorldManager worldManager = (WorldManager)FindObjectOfType (typeof(WorldManager));
		
		return worldManager.getBlobDataFromPrefab(firstBlobPrefab);
	}
	
	public void genMusicManager ()
	{
		if (musicManager == null) {
			musicManager = Resources.Load ("Prefabs/Music Managers/default") as GameObject;
			Debug.Log ("incomplete levelData: using default musicManager");
		}
		
		Instantiate (musicManager);
	}
}
