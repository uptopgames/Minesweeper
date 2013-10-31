using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour 
{
	/*public GameObject bullet;
	public Transform gunPosition;
	
	// arrary com pontos onde o player podera ser atingido
	public Transform[] enemyArray = new Transform [6];
	public float power = 100f;
	
	// o inimigo tera o mesmo script
	public PlayerControl enemyScript;
	
	// posicao da arma
	public Transform gunFather, myFoot;
	private Transform enemy;
	
	// lista que contem todas as armas que estao dentro da hierarquia do gunFather
	private List<Transform> guns = new List<Transform>();
	
	//private bool shootOn = false;
	// booleana que definir qual dos personagens eh o player
	public bool isPlayer = false; 
	
	[HideInInspector] public bool gameOver = false;
	
	private Gameplay gameplay;

	// Use this for initialization
	void Start () 
	{
		//  para teste
		Flow.Game.currentGame.myGun = 3;
		Flow.Game.currentGame.myPastGun = 3;
		Flow.Game.currentGame.theirGun = 3;
		Flow.Game.currentGame.theirPastGun = 3;
		
		gameplay = Camera.mainCamera.GetComponent<Gameplay>();
		
		foreach (Transform child in gunFather)
		{
			// seta todas as armas como falsa (todas as armas sao filhas da gunFather (armas). Olhas hierarquia dentro do player
			guns.Add (child);
			child.gameObject.SetActive(false);
		}
		
		// estabelece qual arma que estara com cada usuario
		if (isPlayer) 
		{
		    if (!Flow.isSingle) guns[Flow.Game.currentGame.myGun - 1].gameObject.SetActive(true);
			else guns[Flow.singleMyGun - 1].gameObject.SetActive(true);
			//Debug.Log ("MyGun2: " + Flow.Game.currentGame.myGun);
			//Debug.Log ("theirGun2: " + Flow.Game.currentGame.theirGun);
			if (Flow.Game.path == "replay")
				guns[Flow.Game.currentGame.myPastGun-1].gameObject.SetActive(true);
			else
				guns[Flow.Game.currentGame.myGun-1].gameObject.SetActive(true);
		}
		else
		{
			// seta que a arma do adversario sera a 1 quando estiver criando o turno
			if (Flow.Game.path == "createGame" || Flow.Game.path == "newTurn") guns[0].gameObject.SetActive(true);
			
			else if (Flow.Game.path == "replay") 
				guns[Flow.Game.currentGame.theirPastGun-1].gameObject.SetActive(true);
			else
				guns[Flow.Game.currentGame.theirGun - 1].gameObject.SetActive(true);		
		}
	}
	
	// esse metodo eh chamado pelo invoke dentro do update. Ele tem um delay de 1 segundo que eh o tempo da animacao da camera de se mover
	public void RestartPosition()
	{	
		Debug.Log ("restartPosition");
		animation.CrossFade ("idle");
		enemyScript.animation.CrossFade ("idle");
		//Camera.mainCamera.animation.Stop();
		
		if (Flow.Game.path == "replay")
		{
			if (gameplay.round < 5)
			{
				gameplay.round ++;
				gameplay.showSigns = true;
				
				gameplay.hasShot = false;
			}
			else
				gameplay.showReplayFinalTable = true;
				gameplay.ShowResultsTable();
		}
		
		else
		{
			Debug.Log ("chameiResultsTableUp");
			//gameplay.showingResultsTable = true;
			gameplay.ShowResultsTable();
		}
	}
	
	void PlayAnimationWithDelay()
	{
		Debug.Log ("playAnimationWithDelay");
		enemyScript.animation.CrossFade ("win" + UnityEngine.Random.Range(1,3));
		
		Invoke ("RestartPosition", 3f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		// teste
		if (Flow.Game.path == "createGame" || Flow.Game.path == "newTurn" || (gameplay.shotOnFoot && gameplay.enemyShotOnFoot))
		{
			if (gameOver)
			{
				if (gameplay.timeToBang - gameplay.bangTime[gameplay.round-1] >= 0 && !gameplay.showBang)
				{
					Debug.Log ("invoqueiRestart");
					Invoke ("RestartPosition", 2f);
					gameOver = false;
				}
			}
			
		}
		
		else if (gameOver && !animation.isPlaying)
		{
		
			Invoke("PlayAnimationWithDelay", 3f);
				
			// animacao da camera que foca no usuario
			//Camera.mainCamera.animation.CrossFade("CameraWinAnim");
				
			gameOver = false;
			
		}
	}
	
	// metodo que eh chamado dentro da animacao do player na Unity
	public void shoot()
	{
		Debug.Log ("entreiNoShoot1");
		//if (animation["sheath"].speed == -5) // soh vai tocar se rodar de tras para frente.
		//{
			// instancia o tiro
		GameObject tempBullet = GameObject.Instantiate (bullet, gunPosition.position, gunPosition.rotation) as GameObject;
		
		// pega do script bullet 2 que o target a ser seguido pela bala eh o inimigo
		//tempBullet.GetComponent <Bullet2>().target = enemy;
		
		// defini randomicamente qual ponto do usuario sera atingido pela bala 
		if ((Flow.Game.path != "createGame" && gameplay.timeToBang >= gameplay.bangTime[gameplay.round-1] && Flow.Game.path != "Single") || 
			(Flow.Game.path == "Single"))
		{
			Debug.Log ("entreiNoShoot2");
			enemy = enemyArray[UnityEngine.Random.Range (0, enemyArray.Length - 1)];
		}
		
		// descomentar a linha abaixo quando ajustar os tiros do "createGame" e do "newTurn"
		//else if ( (Flow.Game.path == "createGame" || Flow.Game.path == "newTurn") && playAnim.timeToBang >= playAnim.bangTime[playAnim.roundsSignsInt]) enemy = enemyArray[enemyArray.Length -1];
		else if (Flow.Game.path == "createGame") enemy = enemyArray[0];
		else enemy = null;
		
		Debug.Log ("enemy: " + enemy);
		// adiciona o force para a bala ir para frente
		//tempBullet.rigidbody.AddForce (power * gunPosition.forward);
		
		// defini a posicao para qual a bala ira se mover, o tempo e o tipo de curva. 
		// "position", "time" e "oncomplete" sao argumentos dessa funcao do iTwee.MoveTo
		if (enemy != null)
			iTween.MoveTo (tempBullet, iTween.Hash ("position", enemy.position, "time", 2f, "oncomplete", "GetBulletHit", "easetype", iTween.EaseType.spring)); 
	}
	
	// ajustar depois que Raphael fizer a animacao completa
	public void shootOnFoot()
	{
		GameObject tempBullet = GameObject.Instantiate (bullet, gunPosition.position, gunPosition.rotation) as GameObject;
		iTween.MoveTo (tempBullet, iTween.Hash ("position", myFoot.position, "time", 2f, "oncomplete", "GetBulletHit", "easetype", iTween.EaseType.spring)); 
	}
	
	// metodo que eh chamado no script da bala qdo verificado a colisao
	public void GetBulletHit()
	{
		if (gameplay.shotOnFoot && gameplay.enemyShotOnFoot)
		{
			Debug.Log ("ambosAtiraramNoPe");
			gameOver = true;
		}
		
		Debug.Log ("ShotOnFoot: " + gameplay.shotOnFoot);
		// se eu atirei no pe enquanto criava o nivel chama o gameover
		if (Flow.Game.path == "createGame" || Flow.Game.path == "newTurn")
		{
			if (gameplay.shotOnFoot) 
			{
				gameOver = true;
				Debug.Log ("gameOver: " + gameOver);
			}
			else gameOver = true;
		}
		
		//if (Flow.Game.path != "createGame" && playAnim.timeToBang >= playAnim.bangTime && !playAnim.shotOnFoot)
		else if (gameplay.timeToBang >= gameplay.bangTime[gameplay.round-1])
		{
			Debug.Log ("entreiNoLose");
			
			animation.CrossFade ("lose" + UnityEngine.Random.Range(1,2));
			//animation.CrossFadeQueued ("lose" + UnityEngine.Random.Range (1,2), 0.1f, QueueMode.PlayNow);
			gameOver = true;
			if(isPlayer)
			{
				Camera.mainCamera.animation.CrossFade("CameraLoseAnim");
			}
		}
	}*/	
}