using UnityEngine;

//using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class JoinButton : Button
{
  protected override void Start (){
    base.Start ();
    onClick.AddListener (delegate {
      //Turn the wait screen on
      transform.parent.parent.GetComponent<MultiplayerLobby>().toggleWaitScreen(true,"Joining match.");
      GameObject.Find ("GameManager").GetComponent<GameManager> ().JoinMatchmakerGame (
        GameObject.Find ("MainMenu").transform.FindChild ("MultiplayerLobby").GetComponent<MultiplayerLobby> ().getSelectedMatch ());
    });
  }
}


