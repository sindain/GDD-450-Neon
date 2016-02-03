using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetPlayer : NetworkBehaviour
{
  [SyncVar]
  public int iShipChoice = -1;
  [SyncVar]
  public int iSeat = -1;
  public GameObject ship;

  void Start (){
    
  }

  void Update (){
  }

  public void setShipChoice (int piChoice){
    iShipChoice = piChoice;
    CmdChangeShip (piChoice);
  }

  [ClientRpc]
  public void RpcSetSeat (int piSeat){
    iSeat = piSeat;

    Transform bayTransform = GameObject.Find ("Bays").transform.GetChild (iSeat).transform;
    UpdateCameraPosition (bayTransform.position, bayTransform.rotation);
  }

  public void UpdateCameraPosition (Vector3 pos, Quaternion rot){
    if (isLocalPlayer)
      GameObject.Find ("MainMenu").GetComponent<MenuScript> ().setCameraTarget (pos, rot);
    print ("test");
  }


  [Command]
  private void CmdChangeShip (int piChoice){
    if (ship != null)
      Destroy (ship);
    GameObject[] shipArray = GameObject.Find ("GameManager").GetComponent<GameManager> ().spawnPrefabs.ToArray ();
    Vector3 spawnPos = GameObject.Find ("Bays").transform.GetChild (iSeat).transform.FindChild ("ShipSpawn").transform.position;
    GameObject newShip = (GameObject)Instantiate (
                           shipArray [piChoice],
                           spawnPos,
                           Quaternion.identity);
    
    ship = newShip;
    NetworkServer.Spawn (newShip);
  }
}
