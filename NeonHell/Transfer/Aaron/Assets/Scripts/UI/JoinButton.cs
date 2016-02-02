using UnityEngine;

//using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class JoinButton : Button
{
  protected override void Start (){
    base.Start ();
    onClick.AddListener (delegate {
      GameObject.Find ("GameManager").GetComponent<GameManager> ().JoinMatchmakerGame (
        GameObject.Find ("MainMenu").transform.FindChild ("MultiplayerLobby").GetComponent<MultiplayerLobby> ().getSelectedMatch ());
    });
  }
}


