using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fader : MonoBehaviour {

  public enum FADE_STATE {FadeOut, Stay, FadeIn};

  public float fFadeTime = 1.0f;
  private float refA;
  private Image fader;
  public FADE_STATE fadeState = FADE_STATE.Stay;
  public NetPlayer _NP;
  public bool bChangePlayerState = false;

	// Use this for initialization
	void Start () {
    fader = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {

    if (fadeState != FADE_STATE.Stay)
      fade (fadeState == FADE_STATE.FadeOut ? 1 : 0);
	}

  private void fade(int liTarget){
    float lfDiff = Mathf.Abs(fader.color.a - liTarget);
    //If we're close enough to the target alpha
    if (lfDiff < 0.02f){
      fader.color = new Color (0, 0, 0, liTarget);

      //If the change player state flag is on, change players state to next logical place in the sequence
      if (bChangePlayerState){
        bChangePlayerState = false;
        _NP.setPlayerState ((fadeState == FADE_STATE.FadeIn ? NetPlayer.PLAYER_STATE.RaceReady : NetPlayer.PLAYER_STATE.SceneChangeReady));
      }

      fadeState = FADE_STATE.Stay;
      if (liTarget == 0)
        gameObject.SetActive (false);
    } //End if (lfDiff < 0.1f)
    else{
      fader.color = new Color (0, 0, 0, Mathf.SmoothDamp (fader.color.a, (float)liTarget, ref refA, fFadeTime));
    }
  }

  public void setFadeState(FADE_STATE state, bool pbChangePlayerState = true){
    fadeState = state;
    bChangePlayerState = pbChangePlayerState;
  }
}
