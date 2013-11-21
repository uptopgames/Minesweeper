using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;

public class CustomLevelScroll : MonoBehaviour
{
	public GameObject[] customLevelPrefabs;
	
	public GameObject letterPrefab;
	public GameObject friendPrefab;
	public UIScrollList scroll;
	public SpriteText noFriendsLabel;
	
	public CustomStage currentCustomStage;
	
	public bool isRankingScreen = false;
	
	// Use this for initialization
	void Start ()
	{
		if(!isRankingScreen)
		{
			transform.parent.GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(InitCustomLevelScroll);
			transform.parent.GetComponent<UIInteractivePanel>().transitions.list[1].AddTransitionStartDelegate(InitCustomLevelScroll);
			transform.parent.GetComponent<UIInteractivePanel>().transitions.list[2].AddTransitionStartDelegate(InitCustomLevelScroll);
			transform.parent.GetComponent<UIInteractivePanel>().transitions.list[3].AddTransitionStartDelegate(InitCustomLevelScroll);
		}
	}
	
	void InitCustomLevelScroll(EZTransition transition)
	{
		InitCall();
	}
	
	public void InitCall()
	{
		GetComponent<UIScrollList>().ClearList (true);
		if(Flow.customStages != null)
		{
			foreach(CustomStage c in Flow.customStages)
			{
				AddContainer(c);
			}
		}
		
		scroll = Flow.config.GetComponent<ConfigManager>().challengeInviteScroll;
		scroll.transform.parent = transform;
		scroll.transform.localPosition = new Vector3(-0.1220818f, -0.584363f, -7.011475f);
	}
	
	public void AddContainer(CustomStage c)
	{
		GameObject stage = GetComponent<UIScrollList>().CreateItem(customLevelPrefabs[c.world]).gameObject;
		stage.GetComponent<CustomLevelButton>().world = c.world;
		stage.GetComponent<CustomLevelButton>().level = Flow.customStages.IndexOf(c);
	}
	
	public void StartStageCreator()
	{
		Flow.config.GetComponent<ConfigManager>().inviteAllScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().invitePlayingScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().challengeInviteScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.currentMode = GameMode.SinglePlayer;
		Debug.Log(Flow.currentMode);
		Application.LoadLevel("CustomStage");
	}
	
	public void BackButton()
	{
		EraseFriendsList();
		//GetComponent<UIScrollList>().ClearList (true);
		UIPanelManager.instance.BringIn("LevelSelectionPanel", UIPanelManager.MENU_DIRECTION.Backwards);
	}
	
	public void SendToFriend(CustomStage level)
	{
		if(isRankingScreen)
		{
			InitCall();
		}
		
		currentCustomStage = level;
		EnteredInvite();
	}
	
	public void Connect(string friendID)
	{
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "mines/sharestage.php", OnShareStage);
		WWWForm form = new WWWForm();
		form.AddField("stageName", currentCustomStage.name);
		form.AddField("stageID", -1);
		form.AddField("friendID", friendID);
		
		int firstWorld = 9999; foreach(KeyValuePair<int,World> w in Flow.worldDict) {if(w.Key < firstWorld) firstWorld = w.Key;}
		form.AddField("world", (firstWorld+currentCustomStage.world).ToString());
		
		string tileset = "";
		foreach(List<int> listInt in currentCustomStage.tileset) foreach(int i in listInt) tileset += i;
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
			scroll.transform.gameObject.SetActive(false);
			EraseFriendsList();
		}
		else
		{
			Debug.Log(data);
			
			currentCustomStage.id = data.Int32Value;
			Flow.UpdateXML();
			
			//currentCustomStage.id = data["stageID"].Int32Value;
			
			Flow.game_native.showMessage("Stage Sent", "You can check the online ranking of your stage on the 'Challenges' Menu", "Ok");
			scroll.transform.gameObject.SetActive(false);
			EraseFriendsList();
			
			UIPanelManager.instance.BringIn("ChallengesScenePanel", UIPanelManager.MENU_DIRECTION.Backwards);
		}
	}
	
	public void GetFriends()
	{
		Flow.game_native.startLoading();
		UIManager.instance.blockInput = true;
		GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/list.php", HandleGetFriends);
		conn.connect(null, 10);
	}
	
	public void EnteredInvite()
	{	
		scroll.gameObject.SetActive(true);
		
		if(scroll.Count == 0)
		{
			GetFriends();
		}
	}
	
	// Obtem as informacoes dos amigos do usuario
	void HandleGetFriends(string error, IJSonObject data, object counter_o)
	{
		Debug.Log(data);
		int counter = (int) counter_o;
		
		Flow.game_native.stopLoading();
		UIManager.instance.blockInput = false;
		
		if (error != null || data == null)
		{
			if (counter > 0)
			{
				GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/friends/list.php", HandleGetFriends);
				conn.connect(null, counter - 1);
				return;
			}
			
			Flow.game_native.showMessage("Error", error);
			return;
		}
		
		string allLetter = "";
		string playingLetter = "";
		if(data.Count>0)
		{
			allLetter = data[0]["name"].StringValue.Substring(0,1).ToUpper();
			GameObject firstL = GameObject.Instantiate(letterPrefab) as GameObject;
			
			firstL.transform.FindChild("Letter").GetComponent<SpriteText>().Text = allLetter.ToUpper ();
			scroll.AddItem(firstL);
		}
		
		foreach (IJSonObject friend in data.ArrayItems)
		{
			GameObject allContainer = CreateFriendContainer(friend);
			
			if(friend["name"].StringValue.Substring(0,1).ToUpper() != allLetter)
			{
				allLetter = friend["name"].StringValue.Substring(0,1).ToUpper();
				GameObject l = GameObject.Instantiate(letterPrefab) as GameObject;
				l.transform.FindChild("Letter").GetComponent<SpriteText>().Text = allLetter.ToUpper ();
				scroll.AddItem(l);
			}
			scroll.AddItem(allContainer);
		}
		
		if(data.Count==0)
		{
			noFriendsLabel.gameObject.SetActive(true);
		}
		else
		{
			noFriendsLabel.gameObject.SetActive(false);
		}
	}
	
	GameObject CreateFriendContainer(IJSonObject friend)
	{
		GameObject t = GameObject.Instantiate(friendPrefab) as GameObject;
		t.GetComponent<Friend>().SetFriend(
			friend["user_id"].ToString(), 
			friend["facebook_id"].ToString(),
			friend["name"].ToString(),
			friend["from_facebook"].BooleanValue? FriendshipStatus.FACEBOOK: FriendshipStatus.STANDALONE,
			friend["is_playing"].BooleanValue
			//null,
			);
		
		t.transform.FindChild("Name").GetComponent<SpriteText>().Text = friend["name"].ToString();
		
		if(t.GetComponent<Friend>().status == FriendshipStatus.FACEBOOK)
		{
			t.transform.FindChild("FacebookIcon").gameObject.SetActive(true);
		}
		else
		{
			t.transform.FindChild("StandaloneIcon").gameObject.SetActive(true);
		}
		
		return t;
	}
	
	GameObject CreateFriendContainer(Friend friend)
	{
		GameObject t = GameObject.Instantiate(friendPrefab) as GameObject;
		t.GetComponent<Friend>().SetFriend(
			friend.id, 
			friend.facebook_id,
			friend.name,
			friend.status,
			friend.is_playing);
		
		t.transform.FindChild("Name").GetComponent<SpriteText>().Text = friend.name;
		
		if(t.GetComponent<Friend>().status == FriendshipStatus.FACEBOOK)
		{
			t.transform.FindChild("FacebookIcon").gameObject.SetActive(true);
		}
		else
		{
			t.transform.FindChild("StandaloneIcon").gameObject.SetActive(true);
		}
		
		return t;
	}
	
	public void EraseFriendsList()
	{
		scroll.transform.gameObject.SetActive(false);
		scroll.transform.parent = Flow.config.transform.FindChild("-Lists-");
	}
}