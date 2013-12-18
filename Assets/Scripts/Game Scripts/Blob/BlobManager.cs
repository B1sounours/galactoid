using UnityEngine;
using System.Collections;

public class BlobManager : MonoBehaviour
{
	public struct BlobAction
	{
		/*
		when a blob isn't finished being built by collisionManager, but has been sent a command like 
		setDestroy or setCorrupt, it must store the command to execute when it's ready.
		*/
		public IntVector3 point;
		public string action;

		public BlobAction (IntVector3 point, string action)
		{
			this.point = point;
			this.action = action;
		}
	}
	
	public BlobData blobData;
	public string blobName;
	public int[,,] mapData;
	public GameObject[,,] voxels;
	public GameObject voxelPrefab;
	public bool isInitialized;
	private int iSize;
	private int jSize;
	private int kSize;
	private IntVector3 direction;
	private float driftFuel;
	public int voxelCount;
	private int expectedDistance;
	private Vector3 velocity;
	private int lastDistanceTravelled;
	private ArrayList blobActions;
	private bool isMoving;
	public bool isColliding;
	public bool isDrifting;
	public IntVector3 position;
	private float redTimer;
	private ViroidManager viroidManager;
	private WorldManager worldManager;
	private Prism prism;
	private ViroidSpecies viroidSpecies;
	private PlayerCounter playerCounter;
	
	void Awake ()
	{
		viroidManager = new ViroidManager (this);
		worldManager = (WorldManager)FindObjectOfType (typeof(WorldManager));
		prism = new Prism (0, 0, 0, 0, 0, 0);
		redTimer = 0;
		lastDistanceTravelled = 0;
		driftFuel = 0;
		voxelCount = 0;
		isInitialized = false;
		isColliding = false;
		isDrifting = false;
		blobActions = new ArrayList ();
	}
	
	void Update ()
	{
		if (isMoving) {
			this.transform.position += velocity * Time.deltaTime;
			
			if (! isDrifting) {
				int distanceTravelled = getDistanceTravelled ();
				if (distanceTravelled != lastDistanceTravelled) {
					lastDistanceTravelled = distanceTravelled;
					//worldManager.updateWorldDomain (genPrism ());
					expectedDistance = worldManager.getExpectedDistance (blobName);
					//Debug.Log (blobName+ " travelled: "+getDistanceTravelled () + " expected: " + expectedDistance);
					if (expectedDistance > -1 && distanceTravelled >= expectedDistance)
						worldManager.blobFinishedExpectedDistance (blobName);
				}
			}
			driftFuel -= (velocity.x + velocity.y + velocity.z) * Time.deltaTime;
			if (driftFuel < 0)
				worldManager.removeBlob (blobName);
		}
		
		if (isInitialized && (!isMoving || isDrifting || getBlobData ().viroid.spreadWhileArriving)) {
			redTimer -= Time.deltaTime;
			if (redTimer < 0) {
				viroidManager.runHeartbeat ();
				redTimer = GameSettings.redInterval;
			}
		}
	}
	
	void onDestroy ()
	{
		destroyAllVoxels ();
	}
	
	private void destroyAllVoxels ()
	{
		voxelCount = 0;
		for (int i=0; i<iSize; i++) {
			for (int j=0; j<jSize; j++) {
				for (int k=0; k<kSize; k++) {
					GameObject voxel = voxels [i, j, k];
					if (voxel != null)
						Destroy (voxel, 0);
				}
			}
		}
	}
	
	private PlayerCounter getPlayerCounter ()
	{
		if (playerCounter == null)
			playerCounter = (PlayerCounter)FindObjectOfType (typeof(PlayerCounter));
		
		return playerCounter;
	}
	
	public BlobData getBlobData ()
	{
		if (blobData == null)
			blobData = worldManager.getLevelData ().getFirstBlob ();
		return blobData;
	}
	
	public bool isContagious (IntVector3 point)
	{
		//returns true if i,j,k is infected and next to healthy
		if (! isInsideBlob (point))
			return false;
		if (! isCorrupt (point))
			return false;
		
		for (int iOffset=-1; iOffset<=1; iOffset++) {
			for (int jOffset=-1; jOffset<=1; jOffset++) {
				for (int kOffset=-1; kOffset<=1; kOffset++) {
					IntVector3 iterPoint = new IntVector3 (point.x + iOffset, point.y + jOffset, point.z + kOffset);
					if (MDView.isVoxelOccupied (mapData, iterPoint) && 
						! isCorrupt (iterPoint))
						return true;
				}
			}
		}
		
		return false;
	}
	
	public bool isQuarantined ()
	{
		return viroidManager.isQuarantined () || isDrifting || expectedDistance == -1;
	}
	
	public bool isCorrupt (IntVector3 point)
	{
		if (! MDView.isVoxelOccupied (mapData, point))
			return false;
		
		GameObject voxel = voxels [point.x, point.y, point.z];
		if (voxel == null)
			return false;
		VoxelManager vm = voxel.GetComponent<VoxelManager> ();
		return vm.isCorrupt ();
	}
	
	public bool isHealthy (IntVector3 point)
	{
		if (! MDView.isVoxelOccupied (mapData, point))
			return false;
		
		GameObject voxel = voxels [point.x, point.y, point.z];
		if (voxel == null)
			return false;
		VoxelManager vm = voxel.GetComponent<VoxelManager> ();
		return !vm.isCorrupt ();
	}
	
	public bool isInsideBlob (IntVector3 point)
	{
		return MDView.isInsideMapData (mapData, point);
	}
	
	public IntVector3 getRealPosition ()
	{
		float x = transform.position.x;
		float y = transform.position.y;
		float z = transform.position.z;
		
		int i = position.x > transform.position.x ? Mathf.CeilToInt (x) : Mathf.FloorToInt (x);
		int j = position.y > transform.position.y ? Mathf.CeilToInt (y) : Mathf.FloorToInt (y);
		int k = position.z > transform.position.z ? Mathf.CeilToInt (z) : Mathf.FloorToInt (z);
		
		return new IntVector3 (i, j, k);
	}
	
	public int getDistanceTravelled ()
	{
		IntVector3 realPosition = getRealPosition ();
		int iDistance = Mathf.Abs (position.x - realPosition.x);
		int jDistance = Mathf.Abs (position.y - realPosition.y);
		int kDistance = Mathf.Abs (position.z - realPosition.z);
		
		return iDistance + jDistance + kDistance;
	}
	
	public Prism genPrism ()
	{
		/*
		returns a prism which is an approximation of where that blob is.
		*/
		IntVector3 realPosition = getRealPosition ();
		prism = new Prism (realPosition.x, realPosition.y, realPosition.z, iSize, jSize, kSize);
		return prism;
	}
	
	public Prism getPrism ()
	{
		//runs genPrism only if necessary, only size is valid, position may have changed
		if (prism.volume == 0)
			genPrism ();
		return prism;
	}
	
	public void setBroken (IntVector3 point)
	{
		/*
		allows you to set a voxel to broken. kills that voxel, and potentially detaches other voxels
		
		future optimization:
		we iterate over detachedVoxels twice. ick
		*/
		
		//Debug.Log ("setBroken: i: " + i + " j: " + j + " k: " + k);
		
		if (isColliding) {
			blobActions.Add (new BlobAction (point, "setBroken"));
			//Debug.Log ("adding setBroken");
		}
		
		getPlayerCounter ().breakCount++;
		if (isCorrupt (point))
			getPlayerCounter ().corruptBreakCount++;
		
		if (Input.GetMouseButton (1))
			getPlayerCounter ().laserBreakCount++;
		
		setDestroyed (point);
		int stopCode = worldManager.getVoxelDataManager ().getWithName ("gravity plate").voxelCode;
		ArrayList detachedVoxels = MDPathfinder.findDetachedVoxels (mapData, stopCode, point);
		
		if (detachedVoxels.Count == 0)
			return;
		
		Prism detachedPrism = ZTools.getPrismFromPoints (detachedVoxels);
		
		int[,,] newMapData = new int[detachedPrism.width, detachedPrism.height, detachedPrism.depth];
		GameObject[,,] newVoxelGrid = new GameObject[detachedPrism.width, detachedPrism.height, detachedPrism.depth];
		
		IntVector3 newPosition = new IntVector3 (position.x + detachedPrism.x, position.y + detachedPrism.y, position.z + detachedPrism.z);
		
		BlobManager newBlobManager = worldManager.getNewBlob ();
		ArrayList newContagiousList = new ArrayList ();
		
		foreach (IntVector3 vector in detachedVoxels) {	
			int newi = vector.x - detachedPrism.x;
			int newj = vector.y - detachedPrism.y;
			int newk = vector.z - detachedPrism.z;
			
			newMapData [newi, newj, newk] = mapData [vector.x, vector.y, vector.z];
			GameObject voxel = voxels [vector.x, vector.y, vector.z];
			if (voxel != null) {
				VoxelManager voxelManager = voxel.GetComponent<VoxelManager> ();
				voxelManager.reassign (newBlobManager, new IntVector3 (newi, newj, newk));
				newVoxelGrid [newi, newj, newk] = voxel;
			
				if (voxelManager.isCorrupt ())
					newContagiousList.Add (new IntVector3 (newi, newj, newk));
			}
				
			mapData [vector.x, vector.y, vector.z] = 0;
			voxels [vector.x, vector.y, vector.z] = null;

		}
		
		Vector3 newVelocity = new Vector3 (velocity.x, GameSettings.detachSpeed, velocity.z);
		
		newBlobManager.setInitial (newMapData, newPosition, newBlobManager.blobName);
		newBlobManager.setVelocity (newVelocity, GameSettings.detachDistance);
		newBlobManager.setAdrift ();
		newBlobManager.setVoxelGrid (newVoxelGrid, true);
		newBlobManager.setContagiousList (newContagiousList);
	}
	
	public void setCollisionStatus ()
	{
		setStopped ();
		snapToGrid ();
		isColliding = true;
		direction = new IntVector3 (0, 0, 0);
		
	}
	
	private void setAdrift ()
	{
		driftFuel = GameSettings.detachDistance;
		isDrifting = true;
	}
	
	public void setCorruptAll ()
	{
		foreach (GameObject voxel in voxels) {
			if (voxel != null)
				voxel.GetComponent<VoxelManager> ().setCorrupt ();
		}
	}
	
	public void setCorrupt (IntVector3 point)
	{		
		if (isColliding) {
			blobActions.Add (new BlobAction (point, "setCorrupt"));
			//Debug.Log ("adding blobAction setCorrupt "+ZTools.toString(point));
		}
		
		if (! MDView.isVoxelOccupied (mapData, point)) {
			createVoxel (1111, point);
			Debug.Log ("setCorrupt for " + name + " failed. " + ZTools.toString (point) + " not occupied. creating placeholder");
			return;
		}
		GameObject voxelGO = voxels [point.x, point.y, point.z];
		if (voxelGO == null) {
			Debug.Log ("setCorrupt for " + name + " failed. " + ZTools.toString (point) + " is null voxel.");
			return;
		}
		VoxelManager voxelManager = voxelGO.GetComponent<VoxelManager> ();
		voxelManager.setCorrupt ();
		
		if (isContagious (point))
			viroidManager.addContagious (point);
	}
	
	public void setDestroyed (IntVector3 point)
	{
		//Debug.Log ("setDestroyed: i: " + i + " j: " + j + " k: " + k);
		//Debug.Log ("setDestroyed " + this + " " + mapData + "- ");
		if (! MDView.isVoxelOccupied (mapData, point)) {
			Debug.Log ("setDestroyed got unoccupied request " + ZTools.toString (point));
			return;
		}
		
		voxelCount--;
		
		GameObject voxel = voxels [point.x, point.y, point.z];
		Destroy (voxel, 0);
		
		voxels [point.x, point.y, point.z] = null;
		mapData [point.x, point.y, point.z] = 0;
	}
	
	public void setVelocity (Vector3 newVelocity, int fuel)
	{
		direction = getUnitVector (newVelocity);
		velocity = newVelocity;
		driftFuel = fuel;
		isMoving = direction.x != 0 || direction.y != 0 || direction.z != 0;
	}
	
	public void setStopped ()
	{
		isMoving = false;
		velocity = new Vector3 (0, 0, 0);
	}
	
	public int getVoxelCount ()
	{
		return voxelCount;
	}
	
	private IntVector3 getUnitVector (Vector3 vector)
	{
		//only returns a unit vector when just one dimension is used
		int i = vector.x == 0 ? 0 : (int)(vector.x / Mathf.Abs (vector.x));
		int j = vector.y == 0 ? 0 : (int)(vector.y / Mathf.Abs (vector.y));
		int k = vector.z == 0 ? 0 : (int)(vector.z / Mathf.Abs (vector.z));
		
		return new IntVector3 (i, j, k);
	}
	
	public ArrayList getContagiousList ()
	{
		return viroidManager.contagiousList;
	}
	
	public void setContagiousList (ArrayList newContagiousList)
	{
		viroidManager = new ViroidManager (this);
		viroidManager.contagiousList = newContagiousList;
	}
	
	public void setSpores (int sporeCount)
	{
		//infects sporeCount voxels found at the highest points possible.
		
		ArrayList usedPoints = new ArrayList ();
		
		for (int j=jSize; j>=0; j--) {
			for (int i=0; i<iSize; i++) {
				for (int k=0; k<kSize; k++) {
					IntVector3 iterPoint = new IntVector3 (i, j, k);
					if (isHealthy (iterPoint)) {
						IntVector3 usedPoint = new IntVector3 (i, 0, k);
						if (! usedPoints.Contains (usedPoint)) {
							usedPoints.Add (usedPoint);
							setCorrupt (iterPoint);
							sporeCount--;
						}
					}
					if (sporeCount <= 0)
						return;
				}
			}
		}

	}
	
	public void setVoxelGrid (GameObject [,,] VoxelGrid, bool reassign)
	{
		//Debug.Log ("setVoxelGrid");
		voxels = VoxelGrid;
		if (reassign) {
			foreach (GameObject voxel in voxels) {
				if (voxel != null)
					voxel.transform.parent = this.transform;
			}
		}
	}
	
	public void setMapData (int[,,] newMapData)
	{
		mapData = newMapData;
	}
	
	public void setInitial (int[,,] mapData, IntVector3 startPosition, string BlobName)
	{
		position = startPosition;
		blobName = BlobName;
		isInitialized = true;
		
		this.mapData = mapData;
		
		iSize = mapData.GetLength (0);
		jSize = mapData.GetLength (1);
		kSize = mapData.GetLength (2);
	}
	
	public void addContagious (IntVector3 point)
	{
		viroidManager.addContagious (point);
	}
	
	public void applyActions (BlobManager blobManager)
	{
		/*
		when a blob is in the process of moving its voxels to another blob, it keeps track
		of actions made to it like setCorrupt and setBroken, so it can apply them to the new blob
		as well once it finishes building.
		*/
		IntVector3 blobPosition = blobManager.getRealPosition ();
		IntVector3 offset = new IntVector3 (blobPosition.x - getRealPosition ().x,
			blobPosition.y - getRealPosition ().y,
			blobPosition.z - getRealPosition ().z);
		/*
		Debug.Log ("applyActions blobPosition: "+ZTools.toString(blobPosition));
		Debug.Log ("applyActions position: "+ZTools.toString(position));
		Debug.Log ("applyActions getRealPosition: "+ZTools.toString(getRealPosition()));
		*/
		foreach (BlobAction action in blobManager.blobActions) {
			IntVector3 point = new IntVector3 (action.point.x + offset.x,
			action.point.y + offset.y,
			action.point.z + offset.z);
			
			if (action.action == "setBroken") {
				//Debug.Log("zz setBroken "+iOffset+" joff "+jOffset+" koff "+kOffset);
				setBroken (point);
				continue;
			}
			if (action.action == "setCorrupt") {
				//Debug.Log("ioff "+iOffset+" joff "+jOffset+" koff "+kOffset);
				setCorrupt (point);
				continue;
			}
		}
		
		blobManager.blobActions = new ArrayList ();
		
	}
	
	public void setName (bool isMoving)
	{
		if (isMoving) {
			this.name = blobName + " (moving)";
		} else {
			this.name = blobName;	
		}
	}
	
	public void setBlobData (BlobData blobData)
	{
		this.blobData = blobData;
	}
	
	public void createVoxels ()
	{
		//Debug.Log ("createVoxels");
		voxels = new GameObject [iSize, jSize, kSize];
		ArrayList corruptPoints = new ArrayList ();
		
		bool hasSetCorrupt = false;
		IntVector3 firstPoint = new IntVector3 (-1, 0, 0);
		for (int i=0; i<iSize; i++) {
			for (int j=0; j<jSize; j++) {
				for (int k=0; k<kSize; k++) {
					int voxelCode = mapData [i, j, k];
					if (voxelCode > 0) {
						IntVector3 iterPoint = new IntVector3 (i, j, k);
						if (firstPoint.x == -1)
							firstPoint = iterPoint;
						createVoxel (voxelCode, iterPoint);
						if (getBlobData ().viroidInfectRoll (j)) {
							hasSetCorrupt = true;
							corruptPoints.Add (iterPoint);
						}
					}
					
				}
			}
		}
		
		foreach (IntVector3 point in corruptPoints)
			setCorrupt (point);
		
		//guarentees at least one voxel is infected if blob is infected
		if (getBlobData ().viroid.exposure > 0 && !hasSetCorrupt && corruptPoints.Count == 0) 
			setCorrupt (firstPoint);
		
	}
	
	public void snapToGrid ()
	{
		Vector3 position = new Vector3 (Mathf.Round (transform.position.x),
		Mathf.Round (transform.position.y),
		Mathf.Round (transform.position.z));
		transform.position = position;
	}
	
	public void setSpeed (float speed)
	{
		isMoving = speed > 0;
		velocity = new Vector3 ((float)direction.x, (float)direction.y, (float)direction.z) * speed;
	}
	
	public void setExpectedDistance (int expectedDistance)
	{
		this.expectedDistance = expectedDistance;
	}
	
	private void createVoxel (int voxelCode, IntVector3 point)
	{
		//Debug.Log ("create voxel: i: " + i + " j: " + j + " k: " + k);
		if (! isInsideBlob (point)) {
			print ("createVoxel bad coordinates: " + ZTools.toString (point));
			return;
		}
		
		voxelCount++;
		Vector3 createPosition = new Vector3 (point.x + position.x, point.y + position.y, point.z + position.z);
		
		GameObject voxel = Instantiate (worldManager.getVoxelPrefab (), createPosition, Quaternion.identity) as GameObject;
		VoxelData voxelData = worldManager.getVoxelDataManager ().getWithVoxelCode (voxelCode);
		voxel.GetComponent<VoxelManager> ().initialize (voxelData, this, point);
		
		if (voxelData.isRotationRandom) {
			voxel.transform.eulerAngles = Angles.getRandom ();
		} else {
			voxel.transform.eulerAngles = Angles.getFlat ();
		}
		voxel.transform.parent = transform;
		
		voxels [point.x, point.y, point.z] = voxel;
		mapData [point.x, point.y, point.z] = voxelCode;
	}
	
}