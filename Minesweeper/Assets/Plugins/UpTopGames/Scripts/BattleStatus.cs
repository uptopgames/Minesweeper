using UnityEngine;
using System;
using System.Collections;

public class BattleStatus : MonoBehaviour
{
	// BATTLE STATUS PADRAO
	
	//public SpriteText userName, friendName, turnsLost, turnsWon, userScore, friendScore;
	public UIInteractivePanel lastPastPanel, firstPastPanel, winLosePanel;
	//public SpriteText[] userTimes, friendTimes;
	public GameObject multiplayerPrefab;
	public GameObject coinsGained;
	//public UIScrollList scroll;
	
	// WIN LOSE BANG
	
	public SpriteText winLoseLabel, coinsLabel, userName, friendName, turnsLost, turnsWon;
	
	public MeshRenderer userPicture, friendPicture;
	
	void Awake()
	{
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(FillValues);
	}
	
	// Use this for initialization
	void Start ()
	{
		//Debug.Log ("turnsWon: " + Flow.currentGame.turnsWon);
		//Debug.Log ("turnsLost: " + Flow.currentGame.turnsLost);
		
		firstPastPanel.transform.position = firstPastPanel.transitions.list[0].animParams[0].axis;
			
	}
	
	void HandleGetFriendPicture(string error, WWW conn)
	{
		if(this==null) return; //se mudou de cena, nao precisa mais fazer nada
		
		if (error != null || conn.error != null || conn.bytes.Length == 0)
		{
			Flow.currentGame.friend.rawText = Flow.transparent;
			Flow.currentGame.friend.got_picture = true;
			return;
		}
		
		Flow.currentGame.friend.got_picture = true;
		
		Flow.currentGame.friend.rawText = conn.texture;
		friendPicture.material.mainTexture = conn.texture;
	}
	
	void HandleGetPlayerPicture(string error, WWW data)
    {
        if (error != null)
        {
            if (error.IndexOf("404") >= 0)
            {
				Flow.playerPhoto = Flow.transparent;
            }
        }
        else
        {
            Flow.playerPhoto = data.texture;
			userPicture.material.mainTexture = data.texture;
        }
    }
	
	void FillValues(EZTransition transition)
	{
		//Debug.Log ("FillValues");
		Debug.Log(Save.GetString(PlayerPrefsKeys.NAME.ToString()));
		
		Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(false);
		
		if(Flow.playerPhoto != null) userPicture.material.mainTexture = Flow.playerPhoto;
		else
		{
			GameRawAuthConnection conn2 = new GameRawAuthConnection(Flow.URL_BASE + "login/picture.php", HandleGetPlayerPicture);
            WWWForm form2 = new WWWForm();
            form2.AddField("user_id", "me");
            conn2.connect(form2);
		}
		
		if(Flow.currentGame.friend.rawText) friendPicture.material.mainTexture = Flow.currentGame.friend.rawText;
		else
		{
			GameRawAuthConnection conn = new GameRawAuthConnection(Flow.URL_BASE + "login/picture.php", HandleGetFriendPicture);
			WWWForm form = new WWWForm();
			form.AddField("user_id", Flow.currentGame.friend.id);
			
			conn.connect(form);
		}
		
		userName.Text = Save.GetString(PlayerPrefsKeys.NAME.ToString());
		friendName.Text = Flow.currentGame.friend.name;
		
		turnsWon.Text = Flow.currentGame.turnsWon.ToString();
		turnsLost.Text = Flow.currentGame.turnsLost.ToString();
		
		if(Flow.path == TurnStatus.BeginGame)
		{
			winLoseLabel.transform.localPosition = new Vector3(0,-0.9547787f,0);
			winLoseLabel.Text = "Waiting Your Friend Answer";
			
			coinsGained.gameObject.SetActive(false);
			
			turnsLost.transform.parent.gameObject.SetActive(false);
			turnsWon.transform.parent.gameObject.SetActive(false);
			
			/*Flow.currentGame.myTotalScore = 0;
			foreach(Round r in Flow.currentGame.myRoundList)
			{
				Flow.currentGame.myTotalScore += r.score;
			}*/
			
			/*for(int i = 0; i<friendTimes.Length; i++) friendTimes[i].Text = "";
			//friendScore.Text = "Waiting...";
			for(int i = 0; i<userTimes.Length; i++)
			{
				userTimes[i].Text = "Time " + (i+1).ToString() + ": " + Flow.currentGame.myRoundList[i].time.ToString();
			}*/
		}
		else
		{
			turnsLost.transform.parent.gameObject.SetActive(true);
			turnsWon.transform.parent.gameObject.SetActive(true);
			
			Color winLoseLabelColor = winLoseLabel.GetComponent<SpriteText>().color;
			
			coinsGained.gameObject.SetActive(true);
			if (Flow.playerWin && !Flow.enemyWin) 
			{
				
				winLoseLabelColor.r = 23f/255f;
				winLoseLabelColor.g = 247f/255f;
				winLoseLabelColor.b = 40f/255f;
				
				winLoseLabel.GetComponent<SpriteText>().SetColor(winLoseLabelColor);
				
				winLoseLabel.transform.localPosition = new Vector3(0,0,0);
				winLoseLabel.Text = "You Win!";
				coinsLabel.Text = "x"+ServerSettings.GetInt("GAME_WIN");
			}
			else if (!Flow.playerWin)
			{
				if (Flow.enemyWin)
				{
					winLoseLabelColor.r = 232f/255f;
					winLoseLabelColor.g = 33f/255f;
					winLoseLabelColor.b = 34f/255f;
					winLoseLabel.GetComponent<SpriteText>().SetColor(winLoseLabelColor);
					winLoseLabel.transform.localPosition = new Vector3(0,0,0);
					winLoseLabel.Text = "You Lose!";
					
					coinsLabel.Text = "x"+ServerSettings.GetInt("GAME_LOSE");
				}
				else 
				{
					winLoseLabelColor.r = 0f/255f;
					winLoseLabelColor.g = 0f/255f;
					winLoseLabelColor.b = 0f/255f;
					winLoseLabel.GetComponent<SpriteText>().SetColor(winLoseLabelColor);
					winLoseLabel.transform.localPosition = new Vector3(0,0,0);
					winLoseLabel.Text = "Draw!";
					
					coinsLabel.Text = "x"+ServerSettings.GetInt("GAME_DRAW");
				}				
			}
			
			//turnsLost.Text = "Victories: " + Flow.currentGame.turnsLost.ToString();
			//turnsWon.Text = "Defeats: " + Flow.currentGame.turnsWon.ToString();
		}
		
		if (Flow.path == TurnStatus.ShowPast)
		{
			Debug.Log ("ShowPastPanel "+Flow.currentGame.id+PlayerPrefsKeys.COINS+Flow.currentGame.lastTurnID );
			
			
			if(!Save.HasKey(Flow.currentGame.id+PlayerPrefsKeys.COINS+Flow.currentGame.lastTurnID))
			{
				int playerPoints = 0;
				int enemyPoints = 0;
				for(int i = 0 ; i < Flow.ROUNDS_PER_TURN ;  i++)
				{
					playerPoints += Flow.currentGame.pastMyRoundList[i].playerRoundWin;
					enemyPoints += Flow.currentGame.pastTheirRoundList[i].playerRoundWin;
				}
				
				if(playerPoints > enemyPoints)
				{
					// vitoria
					Flow.header.coins += ServerSettings.GetInt("GAME_WIN");
				}
				else if(enemyPoints > playerPoints)
				{
					// derrota
					Flow.header.coins += ServerSettings.GetInt("GAME_LOSE");
				}
				else
				{
					// empate
					Flow.header.coins += ServerSettings.GetInt("GAME_DRAW");
				}
				
				Save.Set(Flow.currentGame.id+PlayerPrefsKeys.COINS+Flow.currentGame.lastTurnID, true, true);
			}
			
			ShowPastPanel();
		}
		
		/*else if(Flow.path == TurnStatus.AnswerGame)
		{	
			/*Flow.currentGame.myTotalScore = 0;
			foreach(Round r in Flow.currentGame.myRoundList)
			{
				//Flow.currentGame.myTotalScore += r.score;
			}
			Flow.currentGame.theirTotalScore = 0;
			foreach(Round r in Flow.currentGame.theirRoundList)
			{
				//Flow.currentGame.theirTotalScore += r.score;
			}
			
			for(int i = 0; i<friendTimes.Length; i++)
			{
				friendTimes[i].Text = "Time " + (i+1).ToString() + ": " + Flow.currentGame.theirRoundList[i].time.ToString();
			}
			//friendScore.Text = "Score: " + Flow.currentGame.theirTotalScore.ToString();
			for(int i = 0; i<userTimes.Length; i++)
			{
				userTimes[i].Text = "Time " + (i+1).ToString() + ": " + Flow.currentGame.myRoundList[i].time.ToString();
			}
		}*/
		/*else if(Flow.path == TurnStatus.ShowPast)
		{
			Flow.currentGame.myTotalScore = 0;
			foreach(Round r in Flow.currentGame.pastMyRoundList)
			{
				//Flow.currentGame.myTotalScore += r.score;
			}
			Flow.currentGame.theirTotalScore = 0;
			foreach(Round r in Flow.currentGame.pastTheirRoundList)
			{
				//Flow.currentGame.theirTotalScore += r.score;
			}
			
			Debug.Log(Flow.currentGame.pastTheirRoundList.Count);
			
			for(int i = 0; i<friendTimes.Length; i++)
			{
				friendTimes[i].Text = "Time " + (i+1).ToString() + ": " + Flow.currentGame.pastTheirRoundList[i].time.ToString();
			}
			//friendScore.Text = "Score: " + Flow.currentGame.theirTotalScore.ToString();
			for(int i = 0; i<userTimes.Length; i++)
			{
				userTimes[i].Text = "Time " + (i+1).ToString() + ": " + Flow.currentGame.pastMyRoundList[i].time.ToString();
			}
			
			ShowPastPanel();
		}*/
		
		//userScore.Text = "Score: " + Flow.currentGame.myTotalScore.ToString();
	}
	
	public void ShowPastPanel()
	{
		Debug.Log("ShowPastPanel");
		StartPastTransition();
	}
	
	bool clickedNext = false;
	
	void NextButton()
	{
		clickedNext = true;
		
		if(Flow.path == TurnStatus.BeginGame)
		{
			Debug.Log ("current Game ID: " + Flow.currentGame.id);
			Debug.Log ("past Index: " + Flow.currentGame.pastIndex);
			if (Flow.currentGame.id != -1) 
			{
				Flow.gameList[Flow.currentGame.pastIndex].whoseMove = "their";
				Flow.gameList[Flow.currentGame.pastIndex].friend.rawText = Flow.currentGame.friend.rawText;
				Flow.gameList[Flow.currentGame.pastIndex].friend.got_picture = Flow.currentGame.friend.got_picture;
				
				if(Flow.yourTurnGames == 1)
				{
					// os jogos do your turn acabaram, remover a label da gamelist
					Flow.gameList.RemoveAt(0);
				}
				
				// adiciona their turn caso necessario...
				if(Flow.theirTurnGames == 0)
				{
					Game g = new Game();
					g.id = -999;
					g.whoseMove = "their";
					g.lastUpdate = new DateTime(2999,12,31);
					g.friend = new Friend();
					g.friend.id = "-999";
					if(Flow.yourTurnGames > 0) g.pastIndex = Flow.yourTurnGames+1;
					else g.pastIndex = 0;
					Debug.Log("adicionei label theirturn na gamelist");
					Flow.gameList.Add(g);
				}
				
				Flow.yourTurnGames--;
				Flow.theirTurnGames++;
				
			}
			else
			{
				//tempContainer.transform.FindChild("ContainerButton").gameObject.SetActive(false);
				//tempContainer.transform.FindChild("ContainerSprite").gameObject.SetActive(true);
				
				Debug.Log ("id : " + Flow.currentGame.id);
				Debug.Log ("listCount: " + Flow.gameList.Count);
				Debug.Log ("tt count: " +Flow.theirTurnGames);
				
				if(Flow.theirTurnGames == 0)
				{
					Game g = new Game();
					g.id = -999;
					g.whoseMove = "their";
					g.lastUpdate = new DateTime(2999,12,31);
					g.friend = new Friend();
					g.friend.id = "-999";
					if(Flow.yourTurnGames > 0) g.pastIndex = Flow.yourTurnGames+1;
					else g.pastIndex = 0;
					Debug.Log("adicionei label theirturn na gamelist");
					Flow.gameList.Add(g);
				}
				
				Flow.theirTurnGames++;
				Flow.currentGame.whoseMove = "their";
				
				Flow.currentGame.pastIndex = Flow.gameList.Count;
				Flow.gameList.Add (Flow.currentGame);
			}
			
			//Flow.currentGame.ResetGame();
			
			UIPanelManager.instance.BringIn("MultiplayerScenePanel");
		}
		else if(Flow.path == TurnStatus.AnswerGame)
		{
			Flow.path = TurnStatus.BeginGame;
			UIPanelManager.instance.BringIn("GunSelectionScenePanel");
		}
		else if(Flow.path == TurnStatus.ShowPast)
		{
			Flow.path = TurnStatus.AnswerGame;
			
			UIPanelManager.instance.BringIn("GunSelectionScenePanel");
			//Flow.config.GetComponent<ConfigManager>().inviteAllScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
			//Flow.config.GetComponent<ConfigManager>().invitePlayingScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
			
			//Application.LoadLevel("Scenario1");
		}
	}
	
	void StartPastTransition()
	{
		if(clickedNext) return;
		BringFirstPastPanel(firstPastPanel.transitions.list[0]);
		//lastPastPanel.transform.position = lastPastPanel.transitions.list[0].animParams[0].axis;
		winLosePanel.transform.position = winLosePanel.transitions.list[0].animParams[0].axis;
	}
	
	void BringFirstPastPanel(EZTransition transition)
	{
		if(clickedNext) return;
		firstPastPanel.transitions.list[0].AddTransitionEndDelegate(DismissFirstPastPanel);
		firstPastPanel.BringIn();
	}
	
	void DismissFirstPastPanel(EZTransition transition)
	{
		if(clickedNext) return;
		firstPastPanel.transitions.list[2].AddTransitionEndDelegate(BringSecondPastPanel);
		firstPastPanel.Dismiss();
	}
	
	void BringSecondPastPanel(EZTransition transition)
	{
		if(clickedNext) return;
		//lastPastPanel.transitions.list[0].AddTransitionEndDelegate(DismissSecondPastPanel);
		//lastPastPanel.BringIn();
		winLosePanel.transitions.list[0].AddTransitionEndDelegate(DismissSecondPastPanel);
		winLosePanel.BringIn();
	}
	
	void DismissSecondPastPanel(EZTransition transition)
	{
		if(clickedNext) return;
		//lastPastPanel.transitions.list[2].AddTransitionEndDelegate(StartGame);
		//lastPastPanel.Dismiss();
		winLosePanel.transitions.list[2].AddTransitionEndDelegate(StartGame);
		winLosePanel.Dismiss();
	}
	
	void StartGame(EZTransition transition)
	{
		if(clickedNext) return;
		if(Flow.path == TurnStatus.AnswerGame)
		{
			Flow.path = TurnStatus.BeginGame;
			// Fix Me UPTOP Mandar para world Selection
			//Flow.nextPanel = PanelToLoad.GunSelection;
			//Application.LoadLevel("Mainmenu");
			UIPanelManager.instance.BringIn("GunSelectionScenePanel");
		}
		else if(Flow.path == TurnStatus.ShowPast)
		{
			Flow.path = TurnStatus.AnswerGame;
			//Flow.nextPanel = PanelToLoad.GunSelection;
			//Application.LoadLevel("Mainmenu");
			UIPanelManager.instance.BringIn("GunSelectionScenePanel");
		}
	}
}
