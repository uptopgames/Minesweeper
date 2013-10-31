using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeTitans.JSon;
using System;

public enum GameMode { Multiplayer, SinglePlayer, None }

[System.Serializable]
public class Gun 
{
	public int id;
	public float reaction;
	public string name;
	public string description;
}

public class CustomStage
{
	public int world = 0;
	public List<List<int>> tileset;
	public int numberOfMines = 0;
	public string name = "New Stage";
}

public class PlayerPrefsKeys
{
	public static string TOKEN = "LjA3ZBho8s",
	TOKEN_EXPIRATION = "JyCoCznHRy",
	FACEBOOK_TOKEN = "VkhEzPRqc0",
	NAME = "jg5DhoIXqa",
	ID = "JvaVoUOrcm",
	EMAIL = "n5EkU4DSlu",
	PASSWORD = "pnz5tkIUT9",
	FACEBOOK_ID = "byuzSrWPFf",
	FIRST_NAME = "3tcOPoenCF",
	LAST_NAME = "QDCAHcyfaR",
	LOCATION = "wFREllNX3A",
	GENDER = "Jwk8zvfAoA",
	DATE_DAY = "rOkbQHqs03",
	DATE_MONTH = "266NDq0ojh",
	DATE_YEAR = "aJbdakz7KO",
	APP_ID = "gKYCpFGlOu",
	COINS = "3MhqRjyPEZ",
	NOADS = "ekdfuvtNzT",
	VOLUME = "vyNlbfVkeG",
	PUSHNOTIFICATIONS = "cPgIPg6Wtq",
	LIKE = "4CrIiU5lw9",
	SHARE = "aXI1XTZEbq",
	RATE = "0NArcBvaJi",
	VIDEO = "bHneDidbci",
	WIDGET = "nvq5NmUoSw",
	INVITE = "Ms1GNXuYIP",
	ITEM = "qpsHjQx2TC",
	ITEMSPACK = "MgQTtHaxYF";
}

public enum PanelToLoad
{
	Menu,
	WinLose,
	BattleStatus,
	EndLevel,
	Multiplayer,
	GunSelection
}

public enum TurnStatus
{
	BeginGame, AnswerGame, ShowPast
}

public class Flow: MonoBehaviour
{
	public static float playerExperience = 0;
	public static int playerLevel = 1;
	public static List<CustomStage> customStages;
	public static int currentCustomStage = -1;
	
	public static int mapLevel = 1;
	public static int radarLevel = 1;
	public static int hpLevel = 1;
	
	public static void AddCustomLevel(List<List<int>> customTileset, int customWorld, int numberOfMines, string name)
	{
		if(customStages == null)
		{
			customStages = new List<CustomStage>();
		}
		CustomStage c = new CustomStage();
		c.tileset = customTileset;
		c.world = customWorld;
		c.numberOfMines = numberOfMines;
		c.name = name;
		customStages.Add(c);
	}
	
	public static GameNativeGUI game_native = new GameNativeGUI();
	
	public static bool playerWin = false, enemyWin = false;
	
#if UNITY_IPHONE || UNITY_WEBPLAYER
	public static string URL_BASE = "https://uptopgames.com/";
#elif UNITY_ANDROID
	public static string URL_BASE = "http://uptopgames.com/";
#endif
	
	private static Texture2D _transparent;
	public static Texture2D transparent
	{
		get
		{
			if(_transparent == null)
			{
				_transparent = Resources.Load("transparentpixel") as Texture2D;
			}
			return _transparent;
		}
	}
	
	private static Header _header;
	public static Header header
	{
		get
		{
			if(_header == null)
			{
				_header = config.GetComponent<ConfigManager>().headerObject.GetComponent<Header>();
			}
			return _header;
		}
	}
	
	//public static Header header;
		
	private static GameObject _loadingDialog;
	public static GameObject loadingDialog
	{
		get
		{
			if(_loadingDialog == null)
			{
				_loadingDialog = config.GetComponent<ConfigManager>().loading;
			}
			return _loadingDialog;
		}
	}
	
	private static GameObject _messageOkDialog;
	public static GameObject messageOkDialog
	{
		get
		{
			if(_messageOkDialog == null)
			{
				_messageOkDialog = config.GetComponent<ConfigManager>().messageOk;
			}
			return _messageOkDialog;
		}
	}
	
	private static GameObject _messageOkCancelDialog;
	public static GameObject messageOkCancelDialog
	{
		get
		{
			if(_messageOkCancelDialog == null)
			{
				_messageOkCancelDialog = config.GetComponent<ConfigManager>().messageOkCancel;
			}
			return _messageOkCancelDialog;
		}
	}
	
	private static GameObject _config;
	public static GameObject config
	{
		get
		{
			if(_config == null)
			{
				_config = GameObject.FindGameObjectWithTag(ConfigManager.API);
			}
			return _config;
		}
	}
	
	private static ShopManager _shopManager;
	public static ShopManager shopManager
	{
		get
		{
			if(_shopManager == null)
			{
				_shopManager = config.GetComponent<ShopManager>();
			}
			return _shopManager;
		}
	}
	
	//public static GameObject config;
	//public static GameObject messageOkCancelDialog;
	//public static GameObject messageOkDialog;
	//public static GameObject loadingDialog;
	//public static ShopManager shopManager;
	
	//variáveis que estavam dentro de Game
	public static string[] availableMaps = { "usa", "brazil", "uk", "southafrica", "world", "china", "france", "australia" };
	
	public static PanelToLoad nextPanel = PanelToLoad.Menu;
	
	public static List<Game> gameList = new List<Game>();
	//public static List<Texture> gamePictures = new List<Texture>();
	public static int yourTurnGames = 0;
	public static int theirTurnGames = 0;
	public static Game currentGame = new Game();
	public static GameMode currentMode = GameMode.None;
	public static int selectedListIndex = -1;
	
	private static TurnStatus _path;
	public static TurnStatus path
	{
		get
		{
			if(_path == null) _path = TurnStatus.BeginGame;
			return _path;
		}
		set
		{
			Debug.Log("mudaram o path: "+value.ToString());
			_path = value;
		}
	}
	
	// setar aqui o nome do arquivo xml a ser usado, se houver necessidade de mudar
	public static string xmlFileName = "gameData.xml";
	public static Texture2D playerPhoto = null;
	public static int gamesPassedMulti = 0;
	public static int gamesPassedSingle = 0;
	public static bool isDownloadingPlayerPhoto = false;
	public static DateTime lastUpdate = new DateTime(1970,1,1);
	public static int selectedWorldID = -1;
	
	public static Dictionary<int,World> worldDict = new Dictionary<int,World>();
	public static DateTime gameDataLastDate = new DateTime(1970,1,1);
	
	public static List<Theme> localThemes = new List<Theme>();
	public static List<List<Locale>> currentLevelList = new List<List<Locale>>();
	public static Dictionary<string, List<Locale>> levelDict = new Dictionary<string, List<Locale>>();
	public static int currentLevel;
	public static int currentScore;
	public static int MAX_WORLD_NUMBER = 5;
	public static int MAX_LEVEL_NUMBER = 9;
	public const int ROUNDS_PER_TURN = 5;
	
	public static void getPlayerPhoto(string error, WWW data)
    {
        Flow.isDownloadingPlayerPhoto = false;
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
        }
    }
	
	
	
	public static void Reset()
	{
		//playerName = "";
		playerPhoto = null;
		lastUpdate = new DateTime(1970,1,1);
		Game.Reset();
		gameList = new List<Game>();
		yourTurnGames = 0;
		theirTurnGames = 0;
	}
	
	public static void OnLogoutFromServer()
	{
		// Deletar aqui todas as suas keys do Save que devem ser deletadas quando um usuario da logout
		
		Debug.Log("Execute flow! (OnLogoutFromServer)");
		Flow.config.GetComponent<ConfigManager>().OnLogoutFromServer();
		Flow.Reset();
		
		/*Debug.Log("ct"+localThemes.Count);
		for(int i = 0 ; i < 100 ; i++)
		{
			//Debug.Log("zabumba"+i);
			Save.Delete("purchasedTheme"+i);
			Save.Delete("datePurchasedTheme"+i);
		}
		
		Save.Delete("FeatureInvite");
		Save.Delete("FeatureLike");
		Save.Delete("FeatureVideo");
		Save.Delete("FeatureRate");
		Save.Delete("FeatureWidget");
		Save.Delete("FeatureShare");
		Save.Delete("userFeatures");
		Save.Delete("purchasedItems");
		Save.Delete("datePurchasedItems");*/
		
		Save.SaveAll();
		
		// Zerar coins
	}
}

[System.Serializable]
public class Theme
{
	public int id;
	public string name;
	public string code;
	public int tier;
	public int parentTheme;
	public int pointsToUnlock;
	public int themeGroup;
	public Texture2D picture;
	public string appleBundle;
	public string androidBundle;
	public bool isDownloading = false;
	
	public Theme(int themeID, string themeName, string mapCode, int tGroup, int priceTier, int pTheme, int points, string apple, string android)
	{
		this.id = themeID;
		this.name = themeName;
		this.code = mapCode;
		this.tier = priceTier;
		this.themeGroup = tGroup;
		this.parentTheme = pTheme;
		this.pointsToUnlock = points;
		this.appleBundle = apple;
		this.androidBundle = android;
	}
}

[System.Serializable]
public class Locale
{
	public int localeID;
	public Theme localeTheme;
	public float latitude;
	public float longitude;
	public string localeName;
	public DateTime date;
	
	public Locale(int id, string name, Theme theme, float latitude, float longitude, DateTime date)
	{
		this.localeID = id;
		this.localeName = name;
		this.localeTheme = theme;
		this.latitude = latitude;
		this.longitude = longitude;
		this.date = date;
		
	}
}