using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ChallengesButton : MonoBehaviour
{
	public int challengeIndex = -1;
	public int customLevelsIndex = -1;
	public int gameIndex = -1;
	
	// Use this for initialization
	void Start () {
	
	}
	
	void RankingScreen ()
	{
		Flow.currentRank = Flow.customGames[challengeIndex];
		Flow.currentCustomGame = gameIndex;
		Debug.Log("currentCustomGame (ChallengeButton) " + Flow.currentCustomGame);
		
		UIPanelManager.instance.BringIn("RankingsScenePanel", UIPanelManager.MENU_DIRECTION.Backwards);
	}
	
	void CreateGame()
	{
		Flow.currentRank = Flow.customGames[challengeIndex];
		Flow.currentCustomGame = gameIndex;
		Debug.Log("currentCustomGame (ChallengeButton) " + Flow.currentCustomGame);
		
		Flow.currentMode = GameMode.Multiplayer;
		
		Flow.currentGame = new Game();
		Flow.currentCustomStage = customLevelsIndex;
		
		Flow.path = TurnStatus.AnswerGame;
		Flow.currentGame.friend = new Friend();
		
		Flow.currentGame.theirRoundList = new List<Round>();
		Flow.currentGame.myRoundList = new List<Round>(){new Round(-1,-1,-1,-1,-1,-1)};
		
		Flow.config.GetComponent<ConfigManager>().inviteAllScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().invitePlayingScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().challengeInviteScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		
		Application.LoadLevel("Game");
	}
}
