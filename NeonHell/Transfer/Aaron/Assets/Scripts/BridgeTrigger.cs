using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BridgeTrigger : NetworkBehaviour {
	public GameObject Bridge;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	}

	[Command]
	public void CmdBridgeOff(){
		Bridge.gameObject.SetActive (false);
		RpcBridgeOff();
	}
	[ClientRpc]
	public void RpcBridgeOff(){
		Bridge.gameObject.SetActive (false);
	}

	[Command]
	public void CmdBridgeOn(){
		Bridge.gameObject.SetActive (true);
		RpcBridgeOn ();
	}
	[ClientRpc]
	public void RpcBridgeOn(){
		Bridge.gameObject.SetActive (true);
	}
}
