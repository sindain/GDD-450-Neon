using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : NetworkManager
{
  public enum GAME_MODE {None=0, Singleplayer, Multiplayer};
  public enum GAME_STATE {None=0, VehicleSelection, LevelSelection, SceneChange, RacePrep, Racing, RaceFinished}
  public enum TRANSITION {None=0, In, Out}

  public GameObject[] players;

  private readonly int PLAYER_COUNT = 8;
  private int iRaceCounter;
  private float fTimer;
  private float fUpdPlaceTime = 200.0f;
  public GAME_MODE GameMode;
  public GAME_STATE GameState;
  private string[] circuitScenes = new string[3]{"CitySmall", "CitySmall", "CityMed"};
  private string[] trackNames = new string[3]{"InfTrack", "T-Track", "OverUnder"};

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

  void Update(){
    //Update places
    if(fTimer + fUpdPlaceTime >= Time.time && GameState == GAME_STATE.Racing){
      fTimer = Time.time;
      UpdatePlaces ();
    }

  }
    
  //--------------------------------------------------------------------------------------------------------------------
  //Name:         OnServerAddPlayer
  //Description:  overriden function that handles what happens when a player is added to the server
  //Parameters:   NetworkConnection conn, short playerControllerId
  //Returns:      none
  //--------------------------------------------------------------------------------------------------------------------
  public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId){
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
  public override void OnServerRemovePlayer (NetworkConnection conn, UnityEngine.Networking.PlayerController player){
    base.OnServerRemovePlayer (conn, player);
    //players.Remove (player.gameObject);
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
    matchMaker.ListMatches (0, 5, "", OnMatchList);
  }

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

  public void StopLocalGame(){
    NetworkManager.singleton.StopHost ();
  }

  public void returnToMain(){
    if (matchMaker != null)
        print (true);
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

  public override void OnClientSceneChanged (NetworkConnection conn)
  {
    base.OnClientSceneChanged (conn);
    foreach(GameObject p in players){
      
      if (p == null)
        continue;
      NetPlayer _NetPlayer = p.GetComponent<NetPlayer> ();
      //Set up player if the connection matches or is not human.
      if (_NetPlayer.connectionToServer == conn && _NetPlayer.isHuman ())
        _NetPlayer.setPlayerState (NetPlayer.PLAYER_STATE.SceneLoaded);
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
      if (_NetPlayeri.PlayerState == NetPlayer.PLAYER_STATE.RaceFinished)
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
    case GAME_STATE.SceneChange:
      checkPlayersLoadedScene ();
      break;
    case GAME_STATE.RacePrep:
      checkPlayersRacePrep ();
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

  private void checkVehicleSelectReady (){
    if (!arePlayerStatesSynced (NetPlayer.PLAYER_STATE.VehicleSelectReady))
      return;
    
    //foreach (GameObject p in players){
    for(int i = 0; i < PLAYER_COUNT; i++){
      //Add NPC's for any empty spots
      if(players[i]==null){
        GameObject NpcPlayer = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetPlayer _NetPlayer = NpcPlayer.GetComponent<NetPlayer> ();

        NetworkServer.Spawn(NpcPlayer);
        _NetPlayer.Setup (i, false);
        players[i] = NpcPlayer;
      }

      players[i].GetComponent<NetPlayer> ().PlayerState = NetPlayer.PLAYER_STATE.LoadingScene;
    }
    GameState = GAME_STATE.SceneChange;
    ServerChangeScene (circuitScenes[iRaceCounter]);
  }

  private void checkLevelSelectReady(){
    if (!arePlayerStatesSynced (NetPlayer.PLAYER_STATE.LevelSelectReady))
      return;

    GameState = GAME_STATE.SceneChange;
  }

  private void checkPlayersLoadedScene(){
    if (!arePlayerStatesSynced(NetPlayer.PLAYER_STATE.SceneLoaded))
      return;

    //Everyone is ready
    GameState = GAME_STATE.RacePrep;
    foreach (GameObject p in players){
      if (p == null)
        continue;
      NetPlayer _NetPlayer = p.GetComponent<NetPlayer> ();
      _NetPlayer.RpcSetTrack (trackNames [iRaceCounter]);
      _NetPlayer.RpcGiveCarCameraControl ();
    }
  }

  private void checkPlayersRacePrep(){
    if(players[0].GetComponent<NetPlayer>().getPlayerState() == NetPlayer.PLAYER_STATE.Racing)
      
    //Check if everyone has started racing to change game state
    if (arePlayerStatesSynced (NetPlayer.PLAYER_STATE.Racing))
      GameState = GAME_STATE.Racing;
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

  private void checkRaceFinished(){
    if (!arePlayerStatesSynced (NetPlayer.PLAYER_STATE.RaceFinished))
      return;

    //TODO:  Insert end race screen and transition phase
    int[] liPoints = {9,8,5,4,3,2,1,0};
    GameState = GAME_STATE.RaceFinished;
    foreach(GameObject p in players){
      NetPlayer _NetPlayer = p.GetComponent<NetPlayer>();
      _NetPlayer.incPoints(liPoints[_NetPlayer.iPlace-1]);
      if(_NetPlayer.isHuman())
        _NetPlayer.RpcShowScoreboard();
      else
        _NetPlayer.setPlayerState(NetPlayer.PLAYER_STATE.SceneChangeReady);
    } //End foreach(GameObject p in players)
  }

  private void checkChangeScene(){
    if (!arePlayerStatesSynced (NetPlayer.PLAYER_STATE.SceneChangeReady))
      return;
    
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

}
