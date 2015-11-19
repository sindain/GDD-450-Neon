using UnityEngine;
using System.Collections;

public class BulletMover : MonoBehaviour {
	private GameObject rotator;
	private GameObject player;
	public int speed;
	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag ("Player");
		transform.forward = GameObject.FindWithTag("ShotSpawn").transform.forward;
		rotator = GameObject.FindWithTag ("GameController");
		transform.parent = rotator.transform;
		GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Enemy"||other.gameObject.tag == "Finish") 
		{
			Destroy(other.gameObject);
			if (other.gameObject.tag == "Enemy")
			{
				Destroy(gameObject);
			}
		}
	}
}
