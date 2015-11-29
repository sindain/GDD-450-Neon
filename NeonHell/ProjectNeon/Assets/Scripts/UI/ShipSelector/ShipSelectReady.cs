using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ShipSelectReady : MonoBehaviour {

	public static string lvlName="none";
	private bool isShipSelected=false;
	public static string ShipName="none";
	// Use this for initialization
	void Start () {
		isShipSelected = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (ShipName != "none" && !isShipSelected) 
		{
			isShipSelected=true;
			gameObject.GetComponent<Button>().interactable=true;
		}
		
	}
	public void Transition(){
		if (isShipSelected) {
			lvlName=ReadyButtScript.lvlName;
			PlayerPrefs.SetInt("place", 0);
			UIHandler.lose = false;
			Time.timeScale = 1;
			Application.LoadLevel (lvlName);
		}
	}
}
