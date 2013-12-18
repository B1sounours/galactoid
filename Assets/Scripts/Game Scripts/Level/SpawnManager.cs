using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager :MonoBehaviour
{
	public class SpawnEvent
	{
		public BlobData blobData;
		public float time;
		
		public SpawnEvent (BlobData blobData, float time)
		{
			this.blobData = blobData;
			this.time = time;
		}
	}
	
	private WorldManager worldManager;
	private LevelData levelData;
	private ArrayList spawnEvents;
	private float spawnTimeElapsed;
	private bool isLevelDone;
	
	void Awake ()
	{
		worldManager = (WorldManager)FindObjectOfType (typeof(WorldManager));
		levelData = worldManager.getLevelData ();
		setSpawnEvents ();
		isLevelDone=false;
	}
	
	void Update ()
	{
		spawnTimeElapsed += Time.deltaTime;
		runNextSpawn ();
		
		if (isSpawnFinished() && !GameSettings.sandboxEnabled && !isLevelDone && worldManager.isQuarantined())
			endLevel();
	}
	
	private void endLevel(){
		isLevelDone=true;
		worldManager.getPlayer().GetComponent<PlayerController>().setInput(false);
		GUIManager guiManager = (GUIManager)FindObjectOfType (typeof(GUIManager));
		guiManager.endLevel();
	}
	
	private bool isSpawnFinished(){
		return spawnEvents.Count == 0;
	}
	
	private void runNextSpawn ()
	{
		if (isSpawnFinished())
			return;
		
		SpawnEvent spawnEvent = (SpawnEvent)spawnEvents [0];
		
		if (spawnEvent.time > spawnTimeElapsed)
			return;
		
		worldManager.genBlob (spawnEvent.blobData);
		spawnEvents.RemoveAt (0);
	}
	
	private void setSpawnEvents ()
	{
		spawnEvents = new ArrayList ();
		
		foreach (WaveData waveData in levelData.waveDatas) {
			if (waveData.blobPrefab==null)
				continue;
			
			if (waveData.frequency.shower) {
				addShowerEvent (waveData);
			} else {
				addFrequencyEvent (waveData);
			}
		}
	}
	
	private void addShowerEvent (WaveData waveData)
	{
		//shower, ignore collision timing safeguards, same direction
		
		//gen direction and size to use on all shower blobs
		BlobData showerBlobData = worldManager.getBlobDataFromPrefab (waveData.blobPrefab);
		applyWaveData (showerBlobData, waveData, 0);
		
		int[,,] mainMapData = worldManager.getMainBlob ().getMapData ();
		IntVector3 direction=showerBlobData.getMoveDirectionUnitVector();
		IntVector2 areaSize = MDCollision.getCollisionArraySize (mainMapData, direction);
		IntVector2 cellSize = MDCollision.getCollisionArraySize (showerBlobData.getSizeVector(), direction);
		
		//Debug.Log ("cellsize: "+ZTools.toString(cellSize));
		
		int count=waveData.frequency.count;
		int spacing=waveData.frequency.showerSpacing;
		ArrayList points = ZTools.getGridSquares (areaSize, cellSize, count,spacing);
		ZTools.shuffleArraylist(points);
		
		int iterIndex = -1;
		foreach (IntVector2 point in points) {
			iterIndex++;
			
			BlobData blobData = worldManager.getBlobDataFromPrefab (waveData.blobPrefab);
			
			blobData.setMoveDirection (showerBlobData.getMoveDirection ());
			blobData.size = showerBlobData.size;
			blobData.setOffset (point);
			blobData.offset.guarenteeHit=false;
			
			float deltaTime = waveData.getDeltaTimeFromIndex (iterIndex);
			float spawnTime = waveData.getStartTime () + deltaTime;
			addSpawnEvent (blobData, spawnTime, false);
		}
	}
	
	private void addFrequencyEvent (WaveData waveData)
	{
		//no shower, use collision timing safeguards, potentially different directions
		for (int i=0; i<waveData.frequency.count; i++) {
			BlobData blobData = worldManager.getBlobDataFromPrefab (waveData.blobPrefab);
			
			applyWaveData (blobData, waveData, i);
				
			float deltaTime = waveData.getDeltaTimeFromIndex (i);
			float spawnTime = waveData.getStartTime () + deltaTime;
			addSpawnEvent (blobData, spawnTime, true);
		}
	}
	
	private void addSpawnEvent (BlobData blobData, float preferredSpawnTime, bool useFrequencySafeguard)
	{
		/*
		adds to the spawn event list, taking into account the direction and speed of the blobData
		
		useFrequencySafeguard ensures collisions will not be too close together
		*/
		
		float spawnTime = preferredSpawnTime;
		if (useFrequencySafeguard) {
			float safeSpawnTime = getSafeSpawnTime (blobData);			
			spawnTime = Mathf.Max (safeSpawnTime, preferredSpawnTime);
			if (spawnTime != preferredSpawnTime)
				Debug.Log ("info: used safeSpawnTime: " + safeSpawnTime + " instead of preferredSpawnTime: " + preferredSpawnTime);
		}
		
		SpawnEvent spawnEvent = new SpawnEvent (blobData, spawnTime);
		
		bool added = false;
		for (int i=0; i<spawnEvents.Count; i++) {
			SpawnEvent compareEvent = (SpawnEvent)spawnEvents [i];
			if (spawnEvent.time < compareEvent.time) {
				spawnEvents.Insert (i, spawnEvent);
				added = true;
				break;
			}
		}
		
		if (!added)
			spawnEvents.Add (spawnEvent);
		
	}
	
	private float getSafeSpawnTime (BlobData blobData)
	{
		//returns the earliest time it is safe to spawn the blob from its direction
		
		float highestCollisionTime = 0;
		foreach (SpawnEvent spawnEvent in spawnEvents) {
			if (spawnEvent.blobData.getMoveDirection () != blobData.getMoveDirection ())
				continue;
			float driftTime = spawnEvent.blobData.getDriftTime ();
			float collisionTime = driftTime + spawnEvent.time;
			
			highestCollisionTime = Mathf.Max (highestCollisionTime, collisionTime);
		}
		
		if (highestCollisionTime == 0) {
			return 0;
		} else {
			float safeTime = highestCollisionTime - blobData.getDriftTime ();
			safeTime += GameSettings.collisionWait;
			//Debug.Log (highestCollisionTime + " " + blobData.getDriftTime () + " " + GameSettings.collisionWait);
			return safeTime;
		}
		
	}
	
	private void applyWaveData (BlobData blobData, WaveData waveData, int frequencyIndex)
	{		
		Directions.Cardinal direction = waveData.genDirection ();
		if (direction != Directions.Cardinal.unknown)
			blobData.moveDirection = direction;
		
		float sizeMultiplier = waveData.genSizeMultiplier (frequencyIndex);
		IntVector3 blobSize = blobData.getSize ();
		blobData.setSize (blobSize * sizeMultiplier);
		
		Vector2 offsetMultiplier = waveData.genOffset ();
		IntVector3 mainSize = worldManager.getMainSizeVector ();
		IntVector2 collisionSize= MDCollision.getCollisionArraySize(blobData.getSizeVector(),blobData.getMoveDirectionUnitVector());
		blobData.offset.x = (int)(collisionSize.x * offsetMultiplier.x);
		blobData.offset.y = (int)(collisionSize.y * offsetMultiplier.y);
	}
	
}








