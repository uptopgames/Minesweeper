using UnityEngine;
using System.Collections;

public class ChallengesButton : MonoBehaviour
{
	public int index = -1;
	
	// Use this for initialization
	void Start () {
	
	}
	
	void RankingScreen ()
	{
		//Flow.currentRank = Flow.customStages[index];
		Flow.currentRank = Flow.customGames[index];
		UIPanelManager.instance.BringIn("RankingsScenePanel", UIPanelManager.MENU_DIRECTION.Backwards);
	}
	
	void CreateGame()
	{
		Flow.game_native.showMessage("Feature not Implemented Yet", "Please wait until we implement this feature", "Ok");
	}
}
