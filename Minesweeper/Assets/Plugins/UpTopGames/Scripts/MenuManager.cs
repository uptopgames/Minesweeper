using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class MenuManager : MonoBehaviour 
{
	public GameObject[] worldObjects; 
	
	// Use this for initialization
	void Start ()
	{
		Connect();
		Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(true);
		switch(Flow.nextPanel)
		{
			case PanelToLoad.Menu:
				UIPanelManager.instance.BringIn("MenuScenePanel");
			break;
			
			case PanelToLoad.BattleStatus:
				UIPanelManager.instance.BringIn("BattleStatusScenePanel");
			break;
			
			case PanelToLoad.WinLose:
				UIPanelManager.instance.BringIn("WinLoseScenePanel");
			break;
			
			// GLA
			case PanelToLoad.EndLevel:
				UIPanelManager.instance.BringIn("EndLevelPanel");
			break;
			
			case PanelToLoad.Multiplayer:
				UIPanelManager.instance.BringIn("MultiplayerScenePanel");
			break;
			
			case PanelToLoad.LevelSelection:
				UIPanelManager.instance.BringIn("LevelSelectionScenePanel");
			break;
		}
		
		// Se a foto do usuario estiver nula, abrir conexao pra baixar ela
		if(Flow.playerPhoto == null && !Flow.isDownloadingPlayerPhoto)
		{
			GameRawAuthConnection conn2 = new GameRawAuthConnection(Flow.URL_BASE + "login/picture.php", Flow.getPlayerPhoto);
            WWWForm form2 = new WWWForm();
            form2.AddField("user_id", "me");
            conn2.connect(form2);
            Flow.isDownloadingPlayerPhoto = true;
		}
		
		Advertisement.Banner.Show();
		
		// TESTE
		//Flow.header.coins = 2000;
		int firstWorld = 9999;
		foreach(KeyValuePair<int,World> w in Flow.worldDict)
		{
			if(w.Key < firstWorld) firstWorld = w.Key;
		}
		
		for(int i = 0 ; i < worldObjects.Length ; i++)
		{
			int levelCounter = 0;
			
			worldObjects[i].GetComponent<World>().id = i+firstWorld;
			
			foreach(Transform child in worldObjects[i].transform)
			{
				int firstLevel = 9999;
				
				foreach(KeyValuePair<int,Level> l in Flow.worldDict[firstWorld+i].levelDict)
				{
					if(l.Key < firstLevel) firstLevel = l.Key;
				}
				
				Debug.Log("firstLevel "+firstLevel);
				
				if(Flow.worldDict[firstWorld+i].levelDict[firstLevel+ levelCounter].id != 7)
				{
					Flow.worldDict[firstWorld+i].levelDict[firstLevel+ levelCounter].toUnlock = 
						Flow.worldDict[firstWorld+i].levelDict[firstLevel+ levelCounter-1].id;
				}
				else 
				{
					Flow.worldDict[firstWorld+i].levelDict[firstLevel+ levelCounter].toUnlock = 0;
				}
				
				child.GetComponent<Level>().SetLevel(Flow.worldDict[firstWorld+i].levelDict[firstLevel+ levelCounter]);
				
				levelCounter++;
			}
		}
		
	}
	
	void Connect()
	{
		GameJsonConnection conn = new GameJsonConnection(Flow.URL_BASE + "mines/getsingle.php", OnGetSingle);
		WWWForm form = new WWWForm();
		form.AddField("date", Flow.lastUpdate.ToString());
		
		conn.connect(form);
	}
	
	public void OnGetSingle(string error, IJSonObject data)
	{
		if(error != null) Debug.Log(error);
		else
		{
			Debug.Log(data);
			
			foreach(IJSonObject level in data.ArrayItems)
			{	
				if(DateTime.Parse(level["lastUpdate"].StringValue) > Flow.lastUpdate)
				{
					/*foreach(KeyValuePair<int,World> w in Flow.worldDict)
					{
						Debug.Log("worldID: " + w.Key);
						foreach(KeyValuePair<int,Level> l in w.Value.levelDict)
						{
							Debug.Log("levelID: " + l.Key);
						}
					}*/
					
					//Debug.Log(Flow.worldDict[3].levelDict[7].id);
					
					Debug.Log("world: "+level["worldID"].Int32Value);
					Debug.Log("level: "+level["levelID"].Int32Value);
					
					Flow.worldDict[level["worldID"].Int32Value].levelDict[level["levelID"].Int32Value].lastUpdate = level["lastUpdate"].DateTimeValue;
					
					Flow.worldDict[level["worldID"].Int32Value].levelDict[level["levelID"].Int32Value].tileset = new List<int>();
					
					//string[] teste = level["levelTileset"].StringValue.
					
					for(int i = 0; i < level["levelTileset"].StringValue.Length; i++)
					{
						Flow.worldDict[level["worldID"].Int32Value].levelDict[level["levelID"].Int32Value].tileset.Add
							(int.Parse(level["levelTileset"].StringValue[i].ToString()));
					}
				}
			}
			
			//for(int i = 0 ; 
			
			//Application.LoadLevel("Game");
		}
	}
}
