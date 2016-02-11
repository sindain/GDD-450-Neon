using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void startGame(){
    GameObject.Find ("GameManager").GetComponent<GameManager> ().StartLocalGame ();
  }
  public void stopGame(){
    GameObject.Find ("GameManager").GetComponent<GameManager> ().StopLocalGame ();
  }
}
