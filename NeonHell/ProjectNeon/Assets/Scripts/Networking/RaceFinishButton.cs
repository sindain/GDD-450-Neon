using UnityEngine;
using System.Collections;
/*
 * This makes the menu button
 * work when the player finishes
 * the game and or is trying to exit
 * to the main menu
 */
public class RaceFinishButton : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Transition()
    {
        Application.LoadLevel("Title");
        Time.timeScale = 1;
    }
}
