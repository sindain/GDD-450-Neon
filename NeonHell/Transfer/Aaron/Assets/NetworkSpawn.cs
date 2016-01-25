using UnityEngine;
using System.Collections;

public class NetworkSpawn : MonoBehaviour {
	// Use this for initialization
	void Start () {

        Vector3 temp = gameObject.transform.position;
        Quaternion temp2 = gameObject.transform.rotation;

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = new Quaternion();

        gameObject.transform.FindChild("Ship").transform.position = temp;
        gameObject.transform.FindChild("Ship").transform.rotation = temp2;
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
