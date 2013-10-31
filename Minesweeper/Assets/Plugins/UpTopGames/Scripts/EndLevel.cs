using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CodeTitans.JSon;


public class EndLevel : MonoBehaviour 
{
	public enum EndLevelButtonClicked { None, Like, ShareProgress };
	
	public EndLevelButtonClicked buttonCliked = EndLevelButtonClicked.None;
	
	public SpriteText endLevelLabel;
	public GameObject[] enemies;
	public SpriteText enemyName;
	public Transform offerPanel;
	public GameObject bigPlayAgain;
	public GameObject smallPlayAgain;
	
	GameFacebook _fbAccount;
	GameFacebook fb_account
	{
		get
		{
			if(_fbAccount == null)
			{
				_fbAccount = new GameFacebook(HandleLinkFacebook);
			}
			return _fbAccount;
		}
	}
	
	FacebookAPI _facebook;
	public FacebookAPI facebook
	{
		get
		{
			if(_facebook == null)
			{
				_facebook = new FacebookAPI();
				_facebook.UpdateToken();
			}
			return _facebook;
		}
	}
		
	// Quantidade de checks do Like
    int likeCheckTries = 0;
	
	// string usada pro login
	string authentication = "";
	
	void Awake()
	{
		transform.GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(InitEndLevel);
	}
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	void InitEndLevel(EZTransition transition)
	{
		//Debug.Log(Flow.currentMode.ToString());
		
		Debug.Log("estamos no level: "+Flow.currentGame.level.id);
		
		enemyName.Text = Flow.currentGame.level.name;
		enemies[Flow.currentGame.level.id-1].SetActive(true);
		
		//enemyPicture.material.mainTexture = Flow.currentGame.friend.rawText;	
		
		FixScreenButtons();
		
		if (Flow.playerWin) 
		{
			//Debug.Log ("EndLevelWin");
			//Debug.Log ("world: " + Flow.currentGame.world.id);
			//Debug.Log ("level: " + Flow.currentGame.level.id);
			
			endLevelLabel.Text = "You Win";
			
			if (Flow.currentGame.world.id == Flow.MAX_WORLD_NUMBER && Flow.currentGame.level.id == Flow.MAX_LEVEL_NUMBER)
			{
				transform.FindChild("NextButton").gameObject.SetActive(false);
				//Debug.Log ("NextButtonFalse");
			}
			else
			{
				transform.FindChild("NextButton").gameObject.SetActive(true);
				//Debug.Log ("NextButtonTrue");
			}
			
			Debug.Log("estrelas no level: "+Flow.worldDict[Flow.currentGame.world.id-1].levelDict[Flow.currentGame.level.id-1].stars);
			for (int i = 0; i < Flow.worldDict[Flow.currentGame.world.id-1].levelDict[Flow.currentGame.level.id-1].stars; i++)
			{
				transform.FindChild("Star" + (i+1) + "Anim").gameObject.SetActive(true);
				//transform.FindChild("Star" + (i+1) + "Anim").GetComponent<PackedSprite>().playAnimOnStart = true;
			}
		}
		else
		{
			endLevelLabel.Text = "You Lose";
			endLevelLabel.SetColor(new Color(1,0,0,1));
			
			transform.FindChild("NextButton").gameObject.SetActive(false);
			
			for (int i = 0; i < 3; i++)
			{
				transform.FindChild("Star" + (i+1) + "Anim").gameObject.SetActive(false);
				//transform.FindChild("Star" + (i+1) + "Anim").GetComponent<PackedSprite>().playAnimOnStart = false;
			}
		}
		
	}
	
	void FixScreenButtons()
	{
		offerPanel.FindChild("LikeButton").FindChild("Coins").FindChild("Text").GetComponent<SpriteText>().Text = "+"+Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsLike;
		offerPanel.FindChild("RateButton").FindChild("Coins").FindChild("Text").GetComponent<SpriteText>().Text = "Win "+Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsRate;
		offerPanel.FindChild("LoginFacebookButton").FindChild("Coins").FindChild("Text").GetComponent<SpriteText>().Text = "+"+Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsFacebook;
		
		if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN) && Save.HasKey(PlayerPrefsKeys.RATE) && Save.HasKey(PlayerPrefsKeys.LIKE))
		{
			// muda botao play again de lugar e apaga todos os botoes
			bigPlayAgain.SetActive(true);
			smallPlayAgain.SetActive(false);
			offerPanel.FindChild("LikeButton").gameObject.SetActive(false);
			offerPanel.FindChild("LoginFacebookButton").gameObject.SetActive(false);
			offerPanel.FindChild("RateButton").gameObject.SetActive(false);
		}
		else if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN) && Save.HasKey(PlayerPrefsKeys.RATE))
		{
			// chama like porque ja fez share progress e rate
			offerPanel.FindChild("LikeButton").gameObject.SetActive(true);
			offerPanel.FindChild("RateButton").gameObject.SetActive(false);
			offerPanel.FindChild("LoginFacebookButton").gameObject.SetActive(false);
			smallPlayAgain.SetActive(true);
			bigPlayAgain.SetActive(false);
		}
		else if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN))
		{
			// chama rate porque ja ta vinculado com face
			offerPanel.FindChild("LikeButton").gameObject.SetActive(false);
			offerPanel.FindChild("RateButton").gameObject.SetActive(true);
			offerPanel.FindChild("LoginFacebookButton").gameObject.SetActive(false);
			smallPlayAgain.SetActive(true);
			bigPlayAgain.SetActive(false);
		}
		else if(Save.HasKey(PlayerPrefsKeys.RATE))
		{
			// chama share progress porque ja deu rate
			offerPanel.FindChild("LikeButton").gameObject.SetActive(false);
			offerPanel.FindChild("RateButton").gameObject.SetActive(false);
			offerPanel.FindChild("LoginFacebookButton").gameObject.SetActive(true);
			smallPlayAgain.SetActive(true);
			bigPlayAgain.SetActive(false);
		}
		else
		{
			// tira a sorte para aparecer rate ou share progress porque nao fez nenhum
			int chance = UnityEngine.Random.Range(0,2);
		
			if(chance == 0)
			{
				// chama share your progress
				offerPanel.FindChild("LikeButton").gameObject.SetActive(false);
				offerPanel.FindChild("RateButton").gameObject.SetActive(false);
				offerPanel.FindChild("LoginFacebookButton").gameObject.SetActive(true);
				smallPlayAgain.SetActive(true);
				bigPlayAgain.SetActive(false);
			}
			else
			{
				// chama rate
				offerPanel.FindChild("LikeButton").gameObject.SetActive(false);
				offerPanel.FindChild("RateButton").gameObject.SetActive(true);
				offerPanel.FindChild("LoginFacebookButton").gameObject.SetActive(false);
				smallPlayAgain.SetActive(true);
				bigPlayAgain.SetActive(false);
			}
		}
	}
	
	void NextLevel()
	{
		Debug.Log ("crieiTodosWorldsGameObjectsEndLevel");	
		
		int world = Flow.currentGame.world.id;
		int level = Flow.currentGame.level.id;
		
		if (level == Flow.MAX_LEVEL_NUMBER)
		{
			Debug.Log ("Level9");
			world++;
			level = 1;
			Debug.Log ("world: " + world);
			Debug.Log ("level: " + level);
		}
		else
		{
			level++;
			Debug.Log ("world: " + world);
			Debug.Log ("level: " + level);
		}
		
		Flow.currentGame = new Game();
			
		Flow.currentGame.world = 
		GameObject.FindWithTag("LevelSelection").transform.FindChild("ScrollLevels").transform.GetChild(0).transform.GetChild(world-1).GetComponent<World>();
		
		Flow.currentGame.level = 
		GameObject.FindWithTag("LevelSelection").transform.FindChild("ScrollLevels").transform.GetChild(0).transform.GetChild(world-1).FindChild
				("Level " + level + " Panel").GetComponent<Level>();
		//Flow.currentGame.world =  GameObject.FindWithTag("World" + world).GetComponent<World>();
		//Flow.currentGame.level = GameObject.FindWithTag("World" + world).transform.FindChild("Level" + level).GetComponent<Level>()	;
		
		Flow.currentGame.friend = new Friend();
		Flow.currentGame.friend.rawText = Flow.currentGame.level.image;
		
		Debug.Log ("flowWorld: " + Flow.currentGame.world.id);
		Debug.Log ("flow.Level: " + Flow.currentGame.level.id);
		
		Flow.currentGame.theirRoundList = new List<Round>();
			
		for (int i = 0; i < Flow.ROUNDS_PER_TURN; i++)
		{
			Flow.currentGame.theirRoundList.Add(new Round(-1,-1,-1, Flow.currentGame.world.enemyGun, 
				UnityEngine.Random.Range(Flow.currentGame.level.time.x, Flow.currentGame.level.time.y), UnityEngine.Random.Range(1, 5), 0,0));
				//UnityEngine.Random.Range(3, 5), UnityEngine.Random.Range(3, 5), 0,0)); TESTE
		}
		
		UIPanelManager.instance.BringIn("GunSelectionScenePanel",UIPanelManager.MENU_DIRECTION.Forwards);
	}
	
	void ShareProgress()
	{
		buttonCliked = EndLevelButtonClicked.ShareProgress;
		
		if(Save.HasKey(PlayerPrefsKeys.TOKEN))
		{
			// Vincula conta standalone a conta do facebook caso esteja logado com stand
			fb_account.link();
		}
		else
		{
			// Faz login com facebook para dar like
			RequestToken();
		}
	}
	
	// Obtem o resultado da vinculacao com o Facebook
	protected void HandleLinkFacebook(string error, IJSonObject data)
	{
		Flow.game_native.stopLoading();
		
		if (error != null)
		{
			Flow.game_native.showMessage("Error", error);
			Flow.game_native.showMessage("Facebook needed", "To invite your friends and earn coins, you'll need to link your Facebook account.");
			return;
		}
		facebook.SetToken(data["fbtoken"].ToString());
		Flow.game_native.showMessage("Success!","Your account with us is successfully linked to your Facebook account!");
		
		if(buttonCliked == EndLevelButtonClicked.Like)
		{
			StartLike();
		}
		else
		{
			FixScreenButtons();
		}
	}
	
	// Obtem o token com o servidor
	protected void RequestToken()
	{
		if (!Info.HasConnection(true)) 
		{
			Debug.Log("sem conexao");
			return;
		}
		
		Flow.game_native.startLoading();
		
		// Criar o codigo de autenticacao
		authentication = System.Guid.NewGuid().ToString();
		
		// Chama a pagina do Facebook no nosso servidor
		string fb_login_url = Flow.URL_BASE + "login/facebook/";
		fb_login_url += "?app_id=" + Info.appId;
		fb_login_url += "&authentication=" + authentication;
		fb_login_url += "&device=" + SystemInfo.deviceUniqueIdentifier.Replace("-", "");
		
		string device_push = PushNotifications.getPushDevice();
		if (device_push != null) fb_login_url += "&device_push=" + WWW.EscapeURL(device_push);
		fb_login_url += "&app_version=" + Info.version.ToString();
		fb_login_url += "&app_type=" + Info.appType.ToString();

#if UNITY_EDITOR
		fb_login_url += "&app_platform=UnityEditor";
#elif UNITY_WEBPLAYER
		fb_login_url += "&app_platform=Facebook";
#elif UNITY_ANDROID
		fb_login_url += "&app_platform=Android";
#elif UNITY_IPHONE
		fb_login_url += "&app_platform=iOS";
#endif

		Flow.game_native.openUrlInline(fb_login_url);
		
		// Obtem a resposta do servidor
		StartCoroutine(GetFacebookInfo());
	}
	
	// Obtem as informacoes do Facebook no servidor
	private IEnumerator GetFacebookInfo()
	{
		Debug.Log("pegando info face");
		// Numero maximo de tentativas
		int max_attempts = 5;
		
		WWW conn = null;
		
		WWWForm form = new WWWForm();
		form.AddField("device", SystemInfo.deviceUniqueIdentifier.Replace("-", ""));
		form.AddField("authentication", authentication);
		
		while (max_attempts > 0)
		{
			conn = new WWW(Flow.URL_BASE + "login/facebook/fbinfo.php", form);
			yield return conn;
			
			if (conn.error != null || conn.text != "") break;
			
			max_attempts--;
			
			yield return new WaitForSeconds(1);
		}
		
		Flow.game_native.stopLoading();
		
		if (max_attempts == 0 || conn.error != null)
		{
			Debug.LogError("Server error: " + conn.error);
			Flow.game_native.showMessage("Error", GameJsonAuthConnection.DEFAULT_ERROR_MESSAGE);
			
			yield break;
		}
		
		JSonReader reader = new JSonReader();
		IJSonObject data = reader.ReadAsJSonObject(conn.text);
		
		if (data == null || data.Contains("error"))
		{
			Debug.LogError("Json error: " + conn.text);
			Flow.game_native.showMessage("Error", GameJsonAuthConnection.DEFAULT_ERROR_MESSAGE);
			
			yield break;
		}
		
		Debug.Log("data: " + data);
		
		GameToken.save(data);
		Save.Set(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString(), data["fbtoken"].ToString(), true);
		Save.Set(PlayerPrefsKeys.NAME.ToString(), data["username"].ToString(),true);
		Save.Set(PlayerPrefsKeys.ID.ToString(), data["user_id"].ToString(),true);
		if(!data["email"].IsNull) Save.Set(PlayerPrefsKeys.EMAIL.ToString(), data["email"].ToString(), true);
		if(!data["first_name"].IsNull) Save.Set(PlayerPrefsKeys.FIRST_NAME.ToString(), data["first_name"].ToString(), true);
		if(!data["last_name"].IsNull) Save.Set(PlayerPrefsKeys.LAST_NAME.ToString(), data["last_name"].ToString(), true);
		if(!data["location"].IsNull) Save.Set(PlayerPrefsKeys.LOCATION.ToString(), data["location"].ToString(), true);
		if(!data["gender"].IsNull) Save.Set(PlayerPrefsKeys.GENDER.ToString(), data["gender"].ToString(), true);
		
		if(data["new_account"].StringValue == "1")
		{
			Flow.header.coins += Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsFacebook;
		}
		
		if(!data["birthday"].IsNull)
		{
			string day, month, year;
			string[] separator = {"-"};
			string[] birthday = data["birthday"].StringValue.Split(separator,System.StringSplitOptions.None);
			
			day = birthday[2];
			month = birthday[1];
			year = birthday[0];
			
			Save.Set(PlayerPrefsKeys.DATE_DAY.ToString(), day,true);
			Save.Set(PlayerPrefsKeys.DATE_MONTH.ToString(), month,true);
			Save.Set(PlayerPrefsKeys.DATE_YEAR.ToString(), year,true);
		}
		
		// Atualiza token da FacebookAPI
		if (Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString())) 
		{
			facebook.SetToken(Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN.ToString()));
		}	
		
		Save.SaveAll();
		if(buttonCliked == EndLevelButtonClicked.Like)
		{
			StartLike();
		}
		else
		{
			FixScreenButtons();
		}
		
		buttonCliked = EndLevelButtonClicked.None;
	}
	
	void Rate()
	{
		if(Info.IsWeb())
		{
			// fazer rate web
		}
		else
		{
			// Abre aplicativo nativo para o usuario dar Rate
            Application.OpenURL(Info.RateUrl());
			
			// chama o metodo que concede o rate para o usuario depois de um tempo, para nao ficar na cara que nao precisa do rate.
			Invoke("OnRate",1.5f);
		}
	}
	
	void OnRate()
	{
		// Adiciona Feature na database
		
#if UNITY_IPHONE
		if(Save.HasKey(PlayerPrefsKeys.TOKEN)) SetFeature("Rate", "Apple");
#elif UNITY_ANDROID
		if(Save.HasKey(PlayerPrefsKeys.TOKEN)) SetFeature("Rate", "Android");
#endif
		
		// Seta Feature como executada localmente
        Save.Set (PlayerPrefsKeys.RATE, true, true);
		
		// fazer coisas na cena aqui
		FixScreenButtons();
		
		// da as coins para a pessoa pelo rate
		Flow.header.coins += Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsRate;
	}
	
	void Like()
	{
		buttonCliked = EndLevelButtonClicked.Like;
		if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN))
		{
			StartLike();
		}
		else if(Save.HasKey(PlayerPrefsKeys.TOKEN))
		{
			// Vincula conta standalone a conta do facebook caso esteja logado com stand
			fb_account.link();
		}
		else
		{
			// Faz login com facebook para dar like
			RequestToken();
		}
	}
	
	void StartLike()
	{
		Flow.game_native.openUrlInline("https://facebook.com/" + Info.facebookPageId);
					
		// Inicia confirmacao de Like pela FacebookAPI
       facebook.IsLikedPage(Info.facebookPageId, OnCheckLike);
	}
	
	// Callback de cheks do Like pela FacebookAPI
    void OnCheckLike(string error, bool liked, string pageId)
    {
        // Se deu algum erro, break
        if (error != null) return;

        // Se o usuario realmente deu Like na pagina
        if (liked)
        {
            // Se estiver logado, adiciona Feature na database
            if(Save.HasKey(PlayerPrefsKeys.TOKEN)) SetFeature("Like");

            // Seta Feature como executada localmente
            Save.Set (PlayerPrefsKeys.LIKE, true, true);
				
			// fazer coisas na cena aqui
			FixScreenButtons();
			
			// da as coins para a pessoa pelo like
			Flow.header.coins += Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsLike;
        }

        // Caso contrario, faz mais '2' tentativas
        else if (likeCheckTries < 3)
        {
            likeCheckTries++;

            // Inicia confirmacao de Like pela FacebookAPI
            facebook.IsLikedPage(Info.facebookPageId, OnCheckLike);
        }

        // Caso contrario, informar que nenhum Like foi detectado
        else Flow.game_native.showMessage(Info.name, "We couldn't find your like.");
    }
	
	// Adiciona Feature na database
    void SetFeature(string feature, string platform = null)
    {
        // Seta Feature nao global (por plataforma)
        if (platform != null && platform != "") feature += " | " + platform;

        // Faz a conexao com o servidor enviando a feature
        GameJsonAuthConnection conn = new GameJsonAuthConnection(Flow.URL_BASE + "login/items/set_feature.php", OnSetFeature);
		WWWForm form = new WWWForm();
		form.Add("feature", feature);
		
		conn.connect(form);
    }

    // Ao adicionar a Feature na database
    void OnSetFeature(string error, IJSonObject data)
    {
		Debug.Log(error);
		Debug.Log(data);
    }
	
}
