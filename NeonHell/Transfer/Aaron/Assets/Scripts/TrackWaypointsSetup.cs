//AUTHOR: Grant Bordson
//LastEdited: 2/3/2016
//**Used to parse the waypoints into a list runnable by both players and NPCs**//
//Modified:  Aaron Aumann  2/6/16 
//           -  Added comments to closing brackets to improve readability.  Commented out all print statements.
//              Changed setup algorithm to be in seperate function so this can be for multiple tracks.

using UnityEngine;
using System.Collections;

public class TrackWaypointsSetup : MonoBehaviour {

	// Use this for initialization
  void Start()
  {
    setupTrack ();
  }

  private void setupTrack(){
    //Intake the current track being run
    
    //loop through the main track and all subsequent shortcuts
    for (int i = 0; i < transform.childCount; i++)
    {
      //Don't process the Start Positions object
      if (transform.GetChild (i).name == "Start Positions")
        continue;
      //take in the next loop and go through all of the track pieces contained in the loop
      Transform loop = transform.GetChild(i);
      for (int j = 0; j < loop.childCount; j++)
      {
        //run through each piece and get the waypoint containter in each piece
        Transform piece = loop.GetChild(j);
        Transform waypointContainer = piece.FindChild("Waypoints");
        //check the first waypoint on each piece and test which way the waypoints need to be parsed into the list
        if (waypointContainer.GetChild(0).GetComponent<WaypointController>().iDirection == 1)
        {
          //print("FORWARD");
          for (int k = 0; k < waypointContainer.childCount; k++)
          {
            //print("Child Count: " + waypointContainer.childCount);
            //check if the piece has another waypoint
            if ((k + 1) < waypointContainer.childCount)
            {
              //make the waypoint point to the next waypoint in the list
              //print("Setting: " + piece.name + " next waypoint to " + waypointContainer.GetChild(k + 1).gameObject.name);
              waypointContainer.GetChild(k).gameObject.GetComponent<WaypointController>().setNextPoint(waypointContainer.GetChild(k + 1).gameObject, 0);
            } //End if ((k + 1) < waypointContainer.childCount)
            else
            {
              //CHECK IF A NEXT TRACK PIECE EXISTS AND IF THE LOOP IS A SHORTCUT
              if ((j + 1) < loop.childCount)
              {
                //Then either go to next piece, wrap to start line, or manually connect shortcuts in inspector
                if (loop.GetChild(j + 1).FindChild("Waypoints").GetChild(0).gameObject.GetComponent<WaypointController>().iDirection == 1)
                {

                  waypointContainer.GetChild(k).gameObject.GetComponent<WaypointController>().setNextPoint(loop.GetChild(j + 1).FindChild("Waypoints").GetChild(0).gameObject, 0);
                }
                else
                {
                  waypointContainer.GetChild(k).gameObject.GetComponent<WaypointController>().setNextPoint(loop.GetChild(j + 1).FindChild("Waypoints").GetChild(loop.GetChild(j + 1).FindChild("Waypoints").childCount - 1).gameObject, 0);
                } //End else
              } // End if ((j + 1) < loop.childCount)
              else
              {
                waypointContainer.GetChild(k).gameObject.GetComponent<WaypointController>().setNextPoint(waypointContainer.GetChild(k).gameObject.GetComponent<WaypointController>().loopFinish, 0);
              } // End else
            } //End Else
          } //End for (int k = 0; k < waypointContainer.childCount; k++)
        } // End if (waypointContainer.GetChild(0).GetComponent<WaypointController>().iDirection == 1)
        else //Direction is -1 so track piece is going backward
        {
          //print("BACKWARD");
          for (int k = waypointContainer.childCount; k > 0; k--)
          {
            //print("Child Count: " + waypointContainer.childCount);
            //print("k: " + k);
            //check if the piece has another waypoint in the other direction
            if ((k - 2) >= 0)
            {
              //print("Setting: " + piece.name + " next waypoint to " + waypointContainer.GetChild(k - 2).gameObject.name);
              //make the waypoint point to the next waypoint in the list
              waypointContainer.GetChild(k - 1).gameObject.GetComponent<WaypointController>().setNextPoint(waypointContainer.GetChild(k - 2).gameObject, 0);
            } // End if ((k - 2) >= 0)
            else
            {
              //CHECK IF A NEXT TRACK PIECE EXISTS AND IF THE LOOP IS A SHORTCUT
              if ((j + 1) < loop.childCount)
              {
                //Then either go to next piece, wrap to start line, or manually connect shortcuts in inspector
                if (loop.GetChild(j + 1).FindChild("Waypoints").GetChild(0).gameObject.GetComponent<WaypointController>().iDirection == 1)
                {
                  waypointContainer.GetChild(k - 1).gameObject.GetComponent<WaypointController>().setNextPoint(loop.GetChild(j + 1).FindChild("Waypoints").GetChild(0).gameObject, 0);
                } //End if (loop.GetChild(j + 1).FindChild("Waypoints").GetChild(0).gameObject.GetComponent<WaypointController>().iDirection == 1)
                else
                {
                  waypointContainer.GetChild(k - 1).gameObject.GetComponent<WaypointController>().setNextPoint(loop.GetChild(j + 1).FindChild("Waypoints").GetChild(loop.GetChild(j + 1).FindChild("Waypoints").childCount - 1).gameObject, 0);
                } //End Else
              } //End if ((j + 1) < loop.childCount)
              else
              {
                waypointContainer.GetChild(k - 1).gameObject.GetComponent<WaypointController>().setNextPoint(waypointContainer.GetChild(k - 1).gameObject.GetComponent<WaypointController>().loopFinish, 0);
              } //End Else
            } //End Else
          } //End for (int k = waypointContainer.childCount; k > 0; k--)
        } //End else
      } // End for (int j = 0; j < loop.childCount; j++)
    } //End for (int i = 0; i < mainTrack.transform.childCount; i++)
  } // End void setupTrack(string trackName){
}
