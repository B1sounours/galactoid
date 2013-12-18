using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour
{
	private GameObject player;
	private GameObject voxelPrefab;
	public Hashtable blobs;
	private string mainBlobName;
	private ArrayList collisionManagers;
	private int blobCount;
	private int lowestHeight;
	private Prism worldPrism;
	private LevelData levelData;
	private SpawnManager spawnManager;
	private GameObject blobManagersParent;
	public GameObject blobDatasParent;
	private Camera backgroundCamera;
	private VoxelDataManager voxelDataManager;

	void Awake ()
	{
		blobs = new Hashtable ();
		collisionManagers = new ArrayList ();
		blobManagersParent = new GameObject ("Blob Managers");
		blobDatasParent = new GameObject ("Blob Datas");
		voxelDataManager = gameObject.AddComponent<VoxelDataManager> ();
		gameObject.AddComponent<GUIManager> ();
		setFirstBlob ();
		gameObject.AddComponent<SpawnManager> ();
		genSpaceMakers ();
		resetPlayerPosition ();
		getLevelData ().genMusicManager ();
	}
	
	void Update ()
	{
		if (collisionManagers.Count > 0) {
			//Debug.Log("starting subtask");
			CollisionManager collisionManager = (CollisionManager)collisionManagers [0];
			collisionManager.runSubtask ();
			//Debug.Log ("done subtask "+collisionManager.subtaskCounter);
			
			if (collisionManager.isDone) {
				//Debug.Log("is done! "+collisionManager.subtaskCounter);
				collisionManagers.Remove (collisionManager);
				applyCollisionManager (collisionManager);
			}
		}
	}
	
	public bool isQuarantined ()
	{
		bool isStagnant = true;
		foreach (DictionaryEntry pair in blobs) {
			if (!((BlobManager)pair.Value).isQuarantined ()) {
				isStagnant = false;
				break;
			}
		}
		return isStagnant;
	}
	
	public Prism  getWorldPrism(){
		BlobManager blobManager=(BlobManager) blobs[mainBlobName];
		return blobManager.getPrism();
	}
	
	public bool isPlayerInsideWorld ()
	{
		
		if (player.transform.position.x < getWorldPrism().x - GameSettings.leavingMapDistance)
			return false;
		if (player.transform.position.y <  getWorldPrism().y - GameSettings.leavingMapDistance)
			return false;
		if (player.transform.position.z <  getWorldPrism().z - GameSettings.leavingMapDistance)
			return false;
		
		if (player.transform.position.x >  getWorldPrism().x +  getWorldPrism().width + GameSettings.leavingMapDistance)
			return false;
		if (player.transform.position.y >  getWorldPrism().y +  getWorldPrism().height + GameSettings.leavingMapDistance)
			return false;
		if (player.transform.position.z >  getWorldPrism().z +  getWorldPrism().depth + GameSettings.leavingMapDistance)
			return false;
		
		return true;
	}
	
	public void updateWorldDomain (Prism blobPrism)
	{
		/*
		enlarges the world domain to include blobPrism
		*/
		
		IntVector3[] points = new IntVector3[4]{
			worldPrism.getLowestPoint (),
			worldPrism.getHighestPoint (),
			blobPrism.getLowestPoint (),
			blobPrism.getHighestPoint ()};
		
		worldPrism = ZTools.getPrismFromPoints (points);
	}
	
	public LevelData getLevelData ()
	{
		if (levelData == null) {
			GameObject levelDataPrefab = GameSettings.getLevelDataPrefab ();
			GameObject levelDataGO = (GameObject)Instantiate (levelDataPrefab);
			levelDataGO.name = "Level Data";
			levelData = levelDataGO.GetComponent<LevelData> ();
		}
		return levelData;
	}
	
	private GameObject[] getSpaceMakers ()
	{
		if (getLevelData ().spaceMakers.GetLength (0) == 0) {
			Debug.Log ("worldManager is using default spaceMaker.");
			GameObject go = Resources.Load ("Prefabs/Space Makers/default") as GameObject;
			return new GameObject[1] {go};
		} else {
			//Debug.Log("getSpaceMakers for "+levelData.name);
			return getLevelData ().spaceMakers;
		}
	}
	
	private void genSpaceMakers ()
	{
		Vector3 pos = new Vector3 (0, 0, 0);
		int counter = 0;
		GameObject spaceMakerParent = new GameObject ("Space Makers");
		foreach (GameObject spaceMaker in getSpaceMakers()) {
			counter++;
			GameObject go = (GameObject)Instantiate (spaceMaker, pos, transform.rotation);
			go.name = "Space Maker " + counter;
			go.transform.parent = spaceMakerParent.transform;
		}			
	}
	
	private void setFirstBlob ()
	{
		blobCount = 0;
		BlobData blobData = getLevelData ().getFirstBlob ();
		int [,,] mapData = blobData.genMapData ();
		
		BlobManager blobManager = startBlob (getNewBlob (), mapData, new IntVector3 (0, 0, 0));
		mainBlobName = blobManager.blobName;
		blobManager.setBlobData (blobData);
		blobManager.createVoxels ();
		
		worldPrism = blobManager.getPrism ();
	}
	
	private string getNextBlobName ()
	{
		blobCount++;
		return "blob" + blobCount;
	}
	
	private void addToBlobs (BlobManager blob)
	{
		blobs [blob.blobName] = blob;
	}
	
	public BlobData getBlobDataFromPrefab (GameObject blobDataPrefab)
	{
		GameObject go = Instantiate (blobDataPrefab) as GameObject;
		BlobData blobData = go.GetComponent<BlobData> ();
		go.transform.parent = blobDatasParent.transform;
		return blobData;
	}
	
	public int getFinalVoxelCount(){
		BlobManager blob=getBlob(getMainBlobName());
		return blob.getVoxelCount();
	}
	
	public GameObject getVoxelPrefab ()
	{
		if (voxelPrefab == null)
			voxelPrefab = Resources.Load ("Prefabs/Voxel") as GameObject;
		
		return voxelPrefab;
	}
	
	public IntVector3 getMainSizeVector ()
	{
		BlobManager blobManager = getBlob (getMainBlobName ());
		return blobManager.getBlobData ().getSizeVector ();
	}
	
	public BlobData getMainBlob ()
	{
		return getBlob (mainBlobName).getBlobData ();
	}
	
	private BlobManager getNewBlob (string blobName, bool setKey)
	{
		//Vector3 vector = new Vector3 (0, 0, 0);
		GameObject blobGameObject = new GameObject (blobName);
		blobGameObject.transform.parent = blobManagersParent.transform;
		BlobManager blobManager = blobGameObject.AddComponent<BlobManager> ();
		
		blobManager.blobName = blobName;
		
		if (setKey)
			addToBlobs (blobManager);
		
		return blobManager;
	}
	
	public BlobManager getNewBlob ()
	{
		string blobName = getNextBlobName ();
		return getNewBlob (blobName, true);
	}
	
	public Vector3 getResetPosition ()
	{
		//gives a safe coordinate to drop the player back into the map. tries to do center.
		BlobManager blobManager = getBlob (mainBlobName);
		IntVector3 moveDirection = new IntVector3 (0, -1, 0);
		Prism prism = blobManager.getPrism ();
		
		//IntVector3 point = MDCollision.getCenterCollisionPoint (blobManager.mapData, moveDirection);
		IntVector2 preferredPoint = new IntVector2 (prism.width / 2, prism.depth / 2);
		IntVector2 point = MDCollision.getCollisionPoint (blobManager.mapData, preferredPoint, moveDirection);
		
		float i = prism.x + point.x;
		float j = prism.y + prism.height + 2;
		float k = prism.z + point.y;
		
		return new Vector3 (i, j, k);
	}
	
	private BlobManager startBlob (BlobManager blobManager, int [,,] mapData, IntVector3 position)
	{
		blobManager.setInitial (mapData, position, blobManager.blobName);
		blobManager.transform.position = new Vector3 (position.x, position.y, position.z);
		
		return blobManager;
	}
	
	public void removeBlob (string blobName)
	{
		if (blobs.ContainsKey (blobName)) {
			//Debug.Log("removeBlob "+blobName);
			BlobManager blobManager = getBlob (blobName);
			Destroy (blobManager.gameObject, 0);
			blobs.Remove (blobName);
		} else {
			Debug.Log ("removeBlob failed, key not in blobs.");
		}
	}
	
	private void collideBlob (string blobName)
	{
		/*
		sets up a collisionManager and adds it to the list of work to be done in pieces every frame
		*/
		
		//Debug.Log ("collideBlob for " + blobName);
		
		BlobManager blob1 = getBlob (mainBlobName);
		BlobManager blob2 = getBlob (blobName);
		
		blob1.setCollisionStatus ();
		blob2.setCollisionStatus ();
		
		Prism newPrism = CollisionManager.getPrismFromBlobs (blob1, blob2);
		BlobManager newBlob;
		if (blob1.getPrism ().isPrismEqual (newPrism)) {
			newBlob = null;
		} else {
			newBlob = getNewBlob (getNextBlobName (), false);
		}
		
		CollisionManager collisionManager = new CollisionManager (this, blobName, newBlob);
		collisionManagers.Add (collisionManager);
	}
	
	private void applyCollisionManager (CollisionManager collisionManager)
	{
		//Debug.Log ("applyCollisionManager " + collisionManager.blobName1 + " " + collisionManager.blobName2);
		BlobManager newBlob = collisionManager.getBlobManager ();
		addToBlobs (newBlob);
		startBlob (newBlob, collisionManager.newMapData, newBlob.getPrism ().getPosition ());
		mainBlobName = newBlob.blobName;
		
		newBlob.voxelCount=collisionManager.blob1.voxelCount+collisionManager.blob2.voxelCount;
		
		if (!collisionManager.isBlob2InsideBlob1)
			removeBlob (collisionManager.blob1.blobName);
		removeBlob (collisionManager.blob2.blobName);
	}
	
	public VoxelDataManager getVoxelDataManager ()
	{
		return voxelDataManager;
	}
	
	private IntVector3 getBlobsOffset (IntVector3 movingPosition, IntVector3 mapPosition)
	{
		int i = movingPosition.x - mapPosition.x;
		int j = movingPosition.y - mapPosition.y;
		int k = movingPosition.z - mapPosition.z;
		return new IntVector3 (i, j, k);
	}
	
	private void resetPlayerPosition ()
	{	
		getPlayer ().transform.position = getResetPosition ();
	}
	
	public string getMainBlobName ()
	{
		return mainBlobName;
	}
	
	public GameObject getPlayer ()
	{
		if (player == null)
			player = PlayerGenerator.genPlayer ();
		
		return player;
	}
	
	public int getExpectedDistance (string blobName)
	{
		//this function allows blobs to ask the world manager for a new expected distance
		BlobManager mainBlob = getBlob (mainBlobName);
		BlobManager blob = getBlob (blobName);
		IntVector3 offset = getBlobsOffset (blob.position, mainBlob.position);
		//Debug.Log(mainBlob.mapData.GetLength(0));
		return MDCollision.getCollisionDistance (blob.mapData, mainBlob.mapData, offset);
	}
	
	public void blobFinishedExpectedDistance (string blobName)
	{
		/*
		this function is how blobs tell the world manager that they have travelled as far as worldmanager
		told them to. it was expected at the time of intialization that this distance is enough to collide
		with something. either increase expected distance, or run a collision.
		*/
		
		BlobManager blob = getBlob (blobName);
		int expectedDistance = getExpectedDistance (blobName);
		if (expectedDistance == -1) {
			//Debug.Log ("removing " + blobName + " because expectedDistance == -1");
			removeBlob (blobName);
		} else if (blob.getDistanceTravelled () >= expectedDistance) {
			//Debug.Log ("getDistanceTravelled " + blob.getDistanceTravelled () + " expectedDistance " + expectedDistance);
			collideBlob (blobName);
		} else {
			blob.setExpectedDistance (expectedDistance);
		}
	}
	
	public BlobManager genBlob (BlobData blobData)
	{
		/*
		this is all worldManager needs to create a blob using blobData
		*/
		
		IntVector3 startPosition = getBlobStartPosition (blobData);
		BlobManager blobManager = startBlob (getNewBlob (), blobData.getMapData (), startPosition);
		
		blobManager.setBlobData (blobData);
		blobManager.createVoxels ();
		
		blobManager.setVelocity (blobData.getVelocity (), getDriftFuel (blobData));
		return blobManager;
	}
	
	private int getDriftFuel (BlobData blobData)
	{
		/*
		if the blob spawns, and misses the main platform, this is the total distance
		it will cover before disappearing off the other side of the map.
		*/
		BlobManager mainBlob = getBlob (mainBlobName);
		IntVector3 vec = blobData.getMoveDirectionUnitVector ();
		
		int fuel = GameSettings.blobSpawnDistance * 2;
		fuel += mainBlob.getPrism ().width * Mathf.Abs (vec.x);
		fuel += mainBlob.getPrism ().height * Mathf.Abs (vec.y);
		fuel += mainBlob.getPrism ().depth * Mathf.Abs (vec.z);
		
		return fuel;
	}
	
	private BlobManager getBlob (string blobName)
	{
		BlobManager blobManager = (BlobManager)blobs [blobName];
		if (blobManager == null)
			Debug.Log ("warning: worldManager pulled a null blob: " + blobName);
		return blobManager;
	}
	
	private IntVector3 getBlobStartPosition (BlobData blobData)
	{
		Prism mainPrism = getBlob (mainBlobName).genPrism ();
		IntVector3 vec = blobData.getMoveDirectionUnitVector ();
		IntVector3 pos = new IntVector3 (mainPrism.x, mainPrism.y, mainPrism.z);
		
		int xOffset = blobData.offset.x;
		int yOffset = blobData.offset.y;
		
		if (blobData.offset.guarenteeHit) {
			
			IntVector3 moveDirection = blobData.getMoveDirectionUnitVector ();
			IntVector2 blobOffset = MDCollision.getCenterCollisionPoint (blobData.getMapData (), moveDirection);
			
			IntVector2 preferredPoint = new IntVector2 (xOffset, yOffset);
			int[,,] mainMapData = ((BlobManager)getBlob (mainBlobName)).mapData;
			IntVector2 mainOffset = MDCollision.getCollisionPoint (mainMapData, preferredPoint, moveDirection);
			
			xOffset = mainOffset.x - blobOffset.x;
			yOffset = mainOffset.y - blobOffset.y;
		}
		
		IntVector3 sizeVector = blobData.getSizeVector ();
		int nextOffset = xOffset;
		if (vec.x == 0) {
			pos.x += nextOffset;
			nextOffset = yOffset;
		} else {
			pos.x += vec.x * GameSettings.blobSpawnDistance * -1;
			if (vec.x == 1)
				pos.x -= sizeVector.x;
			if (vec.x == -1)
				pos.x += mainPrism.width;
		}
			
		if (vec.z == 0) {
			pos.z += nextOffset;
			nextOffset = yOffset;
		} else {
			pos.z += vec.z * GameSettings.blobSpawnDistance * -1;
			if (vec.z == 1)
				pos.z -= sizeVector.z;
			if (vec.z == -1)
				pos.z += mainPrism.depth;
		}
		
		if (vec.y == 0) {
			pos.y += nextOffset;
		} else {
			pos.y += vec.y * GameSettings.blobSpawnDistance * -1;
			if (vec.y == 1)
				pos.y -= sizeVector.y;
			if (vec.y == -1)
				pos.y += mainPrism.height;
		}
		return pos;
	}
	
}
