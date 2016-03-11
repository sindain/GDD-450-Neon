using UnityEngine;
using System.Collections;

public class MapSelection : MonoBehaviour {

  public GameManager GM;

  void Start(){
    GM = GameObject.Find("GameManager").GetComponent<GameManager>();
  }

  public void onMapButtonClicked(int piChoice){
    GM.CmdChangeCircuit (piChoice);
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
      if (netPlayer.isLocalPlayer)
        netPlayer.setPlayerState(NetPlayer.PLAYER_STATE.VehicleSelect);
    } 
    transform.parent.FindChild ("VehicleSelection").gameObject.SetActive (true);
    gameObject.SetActive (false);
  }
}
