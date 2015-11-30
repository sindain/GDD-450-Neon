using UnityEngine;
using System.Collections;

public class CharacterSelect : MonoBehaviour {

    private int selectedCharacter;
    public GameObject[] ships;
    private int shipSelected;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 1.9f, (2 * Screen.height / 20), Screen.width / 10, Screen.height / 20), "Ship 1"))
        {
            shipSelected = 0;
        }
        if (GUI.Button(new Rect(Screen.width / 1.9f, (3 * Screen.height / 20), Screen.width / 10, Screen.height / 20), "Ship 2"))
        {
            shipSelected = 1;
        }
        if (GUI.Button(new Rect(Screen.width / 1.9f, (4 * Screen.height / 20), Screen.width / 10, Screen.height / 20), "Ship 3"))
        {
            shipSelected = 2;
        }
        if (GUI.Button(new Rect(Screen.width / 1.9f, (5 * Screen.height / 20), Screen.width / 10, Screen.height / 20), "Ship 4"))
        {
            shipSelected = 3;
        }
        if (GUI.Button(new Rect(Screen.width / 1.9f, (6 * Screen.height / 20), Screen.width / 10, Screen.height / 20), "Ship 5"))
        {
            shipSelected = 4;
        }
        if (GUI.Button(new Rect(Screen.width / 1.9f, (7 * Screen.height / 20), Screen.width / 10, Screen.height / 20), "Ship 6"))
        {
            shipSelected = 5;
        }
        if (GUI.Button(new Rect(Screen.width / 1.9f, (8 * Screen.height / 20), Screen.width / 10, Screen.height / 20), "Ship 7"))
        {
            shipSelected = 6;
        }
    }

    // for users to apply settings from their lobby player object to their in-game player object
    /*public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        gamePlayer = ships[shipSelected];
        return true;
    }*/
}
