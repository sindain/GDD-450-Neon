﻿using UnityEngine;
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
    }
    void OnStartHost()
    {
        levelSelector = true;

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
        if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height/25, Screen.width / 10, Screen.height / 20), "T-track"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "NewTTrack";
            RpcTrackSelect("Track Selected: T-Track");
            CmdTrackSelect("Track Selected: T-Track");
            track = "Track Selected: T-Track";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height / 10, Screen.width / 10, Screen.height / 20), "L-Track"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "Track2";
            RpcTrackSelect("Track Selected: L-Track");
            CmdTrackSelect("Track Selected: L-Track");
            track = "Track Selected: L-Track";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height / 6.7f, Screen.width / 10, Screen.height / 20), "Thread Needle"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "ThreadTheNeedle";
            RpcTrackSelect("Track Selected: Thread The Needle");
            CmdTrackSelect("Track Selected: Thread The Needle");
            track = "Track Selected: Thread The Needle";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height / 4, Screen.width / 10, Screen.height / 20), "Springen"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "ramping track";
            RpcTrackSelect("Track Selected: Springen");
            CmdTrackSelect("Track Selected: Springen");
            track = "Track Selected: Springen";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height / 3, Screen.width / 10, Screen.height / 20), "Doom Knot"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "DoomKnot";
            CmdTrackSelect("Track Selected: Doom Knot");
            RpcTrackSelect("Track Selected: Doom Knot");
            track = "Track Selected: Doom Knot";
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

