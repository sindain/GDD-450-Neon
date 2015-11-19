using UnityEngine;
using System.Collections;

public class WaypointController : MonoBehaviour {


	public GameObject nextPoint;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			//other.GetComponent<NPCController>().nextPoint();
		}
	}

	public void setNextPoint(GameObject pNextPoint){
		nextPoint = pNextPoint;
	}

	public GameObject getNextPoint(){
		return nextPoint;
	}

	public GameObject getPoint(){
		return this.gameObject;
	}
}
