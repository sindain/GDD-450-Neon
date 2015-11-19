using UnityEngine;
using System.Collections;

public class LaserBreak : MonoBehaviour {
    public GameObject laser;
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

    void OnTriggerEnter(Collider col)
    {
		if (col.tag == "Finish") {
			print ("HIT");
			Destroy (gameObject);
			Destroy (laser);
		}
	}
}
