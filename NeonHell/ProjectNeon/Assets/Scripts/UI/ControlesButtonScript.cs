using UnityEngine;
using System.Collections;

public class ControlesButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Transition(){
		Time.timeScale =1;
		Application.LoadLevel("Controls");
	}
}
