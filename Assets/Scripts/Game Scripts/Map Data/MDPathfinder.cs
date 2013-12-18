using UnityEngine;
using System.Collections;

public class MDPathfinder
{
	private static ArrayList allVisitedVoxels;
	private static ArrayList frontier;
	private static ArrayList seeds;
	
	public static ArrayList findDetachedVoxels (int [,,] mapData, int stopCode, IntVector3 point)
	{
		/*
		where i,j,k is a recently destroyed block, returns a list of IntVector3s of all blocks that should
		be detached, as well as the size of the blob that would contain them.
		*/
		ArrayList detachedVoxels = new ArrayList ();

		allVisitedVoxels = new ArrayList ();
		ArrayList seeds = MDView.getNeighbors (mapData, point);
		
		for (int index=0; index<seeds.Count; index++) {
			
			IntVector3 seedPoint = (IntVector3)seeds [index];
			
			if (allVisitedVoxels.Contains (seedPoint)) {
				seeds.Remove (seedPoint);
				index--;
				continue;
			}
			
			ArrayList newVoxels = getBlob (mapData, stopCode, seedPoint);
			
			if (newVoxels.Count > 0) {
				detachedVoxels.AddRange (newVoxels);
				
			}
			
		}
		return detachedVoxels;
		
	}
	
	public static ArrayList getBlob (int [,,] mapData, int stopCode, IntVector3 point)
	{
		/*
		returns a list of IntVector3 voxels that are all connected to i,j,k
		
		returns an empty list if it finds a voxelCode=hullBase
		
		prioritize search on neighbors going down, then neighbors at same level, then up
		*/
		ArrayList visitedVoxels = new ArrayList ();
		allVisitedVoxels.Add (point);
		visitedVoxels.Add (point);
		
		frontier = new ArrayList ();
		frontier.Add (point);
		
		//print ("getBlob " + i + " " + j + " " + k);
		
		while (frontier.Count>0) {
			IntVector3 seedPoint = (IntVector3)frontier [0];
			frontier.RemoveAt (0);
			
			foreach (IntVector3 neighbor in MDView.getNeighbors(mapData,seedPoint)) {
				if (! visitedVoxels.Contains (neighbor)) {
					allVisitedVoxels.Add (neighbor);
					visitedVoxels.Add (neighbor);
					
					//print ("adding to visitedVoxels " + neighbor.i + " " + neighbor.j + " " + neighbor.k);
					
					if (neighbor.y < point.y) {
						frontier.Insert (0, neighbor);
					} else if (neighbor.y == point.y) {
						frontier.Insert (frontier.Count / 2, neighbor);
					} else if (neighbor.y > point.y) {
						frontier.Insert (frontier.Count, neighbor);
					}
					
					if (mapData [neighbor.x, neighbor.y, neighbor.z] == stopCode) {
						return new ArrayList ();
					}
				}
			}
		}
		
		return visitedVoxels;
	}
	

	
}













