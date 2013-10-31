using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour
{
	private bool timeOut = false;
	private Rigidbody ragdoll;
	public GameObject blast;
	public float lifeTime = 6;
	
	void Start()
	{
		GameObject.Destroy(this, lifeTime);
	}
	
	public void OnTriggerEnter(Collider hit)
	{
		if(timeOut) return;
		
		if(hit.tag == "Ragdoll")
		{
			GameObject.Instantiate(blast, transform.position, transform.rotation);
			ragdoll = hit.rigidbody;
			timeOut = true;
			Invoke("Explode", 0.1f);
		}
	}
	
	public void Explode()
	{
		Debug.Log("once");
		Vector3 blastCenter = transform.position;
		blastCenter.x += UnityEngine.Random.Range(-1,1);
		blastCenter.y += UnityEngine.Random.Range(-1,-0.1f);
		blastCenter.z += UnityEngine.Random.Range(-1,1);
		ragdoll.AddExplosionForce(UnityEngine.Random.Range(30000,15000), blastCenter, 10);
		Invoke("PrepareForNextBlast", 1);
	}
	
	public void PrepareForNextBlast()
	{
		timeOut = false;
	}
}