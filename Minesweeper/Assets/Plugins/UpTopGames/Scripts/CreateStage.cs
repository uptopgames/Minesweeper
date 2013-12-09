using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CreateStage : MonoBehaviour
{
	public Camera cam;
	public Camera GUICamera;
	public GameObject minePrefab;
	public GameObject player;
	public GameObject diamond;
	public SpriteText minesCounter;
	public Transform mineManager;
	
	private Vector3 startingPosition;
	private int totalMines = 0;
	private int maximumMines = 12;
	
	public int currentWorld = 0;
	
	public PackedSprite expBar;
	public SpriteText levelText;
	public SpriteText experienceText;
	public GameObject inputName;
	
	public AudioClip createMineAudio;
	public AudioClip setPlayerAudio;
	public AudioClip setDiamondAudio;
	public AudioClip stageApprovedAudio;
	public AudioClip stageDeniedAudio;
	
	public enum InsertType
	{
		Mine, Player, Diamond, Null
	}
	public InsertType insertType = InsertType.Mine;
	
	public List<List<int>> tileset = new List<List<int>>()
	{
		new List<int>(){3,0,0,0,0,0,0,0},
		new List<int>(){0,0,0,0,0,0,0,0},
		new List<int>(){0,0,0,0,0,0,0,0},
		new List<int>(){0,0,0,0,0,0,0,0},
		new List<int>(){0,0,0,0,0,0,0,0},
		new List<int>(){0,0,0,0,0,0,0,0},
		new List<int>(){0,0,0,0,0,0,0,0},
		new List<int>(){0,0,0,0,0,0,0,2},
	};
	
	void Start ()
	{
		maximumMines = 10 + Flow.playerLevel * 3;
		if(maximumMines > 62) maximumMines = 62;
		minesCounter.Text = "Mines: " + totalMines.ToString() + "/" + maximumMines.ToString();
		startingPosition = player.transform.position;
		
		Flow.header.levelText.Text = "Level " + Flow.playerLevel.ToString();
		Flow.header.experienceText.Text = "Exp " + Flow.playerExperience.ToString();
		Flow.header.expBar.width = 8 * Flow.playerExperience/(Flow.playerLevel * Flow.playerLevel * 100);
		Flow.header.expBar.CalcSize();
		
		transform.GetChild(0).gameObject.SetActive(true);
		foreach(Transform child in mineManager) child.GetComponent<MineCounter>().UpdateTextEditor();
	}
	
	void Update ()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Insert();
		}
	}
	
	void Insert()
	{
		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
	    //Debug.DrawRay (ray.origin, ray.direction * 10, Color.yellow);
		
		RaycastHit hitGUI;
		if (Physics.Raycast (GUICamera.ScreenPointToRay(Input.mousePosition), out hitGUI))
		{
			if(hitGUI.transform.tag == "GUI") return;
		}
		
		RaycastHit[] hits;
		hits = Physics.RaycastAll(cam.transform.position, ray.direction, Mathf.Infinity);
		
		for(int i = 0; i<hits.Length; i++)
		{
			switch(hits[i].transform.tag)
			{
				case "Mine":
					audio.PlayOneShot(createMineAudio);
					GameObject.Destroy(hits[i].transform.gameObject);
					totalMines--;
					minesCounter.Text = "Mines: " + totalMines.ToString() + "/" + maximumMines.ToString();
				
					tileset[Mathf.RoundToInt((hits[i].transform.position.z - startingPosition.z)/3) * -1]
						[Mathf.RoundToInt((hits[i].transform.position.x - startingPosition.x)/3)] = 0;
				
					Debug.Log("posição " + Mathf.RoundToInt((hits[i].transform.position.x - startingPosition.x)/3) + ", " +
							(Mathf.RoundToInt((hits[i].transform.position.z - startingPosition.z)/3) * -1) + ": " + "0");
				
					foreach(Transform child in mineManager) child.GetComponent<MineCounter>().UpdateTextEditor();
				return;
				case "Player":
				return;
				case "Diamond":
				return;
			}
		}
		
		for(int i = 0; i<hits.Length; i++)
		{
			if(hits[i].transform.tag == "Tile")
			{
				int hitColumn = int.Parse(hits[i].transform.name.Substring(1,1)) - 1;
				int hitRow = int.Parse(hits[i].transform.name.Substring(0,1)) - 1;
				Vector3 insertPosition = new Vector3(startingPosition.x + hitColumn * 3, startingPosition.y, startingPosition.z + hitRow * -3);
				switch(insertType)
				{
					case InsertType.Mine:
						if(totalMines<maximumMines)
						{
							audio.PlayOneShot(createMineAudio);
							GameObject tempFlag = GameObject.Instantiate(minePrefab, insertPosition, minePrefab.transform.rotation) as GameObject;
							totalMines++;
							minesCounter.Text = "Mines: " + totalMines.ToString() + "/" + maximumMines.ToString();
							tileset[hitRow][hitColumn] = 1;
							Debug.Log("posição " + hitColumn + ", " + hitRow + ": " + "1");
							foreach(Transform child in mineManager) child.GetComponent<MineCounter>().UpdateTextEditor();
						}
						return;
					break;
					case InsertType.Player:
						audio.PlayOneShot(setPlayerAudio);
						player.SetActive(true);
						player.transform.position = insertPosition;
						for(int j = 0; j<tileset.Count; j++)
						{
							for(int k = 0; k<tileset[j].Count; k++)
							{
								if(tileset[j][k] == 3)
								{
									tileset[j][k] = 0;
									Debug.Log("posição " + k + ", " + j + ": " + "0");
								}
							}
						}
						tileset[hitRow][hitColumn] = 3;
						Debug.Log("posição " + hitColumn + ", " + hitRow + ": " + "3");
						foreach(Transform child in mineManager) child.GetComponent<MineCounter>().UpdateTextEditor();
						return;
					break;
					case InsertType.Diamond:
						audio.PlayOneShot(setDiamondAudio);
						diamond.SetActive(true);
						diamond.transform.position = insertPosition;
						for(int j = 0; j<tileset.Count; j++)
						{
							for(int k = 0; k<tileset[j].Count; k++)
							{
								if(tileset[j][k] == 2)
								{
									tileset[j][k] = 0;
									Debug.Log("posição " + k + ", " + j + ": " + "0");
								}
							}
						}
						tileset[hitRow][hitColumn] = 2;
						Debug.Log("posição " + hitColumn + ", " + hitRow + ": " + "2");
						foreach(Transform child in mineManager) child.GetComponent<MineCounter>().UpdateTextEditor();
						return;
					break;
				}
				return;
		    }
		}
	}
	
	void SelectMine()
	{
		if(insertType == InsertType.Mine) insertType = InsertType.Null;
		else insertType = InsertType.Mine;
	}
	
	void SelectPlayer()
	{
		if(insertType == InsertType.Player) insertType = InsertType.Null;
		else insertType = InsertType.Player;
	}
	
	void SelectDiamond()
	{
		if(insertType == InsertType.Diamond) insertType = InsertType.Null;
		else insertType = InsertType.Diamond;
	}
	
	public void SaveStage()
	{
		string debugger = "Tileset:";
		foreach(List<int> tile in tileset)
		{
			debugger += "\n";
			for(int i = 0; i<tile.Count; i++)
			{
				debugger += tileset[tileset.IndexOf(tile)][i]+", ";
			}
		}
		Debug.Log(debugger);
		
		GetComponent<Pathfinder>().CheckStage(tileset);
		//começar loading
	}
	
	public void StageApproved()
	{
		audio.PlayOneShot(stageApprovedAudio);
		//terminar loading e lançar janela de level aprovado, com callback das linhas abaixo
		Flow.AddCustomStage(tileset, currentWorld, totalMines, inputName.GetComponent<UITextField>().Text, -1);
		
		Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(false);
		
		Flow.nextPanel = PanelToLoad.CustomStages;
		
		Application.LoadLevel("Mainmenu");
	}
	
	public void StageDenied()
	{
		//Flow.game_native.addActionShowMessage(ResetLevel);
		audio.PlayOneShot(stageDeniedAudio);
		Flow.game_native.showMessage("You created an impossible level",
			"Please remove the mines that prevent the character from reaching the treasure", "Ok");
	}
			
	/*public void ResetLevel(string button)
	{
		Flow.game_native.removeActionShowMessage(ResetLevel);
		
		Application.LoadLevel(Application.loadedLevel);
	}*/
	
	public void OpenNamePanel()
	{
		inputName.SetActive(true);
	}
	
	public void CloseNamePanel()
	{
		inputName.SetActive(false);
	}
	
	public void Back()
	{
		Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(false);
		
		Flow.nextPanel = PanelToLoad.CustomStages;
		
		Application.LoadLevel("Mainmenu");
	}
}