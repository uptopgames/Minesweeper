using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Gunslinger : MonoBehaviour
{
	public enum PlayerStates
	{
		Idle, Prepare, Shoot, ShootOnFoot, Die, Victory, Wait
	}
	public PlayerStates state = PlayerStates.Idle;
	public bool isPlayer = false; //marcar true para o personagem do jogador
	public Gunslinger enemy; //colocar o personagem adversário aqui
	public Gameplay gamePlay; //instancia do script gameplay
	
	public GameObject bullet; //colocar o prefab da bala aqui
	public Transform bulletPosition; //associar o transform que marca a origem da bala aqui
	public Transform target; //associar o ponto no corpo do gunslinger que vai receber a bala
	public Transform foot; //associar o ponto no corpo do gunslinger que vai receber a bala no pé
	
	private float prepareTimeOut = 0; //contador usado para ver quem acertou quem
	//private float[] timeToBang =  new float[5]; //pegar do script gameplay: é o tempo de espera do BANG label aparecer
	public float gunReaction = 0; //= 0.1f; //pegar do script gameplay: é o tempo de reacao da arma do Gunslinger
	
	public SkinnedMeshRenderer charRenderer; // eh o render do personagem. No caso, Pistoleiro
	public Material[] charMaterials;
	public GameObject[] charObjects;
	public GameObject[] guns;
	public int currentChar = 0;
	public int currentGun = 0;
	private bool hasShot = false;
	
	public void StartGame()
	{	
		// UP TOP FIX ME TESTE
		
		for (int i = 0; i < 5; i++)
		{
			if (Flow.path == TurnStatus.ShowPast)
			{
				//Debug.Log ("myPastShotTime: " + i + ": " + Flow.currentGame.pastMyRoundList[i].time);
				//Debug.Log ("theirPastShotTime: " + i + ": " + Flow.currentGame.pastTheirRoundList[i].time);
				//Debug.Log ("theirPastBangTime: " + i + ": " + Flow.currentGame.pastTheirRoundList[i].bangTime);
			}
			else
			{
				//Debug.Log ("theirShotTime" +  i + ": " + Flow.currentGame.theirRoundList[i].time);
				//Debug.Log ("bangTime" +  i + ": " + Flow.currentGame.theirRoundList[i].bangTime);
			}
		}
		
		//prepareTimeOut = shotTime;
		if(isPlayer)
		{
			currentGun = (Flow.path == TurnStatus.ShowPast) ? Flow.currentGame.pastMyRoundList[0].gun.id : Flow.currentGame.myRoundList[0].gun.id ;
			gunReaction = (Flow.path == TurnStatus.ShowPast) ? Flow.currentGame.pastMyRoundList[0].gun.reaction : Flow.currentGame.theirRoundList[0].gun.reaction;
			currentChar = 0;	
		}
		else
		{
			currentGun = (Flow.path == TurnStatus.ShowPast) ? Flow.currentGame.pastTheirRoundList[0].gun.id : Flow.currentGame.theirRoundList[0].gun.id;
			gunReaction = (Flow.path == TurnStatus.ShowPast) ? Flow.currentGame.pastTheirRoundList[0].gun.reaction : 
				Flow.currentGame.theirRoundList[0].gun.reaction;
			
			enemy.gunReaction = (Flow.path == TurnStatus.ShowPast) ? Flow.currentGame.pastMyRoundList[0].gun.reaction : Flow.currentGame.myRoundList[0].gun.reaction;
			
			if (Flow.currentMode == GameMode.SinglePlayer)
			{
				currentChar = Flow.currentGame.level.id;
			}
			else
				currentChar = 1;
			
		}
		
		if (Flow.path != TurnStatus.ShowPast)
		{
			//Debug.Log ("isPlayer: " + isPlayer +  " MyGun: " + Flow.currentGame.myRoundList[0].gun.id);
			//Debug.Log ("isPlayer: " + isPlayer +  " theirGun: " + Flow.currentGame.theirRoundList[0].gun.id);
			//Debug.Log ("isPlayer: " + isPlayer +  " MyGunReaction: " + gunReaction);
			//Debug.Log ("isPlayer: " + isPlayer + " EnemyGunReaction: " + enemy.gunReaction);
		}
		
		charObjects[currentChar].SetActive(true);
		guns[currentGun-1].SetActive(true);
		charRenderer.material = charMaterials[currentChar];
	}
	
	//controla a mudanca de estados e transicao de animacoes
	void ChangeState(string newState, float fadeDuration = 0.1f, bool queued = false, string differentAnimation = "", bool noAnimation = false)
	{
		//converte a string passada para identificar o estado do Enum PlayerStates
		//duration é o tempo de transicao entre uma animacao e a proxima
		state = (PlayerStates)Enum.Parse(typeof(PlayerStates), newState, true);
		
		if(noAnimation) return;
		
		if(queued) //pára a animacao atual e comeca a nova imediatamente
		{
			if(differentAnimation == "")
			{
				animation.CrossFadeQueued (newState, fadeDuration, QueueMode.PlayNow);
			}
			else
			{
				//Debug.Log("dei crossfade na anim " + differentAnimation);
				animation.CrossFadeQueued (differentAnimation, fadeDuration, QueueMode.PlayNow);
			}
		}
		
		else //espera a transicao atual acabar e comeca a nova depois
		{
			if (differentAnimation == "")
			{
				animation.CrossFade(newState, fadeDuration);
			}
			else
			{
				//Debug.Log ("dei crossfade na anim " + differentAnimation);
				animation.CrossFade(differentAnimation, fadeDuration);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Se o jogador apertar em qualquer lugar da Tela no Passado, pula o Replay
		/*if (Flow.path == TurnStatus.ShowPast)
		{
			if (!gamePlay.jumpReplay)
			{
				if (Input.GetMouseButtonDown(0))
				{
					gamePlay.jumpReplay = true;
					
					if (isPlayer)
					{
						ChangeState("idle");
						Debug.Log ("JumpReplay");
					}
					else
					{
						ChangeState("idle");
						Debug.Log ("JumpReplay");
					}
					prepareTimeOut = 0;
					Invoke("NextRound", 0.1f);
				}
			}
		}*/
		
		switch(state)
		{
			case PlayerStates.Idle:
				IdleState();
			break;
			
			case PlayerStates.Prepare:
				PrepareState();
			break;
			
			case PlayerStates.Shoot:
				ShootState();
			break;
			
			case PlayerStates.ShootOnFoot:
				ShootOnFootState();
			break;
			
			case PlayerStates.Die:
				DieState();
			break;
			
			case PlayerStates.Victory:
				VictoryState();
			break;
		}
	}
	
	#region Update dos Estados da Máquina de Estados
	
	void IdleState()
	{
		//counter do bang: quando acabar, muda state para PrepareState e zera prepareTimeOut
		/*if (gamePlay.showSigns)
		{
			//Debug.Log ("ShowBangIdleState");
			//timeToBang[gamePlay.round-1] -= Time.deltaTime;
			Flow.currentGame.theirRoundList[gamePlay.round-1].bangTime -= Time.deltaTime;
		}*/
			
		
		//if (Flow.currentGame.theirRoundList[gamePlay.round-1].bangTime <= 0 && gamePlay.showBang)
		if (gamePlay.showBang)
		{
			//Debug.Log("atira atira atira!");
			state = PlayerStates.Prepare;
			prepareTimeOut = 0;
			return;
		}
		
		// REPLAY
		if (Flow.path == TurnStatus.ShowPast && !gamePlay.jumpReplay)
		{
			if (isPlayer)
			{
				//Debug.Log ("myPastShotTime: " + Flow.currentGame.pastMyRoundList[gamePlay.round-1].time);
				if (Flow.currentGame.pastMyRoundList[gamePlay.round-1].time < 0)
				{
					if (gamePlay.showSigns)
						ChangeState("shootOnFoot");
				}
			}
			else  
			{
				if (Flow.currentGame.pastTheirRoundList[gamePlay.round-1].time < 0)
				{
					if (gamePlay.showSigns)
						ChangeState ("shootOnFoot");	
				}
			}
		}
		
		// TODOS OS OUTROS CASOS
		else if (Flow.path != TurnStatus.ShowPast)
		{
			if(isPlayer)
			{
				if (!hasShot)
					if(Input.GetMouseButtonDown(0))
					{
						hasShot = true;
						ChangeState("shootOnFoot");
						//Debug.Log("acertou no pé, seu apressado...");
						Flow.currentGame.myRoundList[gamePlay.round-1].playerRoundWin = 0;
						gamePlay.myPoints += 0;
						Flow.currentGame.myRoundList[gamePlay.round-1].time = -2;
						//passar para o Gameplay que o jogador atirou no pé
					}
			}
			else
			{
				if (Flow.path != TurnStatus.BeginGame)
				{
					//Debug.Log ("gameplayRound: " + gamePlay.round);
					
					if (Flow.currentGame.theirRoundList[gamePlay.round-1].time < 0)
					{
						if (gamePlay.showSigns) ChangeState ("shootOnFoot");
						gamePlay.enemyPoints += 0;
						Flow.currentGame.theirRoundList[gamePlay.round-1].playerRoundWin = 0;
						//Debug.Log("inimigo idiota se matou");
					}
				}
			}
		}
	}
	
	void PrepareState()
	{
		//Debug.Log ("PrepareState");
		prepareTimeOut += Time.deltaTime;
		//Debug.Log ("prepareTime: " + prepareTimeOut);
		
		// REPLAY
		if (Flow.path == TurnStatus.ShowPast && !gamePlay.jumpReplay)
		{
			if (isPlayer)
			{
				if (Flow.currentGame.pastMyRoundList[gamePlay.round-1].playerRoundWin == 1)
				{
					if (prepareTimeOut >= Flow.currentGame.pastMyRoundList[gamePlay.round-1].time)
					{
						EnterShoot();
					}
				}
			}
			else
			{
				if (Flow.currentGame.pastTheirRoundList[gamePlay.round-1].playerRoundWin == 1)
				{
					if (prepareTimeOut >= Flow.currentGame.pastTheirRoundList[gamePlay.round-1].time)
					{
						EnterShoot();
					}
					
				}
				
			}
		}
		
		// TODOS OS OUTROS CASOS
		else if (Flow.path != TurnStatus.ShowPast)
		{
			
			if(isPlayer)
			{
				if (!hasShot)
				{
					if (Input.GetMouseButtonDown(0))
					{
						hasShot = true;
						// TESTE
						//gamePlay.myPoints = 3;
						
						//Debug.Log ("CLIQUEI");
						//if(prepareTimeOut + gunReaction <= enemy.shotTime + enemy.gunReaction
							//|| enemy.state == PlayerStates.ShootOnFoot)
						if (prepareTimeOut + gunReaction <= Flow.currentGame.theirRoundList[gamePlay.round-1].time + enemy.gunReaction
							|| enemy.state == PlayerStates.ShootOnFoot || Flow.path == TurnStatus.BeginGame)
						{
							//Debug.Log("matei o inimigo");
							//Debug.Log ("prepareTimeOut: " + prepareTimeOut + " gunReaction: " + gunReaction);
							//Debug.Log ("enemyShotTime: " +Flow.currentGame.theirRoundList[gamePlay.round-1].time + " enemyGunReaction: " + enemy.gunReaction);
							EnterShoot();
							//gamePlay.blackScreen = false;
							gamePlay.blackScreen.GetComponent<UIInteractivePanel>().StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
							gamePlay.blackScreen.transform.FindChild("Text").gameObject.SetActive(false);
							//gamePlay.showBang = false;
							
							// TESTE
							gamePlay.myPoints++;
							//gamePlay.myPoints = 3;
							
			
							
							Flow.currentGame.myRoundList[gamePlay.round-1].playerRoundWin = 1;
							Flow.currentGame.myRoundList[gamePlay.round-1].time = prepareTimeOut;
							//passar para o gamePlay que o jogador venceu no tempo prepareTimeOut
						}
						else
						{
							//Debug.Log("caso que n acontece");
						}
					}
				}
			}
			else // ENEMY
			{
				//if (prepareTimeOut >= shotTime)
				if (Flow.path != TurnStatus.BeginGame)
				{
					if (prepareTimeOut >= Flow.currentGame.theirRoundList[gamePlay.round-1].time + gunReaction)
					{
						//Debug.Log("demorou demais, o cara te matou...");
						//Debug.Log ("enemyTotalTime: " + prepareTimeOut + enemy.gunReaction);
						//Debug.Log ("enemyTime: " + enemy.prepareTimeOut);
						//Debug.Log ("gamplayRound: " + gamePlay.round);
						
						EnterShoot();
						//gamePlay.blackScreen = false;
						gamePlay.blackScreen.GetComponent<UIInteractivePanel>().StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
						gamePlay.blackScreen.transform.FindChild("Text").gameObject.SetActive(false);
						//gamePlay.showBang = false;
						gamePlay.enemyPoints++;
						Flow.currentGame.theirRoundList[gamePlay.round-1].playerRoundWin = 1;
						Flow.currentGame.myRoundList[gamePlay.round-1].time = 0;
						//Debug.Log ("flowMyTime: " + Flow.currentGame.myRoundList[gamePlay.round-1].time);
						//Debug.Log ("flowEnemyTime: " + Flow.currentGame.theirRoundList[gamePlay.round-1].time);
						//passar para o gamePlay que o jogador perdeu no tempo enemy.prepareTimeOut
					}
				//counter do tempo de tiro do inimigo: quando acabar, chama EnterShoot
				}
			}
		}
	}
	
	void ShootState()
	{
		//Debug.Log("ShootState");
		if(!animation.isPlaying)
		{
			Debug.Log("terminei de atirar");
			//SendBulletOnEnemy();
			state = PlayerStates.Wait;
		}
	}
	
	void ShootOnFootState()
	{
		//Debug.Log ("ShootOnFootState");
		//verifica se a animacao de atirar no pé parou de rodar e entao troca para a animacao de pulo
		if(!animation.isPlaying)
		{
			animation.CrossFade("jump");
		}
		
		if ( Flow.path != TurnStatus.ShowPast && ((Flow.path == TurnStatus.BeginGame && gamePlay.showBang) ||
			(Flow.currentGame.myRoundList[gamePlay.round-1].time < 0 && Flow.currentGame.theirRoundList[gamePlay.round-1].time < 0 && gamePlay.showBang && isPlayer))
			|| (Flow.path == TurnStatus.ShowPast && gamePlay.showBang && Flow.currentGame.pastMyRoundList[gamePlay.round-1].time < 0 
			&& Flow.currentGame.pastTheirRoundList[gamePlay.round-1].time < 0 && isPlayer) ) 
			
		{
			prepareTimeOut = 0;
			Invoke("NextRound", 0.1f);
			//gamePlay.blackScreen = false;
			gamePlay.blackScreen.GetComponent<UIInteractivePanel>().StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
			gamePlay.blackScreen.transform.FindChild("Text").gameObject.SetActive(false);
		}
	}
	
	void DieState()
	{
		
	}
	
	void VictoryState()
	{
		
	}
	
	#endregion
	
	#region Transicoes Especiais entre os Estados da Máquina de Estados
	
	public void EnterShoot()
	{
		//Debug.Log ("EnterShoot");
		ChangeState("shoot", 0.1f, true);
		enemy.state = PlayerStates.Wait;
		//muda para o estado de Wait e roda 1x a animacao de shoot
	}
	
	public void EnterShootOnFoot() // Quando esta sendo chamado?
	{
		//Debug.Log ("EnterShootOnFoot");
		ChangeState("shootOnFoot");
	}
	
	public void Reset()
	{
		//Debug.Log ("Reset");
		hasShot = false;
		gamePlay.showSigns = false;	
		ChangeState("idle");
		prepareTimeOut = 0;
		//gunReaction = 0;
		if(isPlayer)
		{
			Camera.mainCamera.gameObject.SampleAnimation(Camera.mainCamera.animation["CameraWinAnim"].clip, 0);
			Camera.mainCamera.gameObject.SampleAnimation(Camera.mainCamera.animation["CameraLoseAnim"].clip, 0);
		}
	}
	
	public void EnterDeath()
	{
		//Debug.Log("EnterDeath");
		//essa animacao precisa ser queued porque ela pode acontecer a qualquer momento, diferente das outras
		int randomDie = UnityEngine.Random.Range (1, 2);
		ChangeState("die", 0.1f, true, "die" + randomDie);
		
		Invoke("AnimateCamera", 0.6f);
	}
	
	void AnimateCamera()
	{
		if(isPlayer)
		{
			//Debug.Log("AnimateCameraPlayer");
			Camera.mainCamera.animation.CrossFade("CameraLoseAnim");
			Invoke("EnemyEnterVictory", Camera.mainCamera.animation["CameraLoseAnim"].length*0.95f);
		}
		else
		{
			//Debug.Log("AnimateCameraEnemy");
			Camera.mainCamera.animation.CrossFade("CameraWinAnim");
			Invoke("EnemyEnterVictory", Camera.mainCamera.animation["CameraWinAnim"].length*0.95f);
		}
		
		//rodar animacao de vitória da câmera de cutscene e depois chamar os rounds do script gameplay
	}
	
	void EnemyEnterVictory()
	{
		//Debug.Log("EnemyEnterVictory");
		int randomVictory = UnityEngine.Random.Range (1, 5);
		//Debug.Log ("randomVictory: " + randomVictory);
		enemy.ChangeState("victory", 0.1f, true, "victory"+randomVictory);
		Invoke("NextRound", 2);
	}
	
	void NextRound()
	{
		//Debug.Log ("gunsLingerNextRound");
		gamePlay.ShowResultsTable();
		gamePlay.showBang = false;
	}
	
	#endregion
	
	#region Eventos de Animacao
	
	void SendBulletOnEnemy()
	{
		//Debug.Log("go bullet!");
		GameObject tempBullet = GameObject.Instantiate (bullet, bulletPosition.position, bulletPosition.rotation) as GameObject;
		tempBullet.GetComponent<Bullet>().target = target;
		tempBullet.GetComponent<Bullet>().playerBullet = isPlayer;
		tempBullet.GetComponent<Bullet>().dieOnTouch = false;
	}
	
	void SendBulletOnFoot()
	{
		//Debug.Log("ai, meu pé!");
		GameObject tempBullet = GameObject.Instantiate(bullet, bulletPosition.position, bulletPosition.rotation) as GameObject;
		tempBullet.GetComponent<Bullet>().target = foot;
		tempBullet.GetComponent<Bullet>().playerBullet = isPlayer;
		tempBullet.GetComponent<Bullet>().dieOnTouch = true;
	}
	
	#endregion
	
	void DebugAnim()
	{
		Debug.Log("rodei");
	}
}