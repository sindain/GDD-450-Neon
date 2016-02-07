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

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void changePlayerPortrait (int piPlayer, int piChoice){
    Color[] colors = new Color[11] {
      Color.red, 
      Color.green, 
      Color.blue, 
      Color.cyan, 
      Color.yellow, 
      Color.magenta, 
      new Color (1, 113.0f / 255.0f, 0), 
      Color.grey, 
      Color.grey,
      Color.grey,
      Color.grey
    };
    transform.FindChild ("Players").GetChild (piPlayer).GetComponent<Image> ().color = colors [piChoice];
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
