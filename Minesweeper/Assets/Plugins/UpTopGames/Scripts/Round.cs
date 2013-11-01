using UnityEngine;
using System.Collections;
	
public class Round
{
	public int roundID;	
	public int turnID;
	public int userID;
	public float time = 0f;
	public int deaths = 0;
	
	public Round(int id, int turn, int user, float time, int deaths)
	{
		this.roundID = id;
		this.turnID = turn;
		this.userID = user;
		this.deaths = deaths;
		this.time = time;
	}
}
