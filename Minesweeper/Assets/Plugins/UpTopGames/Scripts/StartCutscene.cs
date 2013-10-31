using UnityEngine;
using System.Collections;

public class StartCutscene : MonoBehaviour
{
	public GameObject[] objectToActivate;
	public Gunslinger[] scriptsToActivate;
	public bool deactivateItself;
	
	public float initialDelay = 2;
	public float translationDuration = 1;
	public float cameraTranslationDuration = 1;
	
	public GameObject[] objectsToTranslate;
	public Transform[] endPosition;
	
	public GameObject cutsceneCamera;
	public Transform gameCamera;
	public Transform lookTarget;
	
	private bool ended = false;
	
	void Start()
	{
		foreach(Gunslinger s in scriptsToActivate)
		{
			s.StartGame();
		}
	}
	
	void EnterWalk()
	{
		for(int i = 0; i < objectsToTranslate.Length; i++)
		{
			iTween.MoveTo(objectsToTranslate[i], iTween.Hash("position", endPosition[i].position, "time", translationDuration, "oncomplete", "EnterIdle",
				"oncompletetarget", gameObject, "easetype", iTween.EaseType.linear));
		}
	}
	
	void Update ()
	{
		if(!animation.isPlaying && !ended)
		{
			iTween.MoveTo(cutsceneCamera, iTween.Hash("position", gameCamera.position, "time", cameraTranslationDuration, "oncomplete", "StartGame",
				"oncompletetarget", gameObject, "easetype", iTween.EaseType.linear, "looktarget", lookTarget));
			
			ended = true;
		}
	}
	
	public void EnterIdle()
	{
		foreach(GameObject g in objectsToTranslate)
		{
			g.animation.CrossFade("idle");
		}
	}
	
	public void StartGame()
	{
		foreach(GameObject g in objectToActivate)
		{
			g.SetActive(true);
		}
		
		foreach(Gunslinger s in scriptsToActivate)
		{
			s.enabled = true;
		}
		
		if(deactivateItself)
		{
			gameObject.SetActive(false);
		}
	}
}