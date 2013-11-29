using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour 
{
	void Start()
	{
		GetComponent<UIInteractivePanel>().transitions.list[1].AddTransitionStartDelegate(EnteredMenu);
	}
	
	void EnteredMenu(EZTransition transition)
	{
		Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(false);
		
		Flow.currentMode = GameMode.None;
	}
	
	void RankingButton()
	{
#if UNITY_IPHONE
		if (Ranking.GameCenterAvailable())
		{
			if (Ranking.UserIsAuthenticated()) Ranking.ShowLeaderboard(GameCenterLeaderboardTimeScope.Week, "id");
			else Flow.game_native.showMessage(Info.name, "You must sign in on GameCenter!");
		}
		else Flow.game_native.showMessage(Info.name, "GameCenter is currently unavailable!");
#else
		Flow.game_native.showMessage(Info.name, "Rankings are currently unavailable!");
#endif
	}
	
	void MultiplayerButton()
	{
		Flow.currentMode = GameMode.Multiplayer;
		if(Save.HasKey(PlayerPrefsKeys.TOKEN.ToString()))
		{
			UIPanelManager.instance.BringIn("MultiplayerScenePanel");
		}
		else
		{
			Flow.panelAfterLogin = "MultiplayerScenePanel";
			UIPanelManager.instance.BringIn("LoginScenePanel");
		}
	}
	
	void SingleplayerButton()
	{
		Flow.currentMode = GameMode.SinglePlayer;
		UIPanelManager.instance.BringIn("LevelSelectionPanel");
	}
}
