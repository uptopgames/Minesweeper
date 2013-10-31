using UnityEngine;

public class Fader : MonoBehaviour
{
	public enum FadeType
	{
		FadeIn,
		FadeOut
	}
	public FadeType fadeType;
	
	public bool backAndForth = false; //if fadeType is FadeIn, changes it to FadeOut afterwards
	public float backAndForthDelay = 1;
	
	public float alphaFadeValue = 1; //starting Alpha: use 1 to FadeOut and 0 to FadeIn
	public float fadeTime = 5; //in seconds, how long will the Fade last
	public Texture blackTexture; //the Texture of the Fade: usually a white or black rectangle
	public float delay = 2; //the time it takes to start the fade
	
	public float faderLifetime = 3;
	
	public void Start()
	{
		if(backAndForth)
		{
			GameObject.Destroy(gameObject, faderLifetime);
		}
		else
		{
			GameObject.Destroy(gameObject, faderLifetime);
		}
	}
	
	public void OnGUI()
	{
		if(delay>0)
		{
			delay -= Time.deltaTime;
			switch(fadeType)
			{
				case FadeType.FadeOut:
					GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height), blackTexture);
				break;
			}
		}
		else
		{
			switch(fadeType)
			{
				case FadeType.FadeIn:
					if(alphaFadeValue>=1)
					{
						GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
						if(backAndForth)
						{
							Invoke("SwitchToFadeOut", backAndForthDelay);
							backAndForth = false;
						}
					}
					else
					{
						alphaFadeValue += Mathf.Clamp01(Time.deltaTime / fadeTime);
						GUI.color = new Color(alphaFadeValue, alphaFadeValue, alphaFadeValue, alphaFadeValue);
						GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height), blackTexture);
					}
				break;
				case FadeType.FadeOut:
					if(alphaFadeValue>0)
					{
						alphaFadeValue -= Mathf.Clamp01(Time.deltaTime / fadeTime);
						GUI.color = new Color(alphaFadeValue, alphaFadeValue, alphaFadeValue, alphaFadeValue);
						GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height), blackTexture);
					}
				break;
			}
		}
	}
	
	void SwitchToFadeOut()
	{
		fadeType = FadeType.FadeOut;
	}
}