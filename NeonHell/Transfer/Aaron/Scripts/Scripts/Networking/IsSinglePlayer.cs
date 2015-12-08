using UnityEngine;
using System.Collections;

public class IsSinglePlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PlayerPrefs.SetFloat("multi", 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
