using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Networking.Match;

public class GameManager : NetworkManager {

  public ArrayList players;
  //public ArrayList players;
  void Start() {
    players = new ArrayList();
  }

  public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
    GameObject player = (GameObject)Instantiate(base.playerPrefab, Vector3.zero, Quaternion.identity);
    players.Add(player);
    player.GetComponent<NetPlayer>().iSeat = players.Count - 1;
    NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
  }
  
  public override void OnServerRemovePlayer (NetworkConnection conn, UnityEngine.Networking.PlayerController player)
  {
    base.OnServerRemovePlayer (conn, player);
  }

  public override void OnMatchList(ListMatchResponse matchList) {
    GameObject.Find("MainMenu").transform.FindChild("MultiplayerLobby").GetComponent<MultiplayerLobby>().matchResponse(matchList);
    //base.OnMatchList(matchList);
  }

  public void StartMatchMaking() {
    StartMatchMaker();
    matchMaker.ListMatches(0, 5, "", OnMatchList);
  }

  public void StartLocalGame() {
    NetworkManager.singleton.StartHost();
  }

  public void JoinLocalGame() {
    NetworkManager.singleton.StartClient();
  }
}
