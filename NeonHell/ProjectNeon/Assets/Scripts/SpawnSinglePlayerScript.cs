using UnityEngine;
using System.Collections;

public class SpawnSinglePlayerScript : MonoBehaviour {
	public GameObject[] Ships;
	// Use this for initialization
	void Start () {
        if (PlayerPrefs.GetFloat("multi") == 0)
        {
			Instantiate(Ships[PlayerPrefs.GetInt("ship")], gameObject.transform.position, gameObject.transform.rotation);
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
