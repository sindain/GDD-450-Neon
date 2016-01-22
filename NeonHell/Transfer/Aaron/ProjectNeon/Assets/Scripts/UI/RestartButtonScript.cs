using UnityEngine;
using System.Collections;

public class RestartButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Transition(){
		UIHandler.lose=false;
		Time.timeScale =1;
		Application.LoadLevel(Application.loadedLevel);
	}
}
