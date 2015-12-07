using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class NetworkedCameraScript : NetworkBehaviour {

    public bool canRender;

	// Use this for initialization
	void Start () {
        canRender = false;
	}
	
	// Update is called once per frame
	void Update () {

        if(canRender){
            GetComponent<Camera>().enabled = true;
        }
        else{
            GetComponent<Camera>().enabled = false;
        }
	}
}
