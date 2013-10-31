using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MineCounter : MonoBehaviour
{
	public enum MineCounterType
	{
		LeftColumn, RightColumn, UpRow, DownRow
	};
	public MineCounterType type;
	public int positionIndex = 0;
	
	public MinesweeperRaider game;
	public CreateStage creator;
	
	public void UpdateText()
	{
		int counter = 0;
		
		switch(type)
		{
			case MineCounterType.LeftColumn:
				if(game.tileset[positionIndex][0]==1) counter ++;
				if(game.tileset[positionIndex][1]==1) counter ++;
				if(game.tileset[positionIndex][2]==1) counter ++;
				if(game.tileset[positionIndex][3]==1) counter ++;
			break;
			case MineCounterType.RightColumn:
				if(game.tileset[positionIndex][4]==1) counter ++;
				if(game.tileset[positionIndex][5]==1) counter ++;
				if(game.tileset[positionIndex][6]==1) counter ++;
				if(game.tileset[positionIndex][7]==1) counter ++;
			break;
			case MineCounterType.UpRow:
				if(game.tileset[0][positionIndex]==1) counter ++;
				if(game.tileset[1][positionIndex]==1) counter ++;
				if(game.tileset[2][positionIndex]==1) counter ++;
				if(game.tileset[3][positionIndex]==1) counter ++;
			break;
			case MineCounterType.DownRow:
				if(game.tileset[4][positionIndex]==1) counter ++;
				if(game.tileset[5][positionIndex]==1) counter ++;
				if(game.tileset[6][positionIndex]==1) counter ++;
				if(game.tileset[7][positionIndex]==1) counter ++;
			break;
		}
		
		GetComponent<SpriteText>().Text = counter.ToString();
	}
	
	public void UpdateTextEditor()
	{
		int counter = 0;
		
		switch(type)
		{
			case MineCounterType.LeftColumn:
				if(creator.tileset[positionIndex][0]==1) counter ++;
				if(creator.tileset[positionIndex][1]==1) counter ++;
				if(creator.tileset[positionIndex][2]==1) counter ++;
				if(creator.tileset[positionIndex][3]==1) counter ++;
			break;
			case MineCounterType.RightColumn:
				if(creator.tileset[positionIndex][4]==1) counter ++;
				if(creator.tileset[positionIndex][5]==1) counter ++;
				if(creator.tileset[positionIndex][6]==1) counter ++;
				if(creator.tileset[positionIndex][7]==1) counter ++;
			break;
			case MineCounterType.UpRow:
				if(creator.tileset[0][positionIndex]==1) counter ++;
				if(creator.tileset[1][positionIndex]==1) counter ++;
				if(creator.tileset[2][positionIndex]==1) counter ++;
				if(creator.tileset[3][positionIndex]==1) counter ++;
			break;
			case MineCounterType.DownRow:
				if(creator.tileset[4][positionIndex]==1) counter ++;
				if(creator.tileset[5][positionIndex]==1) counter ++;
				if(creator.tileset[6][positionIndex]==1) counter ++;
				if(creator.tileset[7][positionIndex]==1) counter ++;
			break;
		}
		
		GetComponent<SpriteText>().Text = counter.ToString();
	}
}