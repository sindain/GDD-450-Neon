using UnityEngine;
using System.Collections;

public class LaserDestroy : MonoBehaviour {

    public static bool isDestroyed;
	// Use this for initialization
	void Start () {
	    isDestroyed= false;
	}
	 
	// Update is called once per frame
	void Update () {
	    if(isDestroyed == true)
        {
            Destroy(this);
        }
	}
}
