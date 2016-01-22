using UnityEngine;
using System.Collections;

public class TempButton : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /*Just a temporary button to get to the multiplayer lobby
     * Uses standard UI for button texture
     */
    
    void OnGUI()
    {
        if (GUI.Button(new Rect(15, 15, Screen.width / 10, Screen.height / 20), "Multiplayer Lobby"))
        {
            Application.LoadLevel("lobbyScene");
        }
    }
}
