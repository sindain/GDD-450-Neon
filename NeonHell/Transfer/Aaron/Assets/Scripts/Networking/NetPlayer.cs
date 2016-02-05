using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class NetPlayer : NetworkBehaviour
{
  [SyncVar] public int iShipChoice = -1;
  [SyncVar] public int iPlayerNum = -1;
  [SyncVar] public int iNumWaypointsHit = 0;
  [SyncVar] public int iLap = 0;
  [SyncVar] public int iPlace = 0;
  [SyncVar] public bool bReady = false;
  public NetworkConnection connection;
  public GameObject ship;

  void Start (){
    DontDestroyOnLoad (transform.gameObject);
  }

  void Update (){}

  public void setShipChoice (int piChoice){CmdChangeShip (piChoice);}

  public bool getReady (){return bReady;}
  public void setReady (bool pbReady){CmdChangeReady (true);}

  public int getPlayerNum (){return iPlayerNum;}
  public void setPlayerNum (int piPlayerNum){iPlayerNum = piPlayerNum;}

  public int getLap (){return iLap;}
  public void setLap (int piLap){iLap = piLap;}

  public int getPlace (){return iPlace;}
  public void setPlace (int piPlace){iPlace = piPlace;}

  public int getNumWaypointsHit(){return iNumWaypointsHit;}
  public void incNumWaypointsHit(int piVal){iNumWaypointsHit += piVal;}
  public void setNumWaypointsHit(int piNumWaypointsHit){iNumWaypointsHit = piNumWaypointsHit;}

  public void Setup (int piPlayerNum){
    setPlayerNum (piPlayerNum);
    setPlace (piPlayerNum);
    RpcSetup (piPlayerNum);
  }

  [ClientRpc]
  public void RpcSetup (int iNum){
    //DontDestroyOnLoad (transform.gameObject);
    if (!isLocalPlayer)
      return;

    Transform target = GameObject.Find ("Garage").transform.FindChild ("CameraPositions").transform.GetChild (iNum);
    GameObject.Find ("MainMenu").GetComponent<MenuScript> ().setCameraTarget (target.position, target.rotation);
    GameObject.Find ("MainMenu").transform.FindChild ("MultiplayerLobby").gameObject.SetActive (false);
    GameObject.Find ("MainMenu").transform.FindChild ("VehicleSelection").gameObject.SetActive (true);
    //GameObject.Find ("MainMenu").GetComponent<MenuScript> ().setCameraTarget (pos, rot);

  }


  [Command]
  private void CmdChangeShip (int piChoice){
    iShipChoice = piChoice;
    if (ship != null)
      NetworkServer.Destroy (ship);
    GameObject selectedShip = GameObject.Find ("GameManager").GetComponent<GameManager> ().spawnPrefabs.ToArray () [piChoice];
    Transform spawnTrans = GameObject.Find ("Garage").transform.FindChild ("VehicleSpawnLocations").transform.GetChild (iPlayerNum).transform;
    GameObject newShip = (GameObject)Instantiate (
                           selectedShip,
                           spawnTrans.position,
                           spawnTrans.rotation);
    
    NetworkServer.SpawnWithClientAuthority (newShip, connectionToClient);
    RpcSetShip (newShip);
    RpcChangePortrait (iPlayerNum, piChoice);
  }

  [Command]
  private void CmdChangeReady (bool pbReady){
    bReady = pbReady;
    GameObject.Find ("GameManager").GetComponent<GameManager> ().checkReady ();
  }

  [ClientRpc]
  private void RpcChangePortrait (int piPlayer, int piChoice){
    GameObject.Find ("MainMenu").transform.FindChild ("VehicleSelection").GetComponent<VehicleSelection> ().changePlayerPortrait (piPlayer, piChoice);
  }

  [ClientRpc]
  public void RpcChangeScene (string pSceneName){
    SceneManager.LoadScene (pSceneName);
  }

  [ClientRpc]
  public void RpcSetShip (GameObject pNewShip){
    ship = pNewShip;
  }

  [ClientRpc]
  public void RpcGiveCarControl (){
    if (!isLocalPlayer)
      return;
    ship.GetComponent<PlayerController> ().setCanMove (true);
    ship.GetComponent<PlayerController> ().setCameraControl (true);
  }

  [ClientRpc]
  public void RpcRaceSetup(string trackName){
    //Find the spawn location
    //Transform spawnPositionGameObject.Find (trackName)
    //Set our ship in its correct spawn location
    //Find and set first waypoint
  }
}
