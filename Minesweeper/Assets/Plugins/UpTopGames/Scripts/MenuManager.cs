using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class MenuManager : MonoBehaviour 
{

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
			
			case PanelToLoad.GunSelection:
				UIPanelManager.instance.BringIn("GunSelectionScenePanel");
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
						Debug.Log("worldID: " + w.Value.id);
						foreach(KeyValuePair<int,Level> l in w.Value.levelDict)
						{
							Debug.Log("levelID: " + l.Value.id);
						}
					}*/
					
					Debug.Log(Flow.worldDict[3].levelDict[7].id);
					
					/*Flow.worldDict[level["worldID"].Int32Value].levelDict[level["levelID"].Int32Value].lastUpdate =
						DateTime.Parse(level["lastUpdate"].StringValue);*/
					
					/*Flow.worldDict[level["worldID"].Int32Value].levelDict[level["levelID"].Int32Value].tileset = new List<int>();
					
					for(int i = 0; i < level["levelTileset"].Length; i++)
					{
						Flow.worldDict[level["worldID"].Int32Value].levelDict[level["levelID"].Int32Value].tileset.Add
							(level["levelTileset"][i].Int32Value);
					}*/
				}
			}
			
			Application.LoadLevel("Game");
		}
	}
}
