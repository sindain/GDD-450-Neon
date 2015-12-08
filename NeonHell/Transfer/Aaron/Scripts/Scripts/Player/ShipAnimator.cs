using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class ShipAnimator : NetworkBehaviour {
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
        RpcAnimate();
        CmdAnimate();
	}
    [Command]
    public void CmdAnimate()
    {
        anim.SetFloat("AnimSpeed", rb.velocity.magnitude / 30);
    }

    [ClientRpc]
    public void RpcAnimate()
    {
        anim.SetFloat("AnimSpeed", rb.velocity.magnitude / 30);
    }

}
