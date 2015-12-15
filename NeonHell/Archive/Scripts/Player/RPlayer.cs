using UnityEngine;
using System.Collections;

public class RPlayer : MonoBehaviour {
	public UIHandler UIH;
	public GameObject canvas;

	// Use this for initialization
	void Start () {
		canvas = GameObject.FindGameObjectWithTag ("UIH");
		UIH = canvas.GetComponent<UIHandler> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider col){
		if (col.tag == "laserTower" && this.tag == "RPlayer") {
			print ("HIT");
			UIH.TakeDamage (1);
		}
	}
}
