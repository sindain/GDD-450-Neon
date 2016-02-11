using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VehicleSelection : MonoBehaviour
{

  // Use this for initialization
  void Start (){
	
  }
	
  // Update is called once per frame
  void Update (){
  }

  public void shipButtonClick (int piChoice){
    GameObject[] players = GameObject.FindGameObjectsWithTag ("NetPlayer");
    
    foreach (GameObject i in players) {
      NetPlayer netPlayer = i.GetComponent<NetPlayer> ();
      if (netPlayer.isLocalPlayer)
        netPlayer.setShipChoice (piChoice);
    }	
  }

  public void onBackClicked(){
    GameObject.Find ("GameManager").GetComponent<GameManager> ().returnToMain ();
    transform.parent.FindChild ("Title").gameObject.SetActive (true);
    gameObject.SetActive (false);
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void changePlayerPortrait (int piPlayer, int piChoice){    
    transform.FindChild("Players").GetChild(piPlayer).GetComponent<Image>().sprite = 
      transform.FindChild("Vehicles").GetChild(piChoice).GetComponent<Image>().sprite;
  }

  public void onReadyClicked (){
    GameObject[] players = GameObject.FindGameObjectsWithTag ("NetPlayer");

    foreach (GameObject i in players) {
      NetPlayer netPlayer = i.GetComponent<NetPlayer> ();
      if (netPlayer.isLocalPlayer)
        netPlayer.setReady (true);
    } 
  }
}
