using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetPlayer : NetworkBehaviour
{
  [SyncVar]
  public int iShipChoice = -1;
  [SyncVar]
  public int iPlayerNum = -1;
  public GameObject ship;

  void Start (){
    
  }

  void Update (){
  }

  public void setShipChoice (int piChoice){
    iShipChoice = piChoice;
    CmdChangeShip (piChoice);
  }

  public void setPlayerNum (int piPlayerNum){iPlayerNum = piPlayerNum;}
  public int getPlayerNum(){return iPlayerNum;}

  public void Setup(int piPlayerNum){
    setPlayerNum(piPlayerNum);
    RpcSetup(piPlayerNum);
  }

  [ClientRpc]
  public void RpcSetup (int iNum){
    if (!isLocalPlayer)
      return;

    Transform target = GameObject.Find("Bays").transform.GetChild(iNum);
    GameObject.Find("MainMenu").GetComponent<MenuScript>().setCameraTarget(target.position, target.rotation);
    GameObject.Find ("MainMenu").transform.FindChild ("MultiplayerLobby").gameObject.SetActive (false);
    GameObject.Find ("MainMenu").transform.FindChild ("VehicleSelection").gameObject.SetActive (true);
    //GameObject.Find ("MainMenu").GetComponent<MenuScript> ().setCameraTarget (pos, rot);

  }


  [Command]
  private void CmdChangeShip (int piChoice){
    if (ship != null)
      Destroy (ship);
    GameObject[] shipArray = GameObject.Find ("GameManager").GetComponent<GameManager> ().spawnPrefabs.ToArray ();
    Vector3 spawnPos = GameObject.Find ("Bays").transform.GetChild (iPlayerNum).transform.FindChild ("ShipSpawn").transform.position;
    GameObject newShip = (GameObject)Instantiate (
                           shipArray [piChoice],
                           spawnPos,
                           Quaternion.identity);
    
    ship = newShip;
    NetworkServer.Spawn (newShip);
  }
}
