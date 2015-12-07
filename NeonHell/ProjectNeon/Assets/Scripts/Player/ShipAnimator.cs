using UnityEngine;
using System.Collections;

public class ShipAnimator : MonoBehaviour {
	private Animator anim;
	private Rigidbody rb;
	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody> ();
		anim=transform.FindChild ("Model").GetComponent<Animator> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		anim.SetFloat ("AnimSpeed", rb.velocity.magnitude/30);
	
	}
}
