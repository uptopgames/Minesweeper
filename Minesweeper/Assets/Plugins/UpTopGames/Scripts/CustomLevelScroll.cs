using UnityEngine;
using System.Collections;

public class CustomLevelScroll : MonoBehaviour
{
	public GameObject[] customLevelPrefabs;
	
	// Use this for initialization
	void Start ()
	{
		GetComponent<UIScrollList>().ClearList (true);
		if(Flow.customStages != null)
		{
			foreach(CustomStage c in Flow.customStages)
			{
				GameObject stage = GetComponent<UIScrollList>().CreateItem(customLevelPrefabs[c.world]).gameObject;
				stage.GetComponent<CustomLevelButton>().world = c.world;
				stage.GetComponent<CustomLevelButton>().level = Flow.customStages.IndexOf(c);
			}
		}
	}
	
	public void StartStageCreator()
	{
		Flow.config.GetComponent<ConfigManager>().inviteAllScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().invitePlayingScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.currentMode = GameMode.SinglePlayer;
		Debug.Log(Flow.currentMode);
		Application.LoadLevel("CustomStage");
	}
	
	public void BackButton()
	{
		GetComponent<UIScrollList>().ClearList (true);
		UIPanelManager.instance.BringIn("LevelSelectionPanel", UIPanelManager.MENU_DIRECTION.Backwards);
	}
	
	public void SendToFriend(CustomLevelButton level)
	{
		Flow.game_native.showMessage("Feature not Implemented", "You cannot send your games to friends yet. Please wait until we implement this feature", "Ok");
	}
}