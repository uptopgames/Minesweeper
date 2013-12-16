using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Level : MonoBehaviour
{
	public int id = 0;
	public string name = "";
	public int stars = 0;
	public int toUnlock;
	public int points;
	public Vector2 time;
	public DateTime lastUpdate = new DateTime(1);
	public Texture2D image;
	public bool isDownloading = false;
	public List<int> tileset = new List<int>();
	
	public void SetLevel(Level level)
	{
		this.id = level.id;
		this.name = level.name;
		this.stars = level.stars;
		this.toUnlock = level.toUnlock;
		this.time = level.time;
		this.lastUpdate = level.lastUpdate;
		this.tileset = level.tileset;
	}
	
	void EnterLevel()
	{
		Flow.currentCustomStage = -1;
		
		if (Save.GetInt (PlayerPrefsKeys.POINTS) >= toUnlock)
		{
			if(Flow.currentMode == GameMode.SinglePlayer) Flow.currentGame = new Game();
			
			Flow.currentGame.world = gameObject.transform.parent.GetComponent<World>();
			Flow.currentGame.level = this;
			
			if(Flow.currentMode == GameMode.SinglePlayer)
			{
				Flow.path = TurnStatus.AnswerGame;
				Flow.currentGame.friend = new Friend();
				Flow.currentGame.friend.rawText = image;
				
				Flow.currentGame.theirRoundList = new List<Round>();
			}
			
			Flow.config.GetComponent<ConfigManager>().inviteAllScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
			Flow.config.GetComponent<ConfigManager>().invitePlayingScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
			Flow.config.GetComponent<ConfigManager>().challengeInviteScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
			
			Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(false);
			Application.LoadLevel("Game");
		}
		else
		{
			Debug.Log ("locked");
			Flow.game_native.showMessage("Locked Level", "You must beat the previous level to unlock this one.", "Ok");
		}
	}
	
	public void Start()
	{
		SetStars();
	}
	
	public void SetStars()
	{
		if(Save.GetInt (PlayerPrefsKeys.POINTS) >= toUnlock)
		{
			transform.FindChild("Locker").gameObject.SetActive(false);
			
			for(int i = 1 ; i <= stars ; i++)
			{
				transform.FindChild("LevelStar"+i).gameObject.SetActive(true);
				transform.FindChild("LevelStarGrey"+i).gameObject.SetActive(false);
			}
		}
		else 
		{
			transform.FindChild("Locker").gameObject.SetActive(true);
			
			for(int i = 1 ; i <= stars ; i++)
			{
				transform.FindChild("LevelStar"+i).gameObject.SetActive(false);
				transform.FindChild("LevelStarGrey"+i).gameObject.SetActive(true);
			}
		}
	}
}