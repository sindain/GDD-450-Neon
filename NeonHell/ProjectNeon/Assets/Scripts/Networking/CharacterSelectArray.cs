using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CharacterSelectArray : NetworkBehaviour {
    public int[] shipSelected;
    public string[] ships;
    public string track;
    public Text selTrack;
    public Text charSelect;
    public int ship1;
    public int ship2;
    public int ship3;
    public int ship4;

	// Use this for initialization
	void Start () {
        ships = new string[7];
        ships[0] = "X-Jet";
        ships[1] = "Bloaty";
        ships[2] = "Gyroscope";
        ships[3] = "Bullet";
        ships[4] = "The Bus";
        ships[5] = "Mag-U";
        ships[6] = "DeathWheel";
        selTrack = GameObject.FindGameObjectWithTag("trackSelectedTXT").GetComponent<Text>();
        charSelect = GameObject.FindGameObjectWithTag("charSelect").GetComponent<Text>();
        shipSelected = new int[10];
        track = "";
	}
	
	// Update is called once per frame
	void Update () {
        if (selTrack != null)
        {
            selTrack.text = track;            
        }
        charSelect.text = "Player 1: " + ships[shipSelected[0]] + "  Player 2: " + ships[shipSelected[1]] + "  Player 3: " + ships[shipSelected[2]] + "  Player 4: " + ships[shipSelected[3]];
        RpcCharSelect();
	}

    [ClientRpc]
    public void RpcCharSelect()
    {
        for (int i = 0; i < 3; i++)
        {
            shipSelected[i] = shipSelected[i];
        }
    }

}
