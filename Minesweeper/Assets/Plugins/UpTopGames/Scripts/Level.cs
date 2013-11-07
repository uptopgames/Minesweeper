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
		//this.points = level.points;
		this.time = level.time;
		this.lastUpdate = level.lastUpdate;
		this.tileset = level.tileset;
	}
	
	void EnterLevel()
	{
		//Debug.Log ("EnterLevel");
		//Debug.Log ("Points: " + points);
		//Debug.Log ("toUnlock: " + toUnlock);
		//Debug.Log ("name: " + transform.name);
		
		if (Save.GetInt (PlayerPrefsKeys.POINTS) >= toUnlock)
		{
			//Debug.Log ("unlocked");
			
			if(Flow.currentMode == GameMode.SinglePlayer) Flow.currentGame = new Game();
			
			Flow.currentGame.world = gameObject.transform.parent.GetComponent<World>();
			Flow.currentGame.level = this;
			//Flow.currentMode = GameMode.SinglePlayer;
			
			
			if(Flow.currentMode == GameMode.SinglePlayer)
			{
				Flow.path = TurnStatus.AnswerGame;
				Flow.currentGame.friend = new Friend();
				Flow.currentGame.friend.rawText = image;
				
				Flow.currentGame.theirRoundList = new List<Round>();
			}
			//Flow.currentGame.myRoundList = new List<Round>();
			//Flow.currentGame.myRoundList.Add(new Round(-1,-1,-1,GameObject.FindWithTag("Guns").GetComponent<Guns>().guns[0],-1,1,0,0));
			
			
			
			/*for (int i = 0; i < Flow.ROUNDS_PER_TURN; i++)
			{
				Flow.currentGame.theirRoundList.Add(new Round(-1,-1,-1, transform.parent.GetComponent<World>().enemyGun, 
					UnityEngine.Random.Range(time.x, time.y), UnityEngine.Random.Range(1, 5), 0,0));
					//UnityEngine.Random.Range(3, 5), UnityEngine.Random.Range(3, 5), 0,0)); TESTE
			}*/
			
			//Debug.Log ("EnterLevelGunId: " + Flow.currentGame.theirRoundList[0].gun.id);
			//Debug.Log ("EnterLevelGunReaction: " + Flow.currentGame.theirRoundList[0].gun.reaction);
			//Debug.Log ("World: " + Flow.currentGame.world.id);
			//Debug.Log ("Level: " + Flow.currentGame.level.id);
			
			Flow.config.GetComponent<ConfigManager>().inviteAllScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
			Flow.config.GetComponent<ConfigManager>().invitePlayingScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
			
			Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(false);
			Application.LoadLevel("Game");
		}
		else
		{
			Debug.Log ("locked");
			Flow.game_native.showMessage("Locked Level", "You must beat the previous level to unlock this one.", "Ok");
		}
	}
	
	void Start()
	{
		//Debug.Log ("LevelSelectionGameObject: " + 
			//GameObject.FindWithTag("LevelSelection").transform.FindChild("ScrollLevels").transform.GetChild(0).transform.GetChild(1).GetChild(1).name);
		//GameObject.FindWithTag("LevelSelection").transform.FindChild("ScrollLevels").transform.GetChild(0).transform.GetChild(4).FindChild("Level 2 Panel").name);
		//Debug.Log ("LevelSelectionGameObject: " + GameObject.FindWithTag("LevelSelection").transform.FindChild("World 2 List Item").name);
		
		//Debug.Log ("world: " + transform.parent.GetComponent<World>().id);
		
		//Debug.Log ("withTagTesteLevel: " + GameObject.FindWithTag("World2").GetComponent<World>().id);
		
		//if (transform.parent.GetComponent<World>().id == 3)
		//{
			//Debug.Log ("worldMaisQuePassado: " + (transform.parent.GetComponent<World>().id-2));
			//Debug.Log ("stars: " + (Save.GetInt ("world" + (transform.parent.GetComponent<World>().id-2) + "_level" + id + "_stars")));
			//Debug.Log ("hasKey: " + Save.HasKey ("world" + (transform.parent.GetComponent<World>().id - 2) + "_level" + Flow.MAX_LEVEL_NUMBER + "_stars"));
		//}
		
		//Debug.Log ("level: " + id);
		
		//Debug.Log ("id: " + id + " stars: " + stars);
		//Debug.Log ("points: " + points);
		//Debug.Log ("toUnlock: " + toUnlock);
		
		
		// PARA QUEM FOR USAR EM JOGO QUE PRECISE SOMAR PONTOS DE LEVEIS ANTERIORES PARA DESTRAVAR O PROXIMO
		
		/*if (transform.parent.GetComponent<World>().id > 1 && id == 1)
		{
			if (!Save.HasKey ("world" + transform.parent.GetComponent<World>().id + "_level" + id + "_stars") && 
				Save.HasKey ("world" + (transform.parent.GetComponent<World>().id - 1) + "_level" + Flow.MAX_LEVEL_NUMBER + "_stars"))
			{
				points = (transform.parent.GetComponent<World>().id -1) * 9 + id;
				//Debug.Log ("id: " + id + "Points1: " + points);
			}
		}
		else
		{
			if (!Save.HasKey ("world" + transform.parent.GetComponent<World>().id + "_level" + id + "_stars") &&
				Save.HasKey ("world" + transform.parent.GetComponent<World>().id + "_level" + (id-1) + "_stars"))
			{
				points = (transform.parent.GetComponent<World>().id -1) * 9 + id;
				//Debug.Log ("id: " + id + "Points2: " + points);
			}
		}
			
		if (Save.HasKey ("world" + transform.parent.GetComponent<World>().id + "_level" + id + "_stars"))
		{
			stars = Save.GetInt ("world" + transform.parent.GetComponent<World>().id + "_level" + id + "_stars");
			points =  (transform.parent.GetComponent<World>().id -1) * 9 + id;
			//Debug.Log ("id: " + id + "Points3: " + points);
		}*/
		
		if(Save.GetInt (PlayerPrefsKeys.POINTS) >= toUnlock)
		{
			//Debug.Log ("id: " + id);
			//Debug.Log ("points: " + points);
			//Debug.Log ("toUnlock: " + toUnlock);
			
			//Debug.Log ("unlocked");
			transform.FindChild("Locker").gameObject.SetActive(false);
			
			for(int i = 1 ; i <= stars ; i++)
			{
				//Debug.Log ("desenhaEstrelasLevel: " + id);
				transform.FindChild("LevelStar"+i).gameObject.SetActive(true);
				transform.FindChild("LevelStarGrey"+i).gameObject.SetActive(false);
				
			}
		}
		else 
		{
			/*Color levelTextureColor = transform.FindChild("LevelTexture").GetComponent<PackedSprite>().color;
			levelTextureColor.a = 0.5f;
			
			transform.FindChild("LevelTexture").GetComponent<PackedSprite>().SetColor(levelTextureColor);*/
			
			transform.FindChild("Locker").gameObject.SetActive(true);
		}
	}
}
