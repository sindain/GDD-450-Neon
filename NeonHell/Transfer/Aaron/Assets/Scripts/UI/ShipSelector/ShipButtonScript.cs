﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ShipButtonScript : MonoBehaviour {
	public GameObject ship;
	public Text Desc;
	public int shipNum;

	// Use this for initialization
	void Start () {
		//Desc.text = "Ship Description";
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void Clicked()
	{
		/*ShipSelector.Old = ShipSelector.New;
		ShipSelector.New = ship;
		ShipSelector.New.SetActive (true);
		if (ShipSelector.Old != null) 
		{
			if(ShipSelector.Old!=ShipSelector.New)
			{
				ShipSelector.Old.SetActive (false);
			}
		}
		ShipSelectReady.ShipName=shipName;
		PlayerPrefs.SetInt ("ship",shipNum);*/
		//Desc.text = shipDesc;
	}
}
