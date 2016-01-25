using UnityEngine;
using System.Collections;

public class WaypointController : MonoBehaviour {
	
	public bool bMagnetize = false;
	public GameObject nextPoint;

	//Setters
	public void setNextPoint(GameObject pNextPoint){nextPoint = pNextPoint;}
	public void setbMagnetize(bool pbMagnetize){bMagnetize = pbMagnetize;}

	//Getters
	public GameObject getNextPoint(){return nextPoint;}
	public GameObject getPoint(){return this.gameObject;}
	public bool getbMagnetize(){return bMagnetize;}	
}
