using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : NetworkManager
{
  public enum GAME_MODE {None=0, Singleplayer, Multiplayer};

  public GameObject[] players;

  private readonly int PLAYER_COUNT = 8;
  private int iRaceCounter;
  public GAME_MODE GameMode;
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
    GameMode = GAME_MODE.None;
    players = new GameObject[PLAYER_COUNT];
    //returnToMain ();
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
        NetworkServer.SpawnWithClientAuthority (NpcPlayer, conn);
        players [i] = NpcPlayer;
        _NetPlayer.Setup (i, false);
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
    NetworkManager.singleton.StartHost ();
  }

  public void StopLocalGame(){
    NetworkManager.singleton.StopHost ();
  }

  public void returnToMain(){
    if (NetworkManager.singleton.isNetworkActive)
      StopLocalGame ();
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
  public void checkReady (){
    bool allReady = true;
    foreach (GameObject player in players)
      if (player != null) {
        if (player.GetComponent<NetPlayer> ().PlayerState != NetPlayer.PLAYER_STATE.VehicleReady)
          allReady = false;
      }

    if (!allReady)
      return;
    
    changeScene ();
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void checkRaceDone(){
    bool bDone = true; //Start pessimistic
    foreach(GameObject p in players)
      if(p != null){
        if (!p.GetComponent<NetPlayer> ().bDoneRacing)
          bDone = false;          
      }

    if (!bDone)
      return;

    //TODO:  Insert end race screen and transition phase

    //Move to next scene
    iRaceCounter++;
    if(iRaceCounter >= 3){
      foreach (GameObject player in players)
        if (player != null) {
//          NetPlayer playerScript = player.GetComponent<NetPlayer> ();
          foreach(GameObject p in players){
            if(p == null)
              continue;
            NetworkServer.Destroy(p.GetComponent<NetPlayer>().ship);
            NetworkServer.DestroyPlayersForConnection(p.GetComponent<NetworkIdentity>().connectionToClient);
          }
          //Network.Disconnect();
//          NetworkServer.DisconnectAll();
//          StopMatchMaker();
//          NetworkManager.singleton.StopHost();
//          SceneManager.LoadScene("_Main");
//          NetworkManager.singleton.StopClient ();
//          playerScript.RpcChangeScene ("_Main");
        } // End if (player != null)
    } //End if(iRaceCounter >= 3)
    else
      changeScene ();
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  private void changeScene(){
    foreach (GameObject player in players)
      if (player != null) {
        NetPlayer playerScript = player.GetComponent<NetPlayer> ();
        playerScript.RpcChangeSceneSetTrack (circuitScenes[iRaceCounter], trackNames[iRaceCounter]);
        playerScript.RpcGiveCarControl ();
        playerScript.ship.GetComponent<Rigidbody> ().velocity = Vector3.zero;
      } // End if (player != null)
  } //End private void changeScene()

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

      //Don't change the place unless the player is actually racing
      if (!_NetPlayeri.bIsRacing)
        continue;

      //loop through each other player and compare their number of waypoint hits.
      int liNewPlace = 1;
      for(int j = 0; j < players.Length; j++){
        if (players [j] == null || j == i)
          continue;
        NetPlayer _NetPlayerj = players [j].GetComponent<NetPlayer> ();
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
      if(!_NetPlayeri.bDoneRacing)
        _NetPlayeri.setPlace (liNewPlace);
    } //End for (int i = 0; i < players.Length; i++) 
  } // End public void UpdatePlaces()

  //-------------------------------------------------------------------------------------------------------------------
  public GAME_MODE getGameMode(){return GameMode;}
  public void setGameMode(GAME_MODE piGameMode){GameMode = piGameMode;}

}
