using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CharacterSelectArray : NetworkBehaviour {
    public int[] shipSelected;
    public string track;
    public Text selTrack;
    public Text charSelect;
	// Use this for initialization
	void Start () {
        
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
            //charSelect.text = "Player 1: " + shipSelected[0] + "  Player 2: " + shipSelected[1] + "  Player 3: " + shipSelected[2] + "  Player 4: " + shipSelected[3];
        }
	}

    [ClientRpc]
    public void RpcCharSelect()
    {
        for (int i = 0; i < 10; i++)
        {
            shipSelected[i] = shipSelected[i];
        }
    }

}
