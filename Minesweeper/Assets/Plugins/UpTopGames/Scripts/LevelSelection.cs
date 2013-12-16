using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelection : MonoBehaviour 
{
	private int actualWorld;
	public UIScrollList scrollLevels;
	public SpriteText worldTitle;
	
	private string[] worldTitles = new string[5]{"Peru", "Tibet", "Greece", "Amazonia", "Egypt"};

	// Use this for initialization
	void Start () 
	{
		//Debug.Log ("StartLevelSelection");
		
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(InitLevelSelection);
	}
	
	void InitLevelSelection(EZTransition transition)
	{
		Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(true);
		
		if (!Save.HasKey ("actualWorld")) 
		{
			actualWorld = 1;
		}
		else
		{
			actualWorld = Save.GetInt ("actualWorld");
		}
		
		scrollLevels.ScrollToItem(actualWorld-1, 0.5f);
		
		//scrollLevels.SetSelectedItem(actualWorld);
		scrollLevels.AddItemSnappedDelegate(SetWorld);
		
		//Debug.Log ("actualWorld: " + actualWorld);
		
		// salva as estrelas ganhas no flow
		
		foreach(KeyValuePair<int, World> w in Flow.worldDict)
		{
			foreach (KeyValuePair<int, Level> l in w.Value.levelDict)
			{
				if (Save.HasKey (PlayerPrefsKeys.LEVELSTARS+l.Key))
				{
					l.Value.stars = Save.GetInt(PlayerPrefsKeys.LEVELSTARS+(l.Key).ToString()); // world1_level1_stars
					Debug.Log("l.Value.stars: " + l.Value.stars + ", level: " + l.Value.id + ", world: " + w.Value.id);
					//l.Value.points = l.Key;
				}
			}
		}
		
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 9; j++)
			{
				int stars = i*9+7+j;
				
				if (Save.HasKey (PlayerPrefsKeys.LEVELSTARS+stars))
				{
					transform.FindChild("ScrollLevels").FindChild("Mover").FindChild("World " + (i+1).ToString() + " List Item").
					FindChild("Level " + (j+1).ToString() + " Panel").GetComponent<Level>().stars = Save.GetInt(PlayerPrefsKeys.LEVELSTARS+stars);
				}
				else
				{
					transform.FindChild("ScrollLevels").FindChild("Mover").FindChild("World " + (i+1).ToString() + " List Item").
					FindChild("Level " + (j+1).ToString() + " Panel").GetComponent<Level>().stars = 0;
				}
				
				transform.FindChild("ScrollLevels").FindChild("Mover").FindChild("World " + (i+1).ToString() + " List Item").
					FindChild("Level " + (j+1).ToString() + " Panel").GetComponent<Level>().SetStars();
			}
		}
	}
	
	void SetWorld(IUIListObject item)
	{
		Debug.Log("snapped item: "+item.Index);
		/*for (int i = 0; i < 5; i++)
		{
			//scrollLevels.GetItem(i).gameObject.SetActive(false);
			Debug.Log ("scrollGetItem: " + scrollLevels.GetItem(i).gameObject.name);
		}*/
		
		//Debug.Log ("scrollItem: " + item);
		actualWorld = item.Index +1;
		//Debug.Log ("actualWorldScroll: " + actualWorld);
		worldTitle.Text = worldTitles[actualWorld-1];
	}
	
	void PreviousWorld()
	{
		Debug.Log ("PreviousWorld");
		if (actualWorld != 1) 
		{
			//scrollLevels.transform.GetChild(0).transform.GetChild(actualWorld-1).gameObject.SetActive(false);
			Debug.Log("actual world: "+actualWorld);
			
			actualWorld --;
			scrollLevels.ScrollToItem (actualWorld-1, 0.5f);
		}
	}
	
	void Next()
	{
		foreach(KeyValuePair<int, World> w in Flow.worldDict)
		{
			//Debug.Log(w.Key);
			//Debug.Log(w.Value);
			foreach(KeyValuePair<int, Level> l in w.Value.levelDict)
			{
			  Debug.Log(l.Key);
			  //Debug.Log(l.Value);
			}
		}
		
		Debug.Log ("NextWorld");
	
		if (actualWorld != Flow.MAX_WORLD_NUMBER)
		{
			//scrollLevels.transform.GetChild(0).transform.GetChild(actualWorld-1).gameObject.SetActive(false);
			actualWorld++;
			scrollLevels.ScrollToItem (actualWorld-1, 0.5f);
		}
	}
	
	void BackButton()
	{
		UIPanelManager.instance.BringIn("MenuScenePanel", UIPanelManager.MENU_DIRECTION.Backwards);
	}
	
	void CustomStage()
	{
		if(Flow.currentMode == GameMode.SinglePlayer)
		{
			UIPanelManager.instance.BringIn("CustomLevelsScenePanel", UIPanelManager.MENU_DIRECTION.Backwards);
		}
		else if(Flow.currentMode == GameMode.Multiplayer)
		{
			UIPanelManager.instance.BringIn("ChallengesScenePanel", UIPanelManager.MENU_DIRECTION.Backwards);
		}
	}
}