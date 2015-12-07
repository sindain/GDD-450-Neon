using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterSelectArray : MonoBehaviour {
    public int[] shipSelected;
    public string track;
    public Text selTrack;
    public Text charSelect;
	// Use this for initialization
	void Start () {
        shipSelected = new int[10];
        track = "";
	}
	
	// Update is called once per frame
	void Update () {
        selTrack.text = track;
        charSelect.text = "Player 1: " + shipSelected[0] + "  Player 2: " + shipSelected[1] + "  Player 3: " + shipSelected[2] + "  Player 4: " + shipSelected[3];
	}
}
