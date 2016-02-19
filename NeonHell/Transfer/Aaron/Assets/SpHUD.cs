using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SpHUD : MonoBehaviour
{
  public NetPlayer _NetPlayer;
  public Text countdownText;
  public Text velocityText;
  public Text PolarityText;
  public Text EnergyText;
  public Text placeText;
  public Text lapsText;
  //public Image menu;
  public float fStartTimer;

  private bool bCountdown;
  private Color[] countdownColors = {Color.red, Color.yellow, Color.yellow, Color.green};

  // Use this for initialization
  void Start (){
    //Initialize all variables to their starting positions\
    bCountdown = false;
    countdownText.color = Color.red;
    countdownText.text = "3";
    PlayerPrefs.SetFloat ("start", 0);
  }

  // Update is called once per frame
  void Update (){
    //Hide/Shows the menu button
    //if (Input.GetKeyDown (KeyCode.Escape)) {
    //  menu.enabled = !menu.enabled;
    //}

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

    if(bCountdown){
      fStartTimer -= Time.deltaTime;
      if(fStartTimer > 0){
        countdownText.text = Mathf.CeilToInt (fStartTimer).ToString();
        countdownText.color = countdownColors [Mathf.CeilToInt (fStartTimer)];
      }
      else if (fStartTimer <= 0 && fStartTimer > -500){
        if (_NetPlayer.PlayerState == NetPlayer.PLAYER_STATE.RaceReady)
          _NetPlayer.setPlayerState (NetPlayer.PLAYER_STATE.Racing);
        countdownText.text = "GO!!!";
        countdownText.color = Color.green;
      } // End if (fStartTimer < 0)
      if (fStartTimer <= -500){
        transform.FindChild ("Canvas").FindChild ("CountdownText").gameObject.SetActive (false);
        bCountdown = false;
      } //End if (fStartTimer <= -500)
    } //End if(bCountdown)
  } //End public void Update()

  public void startCountdown(){
    bCountdown = true;
    fStartTimer = Time.time;
    transform.FindChild ("Canvas").FindChild ("CountdownText").gameObject.SetActive (true);
    countdownText.text = "3";
    countdownText.color = Color.red;
  } //End public void startCountdown()
}