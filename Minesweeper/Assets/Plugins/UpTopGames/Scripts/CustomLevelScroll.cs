using UnityEngine;
using System.Collections;

public class CustomLevelScroll : MonoBehaviour
{
	public MinesweeperRaider game;
	public GameObject[] customLevelPrefabs;
	public GameObject rival;
	public SpriteText buttonText;
	
	// Use this for initialization
	void Start ()
	{
		GetComponent<UIScrollList>().ClearList (true);
		if(Flow.customStages != null)
		{
			foreach(CustomStage c in Flow.customStages)
			{
				GameObject stage = GetComponent<UIScrollList>().CreateItem(customLevelPrefabs[c.world]).gameObject;
				stage.transform.FindChild("Level").GetComponent<LevelButton>().world = c.world;
				stage.transform.FindChild("Level").GetComponent<LevelButton>().level = Flow.customStages.IndexOf(c);
				stage.transform.FindChild("Level").GetComponent<LevelButton>().game = game;
			}
		}
	}
	
	public void Swap()
	{
		if(gameObject.activeSelf)
		{
			gameObject.SetActive(false);
			rival.SetActive(true);
			buttonText.Text = "Custom Stages";
		}
		else
		{
			gameObject.SetActive(true);
			rival.SetActive(false);
			buttonText.Text = "Regular Stages";
		}
	}
	
	public void StartStageCreator()
	{
		Application.LoadLevel("CustomStage");
	}
}