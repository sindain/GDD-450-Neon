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
        if (GameObject.FindGameObjectWithTag("lobby").GetComponent<NetworkLobbyM>() != null)
        {
            manager = GameObject.FindGameObjectWithTag("lobby").GetComponent<NetworkLobbyM>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Transition()
    {
        
        Application.LoadLevel("MainMenu");
        Time.timeScale = 1;
        manager.StopClient();
    }
}
