using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WaveData
{	
	public GameObject blobPrefab;
	
	[System.Serializable]
	public class SpawnTime
	{
		public float start = 0;
		public float plusMinus = 0;
	}
	public SpawnTime time;
	
	[System.Serializable]
	public class SpawnFrequency
	{
		//if count is greater than one, multiple spawns will occur, time spread out using iterval vars
		public int count = 1;
		//where start time is the "position", interval is the "velocity"
		public float interval = 10;
		//where start time is the "position", interval delta is the "acceleration"
		public float intervalDelta = 0;
		/*
		shower event. ignores spawn overflow safeguard, as it spreads out spawns evenly over
		a grid that does not overlap
		*/
		public bool shower=false;
		public int showerSpacing=0;
	}
	public SpawnFrequency frequency;
	
	[System.Serializable]
	public class DirectionProbability
	{
		public Directions.Cardinal direction = Directions.Cardinal.north;
		//the higher probability is, the more likely this direction is to be chosen
		public int probability = 1;
	}
	public DirectionProbability[] directions;
	
	[System.Serializable]
	public class SpawnOffset
	{
		/*
		used to randomly determine offset relative to main blob
		any range is valid. 
		example: yOffset value at x=0.2 is very high. This means blob.offset.y will very likely
		be 20% of mainblob height
		
		even distribution is default
		*/
		public AnimationCurve xOffset;
		public AnimationCurve yOffset;
	}
	public SpawnOffset offset;
	
	[System.Serializable]
	public class SpawnSize
	{
		/*
		blob size is determined by multiplying width,height,depth by multiplier
		
		multiplier= (max-min)*(randomMultiplier+timeMultiplier)/2
		randomMultiplier is from randomCurve
		timeEval is timeMultiplier.eval(time elapsed/time.duration)
		
		*/
		public float minMultiplier = 0.8f;
		public float maxMultiplier = 1.2f;
		//a probability curve
		public AnimationCurve randomCurve;
		//not a probability curve: evaluates at elapsed time divided by total time
		//does nothing if frequency.count=1
		public AnimationCurve timeCurve;
	}
	public SpawnSize size;
	private float lastStartTime = -1;
	
	public float getStartTime ()
	{
		if (lastStartTime != -1)
			return lastStartTime;
		lastStartTime = genStartTime ();
		return lastStartTime;
	}
	
	public float getDeltaTimeFromIndex (int frequencyIndex)
	{
		//when frequency.count>1, this is the amount of time passed after time.start for each new spawn
		
		//frequency velocity
		float deltaTime = frequency.interval * frequencyIndex;
		//frequency acceleration
		deltaTime += frequency.intervalDelta * frequencyIndex * frequencyIndex / 2;
		
		if (deltaTime < 0) {
			deltaTime = 0;
			Debug.Log ("warning: getDeltaTimeFromIndex returned default 0");
		}
		
		return deltaTime;
	}
	
	public float getTimeDuration ()
	{
		float lastDelta = 0;
		for (int i=0; i<frequency.count; i++) {
			float delta = getDeltaTimeFromIndex (frequency.count - 1);
			
			if (lastDelta > delta)
				break;
			
			lastDelta = delta;			
		}
			
		return lastDelta;
	}
	
	public float genStartTime ()
	{
		//rolls a new start time
		float chosenStartTime = time.start + Random.Range (-time.plusMinus, time.plusMinus + 1);
		
		if (chosenStartTime < 0)
			chosenStartTime = 0;
		
		return chosenStartTime;
	}
	
	public Directions.Cardinal genDirection ()
	{		
		//rolls a new direction		
		if (directions.GetLength (0) == 0)
			return Directions.Cardinal.unknown;
		
		int probabilityTotal = 0;
		foreach (DirectionProbability dp in directions) {
			if (dp.probability > 0)
				probabilityTotal += dp.probability;
		}
		
		int rand = Random.Range (0, probabilityTotal);
		foreach (DirectionProbability dp in directions) {
			if (dp.probability > 0)
				rand -= dp.probability;
			if (rand < 0) 
				return dp.direction;
		}
		
		//if probabilities are set to zero, but it has a direction, return first direction
		if (directions.GetLength(0)>1)
			return ((DirectionProbability)directions[0]).direction;
		//if no directions at all are set, return unknown
		return Directions.Cardinal.unknown;
		
	}
	
	public Vector2 genOffset ()
	{
		//rolls a new offset
		int sliceCount = 20;
		
		float xOffset = ZTools.evaluateProbabilityCurve (offset.xOffset, sliceCount);
		float yOffset = ZTools.evaluateProbabilityCurve (offset.yOffset, sliceCount);
		
		return new Vector2 (xOffset, yOffset);
	}
	
	public float genSizeMultiplier (int frequencyIndex)
	{
		float delta = size.maxMultiplier - size.minMultiplier;
		
		float elapsedTime = getDeltaTimeFromIndex (frequencyIndex);
		
		float timeMultiplier = 1;
		float timeDuration = getTimeDuration ();
		if (timeDuration > 0) {
			float timeCurveX = elapsedTime / timeDuration;
			timeCurveX = ZTools.truncate (timeCurveX, 0, 1);
			timeMultiplier = size.timeCurve.Evaluate (timeCurveX);
		}
		
		float randomMultiplier = ZTools.evaluateProbabilityCurve (size.randomCurve, 20);
		
		float averageMultiplier = (randomMultiplier + timeMultiplier) / 2;
		
		return size.minMultiplier + delta * averageMultiplier;
	}
		
}