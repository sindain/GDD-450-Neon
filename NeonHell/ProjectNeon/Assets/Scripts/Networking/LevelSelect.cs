using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
public class LevelSelect : NetworkBehaviour {
    public bool levelSelector;
    public GameObject network;
    public Text selectedLvl;
	// Use this for initialization
    void Start()
    {
        PlayerPrefs.SetInt("place", 0);
        //selectedLvl = FindWithTag("trackSelectedTXT");
        network = GameObject.FindGameObjectWithTag("lobby");
        levelSelector = true;
        print("onstart: " + levelSelector);
    }
    void OnStartHost()
    {
        levelSelector = true;
        print("onhost: " + levelSelector);
    }
	
	// Update is called once per frame
	void Update () {
		
	    	}
    void OnGUI(){
        if(levelSelector){

            if (GUI.Button(new Rect(Screen.width / 1.5f, 15, Screen.width / 10, Screen.height / 20), "T-track"))
            {
                network.GetComponent<NetworkLobbyManager>().playScene = "Ttrack";
                selectedLvl.text = "Track Selected: T-Track";

            }
            if (GUI.Button(new Rect(Screen.width / 1.5f, 50, Screen.width / 10, Screen.height / 20), "L-Track"))
            {
                network.GetComponent<NetworkLobbyManager>().playScene = "Track2";
	            selectedLvl.text = "Track Selected: L-Track";
            }
            if (GUI.Button(new Rect(Screen.width / 1.5f, 85, Screen.width / 10, Screen.height / 20), "Infinity"))
            {
                network.GetComponent<NetworkLobbyManager>().playScene = "Figure 8";
                selectedLvl.text = "Track Selected: Infinity";
            }
            if (GUI.Button(new Rect(Screen.width / 1.5f, 110, Screen.width / 10, Screen.height / 20), "Springen"))
            {
                network.GetComponent<NetworkLobbyManager>().playScene = "ramping track";
                selectedLvl.text = "Track Selected: Springen";
            }
            if (GUI.Button(new Rect(Screen.width / 1.5f, 145, Screen.width / 10, Screen.height / 20), "Mount Doom"))
            {
                network.GetComponent<NetworkLobbyManager>().playScene = "HillTrack";
                selectedLvl.text = "Track Selected: Mount Doom";
            }   

        }

    }
}
