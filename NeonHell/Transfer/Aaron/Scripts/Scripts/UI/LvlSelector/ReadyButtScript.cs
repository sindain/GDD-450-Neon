using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ReadyButtScript : MonoBehaviour {
	public static string lvlName="none";
	private bool isLvlSelected=false;
	//Use this for initialization
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
		Application.LoadLevel ("ShipSelect");
	}
}
