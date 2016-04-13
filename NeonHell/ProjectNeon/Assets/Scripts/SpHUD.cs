﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SpHUD : MonoBehaviour
{
  public enum UI_STATE {
    None=1, //Nothing's goin on...
    FadeIn, //Fading into the scene
    FadeOut, //Fading out of the scene
    Countdown, //Countdown before to race starting
    HUD, //Show in game race hud
    SBWait,  //Wait 3 seconds once race is over before showing the scoreboard (SB stands for scoreboard)
    SBPoints, //Show points won for the race
    SBPointTransfer, //Transfer the points and show commulative value
    SBTransferWait, //Wait for a moment after points are transfered
    SBRearrange, //Rearrange slots in order of points
    SBFinal, //Show final results
    SBDone //All done
  };
  public NetPlayer _NetPlayer;
  public GameObject HUD;
  public GameObject Scorboard;
  public GameObject Menu;
  public Text countdownText;
  public Text velocityText;
  public Text EnergyText;
  public Text HealthText;
  public Text placeText;
  public Text lapsText;
  public Text raceTimeText;
  public Text raceOverTimeText;
  public Image polarityImage;
  public Image fader;
  public Sprite[] Huds;
  public Sprite[] MapBorders;
  public Sprite[] ShipIcons;
  public Sprite[] cDown;
  public GameObject cdtimer;
  //public Image menu;

  public float fTimer;
  public float fRaceTime = 0.0f;
  public float fRaceOverTimer = 0.0f;
  //private GameManager.TRANSITION Trans;
  public UI_STATE UIState;

  private int miniMapZoom = 0;
  private float fFadeTime = 1.5f;
  private float fRefA;
  private float[] fSBTimes = { 
    15.0f, //wait
    12.0f, //points
    9.0f, //ptrans
    6.0f, //transwait
    5.0f, //rear
    3.0f, //final
    0.0f}; //Times for events during the scoreboard
  private Color[] countdownColors = {Color.green, Color.yellow, Color.yellow, Color.red, Color.red};
  public bool IsPaused;
  // Use this for initialization
  void Start (){
    cdtimer = GameObject.FindGameObjectWithTag("countdown");
    cdtimer.GetComponent<Image>().sprite = cDown[3];
    //Initialize all variables to their starting positions\
    UIState = UI_STATE.None;
    PlayerPrefs.SetFloat ("start", 0);
	  IsPaused = false;
  }

  // Update is called once per frame
  void Update (){
    //Hide/Shows the menu button
    if (Input.GetKeyDown (KeyCode.Escape)) {
			IsPaused = !IsPaused;
			_NetPlayer.ship.GetComponent<PlayerController> ().bPaused = IsPaused;
			Menu.SetActive(IsPaused);
    }

    if (_NetPlayer == null)
      return;

    if(Input.GetKeyDown(KeyCode.Tab)){
      if(miniMapZoom < 3){  
        miniMapZoom++;
      }
      else{
        miniMapZoom = 0;
      }
    }
    if (_NetPlayer.getPlayerState() == NetPlayer.PLAYER_STATE.Racing || _NetPlayer.getPlayerState() == NetPlayer.PLAYER_STATE.RaceReady)
    {
      switch (miniMapZoom)
      {
        case 0:
          GameObject.FindGameObjectWithTag("minimap").GetComponent<Camera>().enabled = true;
          HUD.transform.FindChild("MiniMapBack").gameObject.SetActive(true);
          GameObject.FindGameObjectWithTag("minimap").GetComponent<Camera>().orthographicSize = 50;
          break;
        case 1:
          GameObject.FindGameObjectWithTag("minimap").GetComponent<Camera>().enabled = true;
          HUD.transform.FindChild("MiniMapBack").gameObject.SetActive(true);
          GameObject.FindGameObjectWithTag("minimap").GetComponent<Camera>().orthographicSize = 85;
          break;
        case 2:
          GameObject.FindGameObjectWithTag("minimap").GetComponent<Camera>().enabled = true;
          HUD.transform.FindChild("MiniMapBack").gameObject.SetActive(true);
          GameObject.FindGameObjectWithTag("minimap").GetComponent<Camera>().orthographicSize = 150;
          break;
        case 3:
          GameObject.FindGameObjectWithTag("minimap").GetComponent<Camera>().enabled = false;
          HUD.transform.FindChild("MiniMapBack").gameObject.SetActive(false);
          break;
      }
    }

    //Fading in and out
    if(UIState == UI_STATE.FadeIn){
      if (fader.color.a < 0.1f){
        fader.color = new Color (0, 0, 0, 0);
        fader.gameObject.SetActive (false);

      }
      else if (fader.color.a != 0.0f)
        fader.color = new Color (0, 0, 0, Mathf.SmoothDamp (fader.color.a, 0, ref fRefA, fFadeTime));
    }
    else if(UIState == UI_STATE.FadeOut){
      
    }
      

    //------------------------------------
    //---------Update HUD-----------------
    //------------------------------------
    if (UIState == UI_STATE.HUD)
      UpdateHUD ();

    //------------------------------------
    //------Update countdown timer--------
    //------------------------------------
    else if(UIState == UI_STATE.Countdown){
      UpdateHUD ();
      fTimer -= Time.deltaTime;
      if(fTimer > 0){
        if (fTimer > 3)
        {
          cdtimer.GetComponent<Image>().sprite = cDown[Mathf.CeilToInt(fTimer)-1];
        }
        else
        {
          cdtimer.GetComponent<Image>().sprite = cDown[Mathf.CeilToInt(fTimer)];
        }
        //countdownText.color = countdownColors [Mathf.CeilToInt (fTimer)];
      }
      else if (fTimer <= 0 && fTimer > -1){
        if (_NetPlayer.PlayerState == NetPlayer.PLAYER_STATE.RaceReady)
          _NetPlayer.setPlayerState (NetPlayer.PLAYER_STATE.Racing);
        //countdownText.text = "GO!!!";
        cdtimer.GetComponent<Image>().sprite = cDown[0];
        //countdownText.color = Color.green;
      } // End if (fStartTimer < 0)
      if (fTimer <= -1){
        //transform.FindChild ("Canvas").FindChild ("CountdownText").gameObject.SetActive (false);
        UIState = UI_STATE.HUD;
        cdtimer.SetActive(false);
      } //End if (fStartTimer <= -500)
    } //End if(UIState == UI_STATE.Countdown)

    //------------------------------------
    //-------Update Scoreboard------------
    //------------------------------------
    else{
      if (UIState == UI_STATE.None)
        return;

      GameObject[] players = GameObject.FindGameObjectsWithTag ("NetPlayer");
      int[] iPlayerNums = {0,1,2,3,4,5,6,7};

      //Sort player nums by place
      for(int i=0; i<8; i++){
        for(int j=i+1; j<8; j++){
          int iPoints = players [iPlayerNums[i]].GetComponent<NetPlayer> ().getPoints ();
          int jPoints = players [iPlayerNums[j]].GetComponent<NetPlayer> ().getPoints ();
          if (jPoints > iPoints){
            int temp = iPlayerNums [i];
            iPlayerNums [i] = iPlayerNums [j];
            iPlayerNums [j] = temp;
          }            
        }
      }

      fTimer -= Time.deltaTime;
      switch(UIState){
      case UI_STATE.SBWait:
        changeState (1, UI_STATE.SBPoints);
        HUD.SetActive(false);
        GameObject.FindGameObjectWithTag("minimap").GetComponent<Camera>().enabled = false;
        break;

      case UI_STATE.SBPoints:
        changeState (2, UI_STATE.SBPointTransfer);
        float lfTime = 0.0f;
        int liMin = 0;
        int liSec = 0;
        int liRem = 0;
        Scorboard.SetActive(true);
        foreach(GameObject p in players){
          if(p == null)
            continue;
          NetPlayer _NP = p.GetComponent<NetPlayer>();
          Transform slot = Scorboard.transform.FindChild ("Slots").GetChild (_NP.getPlace () - 1);

          //Slot background color
          if (_NP.getPlayerNum () == _NetPlayer.getPlayerNum ())
            slot.FindChild ("Background").GetComponent<Image> ().color = new Color (255.0f, 215.0f, 0.0f, 175.0f);
          //Ship Icon
          slot.FindChild ("Ship").GetComponent<Image> ().sprite = ShipIcons [_NP.iShipChoice];
          //Player# (text)
          slot.FindChild ("Name").GetComponent<Text> ().text = "Player " + (_NP.getPlayerNum() + 1).ToString();
          //Points
          slot.FindChild ("Points").GetComponent<Text> ().text = 
            _NP.getPoints () - GameManager.POINTS [_NP.getPlace () - 1] - (_NP.hasFlag() ? 4 : 0) + "+" + 
            (GameManager.POINTS [_NP.getPlace () - 1] + (_NP.hasFlag() ? 4 : 0));
          //Flag caps
          slot.FindChild ("Flags").GetComponent<Text> ().text = _NP.getFlags ().ToString ();
          //Time
          lfTime = _NP.getRaceTime ();
          liMin = Mathf.FloorToInt (lfTime / 60.0f);
          liSec = Mathf.FloorToInt(lfTime - liMin * 60.0f);
          liRem = Mathf.FloorToInt (1000.0f * (lfTime - Mathf.FloorToInt(lfTime)));
          slot.FindChild ("Time").GetComponent<Text> ().text = liMin + ":" + liSec + ":" + liRem;
        } //End foreach(GameObject p in GameObject.Find("GameManager").GetComponent<GameManager>().players)
        break;

      case UI_STATE.SBPointTransfer:
        changeState (3, UI_STATE.SBTransferWait);
        float fdt = 4 * (fSBTimes [2] - fTimer); //This isn't perfect but it works for now
        foreach (GameObject p in players){
          NetPlayer _NP = p.GetComponent<NetPlayer> ();
          Transform slot = Scorboard.transform.FindChild ("Slots").GetChild (_NP.getPlace () - 1);
          int liPoints = Mathf.FloorToInt (GameManager.POINTS [_NP.getPlace()-1] - fdt + (_NP.hasFlag() ? 4 : 0));
          if (liPoints <= 0.0f)
            slot.FindChild ("Points").GetComponent<Text> ().text = _NP.getPoints ().ToString();
          else
            slot.FindChild ("Points").GetComponent<Text> ().text = 
              _NP.getPoints () - liPoints + " + " + liPoints;
        }
        break;

      case UI_STATE.SBTransferWait:
        changeState (4, UI_STATE.SBRearrange);
        break;

      case UI_STATE.SBRearrange:
        changeState (5, UI_STATE.SBFinal);
        for(int i = 0; i<8; i++){
          //Relavent script and placing slot
          NetPlayer _NP = players[iPlayerNums[i]].GetComponent<NetPlayer> ();
          Transform slot = Scorboard.transform.FindChild ("Slots").GetChild (_NP.getPlace () - 1);
          RectTransform rect = slot.gameObject.GetComponent<RectTransform> ();
          rect.offsetMin = new Vector2 (0, -75 * (i));
          rect.offsetMax = new Vector2 (1, -75 - 75 * (i));
        }
        break;

      case UI_STATE.SBFinal:
        changeState (6, UI_STATE.SBDone);
        break;

      case UI_STATE.SBDone:
        _NetPlayer.setPlayerState (NetPlayer.PLAYER_STATE.SceneChangeReady);
        break;

      default:
        Debug.LogError ("Error: SpHUD.cs 107: UIState not detected.");
        break;
      } //End switch(UIState)
    } //End else

  } //End public void Update()

  private bool changeState(int iTimer, UI_STATE state){
    bool lbResult = false;
    if (fTimer <= fSBTimes [iTimer]){
      UIState = state;
      lbResult = true;
    }
    return lbResult;
  } //End private void changeState(int iTimer, UI_STATE state)

  private void UpdateHUD(){
    int liMin = 0;
    int liSec = 0;
    int liRem = 0;

    PlayerController _PlayerController = _NetPlayer.ship.GetComponent<PlayerController> ();

    //Update HUD text
		velocityText.text =Mathf.Floor(_NetPlayer.ship.GetComponent<Rigidbody> ().velocity.magnitude/* * 3.6f*/).ToString();
    polarityImage.color = _PlayerController.getShipStats().Polarity == 1 ? Color.blue : Color.red;
    EnergyText.text = Mathf.Floor(_PlayerController.getDisplayEnergy()) + "%";
    placeText.text = _NetPlayer.getPlace ().ToString() + "/8";
    lapsText.text = _NetPlayer.getLap () < 1 ? "1" : _NetPlayer.getLap ().ToString ();
	HealthText.text=Mathf.Floor(_PlayerController.getHealth()) + "%";

    //Update Race time
    if (_NetPlayer.PlayerState == NetPlayer.PLAYER_STATE.Racing)
      fRaceTime = _NetPlayer.getRaceTime ();
    liMin = Mathf.FloorToInt (fRaceTime / 60.0f);
    liSec = Mathf.FloorToInt(fRaceTime - liMin * 60.0f);
    liRem = Mathf.FloorToInt (100.0f * (fRaceTime - Mathf.FloorToInt(fRaceTime)));
    raceTimeText.text = liMin.ToString () + ':' + liSec.ToString () + ':' + liRem.ToString ();

    //Update race over timer.
    if(fRaceOverTimer > 0.0f){
      liSec = Mathf.FloorToInt (fRaceOverTimer);
      liRem = Mathf.FloorToInt(100.0f * (fRaceOverTimer - Mathf.FloorToInt(fRaceOverTimer)));
      raceOverTimeText.text = liSec.ToString () + ":" + liRem.ToString ();
      fRaceOverTimer -= Time.deltaTime;
    }
    else
      raceOverTimeText.text = "00:00";
  }

  public void startCountdown(){
    UIState = UI_STATE.Countdown;
    fTimer = 4.0f;
  	HUD.GetComponent<Image>().sprite = Huds [_NetPlayer.iShipChoice];
  	HUD.GetComponent<Image> ().enabled = true;
    HUD.transform.FindChild("MiniMapBack").GetComponent<Image>().sprite = MapBorders[_NetPlayer.iShipChoice];
  } //End public void startCountdown()

  public void startRaceOverTimer(){
    fRaceOverTimer = 20.0f;
  }

  public void startScoreboard(){
    UIState = UI_STATE.SBWait;
    fTimer = fSBTimes[0];

  } //End public void startScoreboard()
	public void ContinueRace(){
		IsPaused = false;
		_NetPlayer.ship.GetComponent<PlayerController> ().bPaused = IsPaused;
		Menu.SetActive(false);
	}
	public void QuitToMenu(){
		//call grants function here.
		_NetPlayer.OnExitClicked();
	}
}