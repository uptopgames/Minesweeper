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
		
		Debug.Log("chamou InitChallenge");
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
			else noFriendsLabel.Text = "No Old Challenges";
			
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
		Flow.currentMode = GameMode.Multiplayer;
		Application.LoadLevel("CustomStage");
	}
}
