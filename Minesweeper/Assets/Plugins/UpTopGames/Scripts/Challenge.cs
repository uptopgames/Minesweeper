using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;
using System;

public class Challenge : MonoBehaviour
{
	public UIStateToggleBtn challengeToggle;
	public UIBistateInteractivePanel newPanel;
	public UIBistateInteractivePanel oldPanel;
	
	public GameObject newContainerPrefab;
	public GameObject oldContainerPrefab;
	
	public SpriteText noFriendsLabel;
	
	public CustomLevelScroll customLevelScroll;
	
	void Start () 
	{
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(InitChallenge);
				
		challengeToggle.SetState(0);
		newPanel.gameObject.SetActive(true);
		oldPanel.gameObject.SetActive(false);
		RefreshNewScroll();
		RefreshOldScroll();
		
		if(newPanel.transform.FindChild("NewScroll").GetComponent<UIScrollList>().Count != 0) noFriendsLabel.Text = "";
		else noFriendsLabel.Text = "No New Challenges";
	}
	
	void InitChallenge(EZTransition transition)
	{
		Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(true);
		
		newPanel.gameObject.SetActive(true);
		oldPanel.gameObject.SetActive(false);
		
		if(newPanel.transform.FindChild("NewScroll").GetComponent<UIScrollList>().Count != 0) noFriendsLabel.Text = "";
		else noFriendsLabel.Text = "No New Challenges";
		
		// Manda carregar o Video, se o sistema de ADS estiver rodando e o cara tiver permissao de assistir aquele video naquela hora
        if (Advertisement.IsRunning()) 
		{
			Debug.Log("ads running");
			if(Save.HasKey(PlayerPrefsKeys.VIDEO))
			{
				Debug.Log("has video key");
				if((DateTime.Parse(Save.GetString(PlayerPrefsKeys.VIDEO)) - DateTime.UtcNow) <= TimeSpan.FromDays(1))
				{
					Debug.Log("already watched for the time being");
					return;
				}
			}
			
			Debug.Log("FETCH VIDEO!");
			Advertisement.Video.Fetch();
		}
		
		Connect();
		
		Debug.Log("chamou InitChallenge");
	}
	
	void Connect()
	{
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "mines/getcustomgames.php", OnGetCustomGames);
		conn.connect();
	}
	
	void OnGetCustomGames(string error, IJSonObject data)
	{
		if(error != null)
		{
			Debug.Log(error);
		}
		else
		{
			Debug.Log(data);
			
			newPanel.transform.FindChild("NewScroll").GetComponent<UIScrollList>().ClearList(true);
			oldPanel.transform.FindChild("OldScroll").GetComponent<UIScrollList>().ClearList(true);
			
			foreach(IJSonObject item in data.ArrayItems)
			{
				CreateChallengeContainerImproved(item, data);
			}
		}
	}
	
	void CreateChallengeContainerImproved(IJSonObject item, IJSonObject data)
	{
		CustomStage cs = new CustomStage();
		cs.creatorName = item["username"].ToString();
		cs.id = item["customLevelID"].Int32Value;
		cs.isChallenge = true;
		if(item["time"].IsNull && item["isMe"].BooleanValue)
		{
			cs.isNew = true;
		}
		else
		{
			cs.isNew = false;
		}
		cs.name = item["name"].ToString();
		cs.world = item["worldID"].Int32Value - 3;
		cs.hostID = item["creatorID"].StringValue;
		
		int numberOfMines = 0;
		string testTileset = "";
		List<List<int>> tileset = new List<List<int>>();
		for(int i = 0; i < 8; i++)
		{
			List<int> row = new List<int>();
			for(int j = 0; j < 8; j++)
			{
				testTileset += item["tileset"].StringValue[i*8+j].ToString();
				row.Add(int.Parse(item["tileset"].StringValue[i*8+j].ToString()));
				if(int.Parse(item["tileset"].StringValue[i*8+j].ToString())==1) numberOfMines++;
			}
			tileset.Add(row);
		}
		cs.numberOfMines = numberOfMines;
		cs.tileset = tileset;
		//falta definir as estrelas, se for ter mesmo
		
		Debug.Log("chegou um challenge de tileset " + testTileset);
		
		Flow.customGames.Add(cs);
		
		if(!cs.isNew)
		{
			foreach(CustomStage c in Flow.customGames)
			{
				if(c.id == cs.id && Flow.customGames.IndexOf(c)!=Flow.customGames.Count-1)
				{
					Debug.Log("impedindo que crie um container novo para um ranking que ja existe");
					return;
				}
			}
			foreach(IJSonObject subItem in data.ArrayItems)
			{
				Debug.Log(subItem);
				if(subItem["customLevelID"].Int32Value == cs.id && subItem["isMe"].BooleanValue)
				{
					Debug.Log("impedindo que crie um container novo para um ranking que ja existe");
					return;
				}
			}
		}
		
		IUIListObject g = null;
		if(cs.isNew)
		{
			g = newPanel.transform.FindChild("NewScroll").GetComponent<UIScrollList>().CreateItem(newContainerPrefab);
			g.transform.FindChild("Name").GetComponent<SpriteText>().Text = cs.creatorName;
			g.transform.FindChild("Mines").GetComponent<SpriteText>().Text = "Mines: " + cs.numberOfMines;
			g.transform.FindChild("StageName").GetComponent<SpriteText>().Text = cs.name;
			g.transform.FindChild("World"+(1+cs.world).ToString()).gameObject.SetActive(true);
			
			Debug.Log("adicionei no novo");
		}
		else if(!cs.isNew)
		{
			g = oldPanel.transform.FindChild("OldScroll").GetComponent<UIScrollList>().CreateItem(oldContainerPrefab);
			g.transform.FindChild("Name").GetComponent<SpriteText>().Text = cs.name;
			g.transform.FindChild("Mines").GetComponent<SpriteText>().Text = "Mines: " + cs.numberOfMines;
			g.transform.FindChild("Host").GetComponent<SpriteText>().Text = "Hosted by " + cs.creatorName;
			g.transform.FindChild("World"+(1+cs.world).ToString()).gameObject.SetActive(true);
			
			Debug.Log("adicionei no velho");
		}
		
		g.transform.GetComponent<ChallengesButton>().challengeIndex = Flow.customGames.IndexOf(cs);
		QuickSwap();
		
		foreach(CustomStage c in Flow.customStages)
		{
			if(c.id == cs.id || (c.tileset == cs.tileset && c.world == cs.world))
			{
				g.transform.GetComponent<ChallengesButton>().customLevelsIndex = Flow.customStages.IndexOf(c);
				return;
			}
		}
		
		Flow.AddCustomStage(cs.tileset, cs.world, cs.numberOfMines, cs.name, cs.id, cs.isNew, cs.isChallenge, cs.creatorName);
		g.transform.GetComponent<ChallengesButton>().customLevelsIndex = Flow.customStages.Count-1;
		customLevelScroll.AddContainer(cs);
		Debug.Log("adicionei o level " + cs.name);
	}
	
	/*void CreateChallengeContainer(IJSonObject item)
	{	
		foreach(CustomStage c in Flow.customStages)
		{
			if(c.id == item["customLevelID"].Int32Value)
			{
				//php is bringing all games of people who belong to a ranking that you belong to;
				//the rankings you belong to are the rankings whose stages are equal to a stage you have received
				
				//is not showing games whose stage is the same of another game already being shown
				
				Debug.Log("O level de id " + c.id + " jah existe aqui");
				c.creatorName = item["playername"].ToString();
				c.isChallenge = true;
				if(item["time"].IsNull) c.isNew = true;
				else c.isNew = false;
		
				RefreshNewScroll();
				RefreshOldScroll();
				QuickSwap();
				
				return;
			}
		}
		
		int numberOfMines = 0;
		List<List<int>> tileset = new List<List<int>>();
		for(int i = 0; i < 8; i++)
		{
			List<int> row = new List<int>();
			for(int j = 0; j < 8; j++)
			{
				if(int.Parse(item["tileset"].StringValue[i+j].ToString())==1) numberOfMines++;
				row.Add(int.Parse(item["tileset"].StringValue[i+j].ToString()));
			}
			tileset.Add(row);
		}
		
		bool isNew = false;
		if(item["time"].IsNull) isNew = true;
		
		Flow.AddCustomStage(tileset, item["worldID"].Int32Value - 3, numberOfMines, item["name"].ToString(), item["customLevelID"].Int32Value, isNew, true,
			item["playername"].ToString());
		Debug.Log("adicionei o level " + item["name"].ToString());
		
		customLevelScroll.AddContainer(Flow.customStages[Flow.customStages.Count-1]);
		
		RefreshNewScroll();
		RefreshOldScroll();
		QuickSwap();
	}
	*/
	
	public void QuickSwap()
	{	
		if(oldPanel.transform.FindChild("OldScroll").GetComponent<UIScrollList>().Count != 0) noFriendsLabel.Text = "";
		else noFriendsLabel.Text = "No Rankings";
			
		if(newPanel.IsShowing)
		{
			newPanel.Hide();
			newPanel.gameObject.SetActive(false);
		}
		oldPanel.gameObject.SetActive(true);
		oldPanel.Reveal();
		
		if(newPanel.transform.FindChild("NewScroll").GetComponent<UIScrollList>().Count != 0) noFriendsLabel.Text = "";
		else noFriendsLabel.Text = "No New Challenges";
		
		newPanel.Reveal();
		newPanel.gameObject.SetActive(true);
		if(oldPanel.IsShowing)
		{
			oldPanel.Hide();
			oldPanel.gameObject.SetActive(false);
		}
	}
	
	public void RefreshNewScroll()
	{
		//Conect with Server and Fill the Arrays stored in Flow
		newPanel.transform.FindChild("NewScroll").GetComponent<UIScrollList>().ClearList(true);
		foreach(CustomStage c in Flow.customStages)
		{
			if(c.isNew && c.isChallenge)
			{
				IUIListObject g = newPanel.transform.FindChild("NewScroll").GetComponent<UIScrollList>().CreateItem(newContainerPrefab);
				g.transform.FindChild("Name").GetComponent<SpriteText>().Text = c.creatorName;
				g.transform.FindChild("Mines").GetComponent<SpriteText>().Text = "Mines: " + c.numberOfMines;
				g.transform.FindChild("StageName").GetComponent<SpriteText>().Text = c.name;
				g.transform.FindChild("World"+(1+c.world).ToString()).gameObject.SetActive(true);
			}
		}
	}
	
	public void RefreshOldScroll()
	{
		//Conect with Server and Fill the Arrays stored in Flow
		oldPanel.transform.FindChild("OldScroll").GetComponent<UIScrollList>().ClearList(true);
		foreach(CustomStage c in Flow.customStages)
		{
			if(!c.isNew && c.isChallenge)
			{
				IUIListObject g = oldPanel.transform.FindChild("OldScroll").GetComponent<UIScrollList>().CreateItem(oldContainerPrefab);
				g.transform.FindChild("Name").GetComponent<SpriteText>().Text = c.name;
				g.transform.FindChild("Mines").GetComponent<SpriteText>().Text = "Mines: " + c.numberOfMines;
				g.transform.FindChild("Host").GetComponent<SpriteText>().Text = "Hosted by " + c.creatorName;
				g.transform.FindChild("World"+(1+c.world).ToString()).gameObject.SetActive(true);
				g.transform.GetComponent<ChallengesButton>().challengeIndex = Flow.customStages.IndexOf(c);
			}
		}
	}
	
	public void Toggle()
	{
		if(challengeToggle.StateName == "New")
		{
			if(newPanel.transform.FindChild("NewScroll").GetComponent<UIScrollList>().Count != 0) noFriendsLabel.Text = "";
			else noFriendsLabel.Text = "No New Challenges";
			
			newPanel.Reveal();
			newPanel.gameObject.SetActive(true);
			if(oldPanel.IsShowing)
			{
				oldPanel.Hide();
				oldPanel.gameObject.SetActive(false);
			}
		}
		else
		{
			if(oldPanel.transform.FindChild("OldScroll").GetComponent<UIScrollList>().Count != 0) noFriendsLabel.Text = "";
			else noFriendsLabel.Text = "No Rankings";
			
			if(newPanel.IsShowing)
			{
				newPanel.Hide();
				newPanel.gameObject.SetActive(false);
			}
			oldPanel.gameObject.SetActive(true);
			oldPanel.Reveal();
		}
	}
	
	public void CreateGame()
	{
		Flow.config.GetComponent<ConfigManager>().inviteAllScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().invitePlayingScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().challengeInviteScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.currentMode = GameMode.Multiplayer;
		Application.LoadLevel("CustomStage");
	}
}
