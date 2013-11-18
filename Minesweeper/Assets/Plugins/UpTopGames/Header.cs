using UnityEngine;
using System.Collections;

public class Header : MonoBehaviour
{
	public SpriteText levelText;
	public SpriteText experienceText;
	public PackedSprite expBar;
	
	private int _coins;
	public int coins
	{
		get
		{
			if(_coins == null) _coins = Save.HasKey(PlayerPrefsKeys.COINS) ? Save.GetInt(PlayerPrefsKeys.COINS) : Flow.config.GetComponent<ConfigManager>().appInitialCoins;
			return _coins;
		}
		set
		{
			ChangeCoins(1.5f, _coins, value);
			//Debug.Log("era "+_coins+" seta para "+value);
			_coins = value;
			//shopButton.Text = _coins.ToString();
			
			Save.Set (PlayerPrefsKeys.COINS, _coins, true);
		}
	}
	public UIButton shopButton;
	
	bool isChangingCoins = false;
	
	void Start()
	{
		
	}
	
	void FixedUpdate()
	{
		if(isChangingCoins)
		{
			float points = 1 * Time.fixedDeltaTime/timePerNumber;
			
			//Debug.Log("ptd: "+points);
			
			if(diff < 0) 
			{
				//Debug.Log("diff");
				if((diff+points) > 0)
				{
					diff = 0;
					growingDiff = originalDiff;
				}
				else
				{
					diff += points;
					growingDiff -= points;
				}
				
			}
			else if(diff > 0) 
			{
				if((diff-points) < 0)
				{
					diff = 0;
					growingDiff = originalDiff;
				}
				else
				{
					diff -= points;
					growingDiff += points;
				}
			}
			
			shopButton.Text = ((int)(growingDiff + oldCoinsNumber)).ToString();
			//Debug.Log("diff: "+diff+" ocn: "+oldCoinsNumber);
			
			if(diff == 0) 
			{
				isChangingCoins = false;
				timePerNumber = 0;
				oldCoinsNumber = coins;
				growingDiff = 0;
				originalDiff = 0;
				//Debug.Log("chegou ao fim.");
			}
		}
	}
	
	float diff = 0;
	float oldCoinsNumber = 0;
	float timePerNumber = 0;
	float growingDiff = 0;
	float originalDiff = 0;
	
	void ChangeCoins(float seconds, int oldAmount, int newAmount)
	{
		oldCoinsNumber = oldAmount;
		diff = newAmount - coins;
		timePerNumber = Mathf.Abs(seconds/diff);
		
		originalDiff = diff;
		
		isChangingCoins = true;
		//Debug.Log("tpn: "+timePerNumber);
		//InvokeRepeating("DoChange", 0, timePerNumber);
		
	}
	
	void OptionsButton()
	{
		if(Application.loadedLevelName == "Mainmenu")
		{
			if(UIPanelManager.instance.CurrentPanel.name != "OptionsScenePanel")
			{
				Flow.currentCustomStage = -1;
				UIPanelManager.instance.BringIn("OptionsScenePanel");
			}
		}
	}
	
	void ChallengesButton()
	{
		if(Application.loadedLevelName == "Mainmenu")
		{
			if(UIPanelManager.instance.CurrentPanel.name != "ChallengesScenePanel")
			{
				if(Save.HasKey(PlayerPrefsKeys.TOKEN.ToString()))
				{
					Flow.currentCustomStage = -1;
					UIPanelManager.instance.BringIn("ChallengesScenePanel");
				}
				else
				{
					Flow.currentCustomStage = -1;
					UIPanelManager.instance.BringIn("LoginScenePanel");
				}
			}
		}
	}
	
	void ShopButton()
	{
		if(Application.loadedLevelName == "Mainmenu")
		{
			if(UIPanelManager.instance.CurrentPanel.name != "MenuScenePanel" &&
				UIPanelManager.instance.CurrentPanel.name != "GunSelectionScenePanel" &&
				UIPanelManager.instance.CurrentPanel.name != "BattleStatusScenePanel" &&
				UIPanelManager.instance.CurrentPanel.name != "ReplayScenePanel")
			{
				UIPanelManager.instance.BringIn("ShopScenePanel");
				Flow.currentCustomStage = -1;
			}
		}
	}
	
}