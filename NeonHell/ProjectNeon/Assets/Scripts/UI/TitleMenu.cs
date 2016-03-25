using UnityEngine;
using System.Collections;

public class TitleMenu : MonoBehaviour
{

  // Use this for initialization
  void Start (){
  }

  // Update is called once per frame
  void Update (){

  }

  public void onSinglePlayerClicked (){
    GameObject.Find ("GameManager").GetComponent<GameManager> ().StartLocalGame ();
    transform.parent.FindChild ("Neon Sign").gameObject.SetActive (false);
    transform.parent.FindChild ("VehicleSelection").gameObject.SetActive (true);
    gameObject.SetActive (false);
  }

  public void onMultiPlayerClicked (){
    GameObject.Find ("GameManager").GetComponent<GameManager> ().StartMatchMaking ();
    transform.parent.FindChild ("MultiplayerLobby").gameObject.SetActive (true);
    gameObject.SetActive (false);
  }

  public void onControlsClicked (){
    transform.parent.FindChild ("Controls").gameObject.SetActive (true);
    gameObject.SetActive (false);
  }

  public void onCreditsClicked (){
    transform.parent.FindChild ("Credits").gameObject.SetActive (true);
    gameObject.SetActive (false);
  }

  public void onExitClicked (){
    Application.Quit ();
  }
}
