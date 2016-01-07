using UnityEngine;
using System.Collections;

public class LvlDisplay : MonoBehaviour {
	public static GameObject Old;
	public static GameObject New;
	// Use this for initialization
	void Start () {
		New=GameObject.FindGameObjectWithTag("FirstS");
	
	}
	
	// Update is called once per frame
	void Update () {
		if(New != null)
			New.transform.Rotate (new Vector3 (0, 50, 0) * Time.deltaTime);
	}
}
