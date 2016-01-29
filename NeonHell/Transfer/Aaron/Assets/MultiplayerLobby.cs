using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MultiplayerLobby : MonoBehaviour {

  public GameObject lobbyRow;
  public int test;
  private GameObject viewPort;
  // Use this for initialization
  void Start () {
    viewPort = transform.FindChild("Lobbies").gameObject.transform.FindChild ("Viewport").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void matchResponse(UnityEngine.Networking.Match.ListMatchResponse matches) {
    //matches.matches.ToArray()
  }
  public void onMatchClicked(GameObject data) {

  }
}
