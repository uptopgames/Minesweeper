using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
	
	// Use this for initialization
	void Start ()
	{
	
		MultiArray a = new MultiArray();
		
		a["x"].value = "xxx";
		a["z"].value = "zzz";
		a["x"]["a"].value = "aaa";
		
		Debug.Log(a["x"].value);
		Debug.Log(a["z"].value);
		Debug.Log(a["x"]["a"].value);
		
		Debug.Log("end");
		
		foreach (KeyValuePair<string, MultiArray> b in a.forEach)
		{
			Debug.Log(b.Value.value);
		}
	
	}
	
	public MultiArray etst;
	
	
	[System.Serializable]
	public class MultiArray
	{
		public Dictionary<string, MultiArray> forEach = new Dictionary<string, MultiArray>();
		
		public object value;
		
		public MultiArray this[string key]
		{
			get
			{
				if (forEach.ContainsKey(key))
					return forEach[key];
				
				forEach.Add(key, new MultiArray());
					
				return forEach[key];
			}
			
			set
			{
				if (forEach.ContainsKey(key))
				{
					forEach[key].value = value;
					return;
				}
				
				forEach.Add(key, new MultiArray());
			}
			
		}
		
	}
			
	/*public class MyTests
	{
		
		public string testB = "xxx";
		
		public string this[string x]
		{
			get
			{
				
				return testB;
			}
			
			set
			{
				this.testB = value;
			}
			
		}
		
	}*/
	
	void Update()
	{
		//if (Input.GetKeyDown(Key.Letter.K))
			//Debug.Log(Save.GetString(GameComponents.User.TOKEN));
	}
	
}
