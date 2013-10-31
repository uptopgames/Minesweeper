using UnityEngine;
using System.Collections;

public class LevelButton : MonoBehaviour {
	
	public MinesweeperRaider game;
	public bool isCustom = false;
	public int world = 0;
	public int level = 0;
	private string[] worldNames = new string[5]{"Peru", "Tibet", "Greece", "Amazonia", "Egypt"};
	
	void Start()
	{
		if(isCustom)
		{
			transform.FindChild("control_text").GetComponent<SpriteText>().Text = "Custom Stage " + (level+1);
			transform.FindChild("name").GetComponent<SpriteText>().Text = Flow.customStages[transform.parent.GetComponent<UIListItem>().Index].name;
			transform.FindChild("mines").GetComponent<SpriteText>().Text = "Mines: " + 
				Flow.customStages[transform.parent.GetComponent<UIListItem>().Index].numberOfMines;
		}
	}
	
	public void StartGame()
	{
		game.StartGame(world, level);
	}
	
	/*public void StartCustomGame()
	{
		Application.LoadLevel("CustomStage");
	}*/
	
	public void StartCustomGame()
	{
		Flow.currentCustomStage = level;
		Application.LoadLevel(Application.loadedLevel);
	}
	
	public void Delete()
	{
		//chamar dialog; se confirmar, executa linhas abaixo
		transform.parent.parent.parent.GetComponent<UIScrollList>().RemoveItem(transform.parent.GetComponent<UIListItem>(), true);
		Flow.customStages.Remove(Flow.customStages[transform.parent.GetComponent<UIListItem>().Index]);
	}
}
