using UnityEngine;
using System.Collections;

public class LevelButton : MonoBehaviour
{
	public bool isCustom = false;
	public int world = 0;
	public int level = 0;
	private string[] worldNames = new string[5]{"Peru", "Tibet", "Greece", "Amazonia", "Egypt"};
	
	void Start()
	{
		if(isCustom)
		{
			transform.FindChild("Level").FindChild("control_text").GetComponent<SpriteText>().Text = "Custom Stage " + (level+1);
			transform.FindChild("Level").FindChild("name").GetComponent<SpriteText>().Text = Flow.customStages[transform.GetComponent<UIListItem>().Index].name;
			transform.FindChild("Level").FindChild("mines").GetComponent<SpriteText>().Text = "Mines: " + 
				Flow.customStages[transform.GetComponent<UIListItem>().Index].numberOfMines;
		}
	}
	
	public void StartGame()
	{
		//lembrar de criar o game, o turno, os rounds e tudo o mais no flow antes de mandar pro game
		Application.LoadLevel("Game");
	}
	
	public void StartCustomGame()
	{
		//lembrar de criar o game, o turno, os rounds e tudo o mais no flow antes de mandar pro game
		Flow.currentCustomStage = level;
		Application.LoadLevel("Game");
	}
	
	public void Delete()
	{
		//chamar dialog; se confirmar, executa linhas abaixo
		transform.parent.parent.GetComponent<UIScrollList>().RemoveItem(transform.GetComponent<UIListItem>(), true);
		Flow.customStages.Remove(Flow.customStages[transform.GetComponent<UIListItem>().Index]);
	}
	
	public void SendToFriend()
	{
		transform.parent.parent.GetComponent<CustomLevelScroll>().SendToFriend(this);
	}
}