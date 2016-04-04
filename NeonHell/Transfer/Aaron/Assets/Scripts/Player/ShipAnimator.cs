using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class ShipAnimator : NetworkBehaviour {
	private Animator anim;
	private Rigidbody rb;
	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody> ();
		anim=transform.FindChild ("Model").GetChild(gameObject.GetComponent<PlayerController>().modelChild).GetComponent<Animator>();
	
	}
	
	// Update is called once per frame
	void Update () {
        anim = transform.FindChild("Model").GetChild(gameObject.GetComponent<PlayerController>().modelChild).GetComponent<Animator>();
		anim.SetFloat ("AnimSpeed", rb.velocity.magnitude/30);
	}

}
