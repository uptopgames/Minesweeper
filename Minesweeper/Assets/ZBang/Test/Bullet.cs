using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	public GameObject triggerBlast;
	public GameObject targetBlast;
	public Transform target;
	
	public float speed = 100;
	public bool playerBullet = true;
	public bool dieOnTouch = false; 
	
	// Use this for initialization
	void Start () 
	{
		GameObject.Instantiate(triggerBlast, transform.position, transform.rotation);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(target)
		{
			transform.LookAt(target.position);
			rigidbody.velocity = transform.forward * speed;
		}
	}
	
	void OnTriggerEnter (Collider hit)
	{
		if(dieOnTouch) GameObject.Destroy(gameObject);
		
		if(playerBullet)
		{
			if (hit.transform.tag == "Enemy")
			{
				Debug.Log("morra inimigo!");
				hit.transform.GetComponent<Gunslinger>().EnterDeath();
				
				GameObject.Instantiate(targetBlast, transform.position, transform.rotation);
				
				GameObject.Destroy(gameObject);
				return;
			}
		}
		else
		{
			if (hit.transform.tag == "Player")
			{
				Debug.Log("morra jogador!");
				hit.transform.GetComponent<Gunslinger>().EnterDeath();
				
				GameObject.Instantiate(targetBlast, transform.position, transform.rotation);
				
				GameObject.Destroy(gameObject);
				return;
			}
		}
	}
}