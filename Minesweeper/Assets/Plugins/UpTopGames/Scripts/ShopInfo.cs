using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CodeTitans.JSon;

public class ShopInfo : MonoBehaviour
{
	public string id;
	public MeshRenderer itemRenderer;
	public bool isInApp = false;
	[HideInInspector] public bool has = false;
	
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
	
	// Dicionario da FacebookAPI com parametros para o share post
    Dictionary<string,string> _sharePost;
	Dictionary<string, string> sharePost
	{
		get
		{
			if(_sharePost == null)
			{
				_sharePost = new Dictionary<string, string>()
			    {
			        // Nome do link a ser exibido
			        {"name", "Let's play " + Info.name + "?" },
			
			        // Descrição do post
			        {"description", Info.name + " is  a mobile and facebook app available on iOS and Android!"},
			
			        // Redireciona para o app ao clicar no link
					{"link", "https://apps.facebook.com/" + Info.facebookCanvas },
					
					// Coloca o icone do app no post
					{"picture", "https://uptopgames.com/static/icon/"+Info.appId+".png" },
					
			        // Abrir share no modo de dialogo
					{"dialog", "true"}
			    };
			}
			return _sharePost;
		}
	}
	
	// metodo chamado so por itens
	void ClickedShopItem()
	{
		Debug.Log(id);
		if(!has) Flow.shopManager.BuyItem(CheckBuyItem, Flow.shopManager.GetShopItem(id));
	}
	
	void CheckBuyItem(ShopResultStatus status, string product)
	{
		Debug.Log("status: "+status);
		Debug.Log("product: "+product);
		if(Flow.shopManager.GetShopItem(product).type == ShopItemType.NonConsumable)
		{
			has = true;
			transform.FindChild("Purchased").gameObject.SetActive(true);
			transform.FindChild("Price").gameObject.SetActive(false);
		}
	}
	
	
	// metodo chamado so por inapps
	void ClickedShopInApp()
	{
		if(id.Contains("com.")) Flow.shopManager.PurchaseInApp(id, OnPurchaseInApp);
		else ClickedFeature();
	}
	
	void ClickedFeature()
	{
		if(id == "Like")
		{
			// Se o cara ja deu like na pagina, o botao nao precisa fazer nada
			if(Save.HasKey(PlayerPrefsKeys.LIKE)) return;
			
			if(Info.IsWeb())
			{
				// programar like na web
			}
			else
			{
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
		}
		else if(id == "Share")
		{
			// Se o cara ja deu like na pagina, o botao nao precisa fazer nada
			if(Save.HasKey(PlayerPrefsKeys.SHARE) && DateTime.Parse(Save.GetString(PlayerPrefsKeys.SHARE)) - DateTime.UtcNow < TimeSpan.FromDays(1)) return;
			
			if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN))
			{
				StartShare();
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
		else if(id == "Rate")
		{
			if(Save.HasKey(PlayerPrefsKeys.RATE)) return;
			
			if(Info.IsWeb())
			{
				// fazer rate web
			}
			else
			{
				// Abre aplicativo nativo para o usuario dar Rate
                Application.OpenURL(Info.RateUrl());
				
				// chama o metodo que concede o rate para o usuario depois de um tempo, para nao ficar na cara que nao precisa do rate.
				Invoke("OnRate", 1.5f);
			}
		}
		else if(id == "Video")
		{
			// Se o cara ja viu o video, o botao nao precisa fazer nada
			if(Save.HasKey(PlayerPrefsKeys.VIDEO) && DateTime.Parse(Save.GetString(PlayerPrefsKeys.VIDEO)) - DateTime.UtcNow < TimeSpan.FromDays(1)) return;
			
			if(Info.IsWeb()) 
			{
				Flow.game_native.showMessage("Videos","Watching videos to get coins are only available within mobile devices");
			}
			else
			{
				 // Checa se o sistema de ADS esta rodando
                if (Advertisement.IsRunning())
                {
                    // Checa se o Video ja foi carregado
                    if (Advertisement.Video.IsDownloaded())
                    {
						// Executa o video
						Advertisement.Video.Play(VideoWatched);
						
                        // Adiciona Feature na database
                        if(Save.HasKey(PlayerPrefsKeys.TOKEN)) SetFeature(id);
                    }
                    else
					{
						Flow.game_native.showMessage("Loading", "The video is currently loading, please wait...");
					}
                }
                else
				{
					Flow.game_native.showMessage("Video unavailable", "Videos are currently unavaible on your device. Try again later!");
				}
			}
		}
		else if(id == "Widget")
		{
			// Se o cara ja clicou no link, o botao nao precisa fazer nada
			if(Save.HasKey(PlayerPrefsKeys.WIDGET) && DateTime.Parse(Save.GetString(PlayerPrefsKeys.WIDGET)) - DateTime.UtcNow < TimeSpan.FromDays(1)) return;
			
			if(Info.IsWeb())
			{
				Flow.game_native.showMessage("More Games", "Browsing for new games are currently available for mobile devices only.");
			}
			else
			{
				if (Advertisement.IsRunning())
                {
                    // Exibe a PopUp de confirmacao para a instalacao de um novo jogo
                    Advertisement.More.Widget(WidgetShowed);
					Flow.game_native.startLoading();
					UIManager.instance.blockInput = true;
                }
			}
		}
		else if(id == "Invite")
		{
			if(Save.HasKey(PlayerPrefsKeys.FACEBOOK_TOKEN))
			{
				StartInvite();
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
	}
	
	void WidgetShowed(bool showed)
	{
		Flow.game_native.stopLoading();
		UIManager.instance.blockInput = false;
		//Debug.Log("YAY! WIDGET APARECEU!!!!!!!");
		
		// Adiciona Feature na database
        if(Save.HasKey(PlayerPrefsKeys.TOKEN)) SetFeature(id);

        // Seta Feature como executada localmente
        Save.Set (PlayerPrefsKeys.WIDGET, DateTime.UtcNow.ToString(), true);
		transform.FindChild("Purchased").gameObject.SetActive(true);
		transform.FindChild("Coins").gameObject.SetActive(false);
		
		Flow.header.coins += Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsWidget;
	}
	
	void VideoWatched()
	{
		//Debug.Log("YAY! VIDEO FOI ASSISTIDO!!!!!!!");
		
		// Seta Feature como executada localmente
		Save.Set (PlayerPrefsKeys.VIDEO, DateTime.UtcNow.ToString(), true);
		transform.FindChild("Purchased").gameObject.SetActive(true);
		transform.FindChild("Coins").gameObject.SetActive(false);
		
		Flow.header.coins += Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsVideo;
	}
	
	void StartInvite()
	{
		if(Info.IsWeb() && !Info.IsEditor())
		{
			Application.ExternalEval("inviteFriendsFromShop()");
		}
		else
		{
			string access = "";
	
			access += "&fbtoken="+WWW.EscapeURL(Save.GetString(PlayerPrefsKeys.FACEBOOK_TOKEN));
		
			Flow.game_native.openUrlInline(Flow.URL_BASE+"login/facebook/invite_shop.php?device="+WWW.EscapeURL(SystemInfo.deviceUniqueIdentifier.Replace("-", ""))+
			"&token="+WWW.EscapeURL(Save.GetString(PlayerPrefsKeys.TOKEN))+"&app_id="+Info.appId+access);
		}
		
		Flow.game_native.showMessage("Inviting friends", "When your friends log in our app, you'll receive coins for each one of them!");
	}
	
	void StartShare()
	{
		facebook.WallPost("me", sharePost, ConfirmShare);
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
        // Se deu algo erro, break
        if (error != null) return;

        // Se o usuario realmente deu Like na pagina
        if (liked)
        {
            // Se estiver logado, adiciona Feature na database
            if(Save.HasKey(PlayerPrefsKeys.TOKEN)) SetFeature(id);

            // Seta Feature como executada localmente
            Save.Set (PlayerPrefsKeys.LIKE, true, true);
			transform.FindChild("Purchased").gameObject.SetActive(true);
			transform.FindChild("Coins").gameObject.SetActive(false);
			
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
	
	void ConfirmShare(string error, string facebookID, Dictionary<string,string> parameters, string post_id)
	{
		if(error != null) 
		{
			Flow.game_native.showMessage("Error", "Connection error");
		}
		else
		{
			if(post_id == null || post_id == "")
			{
				Flow.game_native.showMessage("Nothing shared", "You have to share the game in your wall to earn coins!");
			}
			else
			{
				// Adiciona Feature na database
                if(!Info.IsWeb()) 
				{
	                // Seta Feature como executada localmente
	                Save.Set (PlayerPrefsKeys.SHARE, DateTime.UtcNow.ToString(), true);
					transform.FindChild("Purchased").gameObject.SetActive(true);
					transform.FindChild("Coins").gameObject.SetActive(false);
					
					Flow.header.coins += Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsShare;
				}
			}
				
		}
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
		transform.FindChild("Purchased").gameObject.SetActive(true);
		transform.FindChild("Coins").gameObject.SetActive(false);
		
		// da as coins para a pessoa pelo rate
		Flow.header.coins += Flow.config.GetComponent<ConfigManager>().shopFeatures.coinsRate;
	}
	
	void OnPurchaseInApp(ShopResultStatus status, string product)
	{
		if(status == ShopResultStatus.Success)
		{
			
		}
	}
	
	public void DownloadImage()
	{
		GameRawAuthConnection conn = new GameRawAuthConnection(Flow.URL_BASE + "login/shop/itemimage.php", HandleGetImage);
		
		WWWForm form = new WWWForm();
		
		form.AddField("item_id", id);
		form.AddField("app_id", Info.appId);
		
		conn.connect(form);
	}
	
	void HandleGetImage(string error, WWW data)
	{
		if(error != null) Debug.Log(error);
		else
		{
			for(int i = 0 ; i < Flow.config.GetComponent<ConfigManager>().shopItems.Length ; i++)
			{
				if(Flow.config.GetComponent<ConfigManager>().shopItems[i].id == id)
				{
					Flow.config.GetComponent<ConfigManager>().shopItems[i].image = data.texture;
				}
			}
			//itemRenderer.material.mainTexture = data.texture;
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
		
		if(id == "Like")
		{
			StartLike();
		}
		else if(id == "Share")
		{
			StartShare();
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
		if(id == "Like")
		{
			StartLike();
		}
		else if(id == "Share")
		{
			StartShare();
		}
		else if(id == "Invite")
		{
			StartInvite();
		}
	}
	
}
