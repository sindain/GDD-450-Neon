using UnityEngine;
using System.Collections;

public class MapSelection : MonoBehaviour {

  public void onMapButtonClicked(int piChoice){
    GameObject.Find ("GameManager").GetComponent<GameManager> ().CmdChangeCircuit (piChoice);
  }
}
