using UnityEngine;
using System.Collections;

public class AudioVolumeManager : MonoBehaviour
{
	public AudioSource[] players;
	public UISlider slider;
	
	void Start()
	{
		if(!Save.HasKey(PlayerPrefsKeys.VOLUME))
		{
			Save.Set(PlayerPrefsKeys.VOLUME, 0.5f, true);
		}
		slider.Value = Save.GetFloat(PlayerPrefsKeys.VOLUME);
	}
	
	public void UpdateVolume()
	{
		foreach(AudioSource p in players)
		{
			Debug.Log("volume: " + slider.Value);
			p.volume = slider.Value;
			Save.Set(PlayerPrefsKeys.VOLUME, slider.Value, true);
		}
	}
}