using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : NetworkManager
{
  public enum GAME_MODE {None=0, Singleplayer, Multiplayer};
  public enum GAME_STATE {
    None=0, 
    VehicleSelection, 
    LevelSelection, 
    SceneOutro,
    SceneChange, 
    SceneIntro,
    RaceReady, 
    Countdown,
    Racing, 
    RaceFinished}

  public GameObject[] players;

  private readonly int PLAYER_COUNT = 8;
  public int iRaceCounter;
  private float fTimer;
  private float fUpdPlaceTime = 0.333f;
  public float fRaceOverTimer;
  public GAME_MODE GameMode;
  public GAME_STATE GameState;
  public readonly string[] CIRCUIT_SCENES = new string[9]{"CitySmall", "CitySmall", "CityMed", //He Circuit
                                                 "CitySmall", "CityMed", "CityLarge", //Ne Circuit
                                                 "CityMed", "CityLarge", "CityLarge", //Ar Circuit
														}; //Xe Circuit
  public readonly string[] TRACK_NAMES = new string[9]{"InfTrack", "T-Track", "OverUnder", //He Circuit
                                                        "T-Split", "Mobius","JumpBridge", //Ne Circuit
														"LoopTheLoop","ThreadTheNeedle","Towertrack", //Ar Circuit
														}; 
  public static readonly int[] POINTS = {9,8,5,4,3,2,1,0};
  public string[] circuitScenes;
  public string[] trackNames;

  //--------------------------------------------------------------------------------------------------------------------
  //Name:         Start
  //Description:  Default start function, nothing special here
  //Parameters:   NA
  //Returns:      NA
  //--------------------------------------------------------------------------------------------------------------------
  void Start (){
    iRaceCounter = 0;
    fTimer = Time.time;
    GameMode = GAME_MODE.None;
    GameState = GAME_STATE.None;
    players = new GameObject[PLAYER_COUNT];
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  void Update(){
    //Update places
    if(fTimer + fUpdPlaceTime <= Time.time && GameState == GAME_STATE.Racing){
      fTimer = Time.time;
      UpdatePlaces ();
    }

    //Check race over timer
    if(fRaceOverTimer > 0){ //countdown has begun
      //Race timer has ended
      if(fRaceOverTimer - Time.deltaTime <= 0){

        //Check each place starting with last place
        for(int i=8; i>1; i--){
          foreach(GameObject p in players){
            if (p == null)
              continue;
            NetPlayer _NP = p.GetComponent<NetPlayer> ();
            if (_NP.PlayerState == NetPlayer.PLAYER_STATE.Racing && _NP.getPlace() == i)
              _NP.setPlayerState (NetPlayer.PLAYER_STATE.RaceFinished);
          } //End foreach(GameObject p in players)
        } //End for(int i=7; i>0; i--)

        fRaceOverTimer = 0.0f;
      }//End if(fRaceOverTimer - Time.deltaTime <= 0)
      else
        fRaceOverTimer -= Time.deltaTime;
    }//End if(fRaceOverTimer >= 0)

  }
    
  //--------------------------------------------------------------------------------------------------------------------
  //Name:         OnServerAddPlayer
  //Description:  overriden function that handles what happens when a player is added to the server
  //Parameters:   NetworkConnection conn, short playerControllerId
  //Returns:      none
  //--------------------------------------------------------------------------------------------------------------------
  public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId){
    if (GameState != GAME_STATE.VehicleSelection)
      return;

    GameObject player = (GameObject)Instantiate (playerPrefab, Vector3.zero, Quaternion.identity);
    player.GetComponent<NetPlayer> ().connection = conn;
    NetworkServer.AddPlayerForConnection (conn, player, playerControllerId);
    for (int i = 0; i < PLAYER_COUNT; i++) {
      if (players [i] == null) {
        players [i] = player;
        player.GetComponent<NetPlayer> ().Setup (i, true);
        break;
      } //End if (players [i] == null)
    } // End for (int i = 0; i < PLAYER_COUNT; i++)



    //If Single player, populate other seats with NPC's
    if(GameMode == GAME_MODE.Singleplayer){
      for(int i = 1; i<PLAYER_COUNT; i++){
        GameObject NpcPlayer = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetPlayer _NetPlayer = NpcPlayer.GetComponent<NetPlayer> ();
        _NetPlayer.connection = conn;
        NetworkServer.Spawn(NpcPlayer);
        _NetPlayer.Setup (i, false);
        players [i] = NpcPlayer;
      } //End for(int i = 1; i<PLAYER_COUNT; i++)
    } //End if(GameMode == GAME_MODE.Singleplayer)
  }//End public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId)

  //--------------------------------------------------------------------------------------------------------------------
  //Name:         OnServerRemovePlayer
  //Description:  Overriden function that handles what happens when a player is removed from the server.
  //Parameters:   NetworkConnection conn, UnityEngine.Networking.PlayerController player
  //Returns:      NA
  //--------------------------------------------------------------------------------------------------------------------
  public override void OnServerDisconnect (NetworkConnection conn){
    returnToMain();
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public override void OnMatchList (ListMatchResponse matchList){
    GameObject.Find ("MainMenu").transform.FindChild ("MultiplayerLobby").GetComponent<MultiplayerLobby> ().matchResponse (matchList);
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void StartMatchMaking (){
    StartMatchMaker ();
    refreshMatches ();
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void StopMatchmaking(){
    StopMatchMaker ();
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void refreshMatches (){
    matchMaker.ListMatches (0, 5, "", OnMatchList);
    GameObject.Find ("MainMenu").transform.FindChild ("MultiplayerLobby").GetComponent<MultiplayerLobby> ().matchRequest ();
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void StartMatchmakerGame (UnityEngine.Networking.Match.CreateMatchRequest pRequest){
    pRequest.size = 8;
    pRequest.advertise = true;
    if (pRequest.password == null)
      pRequest.password = "";
    matchMaker.CreateMatch (pRequest, OnMatchCreate);
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public override void OnMatchCreate (CreateMatchResponse matchInfo){
    base.OnMatchCreate (matchInfo);
    GameMode = GAME_MODE.Multiplayer;
    GameState = GAME_STATE.VehicleSelection;
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void JoinMatchmakerGame (UnityEngine.Networking.Match.MatchDesc desc){
    matchMaker.JoinMatch (desc.networkId, "", OnMatchJoined);
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void StartLocalGame (){
    GameMode = GAME_MODE.Singleplayer;
    GameState = GAME_STATE.VehicleSelection;
    NetworkManager.singleton.StartHost ();
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void StopLocalGame(){
    NetworkManager.singleton.StopHost ();
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void returnToMain(){
//    if (matchMaker != null)
//        print (true);
    if (NetworkManager.singleton.isNetworkActive)
      StopLocalGame ();
    iRaceCounter = 0;
    GameMode = GAME_MODE.None;
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void JoinLocalGame (){
    NetworkManager.singleton.StartClient ();
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public override void OnServerSceneChanged (string sceneName){
    base.OnServerSceneChanged (sceneName);

    //Set player state for NPC's to SceneLoaded as they are controlled by the server.
    foreach(GameObject p in players){
      if (p == null)
        continue;
      NetPlayer _NetPlayer = p.GetComponent<NetPlayer> ();
      if (!_NetPlayer.isHuman ())
        _NetPlayer.setPlayerState (NetPlayer.PLAYER_STATE.SceneLoaded);
    }
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public override void OnClientSceneChanged (NetworkConnection conn)
  {
    base.OnClientSceneChanged (conn);
    GameObject[] lPlayers = GameObject.FindGameObjectsWithTag("NetPlayer");

    foreach(GameObject p in lPlayers){
      if(p==null)
        continue;

      if(p.GetComponent<NetworkIdentity>().connectionToServer == conn)
        p.GetComponent<NetPlayer>().setPlayerState(NetPlayer.PLAYER_STATE.SceneLoaded);
    }
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void UpdatePlaces(){
    for (int i = 0; i < players.Length; i++) {
      //Don't process a player that doesn't exist
      if (players [i] == null)
        continue;
      
      NetPlayer _NetPlayeri = players [i].GetComponent<NetPlayer> ();
      //Don't change the place if the player is done racing
      if (_NetPlayeri.PlayerState != NetPlayer.PLAYER_STATE.Racing)
        continue;

      //loop through each other player and compare their number of waypoint hits.
      int liNewPlace = 1;
      for(int j = 0; j < players.Length; j++){
        if (players [j] == null || j == i)
          continue;
        NetPlayer _NetPlayerj = players [j].GetComponent<NetPlayer> ();
        //If other has finished race, increment place and continue to next
        if(_NetPlayerj.getPlayerState() == NetPlayer.PLAYER_STATE.RaceFinished){
          liNewPlace++;
          continue;
        }
        int iDiff = _NetPlayerj.getNumWaypointsHit () - _NetPlayeri.getNumWaypointsHit ();

        //If tied
        if(iDiff == 0){
          //Compare distance from the ships current waypoint
          float iDistance = (_NetPlayeri.ship.transform.position - _NetPlayeri.ship.GetComponent<PlayerController> ().getCurrentPoint ().transform.position).magnitude;
          float jDistance = (_NetPlayerj.ship.transform.position - _NetPlayerj.ship.GetComponent<PlayerController> ().getCurrentPoint ().transform.position).magnitude;
          //If other is further from their waypoint
          if (iDistance < jDistance)
            liNewPlace++;
        }
        //If other is ahead
        else if(iDiff > 0){
          liNewPlace++;
        } // End else if(iDiff > 0){
      } //End for(int j = i; j < players.Length; j++)
      _NetPlayeri.setPlace(liNewPlace);
    } //End for (int i = 0; i < players.Length; i++) 
  } // End public void UpdatePlaces()

//----------------------------------------------------------------------------------------------------------------------
//-----------------------------------State Synchronization--------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void checkPlayerStates(){
    switch (GameState){
    case GAME_STATE.VehicleSelection:
      checkVehicleSelectReady ();
      break;
    case GAME_STATE.LevelSelection:
      checkLevelSelectReady ();
      break;
    case GAME_STATE.SceneOutro:
      checkSceneOutroDone ();
      break;
    case GAME_STATE.SceneChange:
      checkPlayersLoadedScene ();
      break;
    case GAME_STATE.SceneIntro:
      checkSceneIntroDone ();
      break;
    case GAME_STATE.RaceReady:
      checkPlayersRaceReady ();
      break;
    case GAME_STATE.Countdown:
      break;
    case GAME_STATE.Racing:
      checkRaceFinished ();
      break;
    case GAME_STATE.RaceFinished:
      checkChangeScene ();
      break;
    case GAME_STATE.None:
      break;
    default:
      break;
    }
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  private bool arePlayerStatesSynced(NetPlayer.PLAYER_STATE pState){
    bool bResult = true;
    foreach(GameObject p in players){
      if (p == null)
        continue;
      if (p.GetComponent<NetPlayer> ().PlayerState != pState){
        bResult = false;
        break;
      }// End if (p.GetComponent<NetPlayer> ().PlayerState != pState)
    } //End foreach(GameObject p in players)
    return bResult;
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  private void checkVehicleSelectReady (){
    if (!arePlayerStatesSynced (NetPlayer.PLAYER_STATE.VehicleSelectReady))
      return;

    GameState = GAME_STATE.LevelSelection;

    //foreach (GameObject p in players){
    for(int i = 0; i < PLAYER_COUNT; i++){
      //Add NPC's for any empty spots
      if(players[i]==null){
        GameObject NpcPlayer = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetPlayer _NetPlayer = NpcPlayer.GetComponent<NetPlayer> ();

        NetworkServer.Spawn(NpcPlayer);
        _NetPlayer.Setup (i, false);
        players[i] = NpcPlayer;
        players[i].GetComponent<NetPlayer> ().setPlayerState(NetPlayer.PLAYER_STATE.LevelSelectReady);
      }
      else if (i==0){
        players[i].GetComponent<NetPlayer>().setPlayerState(NetPlayer.PLAYER_STATE.LevelSelect);
        players[i].GetComponent<NetPlayer>().RpcStartLevelSelection();
      } 
      else 
        players[i].GetComponent<NetPlayer>().setPlayerState(NetPlayer.PLAYER_STATE.LevelSelectReady);
    } //End for(int i = 0; i < PLAYER_COUNT; i++)
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  private void checkLevelSelectReady(){
    if (!arePlayerStatesSynced (NetPlayer.PLAYER_STATE.LevelSelectReady))
      return;

//    foreach(GameObject p in players)
//      p.GetComponent<NetPlayer>().setPlayerState(NetPlayer.PLAYER_STATE.SceneChangeReady);

    GameState = GAME_STATE.SceneOutro;
    foreach (GameObject p in players){
      NetPlayer _lNP = p.GetComponent<NetPlayer> ();
      _lNP.setPlayerState (_lNP.isHuman() ? NetPlayer.PLAYER_STATE.SceneOutro : NetPlayer.PLAYER_STATE.SceneChangeReady);
    }
    
//    GameState = GAME_STATE.SceneChange;
//    ServerChangeScene (circuitScenes[iRaceCounter]);
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  private void checkSceneOutroDone(){
    //Advance players states once everyone has played the outro
    //Player is advanced to SceneChangeRdy state by the NetPlayer script once fading out is done
    if (!arePlayerStatesSynced (NetPlayer.PLAYER_STATE.SceneChangeReady))
      return;

    foreach(GameObject p in players)
      p.GetComponent<NetPlayer>().setPlayerState(NetPlayer.PLAYER_STATE.LoadingScene);

    GameState = GAME_STATE.SceneChange;
    //If we'er in the main scene, just start races, otherwise we need to update race counter and such
    if(SceneManager.GetActiveScene().name == "_Main")
      ServerChangeScene (circuitScenes[iRaceCounter]);
    else{
      //Move to next scene
      iRaceCounter++;
      if (iRaceCounter >= circuitScenes.Length)
        returnToMain ();
      else{
        GameState = GAME_STATE.SceneChange;
        ServerChangeScene (circuitScenes [iRaceCounter]);
      }
    }
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  private void checkPlayersLoadedScene(){
    if (!arePlayerStatesSynced(NetPlayer.PLAYER_STATE.SceneLoaded))
      return;

    //Everyone has loaded the new scene
    GameState = GAME_STATE.SceneIntro;
    foreach (GameObject p in players){
      if (p == null)
        continue;
      NetPlayer _NetPlayer = p.GetComponent<NetPlayer> ();
      _NetPlayer.setRaceTime (0.0f);
      _NetPlayer.RpcSetTrack (trackNames [iRaceCounter]);
      _NetPlayer.RpcGiveCarCameraControl ();
    }
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  private void checkSceneIntroDone(){
    //Advance players states once everyone has played the into
    if (!arePlayerStatesSynced (NetPlayer.PLAYER_STATE.RaceReady))
      return;
    GameState = GAME_STATE.RaceReady;
    checkPlayersRaceReady ();
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  private void checkPlayersRaceReady(){
    //if(players[0].GetComponent<NetPlayer>().getPlayerState() == NetPlayer.PLAYER_STATE.Racing)
      
    //Check if everyone has started racing to change game state
    if (arePlayerStatesSynced (NetPlayer.PLAYER_STATE.Racing))
      GameState = GAME_STATE.Racing;
      fRaceOverTimer = 0.0f;
    if (!arePlayerStatesSynced (NetPlayer.PLAYER_STATE.RaceReady))
      return;

    foreach (GameObject p in players){
      if (p == null)
        continue;
      NetPlayer _NetPlayer = p.GetComponent<NetPlayer> ();
      _NetPlayer.RpcGiveCarCameraControl ();
      _NetPlayer.RpcStartRaceCountdown ();
    }
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  private void checkRaceFinished(){
    
    //Someone has finished the race, start the countdown timer if it hasn't started already.
    bool result = true;
    foreach (GameObject pl in players)
      if (pl.GetComponent<NetPlayer> ().getPlayerState () != NetPlayer.PLAYER_STATE.Racing)
        result = false;
    
    if(!result && fRaceOverTimer == 0.0f){
      fRaceOverTimer = 20.0f; //20s
      foreach(GameObject p in players){
        if(p==null)
          continue;
        p.GetComponent<NetPlayer>().RpcStartRaceOverTimer();
      }
    }

    if (!arePlayerStatesSynced (NetPlayer.PLAYER_STATE.RaceFinished))
      return;

    GameState = GAME_STATE.SceneOutro;
    foreach(GameObject p in players){
      NetPlayer _NetPlayer = p.GetComponent<NetPlayer>();
      _NetPlayer.incPoints(POINTS[_NetPlayer.iPlace-1] + (_NetPlayer.hasFlag() ? 4 : 0));
      _NetPlayer.incFlags (_NetPlayer.hasFlag()?1:0);
      if(_NetPlayer.isHuman())
        _NetPlayer.RpcShowScoreboard();
      else
        _NetPlayer.setPlayerState(NetPlayer.PLAYER_STATE.SceneChangeReady);
    } //End foreach(GameObject p in players)
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  private void checkChangeScene(){
    if (!arePlayerStatesSynced (NetPlayer.PLAYER_STATE.SceneChangeReady))
      return;
    print ("Changing scene");
    //Move to next scene
    iRaceCounter++;
    if (iRaceCounter >= circuitScenes.Length)
      returnToMain ();
    else{
      GameState = GAME_STATE.SceneChange;
      ServerChangeScene (circuitScenes [iRaceCounter]);
    }
  }

//----------------------------------------------------------------------------------------------------------------------
//--------------------------------------Getters and Setters-------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------
  public GAME_MODE getGameMode(){return GameMode;}
  public void setGameMode(GAME_MODE piGameMode){GameMode = piGameMode;}

  //[Command]
  public void CmdChangeCircuit(int piChoice){
    int liCircuitLength = piChoice == 4 ? 9 : 3;
    int liCircuitStart = piChoice == 4 ? 0 : 3 * piChoice;
    circuitScenes = new string[liCircuitLength];
    trackNames = new string[liCircuitLength];
    for (int i = 0; i < liCircuitLength; i++){
      circuitScenes [i] = CIRCUIT_SCENES [liCircuitStart + i];
      trackNames [i] = TRACK_NAMES [liCircuitStart + i];
    } //End for (int i = 0; i < liCircuitLength; i++)
  } //End public void CmdChangeCircuit(int piChoice)
}
