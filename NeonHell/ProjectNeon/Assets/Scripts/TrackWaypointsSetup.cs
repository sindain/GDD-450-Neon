using UnityEngine;
using System.Collections;

public class TrackWaypointsSetup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		for (int i = 0; i < transform.childCount - 1; i++) {
			transform.GetChild(i).GetComponent<WaypointController>().setNextPoint(transform.GetChild(i + 1).gameObject);
		}
		transform.GetChild(transform.childCount - 1).GetComponent<WaypointController>().setNextPoint(transform.GetChild(0).gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
