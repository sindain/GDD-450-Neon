using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SpHUD : MonoBehaviour
{
  public enum UI_STATE {None=1, Countdown, HUD, Scoreboard};
  public NetPlayer _NetPlayer;
  public GameObject HUD;
  public GameObject Scorboard;
  public GameObject Menu;
  public Text countdownText;
  public Text velocityText;
  public Text PolarityText;
  public Text EnergyText;
  public Text placeText;
  public Text lapsText;
  //public Image menu;

  public float fTimer;
  private GameManager.TRANSITION Trans;
  public UI_STATE UIState;
  private Color[] countdownColors = {Color.green, Color.yellow, Color.yellow, Color.red, Color.red};

  // Use this for initialization
  void Start (){
    //Initialize all variables to their starting positions\
    UIState = UI_STATE.None;
    Trans = GameManager.TRANSITION.None;
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

    //------------------------------------
    //---------Update HUD-----------------
    //------------------------------------
    //Velocity converted from m/s to km/hr
    if(UIState == UI_STATE.HUD){
      PlayerController _PlayerController = _NetPlayer.ship.GetComponent<PlayerController> ();

      velocityText.text = "Velocity: " + Mathf.Floor(_NetPlayer.ship.GetComponent<Rigidbody> ().velocity.magnitude * 3.6f) + " km/h";
      PolarityText.text = _PlayerController.getShipStats().Polarity == 1 ? "Blue" : "Red";
      EnergyText.text = "Energy: " + Mathf.Floor(_PlayerController.getDisplayEnergy()) + "%";
      placeText.text = _NetPlayer.getPlace ().ToString() + "/8";
      lapsText.text = "Lap: " + _NetPlayer.getLap ().ToString();
    } //End if(UIState == UI_STATE.HUD)

    //------------------------------------
    //------Update countdown timer--------
    //------------------------------------
    else if(UIState == UI_STATE.Countdown){
      fTimer -= Time.deltaTime;
      if(fTimer > 0){
        countdownText.text = Mathf.CeilToInt (fTimer).ToString();
        countdownText.color = countdownColors [Mathf.CeilToInt (fTimer)];
      }
      else if (fTimer <= 0 && fTimer > -1){
        if (_NetPlayer.PlayerState == NetPlayer.PLAYER_STATE.RaceReady)
          _NetPlayer.setPlayerState (NetPlayer.PLAYER_STATE.Racing);
        countdownText.text = "GO!!!";
        countdownText.color = Color.green;
      } // End if (fStartTimer < 0)
      if (fTimer <= -1){
        transform.FindChild ("Canvas").FindChild ("CountdownText").gameObject.SetActive (false);
        UIState = UI_STATE.HUD;
      } //End if (fStartTimer <= -500)
    } //End if(UIState == UI_STATE.Countdown)

    //------------------------------------
    //-------Update Scoreboard------------
    //------------------------------------
    else if(UIState == UI_STATE.Scoreboard){
      fTimer -= Time.deltaTime;
      if(fTimer > 0 && fTimer <= 5.0f){
      foreach(GameObject p in GameObject.Find("GameManager").GetComponent<GameManager>().players){
        Scorboard.transform.FindChild("Points").GetChild(p.GetComponent<NetPlayer>().getPlace()-1).GetComponent<Text>().text =
          p.GetComponent<NetPlayer>().getPoints().ToString();
        }
      }
      else if(fTimer <= 0)
        _NetPlayer.setPlayerState(NetPlayer.PLAYER_STATE.SceneChangeReady);
    } //End if(UI_STATE == UI_STATE.Scoreboard)

  } //End public void Update()

  public void startCountdown(){
    UIState = UI_STATE.Countdown;
    fTimer = 4.0f;
    transform.FindChild ("Canvas").FindChild ("CountdownText").gameObject.SetActive (true);
    countdownText.text = "3";
    countdownText.color = Color.red;
  } //End public void startCountdown()

  public void startScoreboard(){
    UIState = UI_STATE.Scoreboard;
    fTimer = 10.0f;
    HUD.SetActive(false);
    Scorboard.SetActive(true);
    foreach(GameObject p in GameObject.Find("GameManager").GetComponent<GameManager>().players){
      if(p == null)
        continue;
      NetPlayer _NetPlayer = p.GetComponent<NetPlayer>();
      //Set relavent scoreboard player name
      Scorboard.transform.FindChild("Names").GetChild(_NetPlayer.getPlace()-1).GetComponent<Text>().text = 
        "Player " + (_NetPlayer.getPlayerNum()+1).ToString() + " " + _NetPlayer.ship.name.Substring(0,_NetPlayer.ship.name.IndexOf('('));
    } //End foreach(GameObject p in GameObject.Find("GameManager").GetComponent<GameManager>().players)
  } //End public void startScoreboard()
}