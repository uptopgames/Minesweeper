using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {
	
	public Transform target;
	private Vector3 lastPosition = new Vector3(0,0,0);
	
	// Use this for initialization
	void Start ()
	{
		InvokeRepeating("Refresh", 0, 0.25f);
	}
	
	// Update is called once per frame
	void Refresh ()
	{
		if(lastPosition != target.position)
		{
			lastPosition = target.position;
			
			Vector3 point = target.position;
			point.x = transform.position.x;
			transform.LookAt(point);
			transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 180, transform.rotation.eulerAngles.z));
		}
	}
}
