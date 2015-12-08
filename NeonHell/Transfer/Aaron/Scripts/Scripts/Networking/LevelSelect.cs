using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
public class LevelSelect : NetworkBehaviour {
    public bool levelSelector;
    public GameObject network;
    public string track;
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
    void Update()
    {

    }
    void OnGUI()
    {
        //if(levelSelector){
        if (isLocalPlayer)
        {
            RpcTrackSelect(track);
            CmdTrackSelect(track);
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, 15, Screen.width / 10, Screen.height / 20), "T-track"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "NewTTrack";
            RpcTrackSelect("Track Selected: T-Track");
            CmdTrackSelect("Track Selected: T-Track");
            track = "Track Selected: T-Track";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, 50, Screen.width / 10, Screen.height / 20), "L-Track"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "Track2";
            RpcTrackSelect("Track Selected: L-Track");
            CmdTrackSelect("Track Selected: L-Track");
            track = "Track Selected: L-Track";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, 85, Screen.width / 10, Screen.height / 20), "Thread Needle"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "ThreadTheNeedle";
            RpcTrackSelect("Track Selected: Thread The Needle");
            CmdTrackSelect("Track Selected: Thread The Needle");
            track = "Track Selected: Thread The Needle";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, 110, Screen.width / 10, Screen.height / 20), "Springen"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "ramping track";
            RpcTrackSelect("Track Selected: Springen");
            CmdTrackSelect("Track Selected: Springen");
            track = "Track Selected: Springen";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, 145, Screen.width / 10, Screen.height / 20), "Mount Doom"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "HillTrack";
            CmdTrackSelect("Track Selected: Mount Doom");
            RpcTrackSelect("Track Selected: Mount Doom");
            track = "Track Selected: Mount Doom";
        }
        // }
    }

    [ClientRpc]
    public void RpcTrackSelect(string track)
    {
        network.GetComponent<CharacterSelectArray>().track = track;
    }
    [Command]
    public void CmdTrackSelect(string track)
    {
        network.GetComponent<CharacterSelectArray>().track = track;
    }
}

