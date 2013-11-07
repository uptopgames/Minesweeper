using UnityEngine;
using System.Collections;

public class WorldSelection : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void StartGame()
	{
		Flow.currentGame.world.id = 1;
		Flow.header.transform.FindChild("OptionsPanel").gameObject.SetActive(false);
		Application.LoadLevel("Game");
	}
}
