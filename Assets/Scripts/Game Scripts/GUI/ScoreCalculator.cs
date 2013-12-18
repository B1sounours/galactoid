using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreCalculator
{

	public static void setScoreDialogue (DialogueMenu dm, PlayerCounter pc, int voxelCount)
	{
		dm.setMainText ("Score: " + getScore (pc, voxelCount).ToString());
		
		dm.addItem ("Blocks saved:", voxelCount);
		
		dm.addItem ("", "");
		dm.addItem ("Score Penalty:", getPenalties (pc));
		
		dm.addItem ("Broken block penalty:", pc.breakCount);
		if (pc.corruptBreakCount > 0)
			dm.addItem ("Broken infected block penalty:", pc.corruptBreakCount);	
		
		dm.addItem ("", "");
		dm.addItem ("Score multiplier from bonuses:", getMultiplier (pc));
		dm.addItem ("Awards:", "");
		dm.addItem ("Clark Kent", !pc.usedFly);
		dm.addItem ("Sammy Lightfoot", !pc.hasFallen);
		dm.addItem ("Kangaroo Hater", !pc.usedJump);
		dm.addItem ("Photophobic", !pc.usedLaser);
	}
	
	public static int getScore (PlayerCounter pc, int voxelCount)
	{
		int score=(int)(voxelCount*getMultiplier(pc));
		score-=getPenalties(pc);
		return score>0?score:0;
	}
	
	public static int getPenalties (PlayerCounter pc)
	{
		return pc.breakCount *3 + pc.corruptBreakCount * 10;
	}
	
	public static float getMultiplier (PlayerCounter pc)
	{
		float multiplier = 1;
		
		if (!pc.usedFly)
			multiplier += 1;
		if (!pc.usedJump)
			multiplier += 3;
		if (!pc.usedLaser)
			multiplier += 2;
		if (!pc.hasFallen)
			multiplier += 0.5f;
		
		return multiplier;
	}
}
