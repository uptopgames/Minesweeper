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
	
	public GameObject flagPrefab;
	public GameObject mine;
	public GameObject ragdoll;
	public GameObject blast;
	public GameObject shieldPrefab;
	public GameObject endFade;
	public GameObject radarPing;
	public GameObject levelUpFade;
	
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
	
	string fileName = "tileset.xml";
	TextAsset tilesetXML;
	string rawXML;
	private List<List<List<int>>> xmlList;
	
	void Start()
	{
		Flow.header.levelText.Text = "Level " + Flow.playerLevel.ToString();
		Flow.header.experienceText.Text = "Exp " + Flow.playerExperience.ToString();
		Flow.header.expBar.width = 7 * Flow.playerExperience/(Flow.playerLevel * Flow.playerLevel * 100);
		Flow.header.expBar.CalcSize();
		
		mapedMines = Flow.mapLevel;
		radarDistance = Flow.radarLevel + 1;
		currentHp = Flow.hpLevel + 2;
		
		upgradesDescription.transform.parent.FindChild("HPButton").FindChild("Level").GetComponent<SpriteText>().Text = Flow.hpLevel.ToString();
		upgradesDescription.transform.parent.FindChild("MapButton").FindChild("Level").GetComponent<SpriteText>().Text = Flow.mapLevel.ToString();
		upgradesDescription.transform.parent.FindChild("RadarButton").FindChild("Level").GetComponent<SpriteText>().Text = Flow.radarLevel.ToString();
		
		for(int i = 0; i<Flow.hpLevel+2; i++)
		{
			hp[i].SetActive(true);
		}
		
		if(Flow.currentCustomStage != -1)
		{
			currentWorld = Flow.customStages[Flow.currentCustomStage].world;
			currentLevel = 0;
			
			tileset = Flow.customStages[Flow.currentCustomStage].tileset;
			
			RealStart();
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
		Flow.currentGame.myRoundList.Add(new Round(-1,-1,-1,-1,-1));
		
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
		testButtons.SetActive(false);
		
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
		//Debug.Log("Previous Row: " + previousRow + ", Previous Column: " + previousColum);
		
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
						gameState = GameState.Null;
					}
				}
			break;
			case GameState.PlayerTurn:
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
		}
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
		
		Debug.Log("Row: " + currentRow + ", Column: " + currentColumn);
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
				Debug.Log(currentRow);
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
				/*GameObject.Instantiate(radarPing, new Vector3(hits[i].transform.position.x, startingPosition.y - 1.4f,
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
					//GameObject.Instantiate(radarPing, nextPosition, Quaternion.identity);
					
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
		GameObject levelUp = GameObject.Instantiate(levelUpFade) as GameObject;
		
		if(Flow.currentMode == GameMode.SinglePlayer)
		{
			if(Flow.currentCustomStage == -1)
			{	
				if(Save.HasKey(PlayerPrefsKeys.LEVELSTARS+Flow.currentGame.level.id))
				{
					Flow.playerExperience += (currentWorld+1) * 10 + (currentLevel+1) * 1;
					levelUp.transform.GetChild(0).GetComponent<SpriteText>().Text = "Exp +" + ((currentWorld+1) * 10 + (currentLevel+1) * 1).ToString();
				}
				else
				{
					Flow.playerExperience += (currentWorld+1) * 100 + (currentLevel+1) * 10;
					levelUp.transform.GetChild(0).GetComponent<SpriteText>().Text = "Exp +" + ((currentWorld+1) * 100 + (currentLevel+1) * 10).ToString();
				}
		
				Save.Set(PlayerPrefsKeys.LEVELSTARS+Flow.currentGame.level.id, Flow.currentGame.level.stars, true);
				Save.Set(PlayerPrefsKeys.POINTS, Flow.currentGame.level.id, true);
			}
			else
			{
				Flow.currentCustomStage = -1;
				Flow.playerExperience += 80;
				levelUp.transform.GetChild(0).GetComponent<SpriteText>().Text = "Exp +80";
			}
		}
		else
		{
			if(Flow.currentCustomStage == -1)
			{
				if(Save.HasKey(PlayerPrefsKeys.LEVELSTARS+Flow.currentGame.level.id))
				{
					Flow.playerExperience += (currentWorld+1) * 10 + (currentLevel+1) * 1;
					levelUp.transform.GetChild(0).GetComponent<SpriteText>().Text = "Exp +" + ((currentWorld+1) * 10 + (currentLevel+1) * 1).ToString();
				}
				else
				{
					Flow.playerExperience += (currentWorld+1) * 100 + (currentLevel+1) * 10;
					levelUp.transform.GetChild(0).GetComponent<SpriteText>().Text = "Exp +" + ((currentWorld+1) * 100 + (currentLevel+1) * 10).ToString();
				}
			}
			else
			{
				Flow.currentCustomStage = -1;
				Flow.playerExperience += 30;
				levelUp.transform.GetChild(0).GetComponent<SpriteText>().Text = "Exp +30";
			}
		}
		
		if(Flow.hpLevel+2==currentHp && Flow.currentGame.level.stars<3) Flow.currentGame.level.stars = 3;
		else if(Flow.hpLevel+1==currentHp && Flow.currentGame.level.stars<2) Flow.currentGame.level.stars = 2;
		else if(Flow.currentGame.level.stars<1) Flow.currentGame.level.stars = 1;
		
		Debug.Log("Current Exp: " + Flow.playerExperience);
		levelUp.GetComponent<UIInteractivePanel>().BringIn();
		GameObject.Destroy(levelUp.GetComponent<Fader>());
		
		if(Flow.playerExperience >= Flow.playerLevel * Flow.playerLevel * 100)
		{
			CheckExperience();
		}
		
		Debug.Log("salvei level " + Flow.playerLevel + " na key " + PlayerPrefsKeys.PLAYERLEVEL);
		
		Save.Set(PlayerPrefsKeys.PLAYERLEVEL, Flow.playerLevel, true);
		Save.Set(PlayerPrefsKeys.PLAYEREXPERIENCE, Flow.playerExperience, true);
		
		Flow.header.experienceText.Text = "Exp " + Flow.playerExperience.ToString();
		Flow.header.expBar.width = 7 * Flow.playerExperience/(Flow.playerLevel * Flow.playerLevel * 100);
		Flow.header.expBar.CalcSize();
		
		shield.gameObject.SetActive(false);
		cameras[currentWorld].parent = winCamera;
		cameras[currentWorld].position = winCamera.position;
		cameras[currentWorld].rotation = winCamera.rotation;
		winCamera.animation.Play ();
		GameObject.Destroy(diamond.gameObject);
		character.animation.CrossFade("win");
		
		if(!leveledUp)
		{
			Invoke("NextLevel", 6.7f);
			Invoke("FadeIn", 5f);
		}
	}
	
	void CheckExperience()
	{
		if(Flow.playerExperience >= Flow.playerLevel * Flow.playerLevel * 100)
		{
			Flow.playerExperience -= Flow.playerLevel * Flow.playerLevel * 100;
			Flow.playerLevel ++;
			Flow.header.levelText.Text = "Level " + Flow.playerLevel.ToString();
			Debug.Log("Level Up!\nCurrent Level: " + Flow.playerLevel);
			if(Flow.playerExperience >= Flow.playerLevel * Flow.playerLevel * 100)
			{
				CheckExperience();
			}
			else
			{
				Invoke("LevelUp", 3);
				leveledUp = true;
			}
		}
	}
	
	void LevelUp()
	{
		GameObject levelUp = GameObject.Instantiate(levelUpFade) as GameObject;
		levelUp.transform.GetChild(0).GetComponent<SpriteText>().Text = "Level " + Flow.playerLevel.ToString() + "!";
		levelUp.GetComponent<UIInteractivePanel>().BringIn();
		
		levelUpPanel.BringIn();
	}
	
	public void FadeIn()
	{
		GameObject.Instantiate(endFade, transform.position, transform.rotation);
	}
	
	public void Respawn()
	{
		if(shieldHp>0)
		{
			gameState = GameState.PlayerTurn;
			shieldHp--;
			hp[shieldHp].transform.FindChild("Shield").gameObject.SetActive(false);
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
				tommyMaterial.mainTexture = tommyTextures[0];
				Application.LoadLevel(Application.loadedLevel);
				Flow.currentCustomStage = -1;
			}
			else
			{
				character.gameObject.SetActive(true);
				Debug.Log(character.position);
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
		else if(Flow.currentMode == GameMode.SinglePlayer && Flow.currentCustomStage == -1)
		{
			tommyMaterial.mainTexture = tommyTextures[0];
			Application.LoadLevel("Mainmenu");
		}
		else if(Flow.currentMode == GameMode.SinglePlayer && Flow.currentCustomStage != -1)
		{
			Flow.currentCustomStage = -1;
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
	
	public void GetShield()
	{
		if(gameState!=GameState.PlayerTurn) return;
		
		shieldHp = currentHp;
		for(int i = 0; i<currentHp; i++)
		{
			hp[i].transform.FindChild("Shield").gameObject.SetActive(true);
		}
		character.animation.CrossFade("standShield");
		shield.gameObject.SetActive(true);
	}
	
	public void GetRadar()
	{
		if(gameState!=GameState.PlayerTurn) return;
		
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
	}
	
	public void GetMap()
	{
		if(gameState!=GameState.PlayerTurn) return;
		
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
				
				Debug.Log(hintList[i]);
			}
		}
	}
	
	void ConfirmUpgrade()
	{
		GameObject levelUp = GameObject.Instantiate(levelUpFade) as GameObject;
		string upgradeText = "";
		levelUp.GetComponent<UIInteractivePanel>().BringIn();
		
		switch(currentUpgrade)
		{
			case "hp":
				Flow.hpLevel++;
				upgradesDescription.transform.parent.FindChild("HPButton").FindChild("Level").GetComponent<SpriteText>().Text = Flow.hpLevel.ToString();
				upgradeText = "HP +1!";
				Save.Set(PlayerPrefsKeys.HPLEVEL, Flow.hpLevel, true);
			break;
			case "map":
				Flow.mapLevel++;
				upgradesDescription.transform.parent.FindChild("MapButton").FindChild("Level").GetComponent<SpriteText>().Text = Flow.mapLevel.ToString();
				upgradeText = "Map +1!";
				Save.Set(PlayerPrefsKeys.MAPLEVEL, Flow.mapLevel, true);
			break;
			case "radar":
				Flow.radarLevel++;
				upgradesDescription.transform.parent.FindChild("RadarButton").FindChild("Level").GetComponent<SpriteText>().Text = Flow.radarLevel.ToString();
				upgradeText = "Radar +1!";
				Save.Set(PlayerPrefsKeys.RADARLEVEL, Flow.radarLevel, true);
			break;
		}
		
		levelUp.transform.GetChild(0).GetComponent<SpriteText>().Text = upgradeText;
		
		Invoke("NextLevel", 5.7f);
		Invoke("FadeIn", 4);
		levelUpPanel.Dismiss();
	}
	
	void UpgradeHP()
	{
		upgradesDescription.Text = "Increases the character's and the shield's HP.";
		currentUpgrade = "hp";
	}
	
	void UpgradeMap()
	{
		upgradesDescription.Text = "Increases the number of revealed mines on the map.";
		currentUpgrade = "map";
	}
	
	void UpgradeRadar()
	{
		upgradesDescription.Text = "Increases the range of the radar.";
		currentUpgrade = "radar";
	}
}