using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ShipSelectReady : MonoBehaviour {

	public static string lvlName="none";
	private bool isLvlSelected=false;
	// Use this for initialization
	void Start () {
		isLvlSelected = false;
		lvlName = "none";
	}
	
	// Update is called once per frame
	void Update () {
		if (lvlName != "none" && !isLvlSelected) 
		{
			isLvlSelected=true;
			gameObject.GetComponent<Button>().interactable=true;
		}
		
	}
	public void Transition(){
		if (isLvlSelected) {
			PlayerPrefs.SetInt("place", 0);
			UIHandler.lose = false;
			Time.timeScale = 1;
			Application.LoadLevel (lvlName);
		}
	}
}
