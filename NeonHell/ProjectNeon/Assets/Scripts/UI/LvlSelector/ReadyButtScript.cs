using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ReadyButtScript : MonoBehaviour {
	//public static string lvlName="none";
	//private bool isLvlSelected=false;
	// Use this for initialization
	void Start () {
        //isLvlSelected = false;
        //lvlName = "none";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Transition(){
		Application.LoadLevel ("ShipSelector");
	}
}
