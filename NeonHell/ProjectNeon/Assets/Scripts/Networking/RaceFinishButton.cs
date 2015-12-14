using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
/*
 * This makes the menu button
 * work when the player finishes
 * the game and or is trying to exit
 * to the main menu
 */
public class RaceFinishButton : MonoBehaviour {

    public NetworkLobbyM manager;
    // Use this for initialization
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("lobby").GetComponent<NetworkLobbyM>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Transition()
    {
        manager.StopClient();
        Application.LoadLevel("MainMenu");
        Time.timeScale = 1;
    }
}
