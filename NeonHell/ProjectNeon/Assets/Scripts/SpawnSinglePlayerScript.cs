using UnityEngine;
using System.Collections;

public class SpawnSinglePlayerScript : MonoBehaviour {
	public GameObject[] Ships;
	// Use this for initialization
	void Start () {
        if (PlayerPrefs.GetFloat("multi") == 0)
        {
			Instantiate(Ships[PlayerPrefs.GetInt("ship")], gameObject.transform.position, gameObject.transform.rotation);
            //GameObject temp = Instantiate(Ships[PlayerPrefs.GetInt("ship")], new Vector3(0.0f, 0.0f, 0.0f), new Quaternion()) as GameObject;
            //temp.transform.FindChild("Ship").transform.position = gameObject.transform.position;
            //temp.transform.FindChild("Ship").transform.rotation = gameObject.transform.rotation;
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
