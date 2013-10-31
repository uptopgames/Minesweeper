using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class World : MonoBehaviour
{
	public Texture2D image;
	public bool isDownloading = false;
	public string imageName;
	public Dictionary<int,Level> levelDict = new Dictionary<int,Level>();
	public string appleBundle;
	public string androidBundle;
	public int id;
	public string name;
	public int starsToUnlock;
	//public Gun enemyGun;
	public DateTime lastUpdate;
	
	public World SetWorld(int id, string name, int stars)//, Gun theirGun)
	{
		id = id;
		name = name;
		starsToUnlock = stars;
		//enemyGun = theirGun;
		
		return this;
	}

}
