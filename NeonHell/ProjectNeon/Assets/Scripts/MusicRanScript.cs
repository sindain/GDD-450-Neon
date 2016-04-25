using UnityEngine;
using System.Collections;

public class MusicRanScript : MonoBehaviour {
	public AudioClip[] MusicTracks;
	// Use this for initialization
	void Start () {
		gameObject.GetComponent<AudioSource> ().clip = MusicTracks [Random.Range (0, MusicTracks.Length)];
		gameObject.GetComponent<AudioSource> ().PlayOneShot (gameObject.GetComponent<AudioSource> ().clip);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
