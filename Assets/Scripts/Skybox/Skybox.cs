using UnityEngine;
using System.Collections;

public class Skybox : MonoBehaviour
{
	private GameObject player;
	public int numberOfPlanes;
	
	//Range at which planes will be placed
	public int distanceFar;
	public int distanceClose;
	
	//Rotation Curves
	public AnimationCurve xRotation = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 1));
	public AnimationCurve yRotation = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 1));
	public AnimationCurve zRotation = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 1));
	
	//Range of scale that will be applied to planes
	public int scaleBig;
	public int scaleSmall;
	public AnimationCurve scaleMix = new AnimationCurve (new Keyframe (0, 1), new Keyframe (1, 1));
	
	
	//Two colours that are combined randomly using Lerp
	public Color color1;
	public AnimationCurve colorMixing1 = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 1));
	public Color color2;
	public AnimationCurve colorMixing2 = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 1));
	public Color color3;
	public AnimationCurve colorMixing3 = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 1));
	
	[System.Serializable]	
	public class SpacePart
	{
		public Texture2D spaceTex;
		
		[System.Serializable]
		public class Mods
		{
			public float spaceTexProbability ;
			public float spaceTexSize;
			public float spaceTexAlpha;
			public float spaceTexBrightness;
			public bool lightYesNo;
		}
		
		public Mods mods;
	}
		
	public SpacePart[] spaceParts;
	
	//Light setup
	//More lights means more even lighting
	public int maxNumberLights;
	// a modifier that changes the overall intensity of all lights, meaning that more lights can fit into a maxTotalLightBrightness
	public float lightIntensity;
	//The additive value of all lights in a scene. 2-3 is generally a good gameplay range. further numbers have interesting effects.
	public float maxTotalLightBrightness;
	public bool debugLightsText;
	
	void Start ()
	{
        GameManager gm = (GameManager)FindObjectOfType(typeof(GameManager));
		player=gm.getPlayer();
		
		int numberOfLights = 0;
		float totalLightBrightness = 0;
		
		for (int n = 0; n <numberOfPlanes; n++) {

			//Generate random orientation
			int rx = (int)(xRotation.Evaluate (Random.Range (0.0F, 1.0F)) * 360);
			int ry = (int)(yRotation.Evaluate (Random.Range (0.0F, 1.0F)) * 360);
			int rz = (int)(zRotation.Evaluate (Random.Range (0.0F, 1.0F)) * 360);
			
			//Select random space texture based upon a probability curve
			//get spacetexture frequency probability.
			
			int textureNum = 0;
			float check = 1.0f;
			float probability = 1.0f;
			int count = 0; //used to limit infinite loop possibility

			do {	
				//choose a random texture as t int		
				textureNum = Random.Range (0, spaceParts.Length);
				
				probability = spaceParts [textureNum].mods.spaceTexProbability;
				
			
				//Pick and random number between 0 and 1.  This will used as a dice roll on the next while to see if we get out of the loop.
				check = Random.Range (0.0f, 1.0f);
				count++;
				//If p isn't bigger or equal to the diceroll, we try again.	Stop after a hundred tries
			} while(probability<check && count<100);
			
			//Generate random distance
			//int distance = (int)(distanceClose + distanceMix.Evaluate (Random.Range (0.0F, 1.0F)) * distanceFar*spaceParts[textureNum].mods.spaceTexDistance);	
		
			float distance = (distanceClose + ((float)n / numberOfPlanes) * (distanceFar - distanceClose));
			
			//Generate random scale
			float scale = (scaleSmall + scaleMix.Evaluate (1 - (float)n / numberOfPlanes) * (scaleBig - scaleSmall)) * spaceParts [textureNum].mods.spaceTexSize;
			
			//distance corrected scale
			float correctedScale = scale * distance / distanceClose;
			
			//cS = colorSelect  Select a random colour
			float cS = Random.Range (0.0F, 1.0F);
				
			Color colorMix = (color1 * colorMixing1.Evaluate (cS)) + (color2 * colorMixing2.Evaluate (cS)) + (color3 * colorMixing3.Evaluate (cS));
		
			//clamp color to 1.0 to get rid of some weird flickering
			if (colorMix.r > 1.0)
				colorMix.r = 1.0f;
			if (colorMix.g > 1.0)
				colorMix.g = 1.0f;
			if (colorMix.b > 1.0)
				colorMix.b = 1.0f;
			
			//Optional Array to set alpha to something than 1 default
			colorMix.a = spaceParts [textureNum].mods.spaceTexAlpha;
			
			//Optional Array to set light brightness modifier to individual space textures
			float spaceTexBrightnessModifer = spaceParts [textureNum].mods.spaceTexBrightness;		
			
			//will it have a light?	The space part needs to be bigger than both half size and closer than hald distance to get a light
			bool light;
			
			//We calculate what effect the new light would have using a temp float
			
			float temptotalLightBrightness = totalLightBrightness + colorMix.grayscale * lightIntensity * spaceTexBrightnessModifer * scale / 100;
						
			if (numberOfLights < maxNumberLights && 
				spaceParts [textureNum].mods.lightYesNo && 
				temptotalLightBrightness < maxTotalLightBrightness) {
				totalLightBrightness = temptotalLightBrightness;
				light = true;
				numberOfLights++;
			} else {
				light = false;
			}
			

			makeSpacePlane (n, rx, ry, rz, distance, correctedScale, scale, colorMix, textureNum, light, spaceTexBrightnessModifer);
			
			// good debug bring to see the total brightness of all lights
			
			if (debugLightsText) {
				Debug.Log ("total light brightness:" + totalLightBrightness + " number of lights:" + numberOfLights);
			}
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		//keeps the whole thing position around main camera
		transform.position = player.transform.position;		
	}

	private void makeSpacePlane (int n, int rx, int ry, int rz, float distance, float correctedScale, float scale, Color colorMix, int t, bool light, float brightness)
	{
        GameObject pp = ResourceLookup.getPlanePrefab();
        //Debug.Log("pp: "+pp==null);
        //Debug.Log("pp.name: " + pp.name);
        GameObject plane = Instantiate(pp, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		plane.layer=9;

        SkyboxPlane sp = plane.GetComponentInChildren<SkyboxPlane>();
		sp.setDistance (distance);
		sp.setColor (colorMix);
		sp.setTexture (spaceParts [t].spaceTex);
		
		plane.transform.localEulerAngles = new Vector3 (rx, ry, rz);
		plane.transform.localScale = new Vector3 (scale, scale, 1);
		plane.transform.parent = transform;
		
		//Optional Light Creation
		if (light) {
			GameObject lightGameObject = new GameObject ("Light" + n);
			lightGameObject.AddComponent<Light> ();
			lightGameObject.light.color = colorMix;
			lightGameObject.light.type = LightType.Directional;
			lightGameObject.light.intensity = scale / 100 * lightIntensity * brightness;
			lightGameObject.transform.parent = plane.transform;
			lightGameObject.transform.localEulerAngles = -(new Vector3 (0, -180, 0));
		}
		
		
	}
	

	
	
}

	




