using UnityEngine;
using System.Collections;

public class IsMultiplayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PlayerPrefs.SetFloat("multi", 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
