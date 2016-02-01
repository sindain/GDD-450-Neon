using UnityEngine;
using System.Collections;

public class VehicleSelection : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void shipButtonClick(int piChoice) {
    GameObject[] players = GameObject.FindGameObjectsWithTag("NetPlayer");
    
    foreach(GameObject i in players){
    	NetPlayer netPlayer = i.GetComponent<NetPlayer>();
      print (players);
    	if(netPlayer.isLocalPlayer)
    		netPlayer.setShipChoice(piChoice);
    }	
  }
}
