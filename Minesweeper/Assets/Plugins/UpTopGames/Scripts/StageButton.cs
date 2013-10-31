using UnityEngine;
using System.Collections;

public class StageButton : MonoBehaviour {
	
	public Transform worlds;
	public CreateStage creator;
	
	void BringStages()
	{
		transform.parent.GetComponent<UIInteractivePanel>().BringIn();
	}
	
	void SwapToPeru()
	{
		foreach(Transform child in worlds)
		{
			child.gameObject.SetActive(false);
		}
		
		creator.currentWorld = 0;
		worlds.FindChild("1 - Peru").gameObject.SetActive(true);
		transform.parent.GetComponent<UIInteractivePanel>().Dismiss();
	}
	
	void SwapToTibet()
	{
		foreach(Transform child in worlds)
		{
			child.gameObject.SetActive(false);
		}
		
		creator.currentWorld = 1;
		worlds.FindChild("2 - Tibet").gameObject.SetActive(true);
		transform.parent.GetComponent<UIInteractivePanel>().Dismiss();
	}
	
	void SwapToGreece()
	{
		foreach(Transform child in worlds)
		{
			child.gameObject.SetActive(false);
		}
		
		creator.currentWorld = 2;
		worlds.FindChild("3 - Greece").gameObject.SetActive(true);
		transform.parent.GetComponent<UIInteractivePanel>().Dismiss();
	}
	
	void SwapToAmazonia()
	{
		foreach(Transform child in worlds)
		{
			child.gameObject.SetActive(false);
		}
		
		creator.currentWorld = 3;
		worlds.FindChild("4 - Amazonia").gameObject.SetActive(true);
		transform.parent.GetComponent<UIInteractivePanel>().Dismiss();
	}
	
	void SwapToEgypt()
	{
		foreach(Transform child in worlds)
		{
			child.gameObject.SetActive(false);
		}
		
		creator.currentWorld = 4;
		worlds.FindChild("5 - Egypt").gameObject.SetActive(true);
		transform.parent.GetComponent<UIInteractivePanel>().Dismiss();
	}
}
