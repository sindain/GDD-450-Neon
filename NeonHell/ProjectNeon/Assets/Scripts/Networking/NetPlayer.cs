using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class NetPlayer : NetworkBehaviour
{
  public enum PLAYER_STATE {None = 0, 
    Testing, 
    VehicleSelect, 
    VehicleSelectReady, 
    LevelSelect, 
    LevelSelectReady, 
    SceneOutro,
    SceneChangeReady, 
    LoadingScene, 
    SceneLoaded, 
    SceneIntro,
    RaceReady, 
    Racing, 
    RaceFinished};

  [SyncVar] public int iShipChoice = -1;
  [SyncVar] public int iPlayerNum = -1;
  [SyncVar] public int iNumWaypointsHit = 0;
  [SyncVar] public int iLap = 0;
  [SyncVar] public int iPlace = 0;
  [SyncVar] public int iPoints = 0;
  [SyncVar] public int iFlags = 0;
  [SyncVar] public float fRaceTime;
  [SyncVar] public bool bHasFlag = false;
  [SyncVar] public bool bIsHuman = false;
  public NetworkConnection connection;
  [SyncVar] public GameObject ship;

  [SyncVar] public PLAYER_STATE PlayerState;
  public string trackName;

  private float fStartTimer;
  private float fFlagCD = 0.0f;
  private float fFlagCDTime = 1.5f;

  void Start (){
    DontDestroyOnLoad (transform.gameObject);
    //PlayerState = PLAYER_STATE.None;
  }

  void Update(){
    //Countdown timer transition into racing
    if(!bIsHuman && PlayerState == PLAYER_STATE.RaceReady && fStartTimer > 0){
      fStartTimer = fStartTimer - Time.deltaTime < 0 ? 0 :fStartTimer - Time.deltaTime;
      if(fStartTimer == 0)
        CmdChangeState(PLAYER_STATE.Racing);
    }
    //Increment race time
    if ((isLocalPlayer || isServer) && PlayerState == PLAYER_STATE.Racing){
      fRaceTime += Time.deltaTime;
    }
    //Decrement flag cooldown timer
    if(isServer && PlayerState == PLAYER_STATE.Racing && fFlagCD > 0.0f)
      fFlagCD = fFlagCD - Time.deltaTime <= 0.0f ? 0.0f : fFlagCD - Time.deltaTime;
      
  }

  public void Setup (int piPlayerNum, bool pbIsHuman){
    setPlayerNum (piPlayerNum);
    setPlace (piPlayerNum+1);
    setIsHuman (pbIsHuman);
    if (pbIsHuman)
      RpcSetup (piPlayerNum);
    else
      ChangeShip (Random.Range (0, 8));
  }

  public void setupRace(){
    //Find the spawn location
    GameObject track = GameObject.Find (trackName);
    if (track == null) {
      print ("ERROR: NetPlayer.56 - Specified Track '" + trackName + "' not found.");
      return;
    } // if (Track == null)
    Transform spawnPosition = track.transform.FindChild ("Start Positions").transform.GetChild (iPlace-1).transform;
    //Set our ship in its correct spawn location and reset values for next race
    ship.transform.position = spawnPosition.position;
    ship.transform.rotation = spawnPosition.rotation;
    ship.GetComponent<Rigidbody>().velocity = Vector3.zero;
    ship.GetComponent<ThrusterController> ().setbMagnetize (false);
    PlayerController _PlayerController = ship.GetComponent<PlayerController> ();
    ShipStats _ShipStats = ship.GetComponent<ShipStats> ();
    _PlayerController.setEnergy (_ShipStats.fMaxEnergy);
    _PlayerController.setHealth (_ShipStats.fMaxHealth);
    _PlayerController.setCurrentThrust (0.0f);
    if (bHasFlag)
      toggleFlag ();
    
    //Find and set first waypoint
    ship.GetComponent<PlayerController> ().setCurrentPoint (track.transform.GetChild (0).FindChild ("start_finish").FindChild ("Waypoints").GetChild (0).gameObject);

    iLap = 0;
    iNumWaypointsHit = 0;

    if (!isLocalPlayer)
      return;

    //Make connection to HUD
    GameObject Hud = GameObject.Find ("UI");
    if (Hud == null) {
      print ("Error: NetPlayer.69 - No HUD found in scene");
      return;
    }

    SpHUD _Hud = Hud.GetComponent<SpHUD> ();
    if (_Hud != null){
      _Hud.setNetPlayer (this);
    }
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
      CmdChangeState(PLAYER_STATE.VehicleSelectReady);
  }

  //Make faders connection to this object and start fader
  private void activateFader(Fader.FADE_STATE pState){
    if (isLocalPlayer && bIsHuman){
      GameObject goFader;
      //Get the fader

      if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName ("_Main"))
        goFader = GameObject.Find ("MainMenu").transform.FindChild ("Fader").gameObject;
      else
        goFader = GameObject.Find ("UI").transform.FindChild ("Canvas").FindChild ("Fader").gameObject;
      
      if (pState == Fader.FADE_STATE.FadeOut)
        goFader.SetActive (true);
        
      Fader fader = goFader.GetComponent<Fader>();//GameObject.FindGameObjectWithTag ("Fader").GetComponent<Fader> ();
      fader._NP = this;
      fader.setFadeState (pState, true);
    }
  }

  [Command]
  private void CmdChangeShip (int piChoice){
    ChangeShip (piChoice);
  }

  [Command]
  private void CmdChangeState(PLAYER_STATE state){
    setPlayerState (state);
  }

  [Command] 
  public void CmdUpdNumWaypointsHit(int iNum){
    iNumWaypointsHit = iNum;
    GameObject.Find ("GameManager").GetComponent<GameManager> ().UpdatePlaces ();
  }
  [Command] 
  public void CmdUpdLap(int piLaps){
    iLap = piLaps;
    //Check if player finished the race
    if(iLap >= GameObject.Find(trackName).GetComponent<TrackInfo>().NumberOfLaps)
      CmdChangeState(PLAYER_STATE.RaceFinished);
  }

  [Command]
  public void CmdUpdRaceTime(float pfValue){
    setRaceTime (pfValue);
  }

  [Command]
  public void CmdRequestFlagChange(int piOtherPlayerNum){
    GameManager GM = GameObject.Find ("GameManager").GetComponent<GameManager> ();
    NetPlayer otherNP = GM.players [piOtherPlayerNum].GetComponent<NetPlayer>();
    if(otherNP.fFlagCD == 0.0f && otherNP.hasFlag()){
      toggleFlag ();
      otherNP.toggleFlag ();
    }
  }

  [Command]
  public void CmdCaptureFlag(){
    bool lbResult = false;
    //Check each player to see if anyone has flag
    foreach (GameObject p in GameObject.Find("GameManager").GetComponent<GameManager>().players)
      if (p.GetComponent<NetPlayer> ().hasFlag ())
        lbResult = true;
    //If no one has the flag, then this player has it.
    if (!lbResult){
      toggleFlag ();
    }
  }

  [ClientRpc]
  public void RpcSetup (int iNum){
    //DontDestroyOnLoad (transform.gameObject);
    if (!isLocalPlayer)
      return;

    Transform target = GameObject.Find ("Garage").transform.FindChild ("CameraPositions").transform.GetChild (iNum);
    GameObject.Find ("MainMenu").GetComponent<MenuScript> ().setCameraTarget (target.position, target.rotation);
    GameObject.Find ("MainMenu").transform.FindChild ("MultiplayerLobby").gameObject.SetActive (false);
    GameObject.Find ("MainMenu").transform.FindChild ("Wait").gameObject.SetActive (false);
    GameObject.Find ("MainMenu").transform.FindChild ("VehicleSelection").gameObject.SetActive (true);
    //GameObject.Find ("MainMenu").GetComponent<MenuScript> ().setCameraTarget (pos, rot);
  }  

  [ClientRpc]
  public void RpcSetTrack(string pTrack){
    trackName = pTrack;
    setupRace ();
    setPlayerState (bIsHuman ? PLAYER_STATE.SceneIntro : PLAYER_STATE.RaceReady);
  }

  [ClientRpc]
  public void RpcStartRaceCountdown(){
    if(!bIsHuman)
      fStartTimer = 3.0f;
      
    if(!isLocalPlayer)
      return;
    
    GameObject.Find ("UI").GetComponent<SpHUD> ().startCountdown ();
  }

  [ClientRpc]
  public void RpcShowScoreboard(){
    if(!isLocalPlayer)
      return;
    GameObject.Find ("UI").GetComponent<SpHUD> ().startScoreboard ();
  }

  [ClientRpc]
  private void RpcChangePortrait (int piPlayer, int piChoice){
    GameObject.Find ("MainMenu").transform.FindChild ("VehicleSelection").GetComponent<VehicleSelection> ().changePlayerPortrait (piPlayer, piChoice);
  }

  [ClientRpc]
  public void RpcStartLevelSelection(){
    if (!bIsHuman)
      return;

    //Turn vehicle selection off
    GameObject.Find ("MainMenu").transform.FindChild ("VehicleSelection").gameObject.SetActive(false);
    if (isLocalPlayer)
      GameObject.Find ("MainMenu").transform.FindChild ("MapSelection").gameObject.SetActive (true);
    else{
      GameObject wait = GameObject.Find ("MainMenu").transform.FindChild ("Wait").gameObject;
      wait.SetActive (true);
      wait.transform.FindChild ("Message").gameObject.GetComponent<Text> ().text = "Player 1 is choosing a circuit.";
    } //End else
  } //End public void RpcStartLevelSelection()

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

  [ClientRpc]
  public void RpcStartRaceOverTimer(){
    if(!isLocalPlayer)
      return;
    GameObject.Find("UI").GetComponent<SpHUD>().startRaceOverTimer();
  }

  [ClientRpc]
  public void RpcVehicleReady(bool pbValue){
    GameObject.Find("MainMenu").transform.FindChild("VehicleSelection").FindChild("Players").GetChild(iPlayerNum).FindChild("Background").GetComponent<Image>().color = new Color(38.0f/255.0f,198.0f/255.0f,0.0f,pbValue?255.0f/255.0f:0.0f);
  }

  [ClientRpc]
  public void RpcReturnToVehicleSelection(){
    if(isLocalPlayer && bIsHuman)
      GameObject.Find("MainMenu").transform.FindChild("MapSelection").GetComponent<MapSelection>().returnToVehicleSelection();
  }

  [ClientRpc]
  public void RpcActivateFader(Fader.FADE_STATE pState){
    activateFader (pState);
  }

//----------------------------------------------Getters and Setters-----------------------------------------------------

  public int getShipChoice(){return iShipChoice;}
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

  public int getFlags(){return iFlags;}
  public void setFlags(int piValue){iFlags = piValue;}
  public void incFlags(int piValue){iFlags += piValue;}

  public float getRaceTime(){return fRaceTime;}
  public void setRaceTime(float pfValue){fRaceTime = pfValue;}

  public bool hasFlag(){return bHasFlag;}
  public void setHasFlag(bool pbHasFlag){bHasFlag = pbHasFlag;}
  public void toggleFlag(){
    bHasFlag = !bHasFlag;
    fFlagCD = fFlagCDTime;
    ship.GetComponent<PlayerController> ().transform.FindChild ("Flag").gameObject.SetActive (bHasFlag);
    RpcToggleFlag (bHasFlag);

  }
  [ClientRpc]
  public void RpcToggleFlag(bool val){
    ship.GetComponent<PlayerController> ().transform.FindChild ("Flag").gameObject.SetActive (val);
  }

  public bool isHuman(){return bIsHuman;}
  public void setIsHuman(bool pbIsHuman){bIsHuman = pbIsHuman;}

  public PLAYER_STATE getPlayerState(){return PlayerState;}
  public void setPlayerState(PLAYER_STATE pState){
    if (isServer){
      PlayerState = pState;
      GameObject.Find ("GameManager").GetComponent<GameManager> ().checkPlayerStates ();

      switch(pState){
      case PLAYER_STATE.VehicleSelectReady:
        RpcVehicleReady (true);
        break;
      case PLAYER_STATE.VehicleSelect:
        RpcVehicleReady (false);      
        RpcReturnToVehicleSelection ();
        break;
      case PLAYER_STATE.RaceFinished:
        setRaceTime (fRaceTime);
        break;
      case PLAYER_STATE.SceneOutro:
        RpcActivateFader (Fader.FADE_STATE.FadeOut);
        break;
      case PLAYER_STATE.SceneIntro:
        RpcActivateFader (Fader.FADE_STATE.FadeIn);
        break;
      default:
        break;
      }
//      if (pState == PLAYER_STATE.VehicleSelectReady)
//      else if (pState == PLAYER_STATE.VehicleSelect){
//      }
//      else if (pState == PLAYER_STATE.RaceFinished)
//      else if (pState == PLAYER_STATE.SceneOutro)
//      else if (pState == PLAYER_STATE.SceneIntro)
    }//End if(isServer)
    else{
      print (isServer + " " + iPlayerNum);
      CmdChangeState (pState);    
    }
//    
//    if (pState == PLAYER_STATE.SceneOutro || pState == PLAYER_STATE.SceneIntro){
//      if (isServer && isLocalPlayer)
//        activateFader ();
//      else if (isServer && !isLocalPlayer)
//        RpcActivateFader ();
//    }
  }
	public void OnExitClicked (){
		CmdPeacOut ();
	}
	[Command]
	public void CmdPeacOut(){
		GameObject.Find ("GameManager").GetComponent<GameManager> ().returnToMain ();
	}
}
