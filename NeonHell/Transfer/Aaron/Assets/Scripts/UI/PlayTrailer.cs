using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent (typeof(AudioSource))]

public class PlayTrailer : MonoBehaviour {

  public MovieTexture movie;
  private new AudioSource audio;
  private float fTimeToSwitch;

	// Use this for initialization
	void Start () {
    GetComponent<RawImage> ().texture = movie as MovieTexture;
    audio = GetComponent<AudioSource> ();
    audio.clip = movie.audioClip;
    fTimeToSwitch = Time.time + movie.duration;
    movie.Play ();
    audio.Play ();
	}
	
	// Update is called once per frame
	void Update () {
    if (Input.anyKeyDown || Time.time >= fTimeToSwitch){
      movie.Stop ();
      transform.parent.FindChild ("Cover").gameObject.SetActive (true);
      SceneManager.LoadScene ("_Main");
    }
	}

}
