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
        if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height/25, Screen.width / 8, Screen.height / 20), "T-track"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "NewTTrack";
            RpcTrackSelect("Track Selected: T-Track");
            CmdTrackSelect("Track Selected: T-Track");
            track = "Track Selected: T-Track";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height / 10, Screen.width / 8, Screen.height / 20), "Mobius Strip"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "MobiusTrack";
            RpcTrackSelect("Track Selected: Mobius Strip");
            CmdTrackSelect("Track Selected: Mobius Strip");
            track = "Track Selected: Mobius Strip";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height / 6.4f, Screen.width / 8, Screen.height / 20), "Thread Needle"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "ThreadTheNeedle";
            RpcTrackSelect("Track Selected: Thread The Needle");
            CmdTrackSelect("Track Selected: Thread The Needle");
            track = "Track Selected: Thread The Needle";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height / 4.7f, Screen.width / 8, Screen.height / 20), "Under Over"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "Under_Over";
            RpcTrackSelect("Track Selected: Under Over");
            CmdTrackSelect("Track Selected: Under Over");
            track = "Track Selected: Under Over";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height / 3.7f, Screen.width / 6, Screen.height / 20), "Doom Knot (Multiplayer Exclusive)"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "DoomKnot";
            CmdTrackSelect("Track Selected: Doom Knot");
            RpcTrackSelect("Track Selected: Doom Knot");
            track = "Track Selected: Doom Knot";
        }
        if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height / 3.06f, Screen.width / 6, Screen.height / 20), "Loop-Duh-Loop"))
        {
            network.GetComponent<NetworkLobbyManager>().playScene = "Looptrack";
            CmdTrackSelect("Track Selected: Loop-Duh-Loop");
            RpcTrackSelect("Track Selected: Loop-Duh-Loop");
            track = "Track Selected: Loop-Duh-Loop";
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

