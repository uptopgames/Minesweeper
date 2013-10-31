using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using CodeTitans.JSon;
using System;
using System.IO;

public class Gameplay : MonoBehaviour 
{
	public UIInteractivePanel roundsSignsPanel, readySignPanel, steadySignPanel, bangSignPanel, boardResultsSignPanel;
	public UIInteractivePanel[] boardResultsRoundsPanel = new UIInteractivePanel[5];
	public SpriteText roundsSignsText, playerNameText, enemyNameText;
	public SpriteText[] totalPlayer1Time = new SpriteText[5];
	public SpriteText[] totalPlayer2Time = new SpriteText[5];
	public SpriteText[] shotTimePlayer1 = new SpriteText[5];
	public SpriteText[] shotTimePlayer2 = new SpriteText[5];
	public SpriteText[] gunReactionPlayer1 = new SpriteText[5];
	public SpriteText[] gunReactionPlayer2 = new SpriteText[5];
	public SpriteText[] winLosePlayer1 = new SpriteText[5];
	public SpriteText[] winLosePlayer2 = new SpriteText[5];
	public SpriteText[] scorePlayer1 = new SpriteText[5];
	public SpriteText[] scorePlayer2 = new SpriteText[5];
	
	public Texture blackTexture; //the Texture of the Fade: usually a white or black rectangle
 	
	[HideInInspector] public bool showBang = false, showReplayFinalTable = false, jumpReplay = false, showSigns = false,
		usedShootAgain = false, showingResultsTable = false, noInternet = false;//, blackScreen = false;
	private float gunReaction = 0;
	[HideInInspector] public int round = 0, myPoints = 0, enemyPoints = 0, gunId = 0;
	
	private string connectionProgress = "notSent";
	
	// Acesso as Classes do Face para fazer as actions e posts
	FacebookAPI facebook;
	GameFacebook fb_account;
		
	// variaveis que contem os personagens e seus scripts
	public Gunslinger player;
	public Gunslinger enemy;
	
	public GameObject fadeIn, fadeOut, sandAttackAnimation, blackScreen;
	
	public MeshRenderer enemyRendererBoard;
	public MeshRenderer playerRendererBoard;
	
	public MeshRenderer enemyRendererScore;
	public MeshRenderer playerRendererScore;
	
	public GameObject enemyWhiteBackBoard;
	public GameObject playerWhiteBackBoard;
	public GameObject enemyWhiteBackScore;
	public GameObject playerWhiteBackScore;
	
	public Texture2D blondiePic;
	public GameObject scoreBoard;
	
	public GameObject retrySendingButton;
	
	void HandleGetFriendPicture(string error, WWW conn)
	{
		if(this==null) return; //se mudou de cena, nao precisa mais fazer nada
		
		if (error != null || conn.error != null || conn.bytes.Length == 0)
		{
			Flow.currentGame.friend.rawText = Flow.transparent;
			return;
		}
		
		
		enemyRendererBoard.material.mainTexture = conn.texture;
		enemyRendererScore.material.mainTexture = conn.texture;
		Flow.currentGame.friend.rawText = conn.texture;
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
			playerRendererBoard.material.mainTexture = Flow.playerPhoto;
			playerRendererScore.material.mainTexture = Flow.playerPhoto;
        }
    }
	
	// Use this for initialization
	void Start () 
	{
		Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(false);
		
		if(Flow.path == TurnStatus.BeginGame) scoreBoard.SetActive(false);
		else scoreBoard.SetActive(true);
		
		if(Flow.currentGame.friend.rawText != null) 
		{
			enemyRendererBoard.material.mainTexture = Flow.currentGame.friend.rawText;
			enemyRendererScore.material.mainTexture = Flow.currentGame.friend.rawText;
		}
		else if(Save.HasKey(PlayerPrefsKeys.TOKEN))
		{
			GameRawAuthConnection conn = new GameRawAuthConnection(Flow.URL_BASE + "login/picture.php", HandleGetFriendPicture);
			WWWForm form = new WWWForm();
			form.AddField("user_id", Flow.currentGame.friend.id);
			
			conn.connect(form);
		}
		
		if(Flow.playerPhoto != null)
		{
			playerRendererBoard.material.mainTexture = Flow.playerPhoto;
			playerRendererScore.material.mainTexture = Flow.playerPhoto;
		}
		else if(Save.HasKey(PlayerPrefsKeys.TOKEN))
		{
			GameRawAuthConnection conn2 = new GameRawAuthConnection(Flow.URL_BASE + "login/picture.php", HandleGetPlayerPicture);
            WWWForm form2 = new WWWForm();
            form2.AddField("user_id", "me");
            conn2.connect(form2);
		}
		else
		{
			playerRendererBoard.material.mainTexture = blondiePic;
			playerRendererScore.material.mainTexture = blondiePic;
		}
		
		if(Flow.currentMode == GameMode.SinglePlayer) 
		{
			enemyWhiteBackBoard.SetActive(true);
			enemyWhiteBackScore.SetActive(true);
			if(!Save.HasKey(PlayerPrefsKeys.TOKEN))
			{
				playerWhiteBackScore.SetActive(true);
				playerWhiteBackBoard.SetActive(true);
			}
		}
		else 
		{
			enemyWhiteBackBoard.SetActive(false);
			enemyWhiteBackScore.SetActive(false);
		}
		
		Debug.Log ("sandAttack: " + Flow.currentGame.theirRoundList[0].sandAttack);
		
		if (Flow.currentMode == GameMode.SinglePlayer)
		{
			enemyNameText.Text = Flow.currentGame.level.name.ToString();
		}
		else
		{
			enemyNameText.Text = Flow.currentGame.friend.name;
		}
		
		if (Save.HasKey (PlayerPrefsKeys.NAME.ToString()))
		{
			playerNameText.Text = Save.GetString(PlayerPrefsKeys.NAME.ToString());
		}
		else
		{
			playerNameText.Text = "Blondie";
		}
		
		if (Flow.path != TurnStatus.ShowPast)
		{
			gunId = Flow.currentGame.myRoundList[0].gun.id;
			gunReaction = Flow.currentGame.myRoundList[0].gun.reaction;
		}
		
		Debug.Log ("gameMode: " + Flow.currentMode);
		
		Debug.Log ("gamePath: " + Flow.path.ToString());
		//Flow.singleWorld = 2;
		
		if (Flow.currentMode == GameMode.SinglePlayer)
		{
			Debug.Log ("world: " + Flow.currentGame.world.id);
			//Flow.singleLevel = 9;
			Debug.Log ("level: " + Flow.currentGame.level.id);
		}
		//Flow.Game.currentGame.myGun = 1;
		//Debug.Log ("myGun: " + Flow.currentGame.myRoundList[0].gun.id);
		//Flow.Game.currentGame.theirGun = 5;
		//Debug.Log ("theirGun: " + Flow.currentGame.theirRoundList[0].gun.id);
		
		//playerNameText.Text = (Flow.userSettings.playerName == "") ? Flow.charName : Flow.userSettings.playerName;
		//enemyNameText.Text = (Flow.Game.path == "Single") ? Flow.enemyNames[Flow.singleLevel-1] : Flow.Game.currentGame.friendName;
		
		// sao os callbacks dos paineis no momento em que eles terminam a transicao de BringInForward
		// se fosse por exemplo o numero 2 da list que eh o de dismiss forward seria o delegate do momento que termina a transicao de DismissForward
		roundsSignsPanel.transitions.list[0].AddTransitionEndDelegate(RoundsSignsEnd);
		readySignPanel.transitions.list[0].AddTransitionEndDelegate(ReadySignEnd);
		steadySignPanel.transitions.list[0].AddTransitionEndDelegate(SteadySignEnd);
		sandAttackAnimation.GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionEndDelegate(SandAttackShowTitle);
		sandAttackAnimation.transform.FindChild("TextPanel").GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionEndDelegate(SandAttackShowEnemy);
		sandAttackAnimation.transform.FindChild("SandPanel").GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionEndDelegate(SandAttackShowPlayer);
		blackScreen.GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionEndDelegate(ShowBlackScreenText);
		sandAttackAnimation.GetComponent<UIInteractivePanel>().transitions.list[2].AddTransitionEndDelegate(DeativateAndResetSandAttack);
		//bangSignPanel.transitions.list[0].AddTransitionEndDelegate (BangSignEnd);
		bangSignPanel.transitions.list[0].AddTransitionStartDelegate(BangSignStart);
		bangSignPanel.transitions.list[0].AddTransitionEndDelegate(BangSignEnd);
		//boardResultsSignPanel.transitions.list[0].AddTransitionEndDelegate (BoardResultsUpEnd);
		boardResultsSignPanel.transitions.list[3].AddTransitionEndDelegate (BoardResultsDownEnd);
		
		// texto que vai aparecer no start. Nos outros rounds vai mudar o numero
		round = 1;
		roundsSignsText.Text = "Round " + round; 		
		
		
		// FAZER VERIFICACAO DO SAND ATTACK
		
		if (Flow.currentGame.theirRoundList[0].sandAttack == 1 && Flow.path != TurnStatus.ShowPast)
		{
			Debug.Log ("startSandAttack");
			Invoke ("SandAttackAnimation", 1f);
			//SandAttackAnimation();
		}
		else
		{
			roundsSignsPanel.StartTransition (UIPanelManager.SHOW_MODE.BringInForward);
		}
		//panelManager.BringIn (roundsSignsPanel);
		
		// GERA A LISTA DE TIROS DO INIMIGO DO SINGLE, E AS LISTAS DE TIRO DO REPLAY //
		// GERA OS BANGTIMES DO SINGLE, CREATE GAME E NEW TURN //
		Debug.Log ("ShowPast");
		
		for (int i = 0; i < 5; i++)
		{
			if (Flow.path == TurnStatus.ShowPast)
			{
				
				if (Flow.currentGame.pastMyRoundList[i].playerRoundWin == 1) myPoints++;
				if (Flow.currentGame.pastTheirRoundList[i].playerRoundWin == 1) enemyPoints++;
			}
		}
	}
	
	void SandAttackAnimation()
	{
		Debug.Log("comecei a animacao de sandattack");
		sandAttackAnimation.gameObject.SetActive(true);
		sandAttackAnimation.GetComponent<UIInteractivePanel>().StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		
		sandAttackAnimation.transform.FindChild("SandAttackAnimation").GetComponent<PackedSprite>().PlayAnim(0);
		//sandAttackAnimation.transform.GetChild(0).transform.GetComponent<PackedSprite>().SetAnimCompleteDelegate(SandAttackAnimCompleted);
	}
	
	void SandAttackShowTitle(EZTransition transition)
	{
		Debug.Log("vaai texto!");
		sandAttackAnimation.transform.FindChild("TextPanel").gameObject.SetActive(true);
		sandAttackAnimation.transform.FindChild("TextPanel").GetComponent<UIInteractivePanel>().StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
	}
	
	void SandAttackShowEnemy(EZTransition transition)
	{
		sandAttackAnimation.transform.FindChild("Enemy").gameObject.SetActive(true);
		sandAttackAnimation.transform.FindChild("TextPanel").gameObject.SetActive(false);
		sandAttackAnimation.transform.FindChild("Enemy").GetComponent<PackedSprite>().SetAnimCompleteDelegate(SandAttackGrowSand);
		sandAttackAnimation.transform.FindChild("Enemy").GetComponent<PackedSprite>().PlayAnim(0);
	}
	
	void SandAttackGrowSand(SpriteBase sprite)
	{
		sandAttackAnimation.transform.FindChild("SandPanel").GetComponent<UIInteractivePanel>().StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
	}
	
	void SandAttackShowPlayer(EZTransition transition)
	{
		sandAttackAnimation.transform.FindChild("Enemy").gameObject.SetActive(false);
		sandAttackAnimation.transform.FindChild("Player").gameObject.SetActive(true);
		Invoke("DismissSandAttack",2f);
	}
	
	void DismissSandAttack()
	{
		sandAttackAnimation.GetComponent<UIInteractivePanel>().StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
		blackScreen.GetComponent<UIInteractivePanel>().StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		//blackScreen = true;
	}
	
	void ShowBlackScreenText(EZTransition transition)
	{
		blackScreen.transform.FindChild("Text").gameObject.SetActive(true);
	}
	
	void DeativateAndResetSandAttack(EZTransition transition)
	{
		Debug.Log ("SandAttackAnimCompleted");
		sandAttackAnimation.gameObject.SetActive(false);
		sandAttackAnimation.transform.FindChild("Player").gameObject.SetActive(false);
		sandAttackAnimation.transform.FindChild("SandPanel").transform.localScale = new Vector3(0,0,1);
		sandAttackAnimation.transform.FindChild("TextPanel").transform.localScale = new Vector3(0,0,1);
		
		roundsSignsPanel.StartTransition (UIPanelManager.SHOW_MODE.BringInForward);
	}
	
	/*void OnGUI()
	{
		if (blackScreen)
		{
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), blackTexture);
			GUI.Label (new Rect (Screen.width /4, Screen.height / 4, Screen.width / 4 * 3, Screen.height / 10), "Turn Your Volume On");
		}
	}*/
	
	void RoundsSignsEnd (EZTransition transition)
	{
		//Debug.Log ("RoundsSignsEnd");
		readySignPanel.StartTransition (UIPanelManager.SHOW_MODE.BringInForward);
		//panelManager.BringIn (readySignPanel);
	}
	
	
	void ReadySignEnd (EZTransition transition)
	{
		//Debug.Log ("ReadySignEnd");
		
		//panelManager.Dismiss();
		readySignPanel.StartTransition (UIPanelManager.SHOW_MODE.DismissForward);
		//readySignPanel.Dismiss();
		steadySignPanel.StartTransition (UIPanelManager.SHOW_MODE.BringInForward);
		//panelManager.BringIn (steadySignPanel);
		
	}
	
	void SteadySignEnd (EZTransition transition)
	{
		//Debug.Log ("SteadySignEnd");
		steadySignPanel.StartTransition (UIPanelManager.SHOW_MODE.DismissForward);
		//showBang = true;
		if (Flow.path == TurnStatus.ShowPast)
		{
			bangSignPanel.transitions.list[0].animParams[0].delay = Flow.currentGame.pastTheirRoundList[round-1].bangTime;
			//bangSignPanel.transitions.list[0].animParams[1].delay = Flow.currentGame.pastTheirRoundList[round-1].bangTime;	
		}
		else
		{
			bangSignPanel.transitions.list[0].animParams[0].delay = Flow.currentGame.theirRoundList[round-1].bangTime;
			//bangSignPanel.transitions.list[0].animParams[1].delay = Flow.currentGame.theirRoundList[round-1].bangTime;
		}
		
		//Debug.Log ("bangSignPanelDelay: " + bangSignPanel.transitions.list[0].animParams[0].delay);
		//Debug.Log ("bangTime: " + Flow.currentGame.theirRoundList[round-1].bangTime);
		bangSignPanel.StartTransition (UIPanelManager.SHOW_MODE.BringInForward);
		
		// Up Top Fix Me TESTE // esta controlando o tiro no pe na classe GunsLinger
		showSigns = true;
		//showBang = true;
		
	}
	
	void BangSignStart (EZTransition transition)
	{
		//showBang = true;
		//Debug.Log ("BangSignStart");
	}
	void BangSignEnd (EZTransition transition)
	{
		showBang = true;
		//Debug.Log ("BangSignEnd");
	}

	void ShowRound (int rounds)
	{
		roundsSignsText.Text = "Round " + round;
		
	}
	
	void JumpReplay()
	{
		jumpReplay = true;
		player.Reset();
		enemy.Reset();
		BoardResultsDown();	
	}
	
	public void ShowResultsTable()
	{
		if (Flow.path == TurnStatus.ShowPast)
		{
			Debug.Log ("ShowPastStart");
			Debug.Log ("SkipReplayButton: " + boardResultsSignPanel.transform.parent.FindChild("SkipReplayButton").name);
			
			boardResultsSignPanel.transform.parent.FindChild("SkipReplayButton").gameObject.SetActive(true);
		}
		else if (Flow.path == TurnStatus.BeginGame)
		{
			if(Flow.currentGame.myRoundList[round-1].sandAttack != 1) boardResultsSignPanel.transform.FindChild("SandAttackButton").gameObject.SetActive(true);
		}	
		
		if (jumpReplay || round == 5)
		{
			boardResultsSignPanel.transform.parent.FindChild("SkipReplayButton").gameObject.SetActive(false);
		}
		
		if (Flow.path != TurnStatus.ShowPast && (Flow.path == TurnStatus.BeginGame || Flow.currentGame.theirRoundList[round-1].playerRoundWin == 1) )
		{
			boardResultsSignPanel.transform.FindChild("ShootAgainButton").gameObject.SetActive(true);
		}
		else
		{
			boardResultsSignPanel.transform.FindChild("ShootAgainButton").gameObject.SetActive(false);
		}
		
		bangSignPanel.StartTransition (UIPanelManager.SHOW_MODE.DismissForward);
		//Debug.Log ("ShowResultsTable");
		
		if (jumpReplay) 
		{
			round = 5;
			//Debug.Log("past my round list count: "+Flow.currentGame.pastMyRoundList.Count);
			//Debug.Log("past their round list count: "+Flow.currentGame.pastTheirRoundList.Count);
			//Debug.Log("score p1 count: "+scorePlayer1.Length);
			//Debug.Log("score p2 count: "+scorePlayer2.Length);
			for (int i = 0; i < round; i++)
			{
				//Debug.Log("prw "+Flow.currentGame.pastMyRoundList[i].playerRoundWin);
				//Debug.Log("erw "+Flow.currentGame.pastTheirRoundList[i].playerRoundWin);
				if (Flow.currentGame.pastMyRoundList[i].playerRoundWin == 1)
				{
					//Debug.Log("senhor rafl V");
					scorePlayer1[i].Text = "V";
				}
				else 
				{
					//Debug.Log("senhor rafl X");
					scorePlayer1[i].Text = "X";
				}
				
				if (Flow.currentGame.pastTheirRoundList[i].playerRoundWin == 1)
				{
					//Debug.Log("senhor rafaiel V");
					scorePlayer2[i].Text = "V";
				}
				else
				{
					//Debug.Log("senhor rafaiel X");
					scorePlayer2[i].Text = "X";
				}
			}
		}
		else
		{
			//if(Flow.path == TurnStatus.BeginGame) scoreBoard.SetActive(false);
		
			// Preenche a tabela de score embaixo
		
			//Debug.Log ("path: "+Flow.path+" prw: "+Flow.currentGame.myRoundList[round-1].playerRoundWin+" jr: "+jumpReplay);
		
			if ( (Flow.path == TurnStatus.AnswerGame && Flow.currentGame.myRoundList[round-1].playerRoundWin == 1) || 
				(Flow.path == TurnStatus.ShowPast && Flow.currentGame.pastMyRoundList[round-1].playerRoundWin == 1))
			{
				scorePlayer1[round-1].Text = "V";
				//Debug.Log("Player V!");
			}
			else 
			{
				scorePlayer1[round-1].Text = "X";
				//Debug.Log("Player X!");
			}
			
			//Debug.Log ("path: "+Flow.path+" erw: "+Flow.currentGame.theirRoundList[round-1].playerRoundWin+" jr: "+jumpReplay);
			
			if ( (Flow.path == TurnStatus.AnswerGame && Flow.currentGame.theirRoundList[round-1].playerRoundWin == 1) || 
				(Flow.path == TurnStatus.ShowPast && Flow.currentGame.pastTheirRoundList[round-1].playerRoundWin == 1))
			{
				scorePlayer2[round-1].Text = "V";
				Debug.Log("Enemy V!");
			}
			else 
			{
				scorePlayer2[round-1].Text = "X";
				Debug.Log("Enemy X!");
			}
		}
		
		// Preenche a Tabela de Resultados que Sobe
		for (int i = 0; i < round; i ++)
		{
			if (i == round-1 && !jumpReplay)
			{
				shotTimePlayer1[i].color.a = 0f;
				shotTimePlayer2[i].color.a = 0f;
				gunReactionPlayer1[i].color.a = 0f;
				gunReactionPlayer2[i].color.a = 0f;
				totalPlayer1Time[i].color.a = 0f;
				totalPlayer2Time[i].color.a = 0f;
				winLosePlayer1[i].color.a = 0f;
				winLosePlayer2[i].color.a = 0f;
		
				boardResultsRoundsPanel[i].StartTransition (UIPanelManager.SHOW_MODE.BringInForward);
				//Debug.Log ("roundPanelForward" + round);
				
			}
			else
			{
				boardResultsRoundsPanel[i].transitions.list[0].animParams[0].duration = 0;
			}
			
			// DESENHA NA TABELA OS RESULTADOS
			
			// PASSADO
			if (Flow.path == TurnStatus.ShowPast)
			{
				shotTimePlayer1[i].Text = Flow.currentGame.pastMyRoundList[i].time > 0 ? Flow.currentGame.pastMyRoundList[i].time.ToString("F3") : "-";
				shotTimePlayer2[i].Text = Flow.currentGame.pastTheirRoundList[i].time > 0 ? Flow.currentGame.pastTheirRoundList[i].time.ToString("F3") : "-";
				gunReactionPlayer1[i].Text = Flow.currentGame.pastMyRoundList[i].gun.reaction.ToString("F3");
				gunReactionPlayer2[i].Text = Flow.currentGame.pastTheirRoundList[i].gun.reaction.ToString("F3");
				totalPlayer1Time[i].Text = Flow.currentGame.pastMyRoundList[i].time > 0 ? (Flow.currentGame.pastMyRoundList[i].time + 
					Flow.currentGame.pastMyRoundList[i].gun.reaction).ToString("F3") : "-";
				totalPlayer2Time[i].Text = Flow.currentGame.pastTheirRoundList[i].time > 0 ? (Flow.currentGame.pastTheirRoundList[i].time + 
					 Flow.currentGame.pastTheirRoundList[i].gun.reaction).ToString("F3") : "-";
			}
			
			// PRESENTE (RESPONDENDO OU CRIANDO)
			else 
			{
			
				shotTimePlayer1[i].Text = Flow.currentGame.myRoundList[i].time > 0 ? Flow.currentGame.myRoundList[i].time.ToString("F3") : "-";
			
				shotTimePlayer2[i].Text = (Flow.path == TurnStatus.BeginGame) ? "-" :
					Flow.currentGame.theirRoundList[i].time > 0 ? Flow.currentGame.theirRoundList[i].time.ToString("F3") : "-";
			
				gunReactionPlayer1[i].Text = Flow.currentGame.myRoundList[0].gun.reaction.ToString("F3");
			
				gunReactionPlayer2[i].Text = (Flow.path == TurnStatus.BeginGame) ? "-" : Flow.currentGame.theirRoundList[0].gun.reaction.ToString("F3");
			
				totalPlayer1Time[i].Text = Flow.currentGame.myRoundList[i].time > 0 ? 
					(Flow.currentGame.myRoundList[i].time + Flow.currentGame.myRoundList[0].gun.reaction).ToString("F3") : "-";
			
				totalPlayer2Time[i].Text = (Flow.path == TurnStatus.BeginGame) ? "Waiting" :
					(Flow.currentGame.theirRoundList[i].time > 0 ? (Flow.currentGame.theirRoundList[i].time + Flow.currentGame.theirRoundList[0].gun.reaction).ToString("F3") : " -");
			}
				
			if (Flow.path != TurnStatus.BeginGame)
			{
				//Debug.Log ("DrawingWinLoseBoard");
				
				if ( (Flow.path != TurnStatus.ShowPast && Flow.currentGame.myRoundList[i].playerRoundWin == 1 ||
					Flow.path == TurnStatus.ShowPast && Flow.currentGame.pastMyRoundList[i].playerRoundWin == 1))
				{
					winLosePlayer1[i].Text = "V";
				}
				else winLosePlayer1[i].Text = "X";
				
				if ( (Flow.path != TurnStatus.ShowPast && Flow.currentGame.theirRoundList[i].playerRoundWin == 1 ||
					Flow.path == TurnStatus.ShowPast && Flow.currentGame.pastTheirRoundList[i].playerRoundWin == 1))
				{
					winLosePlayer2[i].Text = "V";
				}
				else winLosePlayer2[i].Text = "X";
			}
		}
		
		boardResultsSignPanel.StartTransition (UIPanelManager.SHOW_MODE.BringInForward);

	}
	
	void ShootAgain()
	{
		// UP TOP FIX ME --> VERIFICAR COINS ANTES DE FAZER COMPRA
		
		Flow.shopManager.BuyItem(BuyShootAgain, Flow.shopManager.GetShopItem("shootAgain"));
		//usedShootAgain = true;
		
	}
	
		// metodos que precisam ter para fazer as compras do single
	void BuyShootAgain(ShopResultStatus status, string product)
	{
		if(status == ShopResultStatus.Success)
		{
			usedShootAgain = true;
			BoardResultsDown();
		}
		else
		{
			Flow.game_native.showMessage ("Error", "There was an error, please Try Again", "Ok");
		}
		// fazer nova verificacao de sucesso na compra
		// em caso de sucesso
		
		//resultsTableGoingDown = true;
	}
	
	void SandAttack()
	{
		// UP TOP FIX ME --> VERIFICAR COINS ANTES DE FAZER COMPRA
		Flow.shopManager.BuyItem (BuySandAttack, Flow.shopManager.GetShopItem("sandAttack"));
	}
	
	void BuySandAttack (ShopResultStatus status, string product)
	{
		
		if (status == ShopResultStatus.Success)
		{
			Flow.game_native.showMessage("Sand Attack", "Your Friend's screen will be dark during all this round");
			boardResultsSignPanel.transform.FindChild("SandAttackButton").gameObject.SetActive(false);
			Flow.currentGame.myRoundList[round-1].sandAttack = 1;
		}
	}
	
	void BoardResultsDown()
	{
		if(Flow.path == TurnStatus.ShowPast && !jumpReplay)
		{
			Debug.Log("desativa o skip..");
			boardResultsSignPanel.transform.parent.FindChild("SkipReplayButton").gameObject.SetActive(false);
		}
		
		GameObject localFader = GameObject.Instantiate(fadeIn) as GameObject;
		//localFader.GetComponent<Fader>().delay = boardResultsSignPanel.transitions.list[3].animParams[0].delay;
		//localFader.GetComponent<Fader>().fadeTime = boardResultsSignPanel.transitions.list[3].animParams[0].duration;
		
		//Debug.Log ("BoardResultsDown");
		boardResultsSignPanel.StartTransition (UIPanelManager.SHOW_MODE.DismissBack);
	}
	
	void BoardResultsDownEnd(EZTransition transition)
	{
		//Debug.Log ("BoardResultsDownEnd");
		if (jumpReplay) 
		{
			ShowResultsTable();
			round = 5;
			jumpReplay = false;
		}
		else HideResults();
	}
	
	void RestartRound()
	{	
		Debug.Log ("RestartRound");
		round++;
		
		if(usedShootAgain)
		{
			Debug.Log ("round: " + round);
			
			Flow.currentGame.theirRoundList[round-2].playerRoundWin = 0;
			Flow.currentGame.myRoundList[round-2].time = 0;
			//Flow.currentGame.myRoundList.Add (new Round (round-2, -1, -1, new Gun(), -1f, -1, -1, -1));
			
			//Flow.currentGame.myRoundList[0].gun.id = gunId;
			//Flow.currentGame.myRoundList[0].gun.reaction = gunReaction;
			enemyPoints--;
			round--;
			usedShootAgain = false;
		}
		
		if (Flow.currentGame.theirRoundList[round-1].sandAttack == 1 && Flow.path != TurnStatus.ShowPast)
		{
			//Invoke ("SandAttackAnimation", 1);
			SandAttackAnimation();
		}
		else
		{
			roundsSignsText.Text = "Round " + round;
			roundsSignsPanel.StartTransition (UIPanelManager.SHOW_MODE.BringInForward);
		}
	}
	
	private void postActionFacebook()
	{
		Debug.Log ("postActionFacebook");
		
		GameJsonAuthConnection postFb = new GameJsonAuthConnection (Flow.URL_BASE + "bangbang/publish_single.php", actionResponse);
		WWWForm form = new WWWForm();
		form.AddField ("world", Flow.currentGame.world.id);
		form.AddField ("level", Flow.currentGame.level.id);
		form.AddField ("gun", Flow.currentGame.myRoundList[0].gun.id);
		form.AddField ("rounds", myPoints + enemyPoints);
		
		postFb.connect (form);
	}
			
	private void actionResponse(string error, IJSonObject data)
	{
		if (error != null) Debug.Log ("error: " + error);
		else Debug.Log ("data: " + data);
	}
	
	private void HideResults()
	{
		// TESTE UP TOP FIX ME
		
		/*if (Flow.path == TurnStatus.AnswerGame)
		{
			Flow.currentGame.myRoundList[1].playerRoundWin = 0;
			Flow.currentGame.myRoundList[2].playerRoundWin = 0;
			Flow.currentGame.myRoundList[3].playerRoundWin = 0;
			Flow.currentGame.myRoundList[4].playerRoundWin = 1;
			
		}
		
		round = 5;
		Flow.currentGame.myRoundList[1].time = -2;
		Flow.currentGame.myRoundList[2].time = 2.341f;
		Flow.currentGame.myRoundList[3].time = -2;
		Flow.currentGame.myRoundList[4].time = 0.541f;
		
		Flow.currentGame.myRoundList[1].sandAttack = 1;
		Flow.currentGame.myRoundList[2].sandAttack = 1;
		Flow.currentGame.myRoundList[3].sandAttack = 0;
		Flow.currentGame.myRoundList[4].sandAttack = 0;*/
		
		//Debug.Log ("myPoints: " + myPoints);
		//Debug.Log ("enemyPoints: " + enemyPoints);
		
		// verifica se o jogo acabaou no single	
		if ( (myPoints == 3 || enemyPoints == 3) && Flow.currentMode == GameMode.SinglePlayer)
		{
			//Debug.Log ("jogoAcabou");
		
			// PERDI
			if (myPoints < enemyPoints) 
			{
				Flow.playerWin = false; // se perdeu
				Flow.worldDict[Flow.currentGame.world.id-1].levelDict[Flow.currentGame.level.id-1].stars = 0;
				//Debug.Log ("Perdi");
			}
			// GANHEI
			else
			{
				//Debug.Log ("Ganhei");
				Flow.playerWin = true;
				
				float totalReaction =0;
				for (int i = 0; i < myPoints + enemyPoints; i++)
				{
					totalReaction += Flow.currentGame.myRoundList[i].time;
				}
				
				// FACE ACTION POST
				if (Save.HasKey (PlayerPrefsKeys.FACEBOOK_TOKEN.ToString())) postActionFacebook();
					
				
				// SALVA O TEMPO PARA PASSAR PARA O GAME CENTER
#if UNITY_IPHONE
				
				if (!Save.HasKey ("world" + Flow.currentGame.world.id + "level" + Flow.currentGame.level.id + "Time") ||
					totalReaction < Save.GetFloat ("world" + Flow.currentGame.world.id + "level" + Flow.currentGame.level.id + "Time"))
				{
					Save.Set ("world" + Flow.currentGame.world.id + "level" + Flow.currentGame.level.id + "Time", totalReaction);
					
					if (Save.HasKey ("world" + Flow.currentGame.world.id + "_level6_stars") || Flow.currentGame.level.id == 6)
					{
						//Debug.Log ("enviando info para o gamecenter");
						SendToGameCenter();
					}
					
				}
					//Debug.Log ("world" + Flow.currentGame.world.id + "level" + Flow.currentGame.level.id + "Time" + ": "
					//+ Save.GetFloat ("world" + Flow.currentGame.world.id + "level" + Flow.currentGame.level.id + "Time"));	
#endif
			}
			Debug.Log("pontos do inimigo: "+enemyPoints);
			Flow.worldDict[Flow.currentGame.world.id-1].levelDict[Flow.currentGame.level.id-1].stars = 3-enemyPoints; // se foi 5 x 0 ganha 3 estrelas
			// 4 x 1 ganha 2 estrelas
			// 3 x 2 ganha 1 estrela
			
			// se as estrelas ganha forem maiores que as que o usuario tinha anteriormente, seta as novas
			if (Flow.worldDict[Flow.currentGame.world.id-1].levelDict[Flow.currentGame.level.id-1].stars > 
				Save.GetInt ("world" + Flow.currentGame.world.id + "_level" + Flow.currentGame.level.id + "_stars"))
			{
				Save.Set ("world" + Flow.currentGame.world.id + "_level" + Flow.currentGame.level.id + "_stars", 
					Flow.worldDict[Flow.currentGame.world.id-1].levelDict[Flow.currentGame.level.id-1].stars,true);
				Debug.Log ("salveiWorldId: " + Flow.currentGame.world.id + "eLevel: " + Flow.currentGame.level.id);
				Debug.Log ("novasEstrelasSet: " + Save.GetInt ("world" + Flow.currentGame.world.id + "_level" + Flow.currentGame.level.id + "_stars"));
			}
			
			// diminui em 5 a qtde de balas caso nao seja a arma 1
			if (Flow.currentGame.myRoundList[0].gun.id != 1)
			{
				//Save.Set ("gun" + Flow.singleMyGun + "_bullets", Save.GetInt ("gun" + Flow.singleMyGun + "_bullets") - 5);
				Save.Set (PlayerPrefsKeys.ITEM+"gun" + Flow.currentGame.myRoundList[0].gun.id + "_bullets", Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + Flow.currentGame.myRoundList[0].gun.id + "_bullets") - 5,true);
				Debug.Log ("bulletsNovas: " + Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + Flow.currentGame.myRoundList[0].gun.id + "_bullets"));
			}
		
			if (Flow.currentMode == GameMode.SinglePlayer) Flow.nextPanel = PanelToLoad.EndLevel;
			
			Application.LoadLevel("Mainmenu");
		}
		else
		{
			if (round < 5 || usedShootAgain)
			{
				Invoke("RestartRound", 1);
			
				player.Reset();
				enemy.Reset();
			}
			else 
			{
				if (Flow.path != TurnStatus.ShowPast) 
				{
					// diminui em 5 a qtde de balas caso nao seja a arma 1
					if (Flow.currentGame.myRoundList[0].gun.id != 1)
					{
						Save.Set (PlayerPrefsKeys.ITEM+"gun" + Flow.currentGame.myRoundList[0].gun.id + "_bullets", Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + Flow.currentGame.myRoundList[0].gun.id + "_bullets") - 5,true);
						Debug.Log ("bulletsNovas: " + Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + Flow.currentGame.myRoundList[0].gun.id + "_bullets"));
					}
					
					SendResults();
				}
				else 
				{
					Flow.nextPanel = PanelToLoad.BattleStatus;
					Application.LoadLevel("Mainmenu");
				}
			}
		}
	}
	
	// metodo que manda as infos para o servidor
	public void SendResults()
	{
		if(Flow.path == TurnStatus.AnswerGame && !Save.HasKey(Flow.currentGame.id+PlayerPrefsKeys.COINS+Flow.currentGame.turnID))
		{
			if(myPoints > enemyPoints)
			{
				// vitoria
				Flow.header.coins += ServerSettings.GetInt("GAME_WIN");
			}
			else if(myPoints < enemyPoints)
			{
				// derrota
				Flow.header.coins += ServerSettings.GetInt("GAME_LOSE");
			}
			else
			{
				// empate
				Flow.header.coins += ServerSettings.GetInt("GAME_DRAW");
			}
			
			Save.Set(Flow.currentGame.id+PlayerPrefsKeys.COINS+Flow.currentGame.turnID, true);
		}
		
		WWWForm form = new WWWForm();
		
		form.AddField("friendID", Flow.currentGame.friend.id);
		form.AddField("gunID",Flow.currentGame.myRoundList[0].gun.id);
		form.AddField("coins", Flow.header.coins);
		
		for (int i = 0; i < 5; i++)
		{
			// mandar infos para o server, e no callback chamar a proxima cena
			
			Debug.Log ("shotTime: " + Flow.currentGame.myRoundList[i].time);
			//Debug.Log ("tunrId: " + Flow.currentGame.myRoundList[i].turnId);
			Debug.Log ("shotId: " + Flow.currentGame.myRoundList[i].roundID);
			Debug.Log ("bangTime: " + Flow.currentGame.myRoundList[i].bangTime);
			Debug.Log ("sandAttack: " + Flow.currentGame.myRoundList[i].sandAttack.ToString());
				
			//form.AddField ("shot["+i+"][shotTime]", Flow.Game.currentGame.m
			
			form.AddField("shot["+i+"][shotTime]", Flow.currentGame.myRoundList[i].time.ToString());
			form.AddField("shot["+i+"][bangTime]", Flow.currentGame.theirRoundList[i].bangTime.ToString()); // o bang time esta salvo na lista do cara...
			form.AddField("shot["+i+"][sandAttack]", Flow.currentGame.myRoundList[i].sandAttack.ToString());
		}
		
		//game_native.startLoading();
		Flow.game_native.startLoading();
		new GameJsonAuthConnection (Flow.URL_BASE + "bangbang/managegame.php", gameSet).connect(form);
		UIManager.instance.blockInput = true;
	}
	
	void RetrySending()
	{
		WWWForm form = new WWWForm();
		
		form.AddField("friendID", Flow.currentGame.friend.id);
		form.AddField("gunID",Flow.currentGame.myRoundList[0].gun.id);
		form.AddField("coins", Flow.header.coins);
		
		for (int i = 0; i < 5; i++)
		{
			// mandar infos para o server, e no callback chamar a proxima cena
			
			Debug.Log ("shotTime: " + Flow.currentGame.myRoundList[i].time);
			//Debug.Log ("tunrId: " + Flow.currentGame.myRoundList[i].turnId);
			Debug.Log ("shotId: " + Flow.currentGame.myRoundList[i].roundID);
			Debug.Log ("bangTime: " + Flow.currentGame.myRoundList[i].bangTime);
			Debug.Log ("sandAttack: " + Flow.currentGame.myRoundList[i].sandAttack.ToString());
				
			//form.AddField ("shot["+i+"][shotTime]", Flow.Game.currentGame.m
			
			form.AddField("shot["+i+"][shotTime]", Flow.currentGame.myRoundList[i].time.ToString());
			form.AddField("shot["+i+"][bangTime]", Flow.currentGame.theirRoundList[i].bangTime.ToString()); // o bang time esta salvo na lista do cara...
			form.AddField("shot["+i+"][sandAttack]", Flow.currentGame.myRoundList[i].sandAttack.ToString());
		}
		
		//game_native.startLoading();
		Flow.game_native.startLoading();
		new GameJsonAuthConnection (Flow.URL_BASE + "bangbang/managegame.php", gameSet).connect(form);
		
		retrySendingButton.SetActive(false);
	}
	
	void gameSet(string error, IJSonObject data)
	{
		//game_native.stopLoading();
		Flow.game_native.stopLoading();
		UIManager.instance.blockInput = false;
		if(error != null) 
		{
			Debug.Log(error);
			Flow.game_native.showMessage("Error", error);
			retrySendingButton.SetActive(true);
			connectionProgress = "error";
		}
		else 
		{
			Debug.Log(data);
			
			// tratar a cena que vai abrir dependendo do path
			if (Flow.path == TurnStatus.AnswerGame)
			{				
				// Up Top Fix Me
				if (myPoints > enemyPoints) 
				{
					Flow.playerWin = true;
					Flow.enemyWin = false;
					Debug.Log ("beforeTurnsWon: " + Flow.currentGame.turnsWon);
					Flow.currentGame.turnsWon = Flow.currentGame.turnsWon + 1;
					Debug.Log ("AfterTurnsWon: " + Flow.currentGame.turnsWon);
				}
				else if (enemyPoints > myPoints) 
				{
					Flow.playerWin = false;
					Flow.enemyWin = true;
					Debug.Log ("beforeTurnsLost: " + Flow.currentGame.turnsLost);
					Flow.currentGame.turnsLost = Flow.currentGame.turnsLost + 1;
					Debug.Log ("afterTurnsLost: " + Flow.currentGame.turnsLost);
				}
				else
				{
					Flow.playerWin = false;
					Flow.enemyWin = false;
				}
				
				Debug.Log ("myPoints: " + myPoints);
				Debug.Log ("enemyPoints: " + enemyPoints);
				
				//Scene.Load ("BattleStatus");
				Flow.nextPanel = PanelToLoad.BattleStatus;
				Application.LoadLevel("Mainmenu");
			}
			else if (Flow.path == TurnStatus.BeginGame)
			{
				// Se o adversario nao tem o app e eh amigo do facebook, chama o post no mural dele
				if(!Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()).IsEmpty())
				{
					if(!Flow.currentGame.friend.is_playing && !Flow.currentGame.friend.facebook_id.IsEmpty())
					{
						facebook.WallPost(Flow.currentGame.friend.facebook_id, 
							new Dictionary <string, string>()
			        		{
					            // Nome do link a ser exibido
					            {"name",Save.GetString (PlayerPrefsKeys.NAME.ToString())+" challenged you on Bang Bang Free Game!" },
					
					            // Descricao do post
					            {"description", "Play "+Info.name + "against your friends and discover who is the fastest shooter. " + Info.name + "is a mobile game available on iOS, Android and Facebook!"},
					
					            // Redireciona para o app ao clicar no link
								{"link", "https://apps.facebook.com/" + Info.facebookCanvas },
							
								// Vincula o icone do app no post
								{"picture", "https://uptopgames.com/static/icon/"+Info.appId+".png" },
					
					            // Abrir share no modo de dialogo
								{"dialog", "true"}
							},null);
					}
				}
				
				Flow.nextPanel = PanelToLoad.BattleStatus;
				Application.LoadLevel("Mainmenu");
				
				//Flow.currentGame.whoseMove = "their";
				//Flow.currentGame.lastUpdate = DateTime.UtcNow.Subtract(new TimeSpan(TimeSpan.TicksPerDay));
				//Flow.gameList.Add(Flow.currentGame);
				//Scene.Load ("Multiplayer");
				
				/*if (Flow.currentGame.id != -1) 
				{
					Flow.gameList[Flow.currentGame.pastIndex].whoseMove = "their";
					
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
			
				Flow.nextPanel = PanelToLoad.Multiplayer;
				Application.LoadLevel("Mainmenu");
				Game.EndGame();	
				
			}
				
			
			connectionProgress = "sent";
			
			// passar aqui quem ganhou (playerWin x enemyWin
			
			/*for(int i = 0 ; i < data.Count ; i++)
			{
				//Flow.Game.currentGame.myGuessList[i].score = data[i].Int32Value;
				//Flow.Game.currentGame.myTotalScore += data[i].Int32Value;
			}*/
			/*if(thereIsAnotherPlayer) 
			{
				for(int i = 0 ; i < Flow.Game.currentGame.theirGuessList.Count ; i++) 
					Flow.Game.currentGame.theirTotalScore += Flow.Game.currentGame.theirGuessList[i].score;
			}*/
			}	
		}
			//timer = new KTimer(1f,callNextScene);
			 
	}	
	
	void OnEnable()
	{
		//game_native.addActionShowMessage(messagesCheck);
	}
	
	void OnDisable()
	{
		//game_native.removeActionShowMessage(messagesCheck);
	}
	
	void messagesCheck(string button)
	{
		if(noInternet)
		{
			//GameGUI.enableGui();
		}
		noInternet = false;
	}
	
	/*void BuyShootAgain (string id, Items.Status status)
	{
		Debug.Log ("status: " + status);
		Debug.Log ("id: " + id);
		
		if (status == Items.Status.ItemSucessfullyPurchased)
		{
			GameGUI.enableGui();
			//usedSootAgain = true;
			ResultsTableDown();
			
			if (Flow.Game.path != "Single") 
			{
				Header.UpdateCoins();
				// fazer goCoins
			}
		}
		
		else if (status == Items.Status.ItemCancelledPurchase)
		{
			GameGUI.enableGui();
		}
		
		else if (status == Items.Status.ConnectionError)
		{
			noInternet = true;
			game_native.showMessage("No Internet","You seem to be experiencing trouble with your connection..");
		}
	}*/
	
	
	/*void OnFailItem()
	{
		//Debug.Log("cancelou e o bila chamou");
#if UNITY_EDITOR || UNITY_WEBPLAYER
		checkOK("Cancel");
#else
		cancelledPurchase();	
#endif
	}*/
	
	/*void OnPurchaseItem(InApps.Transaction transaction)
    {
		/*if(!offline) 
		{
			return;
		}
		
		Debug.Log("onPurchaseItem");
		InApps.ValidateTransaction(transaction);
	}*/

    /*void OnValidateItem(InApps.Validate validate)
    {
		if(!offline) 
		{
			return;
		}
		
		Debug.Log("OnValidateItem: "+validate.status);
        if (validate.status == InApps.Status.ValidAndCredited)
		{
			BuyShootAgain(validate.item.name, Items.Status.ItemSucessfullyPurchased);
		}
    }*/
	
	private void SendToGameCenter()
	{
		float totalWorldTime = 0;
		
		for (int i = 1; i <= 6; i++)
		{
			totalWorldTime += Save.GetFloat ("world" + Flow.currentGame.world.id + "level" + i + "Time");
			//Debug.Log ("world" + Flow.currentGame.world.id + "level" + i + "Time" + ": " + Save.GetFloat ("world" + Flow.currentGame.world.id + "level" + i + "Time"));
			//Debug.Log ("totalWorldTime: " + totalWorldTime);
		}
		
		if (Info.appType == Info.AppType.Free)
		{
			Ranking.SetScore (Mathf.RoundToInt (totalWorldTime), "com.freegamesandtopapps.bangbang.world" + Flow.currentGame.world.id.ToString());
		}
		else if (Info.appType == Info.AppType.Pay)
		{
			Ranking.SetScore (Mathf.RoundToInt (totalWorldTime), "com.freegamesandtopapps.bangbangfull.world" + Flow.currentGame.world.id.ToString());
		}
	}
}
