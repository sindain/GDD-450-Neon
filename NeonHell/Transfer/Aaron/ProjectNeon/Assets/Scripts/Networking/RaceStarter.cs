using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
public class RaceStarter : NetworkBehaviour {
    public GameObject player; //Local Player Object
    public Text start; // The countdown text
	public Text laps; // The lap counter text
    public Text Energy;
    public Image menu; // The button that allows you you to go to the menu if you press escape
    public float timer; // The timer for the countdown
    private bool finished; // The boolean keeping track if the player has finished the race
    private int placed; // Boolean keeping track if the player has been placed
    public GameObject placeCounter; // The networked object holding the placing counter
    public int place; // Local variable for the local players place
	// Use this for initialization
	void Start () {
        //Initialize all variables to their starting positions
        placed = 0;
        place = 0;
        placeCounter = GameObject.FindWithTag("placeCounter");
        player.GetComponent<NetworkPlayerController>().setLap(0);
        finished = false;
        start.color = Color.red;
        start.text = "3";
        PlayerPrefs.SetFloat("start", 0);
        menu.enabled = false;
	}
	
	// Update is called once per frame
	void Update () { if (placeCounter == null) return;
        //Hide/Shows the menu button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.enabled = !menu.enabled;
        }
        string BoostString = player.GetComponent<NetworkPlayerController>().DispBoost.ToString();
        Energy.text = "Energy: " + BoostString.Substring(0, (BoostString.IndexOf(".") < 0) ? BoostString.Length : BoostString.IndexOf(".")) + "%";
        //Checking if the local player has finished the race
        if (finished == false && player.GetComponent<NetworkPlayerController>().getLap() >= 2)
        {
            //Incrementing the server place counter variable
            placeCounter.GetComponent<PlaceCounter>().placeCounter = placeCounter.GetComponent<PlaceCounter>().placeCounter + 1;
            //finishing the player
            finished = true;
        }
        //Constant check for the servers place counter
        place = placeCounter.GetComponent<PlaceCounter>().placeCounter;
        
        //Check for isLocalPlayer or for SinglePlayer
        if (player.GetComponent<NetworkPlayerController>().canMove || PlayerPrefs.GetFloat("multi") == 0)
        {
            //Update the lap text
            laps.text = "Laps: " + player.GetComponent<NetworkPlayerController>().getLap() + "/2";
            
            //The coundown timer text updating
            if (timer < 5)
            {
                timer += Time.deltaTime;
            }
            else
            {
                if (placed == 0)
                {
                    start.enabled = false;
                }
            }
            if (timer >= 1 && timer < 5)
            {
                start.color = new Color(255, 165, 0);
                start.text = "2";
            }
            if (timer >= 2 && timer < 5)
            {
                start.color = Color.yellow;
                start.text = "1";
            }
            if (timer >= 3 && timer < 5)
            {
                start.color = Color.green;
                start.text = "GO!";
                PlayerPrefs.SetFloat("start", 1);
            }

            //Places the player according to the server variable placeCounter
            if (player.GetComponent<NetworkPlayerController>().getLap() >= 2)
            {
                if (place == 1 && placed == 0)
                {
                    placed = 1;
                    start.enabled = true;
                    start.color = Color.green;
                    start.text = "1st Place :)";
                }
                if (place == 2 && placed == 0)
                {
                    placed = 1;
                    start.enabled = true;
                    start.color = Color.yellow;
                    start.text = "2nd Place :/";
                }
                if (place == 3 && placed == 0)
                {
                    placed = 1;
                    start.enabled = true;
                    start.color = new Color(255, 165, 0);
                    start.text = "3rd Place :|";
                }
                if (place == 4 && placed == 0)
                {
                    placed = 1;
                    start.enabled = true;
                    start.color = Color.red;
                    start.text = "4th Place :(";  
                }
                if (place == 5 && placed == 0)
                {
                    placed = 1;
                    start.enabled = true;
                    start.color = Color.red;
                    start.text = "5th Place :((";
                }
                if (place == 6 && placed == 0)
                {
                    placed = 1;
                    start.enabled = true;
                    start.color = Color.red;
                    start.text = "6th Place :(((";
                }
                if (place == 7 && placed == 0)
                {
                    placed = 1;
                    start.enabled = true;
                    start.color = Color.red;
                    start.text = "7th Place :((((";
                }
                if (place == 8 && placed == 0)
                {
                    placed = 1;
                    start.enabled = true;
                    start.color = Color.red;
                    start.text = "8th Place :(((((";
                }

                menu.enabled = true;
                //end the race somehow
            }
        }
        //This else statement turns off all gui that isnt the local players
        else
        {
            menu.enabled = false;
            start.enabled = false;
            laps.enabled = false;
        }
        
	}
}
//width 282.17
//height 180.2
//x 6
//y 15
//Z 0