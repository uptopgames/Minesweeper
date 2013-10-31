using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GunSelection : MonoBehaviour 
{
	//public GameObject[] guns = new GameObject[5];
	public GameObject buyGun, buyBullets, infiniteTexture, OKbutton, backButton, coins;
	public SpriteText bulletsText, gunreactionText, gunPrice, gunName, gunDescription;
	public UIScrollList scrollGuns;
	//private string[] descriptionOfGuns = new string[5];
	
	public Guns gunsHolder;
	
	//public GameObject messageOkDialog;
	
	private int selectedGun;

	// Use this for initialization
	void Start () 
	{
		
		//descriptionOfGuns[0] = "Old Pistol: This pistol was adopted by Colonel Lee in the Civil War.";
		//descriptionOfGuns[1] = "Bounty Killer: The perfect gun to hide under your poncho.";
		//descriptionOfGuns[2] = "Pacificador: The perfect gun if you are a lawman.";
		//descriptionOfGuns[3] = "Stampede: The bounty for the head of this pistol's owner is the biggest ever.";
		//descriptionOfGuns[4] = "Gentleman: The ultimate gun for ultimate men.";
		
		// Up Top Fix Me TESTE ///
		//Save.Set ("gun4", 1);
		//Save.Set ("gun5", 1);
		//Save.Set ("gun5_bullets", 35);
		
		// Up Top Fix Me --> colocar esse delegate no delegate de transicao do painel pois esse start eh chamado no inicio da cena e nao do painel
		
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(EnteredGunSelection);
		GetComponent<UIInteractivePanel>().transitions.list[1].AddTransitionStartDelegate(EnteredGunSelection);
		
		scrollGuns.AddItemSnappedDelegate (SetGun);
		
		
	}
	
	void EnteredGunSelection(EZTransition transition)
	{
		Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(false);
		
		if (Flow.currentMode == GameMode.SinglePlayer) 
		{
			backButton.gameObject.SetActive(true);
		}
		else if(Flow.currentMode == GameMode.Multiplayer)
		{
			backButton.gameObject.SetActive(false);
			Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(false);
		}
		
		//Debug.Log ("selectedGun: " + Save.HasKey ("selectedGun"));
		
		if (!Save.HasKey ("selectedGun") || Save.GetInt("selectedGun") == 1)
		{
			Debug.Log ("comeceiAqui");
			selectedGun = 1;
			scrollGuns.ScrollToItem(0,0);
			Gun1Selected();
		}
		else
		{
			selectedGun = Save.GetInt ("selectedGun");
			Debug.Log("arminha selecionada eh essa aqui, o: "+selectedGun);
			scrollGuns.ScrollToItem(selectedGun-1,0);
			
			if (Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + selectedGun + "_bullets") == 0) 
			{
				UserDontHaveBullets();
			}
			else 
			{
				UserHaveGunAndBullets();
			}
		}
		
		gunreactionText.Text = "Gun Reaction: " + gunsHolder.guns[selectedGun-1].reaction.ToString() + " s";
		gunDescription.Text = gunsHolder.guns[selectedGun-1].description;//descriptionOfGuns[selectedGun-1];
		gunName.Text = gunsHolder.guns[selectedGun-1].name;
	}
	
	void SetGun (IUIListObject item) 
	{
		Debug.Log("SetGun ------------");
		selectedGun = item.Index + 1;
		
		if (selectedGun == 1) 
		{
			Gun1Selected();
		}
		else if (Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + selectedGun) == 0)
		{
			UserDontHaveGun();
		}
		
		else if (Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + selectedGun) == 1 && Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + selectedGun + "_bullets") == 0)
		{
			UserDontHaveBullets();
		}
		else
		{
			UserHaveGunAndBullets();
		}
		
		gunreactionText.Text = "Gun Reaction: " +  gunsHolder.guns[selectedGun-1].reaction.ToString() + " s";
		gunDescription.Text = gunsHolder.guns[selectedGun-1].description;//descriptionOfGuns[selectedGun-1];
		gunName.Text = gunsHolder.guns[selectedGun-1].name;
	}
	
	void Gun1Selected()
	{
		infiniteTexture.SetActive(true);
		buyGun.SetActive(false);
		buyBullets.SetActive(false);
		OKbutton.SetActive(true);
		bulletsText.Text = "";
		gunPrice.Text = "";
		coins.SetActive(false);
	}
	
	void UserDontHaveGun()
	{
		coins.SetActive(true);
		infiniteTexture.SetActive(false);
		buyGun.SetActive(true);
		buyBullets.SetActive(false);
		OKbutton.SetActive(false);
		bulletsText.Text = Save.HasKey(PlayerPrefsKeys.ITEM+"gun"+selectedGun+"_bullets") ? Save.GetInt(PlayerPrefsKeys.ITEM+"gun"+selectedGun+"_bullets").ToString() : "0";
		
		//if (Flow.currentMode == GameMode.SinglePlayer)
			//gunPrice.Text = "$" + ServerSettings.GetString ("GUN" + selectedGun + "_PRICE_SINGLE");
		//else
			//gunPrice.Text = "$" + ServerSettings.GetString ("GUN" + selectedGun + "_PRICE_MULTI");
		
		gunPrice.Text = Flow.shopManager.GetShopItem("pack_gun"+selectedGun).coinPrice.ToString();
		
	}
	
	void UserDontHaveBullets()
	{
		infiniteTexture.SetActive(false);
		buyGun.SetActive(false);
		buyBullets.SetActive(true);
		OKbutton.SetActive(false);
		bulletsText.Text = "0";
		gunPrice.Text = Flow.shopManager.GetShopItem("pack_bullets"+selectedGun).coinPrice.ToString();
		coins.SetActive(true);
	}
	
	void UserHaveGunAndBullets()
	{
		infiniteTexture.SetActive(false);
		buyGun.SetActive(false);
		buyBullets.SetActive(false);
		OKbutton.SetActive(true);
		bulletsText.Text = Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + selectedGun + "_bullets").ToString();
		gunPrice.Text = "";
		coins.SetActive(false);
	}
	
	void NextGun()
	{
		infiniteTexture.SetActive(false);
		
		Debug.Log ("NextGun");
		if (selectedGun == 5) 
		{
			Debug.Log ("lastGun");	
		}
		else
		{
			//guns[selectedGun-1].SetActive(false);
			selectedGun++;
			//guns[selectedGun-1].SetActive(true);
			scrollGuns.ScrollToItem(scrollGuns.GetItem(selectedGun-1), 0.5f);
			Debug.Log(scrollGuns.GetItem(selectedGun-1).transform.name);
			Debug.Log ("nextSelectedGun: " + selectedGun);
			
			if (Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + selectedGun) == 0)
			{
				UserDontHaveGun();
			}
			else if (Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + selectedGun) == 1 && Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + selectedGun + "_bullets") == 0)
			{
				UserDontHaveBullets();
			}
			else
			{
				UserHaveGunAndBullets();
			}
			
			Debug.Log ("selectedGun");
		}
		
		gunreactionText.Text = "Gun Reaction: " + gunsHolder.guns[selectedGun-1].reaction.ToString() + " s";
		gunDescription.Text = gunsHolder.guns[selectedGun-1].description;//descriptionOfGuns[selectedGun-1];
		gunName.Text = gunsHolder.guns[selectedGun-1].name;
	}
	
	void PreviousGun()
	{
		Debug.Log ("PreviousGun");
		if (selectedGun == 1)
		{
			Debug.Log ("firstGun");
			
		}
		else
		{
			//guns[selectedGun-1].SetActive(false);
			selectedGun--;
			//guns[selectedGun-1].SetActive(true);
			scrollGuns.ScrollToItem(scrollGuns.GetItem(selectedGun-1), 0.5f);
			Debug.Log ("PreviouSelectedGun:" + selectedGun);
			
			if (selectedGun == 1)
			{
				Gun1Selected();
			}
			
			else if (Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + selectedGun) == 0)
			{
				UserDontHaveGun();
			}
			else if (Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + selectedGun) == 1 && Save.GetInt (PlayerPrefsKeys.ITEM+"gun" + selectedGun + "_bullets") == 0)
			{
				UserDontHaveBullets();
			}
			else
			{
				UserHaveGunAndBullets();
			}
		}
		
		gunreactionText.Text = "Gun Reaction: " + gunsHolder.guns[selectedGun-1].reaction.ToString() + " s";
		gunDescription.Text = gunsHolder.guns[selectedGun-1].description;//descriptionOfGuns[selectedGun-1];
		gunName.Text = gunsHolder.guns[selectedGun-1].name;
		
	}
	
	// Up Top Fix Me: Implementar a Compra da Arma
	void BuyGun()
	{
		Flow.shopManager.BuyItem(HandleBuyGun, Flow.shopManager.GetShopItem("pack_gun"+selectedGun));
	}
	
	void HandleBuyGun(ShopResultStatus status, string product)
	{
		if (status == ShopResultStatus.Success)
		{
			// fazer debugs
			UserHaveGunAndBullets();
		}
		else
		{
			
		}
		
	}
	
	// Up Top Fix Me: Implementar a Compra das Balas
	void BuyBullets()
	{
		Flow.shopManager.BuyItem(HandleBuyBullets, Flow.shopManager.GetShopItem("pack_bullets"+selectedGun));
	}
	
	void HandleBuyBullets(ShopResultStatus status, string product)
	{
		if (status == ShopResultStatus.Success)
		{
			// fazer debugs
			UserHaveGunAndBullets();
		}
	}
	
	void SelectedGun()
	{
		Debug.Log ("SelectedGun:" + selectedGun);
		//Debug.Log ("userId: " + int.Parse(PlayerPrefsKeys.ID.ToString()));
		
		for (int i = 0; i < Flow.ROUNDS_PER_TURN; i++)
		{	
			//Flow.currentGame.myRoundList.Add (new Round (-1, -1, Flow.currentMode == GameMode.SinglePlayer ? -1 : int.Parse(PlayerPrefsKeys.ID.ToString()),
			//Flow.currentGame.myRoundList.Add (new Round (i, -1, -1, new Gun(), -1f, -1, -1, -1));
			Flow.currentGame.myRoundList.Add (new Round (-1, -1, -1, new Gun(), -1f, -1, -1, -1));
		}
		
		Flow.currentGame.myRoundList[0].gun.id = selectedGun;
		Flow.currentGame.myRoundList[0].gun.reaction = gunsHolder.guns[selectedGun-1].reaction;
		
		if (Flow.path == TurnStatus.BeginGame)
		{
			Debug.Log ("TurnStatus.BeginGame");
			
			Flow.currentGame.theirRoundList = new List<Round>();
			
			for (int i = 0; i < Flow.ROUNDS_PER_TURN; i++)
			{
				Flow.currentGame.theirRoundList.Add(new Round(-1,-1,-1, new Gun(), 
					-1, UnityEngine.Random.Range(1, 5), 0,0));
					//UnityEngine.Random.Range(3, 5), UnityEngine.Random.Range(3, 5), 0,0)); TESTE
			}
			
			Flow.currentGame.theirRoundList[0].gun.id = 1;
			Flow.currentGame.theirRoundList[0].gun.reaction = gunsHolder.guns[0].reaction;
		}
		
		Debug.Log ("myGun: " + Flow.currentGame.myRoundList[0].gun.id);
		
		Flow.config.GetComponent<ConfigManager>().inviteAllScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().invitePlayingScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		
		//Application.LoadLevel ("Scenario1");
		Save.Set ("selectedGun", selectedGun, true);
		Application.LoadLevelAsync("Scenario1");
	}
	
	void BackButton()
	{
		Save.Set("selectedGun",selectedGun, true);
		if(Flow.currentMode == GameMode.SinglePlayer)
		{
			UIPanelManager.instance.BringIn("LevelSelectionPanel", UIPanelManager.MENU_DIRECTION.Backwards);
		}
		else
		{
			UIPanelManager.instance.BringIn("MultiplayerScenePanel", UIPanelManager.MENU_DIRECTION.Backwards);
		}
	}
}
