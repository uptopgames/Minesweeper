using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour
{
	private List<List<int>> tileset;
	private List<Vector2> path;
	private List<Crossroad> decisions;
	private Vector2 player, end;
	private bool madeIt;
	private int breaker;
	private int tries = 100;
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F))
		{
			player = new Vector2(0,0);
			end = new Vector2(0,0);
			for(int i = 0; i<8; i++)
			{
				player.x = i;
				for(int j = 0; j<8; j++)
				{
					player.y = j;
					for(int k = 0; k<8; k++)
					{
						end.x = k;
						for(int l = 0; l<8; l++)
						{
							//Debug.Log("Player: " + i + ", " + j + "; End: " + k + ", " + l + ";");
							end.y = l;
							CheckFake();
						}
					}
				}
			}
		}
	}
	
	void CheckFake()
	{
		madeIt = false;
		path = new List<Vector2>();
		decisions = new List<Crossroad>();
		breaker = tries;
		
		tileset = new List<List<int>>()
		{
			new List<int>(){0,0,0,0,0,0,0,0},
			new List<int>(){0,0,0,0,0,0,0,0},
			new List<int>(){0,0,0,0,0,0,0,0},
			new List<int>(){0,0,0,0,0,0,0,0},
			new List<int>(){0,0,0,0,0,0,0,0},
			new List<int>(){0,0,0,0,0,0,0,0},
			new List<int>(){0,0,0,0,0,0,0,0},
			new List<int>(){0,0,0,0,0,0,0,0}
		};
		
		tileset[Mathf.RoundToInt(player.x)][Mathf.RoundToInt(player.y)] = 3;
		tileset[Mathf.RoundToInt(end.x)][Mathf.RoundToInt(end.y)] = 2;
		
		//Debug.Log("player: " + player + "\ndiamond: " + end);
		
		path.Add(player);
		
		LoopCheck();
	}
	
	public void CheckStage(List<List<int>> sentTileset)
	{	
		madeIt = false;
		tileset = sentTileset;
		path = new List<Vector2>();
		player = new Vector2(-1,-1);
		end = new Vector2(-1,-1);
		decisions = new List<Crossroad>();
		breaker = tries;
		
		for(int i = 0; i<tileset.Count; i++)
		{
			for(int j = 0; j<tileset[i].Count; j++)
			{
				if(tileset[i][j]==3)
				{
					player = new Vector2(j,i);
				}
				else if(tileset[i][j]==2)
				{
					end = new Vector2(j,i);
				}
			}
		}
		
		Debug.Log("player: " + player);
		Debug.Log("diamond: " + end);
		
		path.Add(player);
		
		LoopCheck();
	}
	
	void LoopCheck()
	{
		while(player != new Vector2(-1,-1) && !madeIt)
		{
			FullCheck();
		}
		
		if(madeIt)
		{
			Debug.Log("cheguei no tesouro em " + (tries - breaker).ToString() + " tentativas");
			GetComponent<CreateStage>().StageApproved();
		}
		else
		{
			if(decisions.Count>0 && breaker>0)
			{
				//Debug.Log("non consegui chegar no tesouro, vou tentar outro caminho");
				while(decisions.Count>0)
				{
					if(decisions[decisions.Count-1].currentDirection == Direction.None)
					{
						decisions.Remove(decisions[decisions.Count-1]);
					}
					else
					{
						break;
					}
				}
				
				breaker--;
				
				if(decisions.Count>0)
				{
					player = decisions[decisions.Count-1].location;
				}
				path.Clear();
				path.Add(player);
				
				LoopCheck();
			}
			else
			{
				Debug.Log("caminho impossivel em " + (tries - breaker).ToString() + " tentativas");
				GetComponent<CreateStage>().StageDenied();
			}
		}
	}
	
	void FullCheck()
	{
		bool gotIt = false;
		Crossroad tempCrossroad = new Crossroad();
		Crossroad previousCrossroad = null;
		tempCrossroad.location = player;
		
		foreach(Crossroad c in decisions)
		{
			if(tempCrossroad.location == c.location)
			{
				tempCrossroad.currentDirection = c.currentDirection;
				previousCrossroad = c;
				break;
			}
		}
		
		if(player.x<7 && SingleCheck(new Vector2(player.x+1, player.y)))
		{//check right
			if(!gotIt && tempCrossroad.CheckStage(Direction.Right))
			{
				gotIt = true;
				player.x++;
				tempCrossroad.currentDirection = Direction.Right;
			}
		}
		
		if(player.x>0 && SingleCheck(new Vector2(player.x-1, player.y)))
		{//check left
			if(!gotIt && tempCrossroad.CheckStage(Direction.Left))
			{
				gotIt = true;
				player.x--;
				tempCrossroad.currentDirection = Direction.Left;
			}
		}
		
		if(player.y<7 && SingleCheck(new Vector2(player.x, player.y+1)))
		{//check down
			if(!gotIt && tempCrossroad.CheckStage(Direction.Down))
			{
				gotIt = true;
				player.y++;
				tempCrossroad.currentDirection = Direction.Down;
			}
		}
		
		if(player.y>0 && SingleCheck(new Vector2(player.x, player.y-1)))
		{//check up
			if(!gotIt && tempCrossroad.CheckStage(Direction.Up))
			{
				gotIt = true;
				player.y--;
				tempCrossroad.currentDirection = Direction.Up;
			}
		}
		
		if(!gotIt)
		{
			player = new Vector2(-1,-1);
			tempCrossroad.currentDirection = Direction.None;
		}
		
		if(previousCrossroad!=null)
		{
			previousCrossroad.currentDirection = tempCrossroad.currentDirection;
		}
		else
		{
			decisions.Add(tempCrossroad);
		}
		
		//Debug.Log("andei para " + player);
		path.Add(player);
		if(player==end)
		{
			madeIt = true;
		}
	}
	
	bool SingleCheck(Vector2 position)
	{
		if(tileset[Mathf.RoundToInt(position.y)][Mathf.RoundToInt(position.x)] == 1) return false;
		
		foreach(Vector2 p in path)
		{
			if(p == position)
			{
				return false;
			}
		}
		
		return true;
	}
}

public class Crossroad
{
	public Vector2 location = new Vector2(-1,-1);
	public Direction currentDirection = Direction.Start;
	public void NextStage()
	{
		currentDirection = (Direction)(int)currentDirection++;
	}
	public bool CheckStage(Direction nextDirection)
	{
		if((int)currentDirection < (int)nextDirection) return true;
		else return false;
	}
}

public enum Direction
{
	Start, Right, Left, Down, Up, None
}