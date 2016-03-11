using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {


  public float val = 0.0f;
  private Rigidbody rb;
	// Use this for initialization
	void Start () {
    rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void FixedUpdate(){
    
    rb.AddTorque (transform.up * val * Input.GetAxis("Horizontal"),ForceMode.Acceleration);
  }

}
