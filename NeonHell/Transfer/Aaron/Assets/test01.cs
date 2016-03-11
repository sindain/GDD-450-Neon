using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class test01 : MonoBehaviour {

  public Rigidbody gm1;
  public Rigidbody gm2;
  public Text t1;
  public Text t2;
  public Text t3;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    t1.text = gm1.angularVelocity.magnitude.ToString();
    t2.text = gm2.angularVelocity.magnitude.ToString();
    t3.text = (gm1.angularVelocity.magnitude == gm2.angularVelocity.magnitude).ToString();
	}
}
