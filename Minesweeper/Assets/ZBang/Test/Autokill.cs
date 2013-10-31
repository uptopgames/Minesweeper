using UnityEngine;
using System.Collections;

public class Autokill : MonoBehaviour 
{
	public float time = 1f;

	// Use this for initialization
	void Start () 
	{
		GameObject.Destroy (gameObject, time);
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
