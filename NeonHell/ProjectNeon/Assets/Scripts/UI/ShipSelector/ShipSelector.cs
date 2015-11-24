using UnityEngine;
using System.Collections;

public class ShipSelector : MonoBehaviour {
	public static GameObject Old;
	public static GameObject New;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		New.transform.Rotate (new Vector3 (0, 50, 0) * Time.deltaTime);
	}
}

