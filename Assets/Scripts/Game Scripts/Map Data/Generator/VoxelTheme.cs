using UnityEngine;
using System.Collections;

public class VoxelTheme:MonoBehaviour
{
	[System.Serializable]
	public class ThemeItem
	{
		public string themeKey;
		public VoxelData voxelData;
	}
	public ThemeItem[] themeItems;
	private VoxelDataManager vdm;
	
	public VoxelDataManager getVoxelDataManager ()
	{
		if (vdm == null)
			vdm = (VoxelDataManager)FindObjectOfType (typeof(VoxelDataManager));
		return vdm;
	}
	
	public int getCode (string themeKey)
	{
		int voxelCode = 0;
		
		foreach (ThemeItem themeItem in themeItems) {
			if (themeKey == themeItem.themeKey) {
				voxelCode = themeItem.voxelData.voxelCode;
				break;
			}
		}
		
		if (voxelCode == 0)
			voxelCode = getVoxelDataManager().getWithName (themeKey).voxelCode;
		
		if (voxelCode==getVoxelDataManager().getWithName("default").voxelCode)
			Debug.Log ("Theme " + name + " is missing themeKey: " + themeKey);
		
		return voxelCode;
	}
	
	public int getRandomCode(){
		ThemeItem ti=themeItems[Random.Range(0,themeItems.Length)];
		return ti.voxelData.voxelCode;
	}
}
