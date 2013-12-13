using UnityEngine;
using System.Collections;

public class AudioPlayer : MonoBehaviour
{
	public bool playOnStart = false;
	
	void Start ()
	{
		if(!Save.HasKey(PlayerPrefsKeys.VOLUME))
		{
			Save.Set(PlayerPrefsKeys.VOLUME, 0.5f, true);
		}
		audio.volume = Save.GetFloat(PlayerPrefsKeys.VOLUME);
		
		if(playOnStart) audio.Play();
	}
}