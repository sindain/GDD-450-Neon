using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapSelection : MonoBehaviour {

  public GameManager GM;

  void Start(){
    GM = GameObject.Find("GameManager").GetComponent<GameManager>();
  }

  public void onMapButtonClicked(int piChoice){
    GM.CmdChangeCircuit (piChoice);
	transform.FindChild ("ContinueBack").FindChild("Ready").GetComponent<Button>().interactable = true;

  }

  public void onReadyButtonClicked(){
    GameObject[] players = GameObject.FindGameObjectsWithTag ("NetPlayer");

    foreach (GameObject i in players) {
      NetPlayer netPlayer = i.GetComponent<NetPlayer> ();
      if (netPlayer.isLocalPlayer)
        netPlayer.setPlayerState(NetPlayer.PLAYER_STATE.LevelSelectReady);
    } 
  }

  public void onBackButtonClicked(){
    GM.GameState = GameManager.GAME_STATE.VehicleSelection;
    GameObject[] players = GameObject.FindGameObjectsWithTag ("NetPlayer");

    foreach (GameObject i in players) {
      NetPlayer netPlayer = i.GetComponent<NetPlayer> ();
      netPlayer.setPlayerState(netPlayer.isHuman()?NetPlayer.PLAYER_STATE.VehicleSelect : NetPlayer.PLAYER_STATE.VehicleSelectReady);
    } 
  }

  public void returnToVehicleSelection(){
    transform.parent.FindChild ("VehicleSelection").gameObject.SetActive (true);
    transform.parent.FindChild("Wait").gameObject.SetActive(false);
    gameObject.SetActive (false);
  }
}
