using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CustomLevelButton : MonoBehaviour
{
	public int world = 0;
	public int level = 0;
	private string[] worldNames = new string[5]{"Peru", "Tibet", "Greece", "Amazonia", "Egypt"};
	public AudioClip startGameAudio;
	public AudioClip deleteAudio;
	public AudioClip confirmDeletionAudio;
	public AudioClip cancelDeletionAudio;
	public AudioClip challengeFriendAudio;
	
	void Start()
	{
		transform.FindChild("Level").FindChild("control_text").GetComponent<SpriteText>().Text = "Custom Stage " + (level+1);
		transform.FindChild("Level").FindChild("name").GetComponent<SpriteText>().Text = Flow.customStages[transform.GetComponent<UIListItem>().Index].name;
		transform.FindChild("Level").FindChild("mines").GetComponent<SpriteText>().Text = "Mines: " + 
			Flow.customStages[transform.GetComponent<UIListItem>().Index].numberOfMines;
	}
	
	public void StartCustomGame()
	{
		audio.PlayOneShot(startGameAudio, PlayerPrefs.GetFloat(PlayerPrefsKeys.VOLUME));
		
		Flow.currentGame = new Game();
		Flow.currentCustomStage = level;
		
		Flow.path = TurnStatus.AnswerGame;
		Flow.currentGame.friend = new Friend();
		
		Flow.currentGame.theirRoundList = new List<Round>();
		Flow.currentGame.myRoundList = new List<Round>(){new Round(-1,-1,-1,-1,-1,-1)};
		
		Flow.config.GetComponent<ConfigManager>().inviteAllScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().invitePlayingScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().challengeInviteScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		
		Application.LoadLevel("Game");
	}
	
	public void Delete()
	{
		audio.PlayOneShot(deleteAudio, PlayerPrefs.GetFloat(PlayerPrefsKeys.VOLUME));
		Flow.game_native.showMessageOkCancel(this, "ConfirmDeletion", ConfirmDeletionDelegate, "", "Are you sure?",
			"If you delete " + Flow.customStages[transform.GetComponent<UIListItem>().Index].name + " you will not be able to recover it ever again!", "Delete", "Return");
	}
	
	public void ConfirmDeletion()
	{
		audio.PlayOneShot(confirmDeletionAudio, PlayerPrefs.GetFloat(PlayerPrefsKeys.VOLUME));
		Flow.messageOkCancelDialog.SetActive(false);
		transform.parent.parent.GetComponent<UIScrollList>().RemoveItem(transform.GetComponent<UIListItem>(), true);
		Flow.RemoveCustomLevel(transform.GetComponent<UIListItem>().Index);
	}
	
	public void ConfirmDeletionDelegate(string buttonPressed)
	{
		if(buttonPressed.ToLower() == "ok") ConfirmDeletion();
	}
	
	public void SendToFriend()
	{
		if(Save.HasKey(PlayerPrefsKeys.TOKEN.ToString()))
		{
			audio.PlayOneShot(challengeFriendAudio, PlayerPrefs.GetFloat(PlayerPrefsKeys.VOLUME));
			transform.parent.parent.GetComponent<CustomLevelScroll>().SendToFriend(Flow.customStages[transform.GetComponent<UIListItem>().Index]);
		}
		else
		{
			audio.PlayOneShot(cancelDeletionAudio, PlayerPrefs.GetFloat(PlayerPrefsKeys.VOLUME));
			Flow.panelAfterLogin = "CustomLevelsScenePanel";
			UIPanelManager.instance.BringIn("LoginScenePanel");
		}
	}
}