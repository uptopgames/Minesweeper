using UnityEngine;
using System.Collections;
	
public class Round
{
	public int roundID;	
	public int turnID;
	public int userID;
	//public Gun gun;
	public float time;
	//public float bangTime;
	//public int sandAttack;
	//public int playerRoundWin;
	
	public Round(int id, int turn, int user, float time)//Gun theGun, float time, float bang, int attack, int roundWin)
	{
		this.roundID = id;
		this.turnID = turn;
		this.userID = user;
		//this.gun = theGun;
		this.time = time;
		//this.bangTime = bang;
		//this.sandAttack = attack;
		//this.playerRoundWin = roundWin;
	}
}
