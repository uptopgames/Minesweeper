using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class RankingsPanel : MonoBehaviour
{
	public SpriteText stageName;
	public SpriteText hostName;
	public GameObject playAgainButton;
	public GameObject challengeButton;
	public GameObject rankingsPrefab;
	public CustomLevelScroll customLevelScroll;
	public UIScrollList scroll;
	public int maximumContainers = 10;
	
	private int containerCounter = 0;

	void Start ()
	{
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(InitRankings);
		GetComponent<UIInteractivePanel>().transitions.list[1].AddTransitionStartDelegate(InitRankings);
		GetComponent<UIInteractivePanel>().transitions.list[2].AddTransitionStartDelegate(InitRankings);
		GetComponent<UIInteractivePanel>().transitions.list[3].AddTransitionStartDelegate(InitRankings);
	}
	
	void InitRankings(EZTransition transition)
	{
		Debug.Log("transition rankings");
		stageName.Text = Flow.currentRank.name;
		hostName.Text = "Hosted by " + Flow.currentRank.creatorName;
		if(Flow.currentRank.hostID == Save.GetString(PlayerPrefsKeys.ID))
		{
			playAgainButton.SetActive(false);
			challengeButton.SetActive(true);
		}
		else
		{
			playAgainButton.SetActive(true);
			challengeButton.SetActive(false);
		}
		transform.FindChild("World"+(Flow.currentRank.world+1).ToString()).gameObject.SetActive(true);
		Connect();
	}
	
	void CreateGame()
	{
		Flow.currentMode = GameMode.Multiplayer;
		
		Flow.currentGame = new Game();
		foreach(CustomStage c in Flow.customStages)
		{
			if(Flow.currentRank.id == c.id)
			{
				Flow.currentCustomStage = Flow.customStages.IndexOf(c);
			}
		}
		
		Flow.path = TurnStatus.AnswerGame;
		Flow.currentGame.friend = new Friend();
		
		Flow.currentGame.theirRoundList = new List<Round>();
		Flow.currentGame.myRoundList = new List<Round>(){new Round(-1,-1,-1,-1,-1,-1)};
		
		Flow.config.GetComponent<ConfigManager>().inviteAllScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().invitePlayingScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().challengeInviteScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		
		Application.LoadLevel("Game");
		//Flow.game_native.showMessage("Feature not Implemented Yet", "Please wait until we implement this feature", "Ok");
	}
	
	void Challenge()
	{
		customLevelScroll.SendToFriend(Flow.currentRank);
		//Flow.game_native.showMessage("Feature not Implemented Yet", "Please wait until we implement this feature", "Ok");
	}
	
	public void AddFriendConnection(string friendID)
	{
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "mines/sharestage.php", OnShareStage);
		WWWForm form = new WWWForm();
		form.AddField("stageName", Flow.currentRank.name);
		form.AddField("stageID", Flow.currentRank.id);
		form.AddField("friendID", friendID);
		
		int firstWorld = 9999; foreach(KeyValuePair<int,World> w in Flow.worldDict) {if(w.Key < firstWorld) firstWorld = w.Key;}
		form.AddField("world", (firstWorld+Flow.currentRank.world).ToString());
		
		string tileset = "";
		foreach(List<int> listInt in Flow.currentRank.tileset) foreach(int i in listInt) tileset += i;
		Debug.Log(tileset);
		form.AddField("tileset", tileset);
		
		conn.connect(form);
	}
	
	public void OnShareStage(string error, IJSonObject data)
	{
		if(error != null)
		{
			Debug.Log(error);
			
			Flow.game_native.showMessage("Error", "Please check your internet connection then try again", "Ok");
			customLevelScroll.scroll.transform.gameObject.SetActive(false);
			customLevelScroll.EraseFriendsList();
		}
		else
		{
			Debug.Log(data);
			
			Flow.game_native.showMessage("Stage Sent", "You can check the online ranking of your stage on the 'Challenges' Menu", "Ok");
			customLevelScroll.scroll.transform.gameObject.SetActive(false);
			customLevelScroll.EraseFriendsList();
			
			Connect();
		}
	}
	
	public void Connect()
	{
		scroll.ClearList(true);
		containerCounter = 0;
		
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "mines/getranking.php", OnGetRanking);
		WWWForm form = new WWWForm();
		form.AddField("customLevelID", Flow.currentRank.id);
		
		conn.connect(form);
	}
	
	void OnGetRanking(string error, IJSonObject data)
	{
		if(error != null)
		{
			Debug.Log(error);
		}
		else
		{
			Debug.Log(data);
			foreach(IJSonObject item in data.ArrayItems) if(containerCounter<maximumContainers) CreateContainer(item);
		}
	}
	
	void CreateContainer(IJSonObject item)
	{
		containerCounter++;
		GameObject g = scroll.CreateItem(rankingsPrefab).gameObject;
		g.transform.FindChild("Name").GetComponent<SpriteText>().Text = item["name"].StringValue;
		g.transform.FindChild("Time").GetComponent<SpriteText>().Text = "Time: " + item["time"].StringValue;
		g.transform.FindChild("Deaths").GetComponent<SpriteText>().Text = "Deaths: " + item["deaths"].StringValue;
		g.transform.FindChild("Matches").GetComponent<SpriteText>().Text = "Tries: " + item["tries"].StringValue;
		g.transform.FindChild("Ranking").GetComponent<SpriteText>().Text = containerCounter.ToString();
	}
}