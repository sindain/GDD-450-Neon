using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SpHUD : MonoBehaviour
{
  public NetPlayer _NetPlayer;
  public Text start;
  public Text velocityText;
  public Text PolarityText;
  public Text EnergyText;
  public Text placeText;
  public Text lapsText;
  public Image menu;
  public float timer;

  // Use this for initialization
  void Start (){
    //Initialize all variables to their starting positions
    start.color = Color.red;
    start.text = "3";
    PlayerPrefs.SetFloat ("start", 0);
    menu.enabled = false;
  }

  // Update is called once per frame
  void Update (){
    //Hide/Shows the menu button
    if (Input.GetKeyDown (KeyCode.Escape)) {
      menu.enabled = !menu.enabled;
    }

    if (_NetPlayer == null)
      return;

    //Velocity converted from m/s to km/hr
    float shipVel = _NetPlayer.ship.GetComponent<Rigidbody> ().velocity.magnitude * 3.6f;
    PlayerController _PlayerController = _NetPlayer.ship.GetComponent<PlayerController> ();

    velocityText.text = "Velocity: " + Mathf.Floor(shipVel) + " km/h";
    PolarityText.text = _PlayerController.getShipStats().Polarity == 1 ? "Blue" : "Red";
    EnergyText.text = "Energy: " + Mathf.Floor(_PlayerController.getDisplayEnergy()) + "%";
    placeText.text = _NetPlayer.getPlace ().ToString() + "/8";
    lapsText.text = "Lap: " + _NetPlayer.getLap ().ToString();


//    string BoostString = player.GetComponent<PlayerController> ().DispBoost.ToString ();
//    Energy.text = "Energy: " + BoostString.Substring (0, (BoostString.IndexOf (".") < 0) ? BoostString.Length : BoostString.IndexOf (".")) + "%";
//    //Checking if the local player has finished the race
//    if (finished == false && player.GetComponent<PlayerController> ().getLap () >= 2) {
//      //Incrementing the server place counter variable
//      placeCounter.GetComponent<PlaceCounter> ().placeCounter = placeCounter.GetComponent<PlaceCounter> ().placeCounter + 1;
//      //finishing the player
//      finished = true;
//    }
//    //Constant check for the servers place counter
//    place = placeCounter.GetComponent<PlaceCounter> ().placeCounter;
  }
}