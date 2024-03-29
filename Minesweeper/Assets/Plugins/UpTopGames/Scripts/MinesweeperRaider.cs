using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using CodeTitans.JSon;

public enum GameState
{
	Start, PlayerTurn, Moving, Null
}

public class MinesweeperRaider : MonoBehaviour
{
	public Transform[] cameras1;
	public Transform[] cameras2;
	public int currentCameraView = 0;
	public int currentWorld = 0;
	public int currentLevel = 0;
	
	public List<Transform> cameraList;
	private Transform[] cameras;
	private Animation currentAnimation;
	private Transform currentCamera;
	
	public Camera GUICamera;
	
	public GameState gameState = GameState.Null;
	
	public GameObject cutsceneHuds;
	public GameObject gameHuds;
	public GameObject flagPrefab;
	public GameObject mine;
	public GameObject ragdoll;
	public GameObject blast;
	public GameObject shieldPrefab;
	public GameObject endFade;
	public GameObject radarPing;
	public GameObject squarePing;
	public GameObject levelUpFade;
	
	public SpriteText timeLabel;
	private float timeCounter = 0;
	
	public List<List<int>> tileset;
	private int currentRow = 0;
	private int currentColumn = 0;
	private int diamondRow = 0;
	private int diamondColumn = 0;
	private int previousRow = 0;
	private int previousColum = 0;
	
	public Transform shield;
	public Transform mineManager;
	public Transform character;
	public Transform winCamera;
	public Transform diamond;
	public GameObject[] hp;
	public GameObject hat;
	public Texture[] tommyTextures;
	public Material tommyMaterial;
	private Vector3 startingPosition;
	
	private int currentHp;
	private int mapedMines;
	private int radarDistance;
	public int respawnTime = 4;
	public GameObject testButtons;
	private bool flagMode = false;
	public int shieldHp = 0;
	private List<Vector2> mineCoordinates;
	private bool leveledUp = false;
	private string currentUpgrade = "hp";
	
	public UIInteractivePanel levelUpPanel;
	public SpriteText upgradesDescription;
	
	public GameObject shieldBlock;
	public GameObject radarBlock;
	public GameObject mapBlock;
	private bool increasingExp;
	
	public SpriteText shieldCount;
	public SpriteText radarCount;
	public SpriteText mapCount;
	
	string fileName = "tileset.xml";
	TextAsset tilesetXML;
	string rawXML;
	private List<List<List<int>>> xmlList;
	
	public AudioClip destroyFlagSound;
	public AudioClip shieldSound;
	public AudioClip mapSound;
	public AudioClip radarSound;
	public AudioClip victorySound;
	public AudioClip experienceSound;
	public AudioClip levelUpSound;
	public AudioClip upgradeSound;
	
	void Start()
	{
		if(Save.HasKey(PlayerPrefsKeys.ITEM+"map")) mapCount.Text = "x" + Save.GetInt(PlayerPrefsKeys.ITEM+"map");
		else mapCount.Text = "x0";
		if(Save.HasKey(PlayerPrefsKeys.ITEM+"radar"))radarCount.Text = "x" + Save.GetInt(PlayerPrefsKeys.ITEM+"radar");
		else radarCount.Text = "x0";
		if(Save.HasKey(PlayerPrefsKeys.ITEM+"shield"))shieldCount.Text = "x" + Save.GetInt(PlayerPrefsKeys.ITEM+"shield");
		else shieldCount.Text = "x0";
		
		gameHuds.SetActive(false);
		cutsceneHuds.SetActive(true);
		
		Flow.header.levelText.Text = "Level " + Flow.playerLevel.ToString();
		Flow.header.experienceText.Text = "Exp " + Flow.playerExperience.ToString();
		Flow.header.expBar.width = 7 * Flow.playerExperience/(Flow.playerLevel * Flow.playerLevel * 100);
		Flow.header.expBar.CalcSize();
		
		mapedMines = Flow.mapLevel;
		radarDistance = Flow.radarLevel + 1;
		currentHp = Flow.hpLevel + 2;
		
		if(Flow.hpLevel < 5)
		{
			upgradesDescription.transform.parent.FindChild("HPButton").FindChild("Level").GetComponent<SpriteText>().Text = Flow.hpLevel.ToString();
		}
		else
		{
			upgradesDescription.transform.parent.FindChild("HPButton").FindChild("Level").GetComponent<SpriteText>().Text = "MAX.";
		}
		if(Flow.mapLevel < 5)
		{
			upgradesDescription.transform.parent.FindChild("MapButton").FindChild("Level").GetComponent<SpriteText>().Text = Flow.mapLevel.ToString();
		}
		else
		{
			upgradesDescription.transform.parent.FindChild("MapButton").FindChild("Level").GetComponent<SpriteText>().Text = "MAX.";
		}
		if(Flow.radarLevel < 5)
		{
			upgradesDescription.transform.parent.FindChild("RadarButton").FindChild("Level").GetComponent<SpriteText>().Text = Flow.radarLevel.ToString();
		}
		else
		{
			upgradesDescription.transform.parent.FindChild("RadarButton").FindChild("Level").GetComponent<SpriteText>().Text = "MAX";
		}
		
		if(Flow.hpLevel >= 5 && Flow.mapLevel >= 5 && Flow.radarLevel >= 5)
		{
			currentUpgrade = "max";
			upgradesDescription.Text = "All upgrades have been bought.";
		}
		
		for(int i = 0; i<Flow.hpLevel+2; i++)
		{
			hp[i].SetActive(true);
		}
		
		if(Flow.currentCustomStage != -1)
		{
			currentWorld = Flow.customStages[Flow.currentCustomStage].world;
			currentLevel = 0;
			
			string testTileset = "";
			foreach(List<int> listInt in Flow.customStages[Flow.currentCustomStage].tileset)
			{
				foreach(int i in listInt) testTileset += i;
			}
			Debug.Log(testTileset);
			
			tileset = Flow.customStages[Flow.currentCustomStage].tileset;
			
			RealStart();
		}
		else
		{
			StartGame(0,0);
		}
	}
	
	public void StartGame(int world, int level)
	{
		currentWorld = Flow.currentGame.world.id - 3;
		currentLevel = Flow.currentGame.level.id - currentWorld*9 - 7;
		
		float tempCounter = 0;
		tileset = new List<List<int>>();
		List<int> tempList = new List<int>();
		foreach(int i in Flow.currentGame.level.tileset)
		{
			if(tempCounter == 8)
			{
				tileset.Add(tempList);
				tempList = new List<int>();
				tempCounter = 0;
			}
			tempList.Add(i);
			tempCounter++;
		}
		
		Flow.currentGame.myRoundList = new List<Round>();
		Flow.currentGame.myRoundList.Add(new Round(-1,-1,-1,-1,-1,-1));
		
		tileset.Add(tempList);
		
		RealStart();
	}
	
	void ChangeFlagMode()
	{
		if(flagMode) flagMode = false;
		else flagMode = true;
	}
	
	void RealStart ()
	{
		if(testButtons != null) testButtons.SetActive(false);
		
		gameState = GameState.Start;
		
		startingPosition = character.position;
		//Debug.Log("starting position: " + startingPosition);
		
		foreach(Transform child in mineManager) child.GetComponent<MineCounter>().UpdateText();
		
		int childCounter = 0;
		foreach(Transform child in transform)
		{
			if(childCounter == currentWorld)
			{
				transform.GetChild(childCounter).gameObject.SetActive(true);
				currentAnimation = transform.GetChild(childCounter).animation;
				currentAnimation.Play();
			}
			else
			{
				transform.GetChild(childCounter).gameObject.SetActive(false);
			}
			
			childCounter++;
		}
		
		int camRandomizer = Mathf.CeilToInt(UnityEngine.Random.Range(-0.99f,1));
		if(camRandomizer == 0)
		{
			cameras = cameras1;
			foreach(Transform tempCamera in cameras2) tempCamera.gameObject.SetActive(false);
			foreach(Transform tempCamera in cameras1) tempCamera.gameObject.SetActive(true);
		}
		else
		{
			cameras = cameras2;
			foreach(Transform tempCamera in cameras2) tempCamera.gameObject.SetActive(true);
			foreach(Transform tempCamera in cameras1) tempCamera.gameObject.SetActive(false);
		}
		
		if(shieldHp>0)
		{
			character.animation.CrossFade("standShield");
			shield.gameObject.SetActive(true);
		}
		else
		{
			character.animation.CrossFade("stand");
			shield.gameObject.SetActive(false);
		}
		
		mineCoordinates = new List<Vector2>();
		
		CreateTiles();
	}
	
	void Update ()
	{
		if(increasingExp)
		{
			//Debug.Log(Flow.header.expBar.width);
			if(Flow.header.expBar.width < 7 * Flow.playerExperience/(Flow.playerLevel * Flow.playerLevel * 100))
			{
				Flow.header.expBar.width += 2 * Time.deltaTime;
				Flow.header.expBar.CalcSize();
				if(Flow.header.expBar.width > 7)
				{
					//Debug.Log("zerei");
					Flow.header.expBar.width = 0;
					Flow.header.expBar.CalcSize();
					CheckExperience();
					leveledUp = true;
				}
			}
			else if(Flow.header.expBar.width > 7)
			{
				//Debug.Log("zerei");
				Flow.header.expBar.width = 0;
				Flow.header.expBar.CalcSize();
				CheckExperience();
				leveledUp = true;
			}
			else
			{
				Flow.header.expBar.width = 7 * Flow.playerExperience/(Flow.playerLevel * Flow.playerLevel * 100);
				Flow.header.expBar.CalcSize();
				increasingExp = false;
		
				Debug.Log("salvei level " + Flow.playerLevel + " na key " + PlayerPrefsKeys.PLAYERLEVEL);
				
				Save.Set(PlayerPrefsKeys.PLAYERLEVEL, Flow.playerLevel, true);
				Save.Set(PlayerPrefsKeys.PLAYEREXPERIENCE, Flow.playerExperience, true);
				
				Flow.header.experienceText.Text = "Exp " + Flow.playerExperience.ToString();
				
				if(!leveledUp)
				{
					Invoke("NextLevel", 3.7f);
					Invoke("FadeIn", 2f);
				}
				else
				{
					Invoke("LevelUp", 0.1f);	
				}
			}
		}
		
		switch(gameState)
		{
			case GameState.Start:
				if(currentAnimation != null)
				{
					if(!currentAnimation.isPlaying)
					{
						iTween.MoveTo(cameras[currentWorld].gameObject, iTween.Hash("position", cameraList[currentCameraView], "time", 1,
						"oncomplete", "StartPlayerTurn", "oncompletetarget", gameObject));
						iTween.RotateTo(cameras[currentWorld].gameObject, iTween.Hash("rotation", cameraList[currentCameraView], "time", 1));
					
						gameHuds.SetActive(true);
						cutsceneHuds.SetActive(false);
		
						cameras[currentWorld].camera.rect = new Rect(0,0,1,1);
						
						gameState = GameState.Null;
					}
				}
			break;
			case GameState.PlayerTurn:
				UpdateTimer();
				if(Input.GetMouseButtonDown(0))
				{
					if(flagMode)
					{
						InstantiateFlag();
					}
					else
					{
						MoveCharacter();
					}
				}
				else if(Input.GetKeyDown(KeyCode.F))
				{
					Victory();
				}
			break;
			case GameState.Moving:
				UpdateTimer();
			break;
		}
	}
	
	void UpdateTimer()
	{
		timeCounter += Time.deltaTime;
		int d = (int)(timeCounter * 100.0f);
	    int minutes = d / (60 * 100);
	    int seconds = (d % (60 * 100)) / 100;
		
		timeLabel.Text = String.Format("{0:00}:{1:00}", minutes, seconds);
	}
	
	void CreateTiles()
	{
		for(int i = 0; i<tileset.Count; i++)
		{
			for(int j = 0; j<tileset[i].Count; j++)
			{
				if(tileset[i][j]==1)
				{
					
				}
				else if(tileset[i][j]==3)
				{
					currentRow = j;
					currentColumn = i;
				}
				else if(tileset[i][j]==2)
				{
					diamondRow = j;
					diamondColumn = i;
				}
			}
		}
		
		previousRow = currentRow;
		previousColum = currentColumn;
		
		//Debug.Log("Row: " + currentRow + ", Column: " + currentColumn);
		character.position = new Vector3(startingPosition.x + currentRow * 3, startingPosition.y, startingPosition.z + currentColumn * -3);
		diamond.position = new Vector3(startingPosition.x + diamondRow * 3, startingPosition.y, startingPosition.z + diamondColumn * -3);
	}
	
	void ChangeCamera()
	{
		if(gameState!=GameState.PlayerTurn) return;
		
		switch(currentCameraView)
		{
			case 0:
				currentCameraView = 1;
			break;
			case 1:
				//Debug.Log(currentRow);
				if(currentRow < 4)
				{
					currentCameraView = 2;
				}
				else
				{
					currentCameraView = 3;
				}
			break;
			case 2:
				currentCameraView = 0;
			break;
			case 3:
				currentCameraView = 0;
			break;
		}

		iTween.MoveTo(cameras[currentWorld].gameObject, iTween.Hash("position", cameraList[currentCameraView], "time", 1,
		"oncomplete", "ChangeCameraParent", "oncompletetarget", gameObject));
		iTween.RotateTo(cameras[currentWorld].gameObject, iTween.Hash("rotation", cameraList[currentCameraView], "time", 1));
	}
	
	void StartPlayerTurn()
	{
		gameState = GameState.PlayerTurn;
		ChangeCameraParent();
	}
	
	void ChangeCameraParent()
	{
		cameras[currentWorld].transform.parent = cameraList[currentCameraView];
	}
	
	void CheckTile()
	{
		if(tileset[currentColumn][currentRow]==1)
		{
			PrepareMine();
		}
		else if(tileset[currentColumn][currentRow]==2)
		{
			Victory();
		}
		else
		{
			if(shieldHp>0) character.animation.CrossFade("standShield");
			else character.animation.CrossFade("stand");
			gameState = GameState.PlayerTurn;
		}
		
		//Debug.Log("Row: " + currentRow + ", Column: " + currentColumn);
	}
	
	void InstantiateFlag()
	{
		Ray ray = cameras[currentWorld].camera.ScreenPointToRay (Input.mousePosition);
	    //Debug.DrawRay (ray.origin, ray.direction * 10, Color.yellow);
		
		RaycastHit hitGUI;
		if (Physics.Raycast (GUICamera.ScreenPointToRay(Input.mousePosition), out hitGUI))
		{
			if(hitGUI.transform.tag == "GUI") return;
		}
		
		RaycastHit[] hits;
		hits = Physics.RaycastAll(cameras[currentWorld].position, ray.direction, Mathf.Infinity);
		
		for(int i = 0; i<hits.Length; i++)
		{
			if(hits[i].transform.tag == "Flag")
			{
				GameObject.Destroy(hits[i].transform.gameObject);
				audio.PlayOneShot(destroyFlagSound, PlayerPrefs.GetFloat(PlayerPrefsKeys.VOLUME));
				/*GameObject.Instantiate(squarePing, new Vector3(hits[i].transform.position.x, startingPosition.y - 1.4f,
					hits[i].transform.position.z), Quaternion.identity);*/
				return;
			}
		}
		
		for(int i = 0; i<hits.Length; i++)
		{
			if(hits[i].transform.tag == "Tile")
			{
				int hitRow = int.Parse(hits[i].transform.name.Substring(1,1)) - 1;
				int hitColumn = int.Parse(hits[i].transform.name.Substring(0,1)) - 1;
				
	            GameObject tempFlag = GameObject.Instantiate(flagPrefab,
					new Vector3(startingPosition.x + hitRow * 3, startingPosition.y, startingPosition.z + hitColumn * -3),
					Quaternion.Euler(new Vector3(0,0,0))) as GameObject;
				tempFlag.GetComponent<LookAt>().target = cameras[currentWorld];
				/*GameObject.Instantiate(radarPing, new Vector3(tempFlag.transform.position.x, startingPosition.y - 1.4f,
					tempFlag.transform.position.z), Quaternion.identity);*/
				
				return;
		    }
		}
	}
	
	void MoveCharacter()
	{
		Ray ray = cameras[currentWorld].camera.ScreenPointToRay (Input.mousePosition);
	    //Debug.DrawRay (ray.origin, ray.direction * 10, Color.yellow);
		
		RaycastHit[] hits;
		hits = Physics.RaycastAll(cameras[currentWorld].position, ray.direction, Mathf.Infinity);
		
		RaycastHit hitGUI;
		if (Physics.Raycast (GUICamera.ScreenPointToRay(Input.mousePosition), out hitGUI))
		{
			if(hitGUI.transform.tag == "GUI") return;
		}
		
		for(int i = 0; i<hits.Length; i++)
		{
			if(hits[i].transform.tag == "Tile")
			{
	            int hitRow = int.Parse(hits[i].transform.name.Substring(1,1)) - 1;
				int hitColumn = int.Parse(hits[i].transform.name.Substring(0,1)) - 1;
				
				if(((hitRow - 1 == currentRow || hitRow + 1 == currentRow) && hitColumn == currentColumn) ||
					((hitColumn - 1 == currentColumn || hitColumn + 1 == currentColumn) && hitRow == currentRow))
				{
					previousRow = currentRow;
					previousColum = currentColumn;
					currentRow = hitRow;
					currentColumn = hitColumn;
					Vector3 nextPosition = new Vector3(startingPosition.x + currentRow * 3, startingPosition.y, startingPosition.z + currentColumn * -3);
					iTween.MoveTo(character.gameObject, iTween.Hash("name", "Character Walk", "position", nextPosition,
						"time", 1, "orienttopath", true, "oncomplete", "CheckTile", "oncompletetarget", gameObject, "easetype", iTween.EaseType.linear));
					gameState = GameState.Moving;
					if(shieldHp>0) character.animation.CrossFade("walkShield");
					else character.animation.CrossFade("walk");
					nextPosition.y = startingPosition.y - 1.4f;
					GameObject.Instantiate(squarePing, nextPosition, Quaternion.identity);
					
					if(currentCameraView == 2 || currentCameraView == 3)
					{
						if(currentRow < 4)
						{
							currentCameraView = 2;
						}
						else
						{
							currentCameraView = 3;
						}
			
						iTween.MoveTo(cameras[currentWorld].gameObject, iTween.Hash("position", cameraList[currentCameraView], "time", 1,
						"oncomplete", "ChangeCameraParent", "oncompletetarget", gameObject));
						iTween.RotateTo(cameras[currentWorld].gameObject, iTween.Hash("rotation", cameraList[currentCameraView], "time", 1));
					}
				}
		    }
				
		}
	}
	
	void PrepareMine()
	{
		GameObject localMine = GameObject.Instantiate(mine, new Vector3(startingPosition.x+(currentRow*3),
			startingPosition.y-1.5f, startingPosition.z-(currentColumn*3)), Quaternion.identity) as GameObject;
		
		mineCoordinates.Add(new Vector2(currentColumn, currentRow));
		
		if(shieldHp>0)
		{
			character.animation.CrossFade("standShield");
			GameObject tempShield = GameObject.Instantiate(blast, character.position, character.rotation) as GameObject;
			Invoke("Defend", 0.1f);
		}
		else
		{
			character.animation.CrossFade("stand");
			Invoke("Explode", 0.1f);
		}
		gameState = GameState.Null;
	}
	
	void Defend()
	{
		currentRow = previousRow;
		currentColumn = previousColum;
		
		iTween.MoveTo(character.gameObject, iTween.Hash("time", 0.1f, "oncomplete", "Respawn", "oncompletetarget", gameObject,
			"position", new Vector3(startingPosition.x + currentRow * 3, startingPosition.y, startingPosition.z + currentColumn * -3)));
	}
	
	void Explode()
	{
		character.gameObject.SetActive(false);
		gameHuds.SetActive(false);
		
		GameObject rag = GameObject.Instantiate(ragdoll, character.position, character.rotation) as GameObject;
		rag.transform.FindChild("Camera").GetComponent<Skybox>().material = cameras[currentWorld].GetComponent<Skybox>().material;
		GameObject.Destroy(rag, respawnTime);
		
		Invoke("Respawn", respawnTime);
		Invoke("FadeIn", respawnTime - (endFade.GetComponent<Fader>().fadeTime+endFade.GetComponent<Fader>().delay));
		
		currentRow = previousRow;
		currentColumn = previousColum;
		
		iTween.MoveTo(character.gameObject, iTween.Hash("time", 1,
			"position", new Vector3(startingPosition.x + currentRow * 3, startingPosition.y, startingPosition.z + currentColumn * -3)));
		
		currentHp--;
		hp[currentHp].SetActive(false);
		if(currentHp==6 || currentHp==4)
		{
			currentHp--;
			hp[currentHp].SetActive(false);
		}
		
		if(currentHp <= 0)
		{
			tommyMaterial.mainTexture = tommyTextures[3];
		}
		else if(currentHp <= 1)
		{
			hat.SetActive(false);
			tommyMaterial.mainTexture = tommyTextures[2];
		}
		else
		{
			tommyMaterial.mainTexture = tommyTextures[1];
		}
	}
	
	void Victory()
	{
		audio.PlayOneShot(victorySound, 1);
		audio.PlayOneShot(experienceSound, 1);
		
		if(Flow.currentCustomStage == -1)
		{
			if(Flow.hpLevel+2==currentHp && Flow.currentGame.level.stars<3) Flow.currentGame.level.stars = 3;
			else if(Flow.hpLevel+1==currentHp && Flow.currentGame.level.stars<2) Flow.currentGame.level.stars = 2;
			else if(Flow.currentGame.level.stars<1) Flow.currentGame.level.stars = 1;
		}
		else
		{
			if(Flow.hpLevel+2==currentHp && Flow.customStages[Flow.currentCustomStage].stars<3) Flow.customStages[Flow.currentCustomStage].stars = 3;
			else if(Flow.hpLevel+1==currentHp && Flow.customStages[Flow.currentCustomStage].stars<2) Flow.customStages[Flow.currentCustomStage].stars = 2;
			else if(Flow.customStages[Flow.currentCustomStage].stars<1) Flow.customStages[Flow.currentCustomStage].stars = 1;
		}
		
		Flow.playerWin = true;
		gameState = GameState.Null;
		
		Flow.currentGame.myRoundList[0].deaths = Flow.hpLevel + 2 - currentHp;
		Flow.currentGame.myRoundList[0].time = Mathf.RoundToInt(timeCounter);
		
		GameObject levelUp = GameObject.Instantiate(levelUpFade) as GameObject;
		
		if(Flow.currentMode == GameMode.SinglePlayer)
		{
			if(Flow.currentCustomStage == -1)
			{	
				if(Save.HasKey(PlayerPrefsKeys.LEVELSTARS+Flow.currentGame.level.id))
				{
					Flow.currentGame.myRoundList[0].expGained = (currentWorld+1) * 10 + (currentLevel+1) * 1;
				}
				else
				{
					Flow.currentGame.myRoundList[0].expGained = (currentWorld+1) * 100 + (currentLevel+1) * 10;
				}
		
				Save.Set(PlayerPrefsKeys.LEVELSTARS+Flow.currentGame.level.id, Flow.currentGame.level.stars, true);
				Save.Set(PlayerPrefsKeys.POINTS, Flow.currentGame.level.id, true);
			}
			else
			{
				Flow.currentGame.myRoundList[0].expGained = 80;
			}
		}
		else
		{
			if(Flow.currentCustomStage == -1)
			{
				if(Save.HasKey(PlayerPrefsKeys.LEVELSTARS+Flow.currentGame.level.id))
				{
					Flow.currentGame.myRoundList[0].expGained = (currentWorld+1) * 10 + (currentLevel+1) * 1;
				}
				else
				{
					Flow.currentGame.myRoundList[0].expGained = (currentWorld+1) * 100 + (currentLevel+1) * 10;
				}
			}
			else
			{
				Flow.currentGame.myRoundList[0].expGained = 30;
			}
		}
		
		//Flow.currentGame.myRoundList[0].expGained = 8008;
		
		Flow.playerExperience += Flow.currentGame.myRoundList[0].expGained;
		levelUp.transform.GetChild(0).GetComponent<SpriteText>().Text = "Exp +" + Flow.currentGame.myRoundList[0].expGained.ToString();
		
		//Debug.Log("Current Exp: " + Flow.playerExperience);
		levelUp.GetComponent<UIInteractivePanel>().BringIn();
		GameObject.Destroy(levelUp.GetComponent<Fader>());
		increasingExp = true;
		
		shield.gameObject.SetActive(false);
		cameras[currentWorld].parent = winCamera;
		cameras[currentWorld].position = winCamera.position;
		cameras[currentWorld].rotation = winCamera.rotation;
		winCamera.animation.Play ();
		GameObject.Destroy(diamond.gameObject);
		character.animation["win"].speed = 3;
		character.animation.CrossFade("win");
		gameHuds.SetActive(false);
	}
	
	void CheckExperience()
	{
		if(Flow.playerExperience >= Flow.playerLevel * Flow.playerLevel * 100)
		{
			if(Save.HasKey(PlayerPrefsKeys.SKILLPOINTS))
			{
				Debug.Log("eu tinha " + Save.GetInt(PlayerPrefsKeys.SKILLPOINTS) + " Skill Points");
				Save.Set(PlayerPrefsKeys.SKILLPOINTS, Save.GetInt(PlayerPrefsKeys.SKILLPOINTS) + 1, true);
			}
			else
			{
				Debug.Log("eu non tinha skillpoints");
				Save.Set(PlayerPrefsKeys.SKILLPOINTS, 1, true);
			}
			
			levelUpPanel.transform.FindChild("Skill Points").GetComponent<SpriteText>().Text = "Unspent Skill Points: " + Save.GetInt(PlayerPrefsKeys.SKILLPOINTS);
			Debug.Log("eu fiquei com " + Save.GetInt(PlayerPrefsKeys.SKILLPOINTS) + " Skill Points");
			
			Flow.playerExperience -= Flow.playerLevel * Flow.playerLevel * 100;
			Flow.playerLevel ++;
			Flow.header.levelText.Text = "Level " + Flow.playerLevel.ToString();
			//Debug.Log("Level Up!\nCurrent Level: " + Flow.playerLevel);
			
			GameObject levelUp = GameObject.Instantiate(levelUpFade) as GameObject;
			levelUp.transform.GetChild(0).GetComponent<SpriteText>().Text = "Level " + Flow.playerLevel.ToString() + "!";
			levelUp.GetComponent<UIInteractivePanel>().BringIn();
			audio.PlayOneShot(levelUpSound, 3);
		}
	}
	
	void LevelUp()
	{	
		Debug.Log("chamei levelup");
		levelUpPanel.BringIn();
		if(Flow.mapLevel < 5 && Flow.radarLevel < 5 && Flow.hpLevel < 5)
		{
			levelUpPanel.transform.FindChild("Skill Points").GetComponent<SpriteText>().Text = "Unspent Skill Points: " + Save.GetInt(PlayerPrefsKeys.SKILLPOINTS);
		}
		else
		{	
			levelUpPanel.transform.FindChild("Skill Points").GetComponent<SpriteText>().Text = "";
		}
	}
	
	public void FadeIn()
	{
		GameObject.Instantiate(endFade, transform.position, transform.rotation);
	}
	
	public void Respawn()
	{
		gameHuds.SetActive(true);
		if(shieldHp>0)
		{
			gameState = GameState.PlayerTurn;
			shieldHp--;
			hp[shieldHp].transform.FindChild("Shield").gameObject.SetActive(false);
			Debug.Log("shieldHP: " + shieldHp);
			if(shieldHp==4 || shieldHp==6)
			{
				shieldHp--;
				hp[shieldHp].transform.FindChild("Shield").gameObject.SetActive(false);
			}
			if(shieldHp<=0)
			{
				shield.gameObject.SetActive(false);
				GameObject tempShield = GameObject.Instantiate(shieldPrefab, character.position, character.rotation) as GameObject;
				Vector3 blastCenter = tempShield.transform.position;
				blastCenter.x += character.forward.x*-1;
				blastCenter.y += UnityEngine.Random.Range(-1,-0.1f);
				blastCenter.z += character.forward.z*-1;
				tempShield.rigidbody.AddExplosionForce(UnityEngine.Random.Range(100,500), tempShield.transform.position, 10);
				GameObject.Destroy(tempShield, 2);
				character.animation.CrossFade("stand");
			}
		}
		else
		{
			if(currentHp <= 0)
			{
				Flow.playerWin = false;
				gameState = GameState.Null;
				tommyMaterial.mainTexture = tommyTextures[0];
				//Flow.currentCustomStage = -1;
				Flow.currentGame.myRoundList[0].deaths = Flow.hpLevel + 2 - currentHp;
				Flow.currentGame.myRoundList[0].time = Mathf.RoundToInt(timeCounter);
				Flow.currentGame.myRoundList[0].expGained = 0;
				Flow.nextPanel = PanelToLoad.EndLevel;
				Application.LoadLevel("Mainmenu");
			}
			else
			{
				character.gameObject.SetActive(true);
				//Debug.Log(character.position);
				gameState = GameState.PlayerTurn;
			}
		}
	}
	
	public void NextLevel()
	{
		if(Flow.currentMode == GameMode.Multiplayer && Flow.currentCustomStage == -1)
		{	
			GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "mines/managegame.php", GameSent);
			WWWForm form = new WWWForm();
			form.AddField("worldID",Flow.currentGame.world.id);
			form.AddField("levelID",Flow.currentGame.level.id);
			form.AddField("friendID", Flow.currentGame.friend.id);
			form.AddField("deaths", Flow.currentGame.myRoundList[0].deaths);
			form.AddField("time", Flow.currentGame.myRoundList[0].time.ToString());
			
			conn.connect(form);
		}
		else if(Flow.currentMode == GameMode.Multiplayer && Flow.currentCustomStage != -1)
		{
			Debug.Log("currentCustomGame (Gameplay) " + Flow.currentCustomGame);
			GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "mines/updatechallenge.php", UpdateChallenge);
			WWWForm form = new WWWForm();
			form.AddField("gameID", Flow.currentCustomGame);
			form.AddField("deaths", Flow.currentGame.myRoundList[0].deaths);
			form.AddField("time", Flow.currentGame.myRoundList[0].time.ToString());
			
			conn.connect(form);
		}
		else if(Flow.currentMode == GameMode.SinglePlayer && Flow.currentCustomStage == -1)
		{
			if(Save.HasKey (PlayerPrefsKeys.FACEBOOK_TOKEN.ToString())) postActionFacebook();
			tommyMaterial.mainTexture = tommyTextures[0];
			Flow.nextPanel = PanelToLoad.EndLevel;
			Application.LoadLevel("Mainmenu");
		}
		else if(Flow.currentMode == GameMode.SinglePlayer && Flow.currentCustomStage != -1)
		{
			//Flow.currentCustomStage = -1;
			Flow.nextPanel = PanelToLoad.EndLevel;
			Application.LoadLevel("Mainmenu");
		}
	}
	
	private void postActionFacebook()
	{
		Debug.Log ("postActionFacebook");
		
		int firstWorld = 9999;
		int firstLevel = 9999;
		foreach(KeyValuePair<int,World> w in Flow.worldDict)
		{
			if(w.Key < firstWorld) firstWorld = w.Key;
		}
		
		firstLevel = 9999;
		foreach(KeyValuePair<int,Level> l in Flow.worldDict[firstWorld].levelDict)
		{
			if(l.Key < firstLevel) firstLevel = l.Key;
		}
		
		GameJsonAuthConnection postFb = new GameJsonAuthConnection (Flow.URL_BASE + "mines/publish_single.php", actionResponse);
		WWWForm form = new WWWForm();
		form.AddField ("world", Flow.currentGame.world.id - firstWorld + 1);
		form.AddField ("level", Flow.currentGame.level.id - firstLevel - (Flow.currentGame.world.id - firstWorld)*9 + 1);
		form.AddField ("time", Mathf.CeilToInt(timeCounter));
		form.AddField ("deaths", Flow.hpLevel + 3 - currentHp);
		
		postFb.connect (form);
	}
	
	private void actionResponse(string error, IJSonObject data)
	{
		if (error != null) Debug.Log ("error: " + error);
		else Debug.Log ("data: " + data);
	}
	
	public void UpdateChallenge(string error, IJSonObject data)
	{
		if(error != null)
		{
			Debug.Log(error);
		}
		else
		{
			Debug.Log(data);
			Debug.Log("current rank: " + Flow.currentRank.id);
			Flow.nextPanel = PanelToLoad.Ranking;
			tommyMaterial.mainTexture = tommyTextures[0];
			Application.LoadLevel("Mainmenu");
		}
	}
	
	public void GameSent(string error, IJSonObject data)
	{	
		if(error != null)
		{
			Debug.Log(error);
		}
		else
		{
			Debug.Log(data);
			Flow.nextPanel = PanelToLoad.BattleStatus;
			tommyMaterial.mainTexture = tommyTextures[0];
			Application.LoadLevel("Mainmenu");
		}
	}
	
	public void BuyRadar()
	{
		Flow.shopManager.BuyItem(BuyItems, Flow.shopManager.GetShopItem("packRadars"));
	}
	
	public void BuyRadarDelegate(string buttonPressed)
	{
		if(buttonPressed.ToLower() == "ok")
		{
			BuyRadar();
		}
	}
	
	public void BuyMap()
	{
		Flow.shopManager.BuyItem(BuyItems, Flow.shopManager.GetShopItem("packMaps"));
	}
	
	public void BuyMapDelegate(string buttonPressed)
	{
		if(buttonPressed.ToLower() == "ok")
		{
			BuyMap();
		}
	}
	
	public void BuyShield()
	{
		Flow.shopManager.BuyItem(BuyItems, Flow.shopManager.GetShopItem("packShields"));
	}
	
	public void BuyShieldDelegate(string buttonPressed)
	{
		if(buttonPressed.ToLower() == "ok")
		{
			BuyShield();
		}
	}
	
	public void GetShield()
	{
		if(gameState!=GameState.PlayerTurn) return;
		
		if(Save.GetInt(PlayerPrefsKeys.ITEM+"shield") > 0)
		{
			Save.Set(PlayerPrefsKeys.ITEM+"shield", Save.GetInt(PlayerPrefsKeys.ITEM+"shield") - 1, true);
			shieldCount.Text = "x"+Save.GetInt(PlayerPrefsKeys.ITEM+"shield").ToString();
		}
		else
		{
			Flow.game_native.showMessageOkCancel(this, "BuyShield", BuyShieldDelegate, "", "Do you want to buy more shields?",
			"You must buy more shields to be able to use them in game", "Ok", "Cancel");
			return;
		}
		
		audio.PlayOneShot(shieldSound, 2);
		
		Debug.Log("currentHP: " + currentHp);
		shieldHp = currentHp;
		if(currentHp==4 || currentHp==6) shieldHp--;
		for(int i = 0; i<shieldHp; i++)
		{
			hp[i].transform.FindChild("Shield").gameObject.SetActive(true);
		}
		character.animation.CrossFade("standShield");
		shield.gameObject.SetActive(true);
		
		shieldBlock.SetActive(true);
	}
	
	public void GetRadar()
	{
		if(gameState!=GameState.PlayerTurn) return;
		
		if(Save.GetInt(PlayerPrefsKeys.ITEM+"radar") > 0)
		{
			Save.Set(PlayerPrefsKeys.ITEM+"radar", Save.GetInt(PlayerPrefsKeys.ITEM+"radar") - 1, true);
			radarCount.Text = "x"+Save.GetInt(PlayerPrefsKeys.ITEM+"radar").ToString();
		}
		else
		{
			Flow.game_native.showMessageOkCancel(this, "BuyRadar", BuyRadarDelegate, "", "Do you want to buy more radars?",
			"You must buy more radars to be able to use them in game", "Ok", "Cancel");
			return;
		}
		
		audio.PlayOneShot(radarSound, 2);
		
		for(int i = 0; i<tileset.Count; i++)
		{
			for(int j = 0; j<tileset[i].Count; j++)
			{
				if(tileset[i][j]==1 && j>currentRow-radarDistance && j<currentRow+radarDistance && i>currentColumn-radarDistance && i<currentColumn+radarDistance)
				{	
					GameObject.Instantiate(radarPing, new Vector3(
						startingPosition.x + j * 3, startingPosition.y - 1.4f, startingPosition.z + i * -3), Quaternion.identity);
				}
			}
		}
		
		radarBlock.SetActive(true);
	}
	
	public void GetMap()
	{
		if(gameState!=GameState.PlayerTurn) return;
		
		if(Save.GetInt(PlayerPrefsKeys.ITEM+"map") > 0)
		{
			Save.Set(PlayerPrefsKeys.ITEM+"map", Save.GetInt(PlayerPrefsKeys.ITEM+"map") - 1, true);
			mapCount.Text = "x"+Save.GetInt(PlayerPrefsKeys.ITEM+"map").ToString();
		}
		else
		{
			Flow.game_native.showMessageOkCancel(this, "BuyMap", BuyMapDelegate, "", "Do you want to buy more maps?",
			"You must buy more maps to be able to use them in game", "Ok", "Cancel");
			return;
		}
		
		audio.PlayOneShot(mapSound, 3);
		
		List<Vector2> hintList = new List<Vector2>();
		for(int i = 0; i<tileset.Count; i++)
		{
			for(int j = 0; j<tileset[i].Count; j++)
			{
				if(tileset[i][j]==1)
				{
					bool okayToAdd = true;
					foreach(Vector2 v2 in mineCoordinates)
					{
						if(v2.x == i && v2.y == j)
						{
							okayToAdd = false;
						}
					}
					if(okayToAdd)
					{
						hintList.Add(new Vector2(i,j));
					}
				}
			}
		}
		
		Shuffle.Shuffle(hintList);
		
		for(int i = 0; i < mapedMines; i++)
		{
			if(hintList.Count>i)
			{
				Vector3 mapPosition = new Vector3(startingPosition.x+(hintList[i].y*3), startingPosition.y-1.5f, startingPosition.z-(hintList[i].x*3));
				
				GameObject.Instantiate(mine, mapPosition, Quaternion.identity);
				mapPosition.y = startingPosition.y - 1.4f;
				GameObject.Instantiate(radarPing, mapPosition, Quaternion.identity);
				
				mineCoordinates.Add(hintList[i]);
				
				//Debug.Log(hintList[i]);
			}
		}
		
		mapBlock.SetActive(true);
	}
	
	void BuyItems(ShopResultStatus status, string x)
	{
		Debug.Log(status);
		mapCount.Text = "x"+Save.GetInt(PlayerPrefsKeys.ITEM+"map").ToString();
		radarCount.Text = "x"+Save.GetInt(PlayerPrefsKeys.ITEM+"radar").ToString();
		shieldCount.Text = "x"+Save.GetInt(PlayerPrefsKeys.ITEM+"shield").ToString();
		Flow.messageOkCancelDialog.SetActive(false);
		Flow.game_native.showMessage("Item Received", "You received 10 units of this item to use in game", "ok");
	}
	
	void ConfirmUpgrade()
	{
		string upgradeText = "";
		
		switch(currentUpgrade)
		{
			case "hp":
				if(Flow.hpLevel>=5)
				{
					Flow.game_native.showMessage("HP is already Maximum", "Choose another upgrade", "Ok");
					return;
				}
				else
				{
					audio.PlayOneShot(upgradeSound, 1);
					Flow.hpLevel++;
					if(Flow.hpLevel < 5)
					{
						upgradesDescription.transform.parent.FindChild("HPButton").FindChild("Level").GetComponent<SpriteText>().Text = Flow.hpLevel.ToString();
					}
					else
					{
						upgradesDescription.transform.parent.FindChild("HPButton").FindChild("Level").GetComponent<SpriteText>().Text = "Max.";
					}
					upgradeText = "HP +1!";
					Save.Set(PlayerPrefsKeys.HPLEVEL, Flow.hpLevel, true);
					hp[Flow.hpLevel+1].SetActive(true);
				}
			break;
			case "map":
				if(Flow.mapLevel>=5)
				{
					Flow.game_native.showMessage("Map is already Maximum", "Choose another upgrade", "Ok");
					return;
				}
				else
				{
					audio.PlayOneShot(upgradeSound, 1);
					Flow.mapLevel++;
					if(Flow.mapLevel < 5)
					{
						upgradesDescription.transform.parent.FindChild("MapButton").FindChild("Level").GetComponent<SpriteText>().Text = Flow.mapLevel.ToString();
					}
					else
					{
						upgradesDescription.transform.parent.FindChild("MapButton").FindChild("Level").GetComponent<SpriteText>().Text = "Max.";
					}
					upgradeText = "Map +1!";
					Save.Set(PlayerPrefsKeys.MAPLEVEL, Flow.mapLevel, true);
				}
			break;
			case "radar":
				if(Flow.radarLevel>=5)
				{
					Flow.game_native.showMessage("Radar is already Maximum", "Choose another upgrade", "Ok");
					return;
				}
				else
				{
					audio.PlayOneShot(upgradeSound, 1);
					Flow.radarLevel++;
					if(Flow.radarLevel < 5)
					{
						upgradesDescription.transform.parent.FindChild("RadarButton").FindChild("Level").GetComponent<SpriteText>().Text = Flow.radarLevel.ToString();
					}
					else
					{
						upgradesDescription.transform.parent.FindChild("RadarButton").FindChild("Level").GetComponent<SpriteText>().Text = "Max.";
					}
					upgradeText = "Radar +1!";
					Save.Set(PlayerPrefsKeys.RADARLEVEL, Flow.radarLevel, true);
				}
			break;
			case "max":
				Invoke("NextLevel", 1.7f);
				FadeIn();
			return;
		}
		
		Debug.Log("eu tinha " + Save.GetInt(PlayerPrefsKeys.SKILLPOINTS) + " Skill Points");
		
		Save.Set(PlayerPrefsKeys.SKILLPOINTS, Save.GetInt(PlayerPrefsKeys.SKILLPOINTS) - 1, true);
		levelUpPanel.transform.FindChild("Skill Points").GetComponent<SpriteText>().Text = "Unspent Skill Points: " + Save.GetInt(PlayerPrefsKeys.SKILLPOINTS);
		
		GameObject levelUp = GameObject.Instantiate(levelUpFade) as GameObject;
		levelUp.transform.GetChild(0).GetComponent<SpriteText>().Text = upgradeText;
		levelUp.GetComponent<UIInteractivePanel>().BringIn();
		
		if(Save.HasKey(PlayerPrefsKeys.SKILLPOINTS))
		{
			Debug.Log("eu fiquei com " + Save.GetInt(PlayerPrefsKeys.SKILLPOINTS) + " Skill Points");
			if(Save.GetInt(PlayerPrefsKeys.SKILLPOINTS) == 0)
			{	
				Invoke("NextLevel", 5.7f);
				Invoke("FadeIn", 4);
				levelUpPanel.Dismiss();
			}
		}
	}
	
	void UpgradeHP()
	{
		upgradesDescription.Text = "Increases the character's and the shield's HP.";
		currentUpgrade = "hp";
		if(Flow.hpLevel >= 5)
		{
			Flow.game_native.showMessage("HP is already Maximum", "Your HP cannot be increased anymore", "Ok");
			if(Flow.mapLevel >= 5 && Flow.radarLevel >= 5)
			{
				upgradesDescription.Text = "All upgrades have been bought.";
				currentUpgrade = "max";
			}
		}
	}
	
	void UpgradeMap()
	{
		upgradesDescription.Text = "Increases the number of revealed mines on the map.";
		currentUpgrade = "map";
		if(Flow.mapLevel >= 5)
		{
			Flow.game_native.showMessage("Map is already Maximum", "Your Map cannot be increased anymore", "Ok");
			if(Flow.hpLevel >= 5 && Flow.radarLevel >= 5)
			{
				upgradesDescription.Text = "All upgrades have been bought.";
				currentUpgrade = "max";
			}
		}
	}
	
	void UpgradeRadar()
	{
		upgradesDescription.Text = "Increases the range of the radar.";
		currentUpgrade = "radar";
		if(Flow.radarLevel >= 5)
		{
			Flow.game_native.showMessage("Radar is already Maximum", "Your Radar cannot be increased anymore", "Ok");
			if(Flow.mapLevel >= 5 && Flow.hpLevel >= 5)
			{
				upgradesDescription.Text = "All upgrades have been bought.";
				currentUpgrade = "max";
			}
		}
	}
	
	void SkipCutscene()
	{
		//Debug.Log("Skip");
		
		iTween.MoveTo(cameras[currentWorld].gameObject, iTween.Hash("position", cameraList[currentCameraView], "time", 1,
		"oncomplete", "StartPlayerTurn", "oncompletetarget", gameObject));
		iTween.RotateTo(cameras[currentWorld].gameObject, iTween.Hash("rotation", cameraList[currentCameraView], "time", 1));
	
		gameHuds.SetActive(true);
		cutsceneHuds.SetActive(false);

		cameras[currentWorld].camera.rect = new Rect(0,0,1,1);
		
		gameState = GameState.Null;
		
		ChangeCamera();
	}
}