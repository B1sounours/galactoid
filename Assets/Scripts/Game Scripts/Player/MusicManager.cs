using UnityEngine;
using System.Collections;

[System.Serializable]
public class MusicManager : MonoBehaviour
{
	private AudioSource audioSource;
	public AudioClip[] musicPlaylist;
	
	void Update ()
	{
		if (!audioSource.isPlaying && GameSettings.musicEnabled)
			playRandomMusic ();
	}
	
	void Awake ()
	{
		name="Music Manager";
		float volume = 0.3f;
		audioSource = getAudioSource ();
		audioSource.volume = volume;
		audioSource.maxDistance = volume;
		audioSource.minDistance = volume;
	}
	
	private AudioSource getAudioSource ()
	{
		WorldManager worldManager = (WorldManager)FindObjectOfType (typeof(WorldManager));
		if (worldManager == null) {
			gameObject.AddComponent<AudioListener>();
			audioSource = gameObject.AddComponent<AudioSource> ();
		} else {
			audioSource = worldManager.getPlayer().AddComponent<AudioSource> ();
		}
		return audioSource;
	}
	
	private AudioClip[] getMusicPlaylist ()
	{
		if (musicPlaylist.GetLength (0) == 0) {
			AudioClip audioClip = Resources.Load ("Music/default") as AudioClip;
			musicPlaylist = new AudioClip[1]{audioClip};
		}
		
		return musicPlaylist;
	}
	
	void playRandomMusic ()
	{
		AudioClip audioClip = musicPlaylist [Random.Range (0, getMusicPlaylist ().GetLength (0))];
		audioSource.clip = audioClip;
		audioSource.Play ();
	}
}
