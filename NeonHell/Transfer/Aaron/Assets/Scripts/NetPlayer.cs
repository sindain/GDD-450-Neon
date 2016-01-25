using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetPlayer : NetworkBehaviour {
  [SyncVar]
  public int iShipChoice = -1;
  public int iSeat = -1;
  public GameObject ship;
  
  void Start(){
    
  }
  
  void Update(){
  }

  public void setShipChoice(int piChoice) {
    iShipChoice = piChoice;
    CmdChangeShip(piChoice);
  }
  
  [Command]
  private void CmdChangeShip(int piChoice){
    if(ship != null)
      Destroy(ship);
    GameObject[] shipArray = GameObject.Find("GameManager").GetComponent<GameManager>().spawnPrefabs.ToArray();
    Vector3 spawnPos = GameObject.Find("Bays").transform.GetChild(iSeat).transform.FindChild("ShipSpawn").transform.position;
    GameObject newShip = (GameObject) Instantiate(
      shipArray[piChoice],
      spawnPos,
      Quaternion.identity);
    
    ship = newShip;
    NetworkServer.Spawn(newShip);
  }
}
