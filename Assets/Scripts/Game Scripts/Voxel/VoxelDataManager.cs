using UnityEngine;
using System.Collections;

public class VoxelDataManager : MonoBehaviour
{	
	private ArrayList voxelDatas;
	private ArrayList voxelThemes;
	
	private ArrayList getVoxelDatas(){
		if (voxelDatas!=null)
			return voxelDatas;
		
		voxelDatas = new ArrayList ();
		foreach (GameObject go in Resources.LoadAll("Prefabs/Voxel Data")) {
			voxelDatas.Add (go.GetComponent<VoxelData> ());
		}
		
		return voxelDatas;
	}
	
	private ArrayList getVoxelThemes(){
		if (voxelThemes!=null)
			return voxelThemes;
		
		voxelThemes = new ArrayList ();
		foreach (GameObject go in Resources.LoadAll("Prefabs/Voxel Themes")) {
			voxelThemes.Add (go.GetComponent<VoxelTheme> ());
		}
		
		return voxelThemes;
	}
	
	public VoxelData getWithName (string voxelName)
	{
		//future optimization: this should obviously be a hashtable
		foreach (VoxelData voxelData in getVoxelDatas()) {
			if (voxelData.name == voxelName)
				return voxelData;
		}
		
		if (voxelName == "default") {
			Debug.Log ("Ah! failed to find default voxel data.");
			return null;
		} else {
			Debug.Log ("warning: failed to find voxelData with name: "+voxelName);
			return getWithName("default");
		}
	}
	
	public VoxelData getWithVoxelCode (int voxelCode)
	{		
		foreach (VoxelData voxelData in getVoxelDatas()) {
			if (voxelData.voxelCode == voxelCode)
				return voxelData;
		}
		
		Debug.Log ("warning: failed to find voxelData with code: "+voxelCode);
		return getWithName("default");
	}
	
	public VoxelTheme getRandomTheme(){
		int index=Random.Range(0,getVoxelThemes().Count);
		VoxelTheme voxelTheme= (VoxelTheme)voxelThemes[index];
		return voxelTheme;
	}
	
}
