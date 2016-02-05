using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class GameManager : NetworkManager
{
  public GameObject[] players;
  private readonly int PLAYER_COUNT = 8;
  //--------------------------------------------------------------------------------------------------------------------
  //Name:         Start
  //Description:  Default start function, nothing special here
  //Parameters:   NA
  //Returns:      NA
  //--------------------------------------------------------------------------------------------------------------------
  void Start (){
    players = new GameObject[PLAYER_COUNT];
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
        player.GetComponent<NetPlayer> ().Setup (i);
        break;
      }
    }
  }
  //End public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId)

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
    //base.OnMatchList(matchList);
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

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void StopMultiplayerServices (){
//    if (matchMaker.isActiveAndEnabled)
//      StopMatchMaker ();
//    if (NetworkManager.singleton.isActiveAndEnabled) {
//      NetworkManager.singleton.StopHost ();
//      NetworkManager.singleton.StopClient ();
//    }

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
    NetworkManager.singleton.StartHost ();
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
        if (!player.GetComponent<NetPlayer> ().getReady ())
          allReady = false;
      }

    if (!allReady)
      return;
    
    foreach (GameObject player in players)
      if (player != null) {
        NetPlayer playerScript = player.GetComponent<NetPlayer> ();
        playerScript.RpcChangeScene ("TestScene02");
        playerScript.RpcRaceSetup ("Track01");
        playerScript.RpcGiveCarControl ();
      }
  }
}
