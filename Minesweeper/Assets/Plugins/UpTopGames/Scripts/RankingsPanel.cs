using UnityEngine;
using System.Collections;

public class RankingsPanel : MonoBehaviour
{
	public SpriteText stageName;
	public SpriteText hostName;

	void Start ()
	{
		GetComponent<UIInteractivePanel>().transitions.list[0].AddTransitionStartDelegate(InitRankings);
		GetComponent<UIInteractivePanel>().transitions.list[1].AddTransitionStartDelegate(InitRankings);
		GetComponent<UIInteractivePanel>().transitions.list[2].AddTransitionStartDelegate(InitRankings);
		GetComponent<UIInteractivePanel>().transitions.list[3].AddTransitionStartDelegate(InitRankings);
	}
	
	void InitRankings(EZTransition transition)
	{
		stageName.Text = Flow.currentRank.name;
		hostName.Text = "Hosted by " + Flow.currentRank.creatorName;
		transform.FindChild("World"+(Flow.currentRank.world+1).ToString()).gameObject.SetActive(true);
	}
	
	void CreateGame ()
	{
		Flow.game_native.showMessage("Feature not Implemented Yet", "Please wait until we implement this feature", "Ok");
	}
}
