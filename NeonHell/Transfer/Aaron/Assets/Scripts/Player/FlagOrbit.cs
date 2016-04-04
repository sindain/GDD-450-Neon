using UnityEngine;
using System.Collections;

public class FlagOrbit : MonoBehaviour {

  public float fRadius;
  public float fRevolveTime;
  public Vector3 center;
  public GameObject Orb;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	  Orb.transform.position = new Vector3(transform.position.x + center.x, 
                                          transform.position.y + center.y,
                                          transform.position.z +  center.z);
	}
}
