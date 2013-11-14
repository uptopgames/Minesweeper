using UnityEngine;
using System.Collections;

public class Replay : MonoBehaviour 
{
	public MeshRenderer enemyPicture;
	public SpriteText enemyName;
	
	// Use this for initialization
	void Awake () 
	{
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(InitReplay);
	}
	
	void InitReplay (EZTransition transition) 
	{
		Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(false);
		if(Flow.currentGame.friend.rawText != null) enemyPicture.material.mainTexture = Flow.currentGame.friend.rawText;
		else
		{
			GameRawAuthConnection conn = new GameRawAuthConnection(Flow.URL_BASE + "login/picture.php", HandleGetFriendPicture);
			WWWForm form = new WWWForm();
			form.AddField("user_id", Flow.currentGame.friend.id);
			
			conn.connect(form);
		}
		
		enemyName.Text = Flow.currentGame.friend.name;
		
		int enemyPoints = 0;
		int myPoints = 0;
		for(int i = 0 ; i < Flow.ROUNDS_PER_TURN ; i++)
		{
			//myPoints += Flow.currentGame.pastMyRoundList[i].playerRoundWin;
			//enemyPoints += Flow.currentGame.pastTheirRoundList[i].playerRoundWin;
		}
		
		if(enemyPoints > myPoints) 
		{
			Flow.enemyWin = true;
			Flow.playerWin = false;
		}
		else if(myPoints > enemyPoints) 
		{
			Flow.playerWin = true;
			Flow.enemyWin = false;
		}
		else
		{
			Flow.playerWin = false;
			Flow.enemyWin = false;
		}
	}
	
	void WatchReplay()
	{
		Flow.config.GetComponent<ConfigManager>().inviteAllScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().invitePlayingScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		Flow.config.GetComponent<ConfigManager>().challengeInviteScroll.transform.parent = GameObject.FindWithTag("RepoFLists").transform;
		//Application.LoadLevel("Scenario1");
		Application.LoadLevelAsync("Scenario1");
	}
	
	void HandleGetFriendPicture(string error, WWW conn)
	{
		if(this==null) return; //se mudou de cena, nao precisa mais fazer nada
		
		if (error != null || conn.error != null || conn.bytes.Length == 0)
		{
			Flow.currentGame.friend.rawText = Flow.transparent;
			Flow.currentGame.friend.got_picture = true;
			return;
		}
		
		Flow.currentGame.friend.got_picture = true;
		
		Flow.currentGame.friend.rawText = conn.texture;
		enemyPicture.material.mainTexture = conn.texture;
	}
}
