using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class NetPlayer : NetworkBehaviour
{
  public enum PLAYER_STATE {None = 0, Testing, VehicleSelect, VehicleSelectReady, LevelSelect, LevelSelectReady, 
                            SceneChangeReady, LoadingScene, SceneLoaded, RaceReady, Racing, RaceFinished};

  [SyncVar] public int iShipChoice = -1;
  [SyncVar] public int iPlayerNum = -1;
  [SyncVar] public int iNumWaypointsHit = 0;
  [SyncVar] public int iLap = 0;
  [SyncVar] public int iPlace = 0;
  [SyncVar] public int iPoints = 0;
  [SyncVar] public bool bIsHuman = false;
  public NetworkConnection connection;
  [SyncVar] public GameObject ship;

  [SyncVar] public PLAYER_STATE PlayerState;
  public string trackName;

  private float fStartTimer;

  void Start (){
    DontDestroyOnLoad (transform.gameObject);
    //PlayerState = PLAYER_STATE.None;
  }

  void Update(){
    if(!bIsHuman && PlayerState == PLAYER_STATE.RaceReady && fStartTimer > 0){
      fStartTimer = fStartTimer - Time.deltaTime < 0 ? 0 :fStartTimer - Time.deltaTime;
      if(fStartTimer == 0)
        CmdChangeState(PLAYER_STATE.Racing);
    }
    
  }

  public void Setup (int piPlayerNum, bool pbIsHuman){
    setPlayerNum (piPlayerNum);
    setPlace (piPlayerNum+1);
    setIsHuman (pbIsHuman);
    if (pbIsHuman)
      RpcSetup (piPlayerNum);
    else
      ChangeShip (Random.Range (0, 7));
  }

  public void setupRace(){
    //Find the spawn location
    GameObject track = GameObject.Find (trackName);
    if (track == null) {
      print ("ERROR: NetPlayer.56 - Specified Track '" + trackName + "' not found.");
      return;
    } // if (Track == null)
    Transform spawnPosition = track.transform.FindChild ("Start Positions").transform.GetChild (iPlace-1).transform;
    //Set our ship in its correct spawn location
    ship.transform.position = spawnPosition.position;
    ship.transform.rotation = spawnPosition.rotation;
    ship.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //Find and set first waypoint
    ship.GetComponent<PlayerController> ().setCurrentPoint (track.transform.GetChild (0).FindChild ("start_finish").FindChild ("Waypoints").GetChild (0).gameObject);
    //TODO: Move this to its appropriate place later.
    iLap = 0;
    iNumWaypointsHit = 0;

    if (!isLocalPlayer)
      return;

    GameObject Hud = GameObject.Find ("HUD");
    if (Hud == null) {
      print ("Error: NetPlayer.69 - No HUD found in scene");
      return;
    }
    SpHUD _Hud = Hud.GetComponent<SpHUD> ();
    _Hud._NetPlayer = this;
  }

//  void OnLevelWasLoaded(int level) {
//    setupRace ();
//    CmdChangeState (PLAYER_STATE.Racing);
//  }

  [Command]
  private void CmdChangeShip (int piChoice){
    ChangeShip (piChoice);
  }

  private void ChangeShip(int piChoice){
    iShipChoice = piChoice;
    if (ship != null)
      NetworkServer.Destroy (ship);
    GameObject selectedShip = GameObject.Find ("GameManager").GetComponent<GameManager> ().spawnPrefabs.ToArray () [piChoice];
    Transform spawnTrans = GameObject.Find ("Garage").transform.FindChild ("VehicleSpawnLocations").transform.GetChild (iPlayerNum).transform;
    GameObject newShip = (GameObject)Instantiate (
      selectedShip,
      spawnTrans.position,
      spawnTrans.rotation);
    if(bIsHuman)
      NetworkServer.SpawnWithClientAuthority (newShip, connectionToClient);
    else
      NetworkServer.Spawn(newShip);
    ship = newShip;
    RpcSetShip (newShip);
    RpcChangePortrait (iPlayerNum, piChoice);
    if (!bIsHuman && isServer)
      PlayerState = PLAYER_STATE.VehicleSelectReady;
  }

  [Command]
  private void CmdChangeState(PLAYER_STATE state){
    PlayerState = state;
    GameObject.Find ("GameManager").GetComponent<GameManager> ().checkPlayerStates ();
  }

  [Command] public void CmdUpdNumWaypointsHit(int iNum){
    iNumWaypointsHit = iNum;
    GameObject.Find ("GameManager").GetComponent<GameManager> ().UpdatePlaces ();
  }
  [Command] public void CmdUpdLap(int piLaps){
    iLap = piLaps;
    //Check if player finished the race
    if(iLap >= 1)//GameObject.Find(trackName).GetComponent<TrackInfo>().NumberOfLaps)
      CmdChangeState(PLAYER_STATE.RaceFinished);
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

  [ClientRpc]
  public void RpcSetTrack(string pTrack){
    trackName = pTrack;
    setupRace ();
    CmdChangeState (PLAYER_STATE.RaceReady);
  }

  [ClientRpc]
  public void RpcStartRaceCountdown(){
    if(!bIsHuman)
      fStartTimer = 4.0f;
      
    if(!isLocalPlayer)
      return;
    GameObject.Find ("HUD").GetComponent<SpHUD> ().startCountdown ();
  }

  [ClientRpc]
  public void RpcShowScoreboard(){
    if(!isLocalPlayer)
      return;
    GameObject.Find("HUD").GetComponent<SpHUD>().startScoreboard();
  }

  [ClientRpc]
  private void RpcChangePortrait (int piPlayer, int piChoice){
    GameObject.Find ("MainMenu").transform.FindChild ("VehicleSelection").GetComponent<VehicleSelection> ().changePlayerPortrait (piPlayer, piChoice);
  }

  [ClientRpc]
  public void RpcSetShip (GameObject pNewShip){
    //ship = pNewShip;
    pNewShip.GetComponent<PlayerController>().setNetPlayer(this);
  }

  [ClientRpc]
  public void RpcGiveCarCameraControl (){
    if (!isLocalPlayer)
      return;
    ship.GetComponent<PlayerController> ().setCameraControl (true);
  }

//----------------------------------------------Getters and Setters-----------------------------------------------------

  public void setShipChoice (int piChoice){CmdChangeShip (piChoice);}

  public int getPlayerNum (){return iPlayerNum;}
  public void setPlayerNum (int piPlayerNum){iPlayerNum = piPlayerNum;}

  public int getLap (){return iLap;}
  public void incLap(){
    if(!isLocalPlayer && !isServer)
      return;
    iLap++;    
    CmdUpdLap(iLap);
  }
  public void setLap (int piLap){iLap = piLap;}

  public int getPlace (){return iPlace;}
  public void setPlace (int piPlace){iPlace = piPlace;}

  public int getNumWaypointsHit(){return iNumWaypointsHit;}
  public void incNumWaypointsHit(int piVal){
    if(!isLocalPlayer && !isServer)
      return;
    iNumWaypointsHit += piVal;
    CmdUpdNumWaypointsHit(iNumWaypointsHit);
  }
  public void setNumWaypointsHit(int piNumWaypointsHit){
    if(!isLocalPlayer && !isServer)
      return;
    iNumWaypointsHit = piNumWaypointsHit;
    CmdUpdNumWaypointsHit(iNumWaypointsHit);
  }

  public int getPoints(){return iPoints;}
  public void setPoints(int piPoints){iPoints = piPoints;}
  public void incPoints(int piValue){iPoints += piValue;}

  public bool isHuman(){return bIsHuman;}
  public void setIsHuman(bool pbIsHuman){bIsHuman = pbIsHuman;}

  public PLAYER_STATE getPlayerState(){return PlayerState;}
  public void setPlayerState(PLAYER_STATE pState){CmdChangeState (pState);}
}
