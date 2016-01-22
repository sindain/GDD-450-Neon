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
		if(New != null)
		{
			New.transform.Rotate (new Vector3 (0, 50, 0) * Time.deltaTime);
			//print (Time.deltaTime);
			//print("is rotating"+New.name);
		}
	}
}

