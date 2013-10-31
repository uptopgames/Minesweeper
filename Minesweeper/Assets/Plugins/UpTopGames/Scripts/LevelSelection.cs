using UnityEngine;
using System.Collections;


public class LevelSelection : MonoBehaviour 
{
	private int actualWorld;
	public UIScrollList scrollLevels;
	public SpriteText worldTitle;

	// Use this for initialization
	void Start () 
	{	
		Debug.Log ("StartLevelSelection");
		
		for (int i = 0; i < Flow.MAX_WORLD_NUMBER; i++)
		{
			GameObject world = GameObject.FindWithTag("World" + (i+1));
			World dictWorld = Flow.worldDict[world.GetComponent<World>().id];
			
			world.GetComponent<World>().levelDict = dictWorld.levelDict;
			
			
			/*if (!Flow.worldDict.ContainsKey(i))
			{
				Flow.worldDict.Add(i, world.GetComponent<World>());
			}
			
			for (int j = 0; j < Flow.MAX_LEVEL_NUMBER; j++)
			{
				if (!Flow.worldDict[i].levelDict.ContainsKey(j))
				{
					Flow.worldDict[i].levelDict.Add(j, world.transform.FindChild("Level " + (j+1) + " Panel").GetComponent<Level>());
				}
			}*/
		}
		
		
		
		// teste 
		/*Save.Set ("world1_level1_stars", 2);
		Save.Set ("world1_level2_stars", 1);
		Save.Set ("world1_level3_stars", 3);
		Save.Set ("world1_level4_stars", 2);
		Save.Set ("world1_level5_stars", 3);
		Save.Set ("world1_level6_stars", 1);
		Save.Set ("world1_level7_stars", 2);
		Save.Set ("world1_level8_stars", 3);
		Save.Set ("world1_level9_stars", 1);
		Save.Set ("world2_level1_stars", 3);
		Save.Set ("world2_level2_stars", 3);
		Save.Set ("actualWorld" , 1);
		Save.Set ("world1_level2_stars", 3);*/
		//Flow.levelStars[0][0] = 2;
		
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
		for (int i = 0; i < Flow.worldDict.Count; i++)
		{
			for (int j = 0; j < Flow.worldDict[i].levelDict.Count; j++)
			{
				if (Save.HasKey ("world" + (i+1) + "_level" + (j+1) + "_stars"))
				{
					Flow.worldDict[i].levelDict[j].stars = Save.GetInt ("world" + (i+1) + "_level" + (j+1) + "_stars"); // world1_level1_stars
					Flow.worldDict[i].levelDict[j].points = (i*9) + (j+1);
					
					
					//scrollLevels.transform.GetChild(0).transform.GetChild(actualWorld-1).transform.GetChild(j).GetComponent<Level>().stars = 
						//Save.GetInt ("world" + (i+1) + "_level" + (j+1) + "_stars");
					//scrollLevels.transform.GetChild(0).transform.GetChild(actualWorld-1).transform.GetChild(j).GetComponent<Level>().points = (i*9) + (j+1);
					
					if (j==8) 
					{
						Debug.Log ("fase9");
						Flow.worldDict[i+1].levelDict[0].points = (i*9) + (j+1);
					}
					else
					{
						//Debug.Log ("faseMenorque9");
						Flow.worldDict[i].levelDict[j+1].points = Flow.worldDict[i].levelDict[j].points + 1;
						//Debug.Log ("pointsFaseSeguinte: " + Flow.worldDict[i].levelDict[j+1].points);	
					}
					
					//Debug.Log ("points: " + Flow.worldDict[i].levelDict[j].points);
					
					
					//Debug.Log ("tem chave: " + "i: " + i + "J: " + j);
					//Debug.Log ("mundoFase: " + "world" + (i+1) + "_level" + (j+1));
					//Debug.Log ("estrelasNoSave: " + Save.GetInt ("world" + (i+1) + "_level" + (j+1) + "_stars"));
					//Debug.Log ("estrelasNoFlowLevel " + (j+1) + ": " + Flow.worldDict[i].levelDict[j].stars);
				}
			}
		}
		//scrollLevels.AddInputDelegate(SetWorldObjects);
		//SetWorldObjects();
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
		worldTitle.Text = "World " + actualWorld;
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
			//SetWorldObjects();
		}
	}
	
	void Next()
	{
		Debug.Log ("NextWorld");
		//Debug.Log ("world: " + actualWorld + " level9stars: " +  Flow.worldDict[actualWorld-1].levelDict[8].stars);
		//Debug.Log ("worldId: " + Flow.worldDict[1].levelDict[1].id);
	
		if (actualWorld != Flow.MAX_WORLD_NUMBER && Flow.worldDict[actualWorld-1].levelDict[Flow.MAX_LEVEL_NUMBER-1].stars > 0)
		{
			//scrollLevels.transform.GetChild(0).transform.GetChild(actualWorld-1).gameObject.SetActive(false);
			actualWorld++;
			scrollLevels.ScrollToItem (actualWorld-1, 0.5f);
			//SetWorldObjects();
		}
		else
		{
			Debug.Log ("MustWinAllLevels");
			// fazer mensagem de que ele precisa vencer todos os leveis
		}
	}
	
	void BackButton()
	{
		UIPanelManager.instance.BringIn("MenuScenePanel", UIPanelManager.MENU_DIRECTION.Backwards);
	}
	
	/*void EnterLevel()
	{
		Flow.currentGame.world.id = actualWorld;
		//Flow.singleLevel = scrollLevels.GetItem(actualWorld).transform.GetChildCount();
		//Debug.Log ("name: " + gameObject.transform.GetComponent<UIButton>().name);
		//Debug.Log ("index: " + worldLevels[]);
		Flow.currentMode = GameMode.SinglePlayer;
		Save.Set ("actualWorld", actualWorld);
		Application.LoadLevel ("Scenario1");
	}*/
	
}
