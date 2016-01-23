using UnityEngine;
using System.Collections;

public class ShipStats : MonoBehaviour {
	public float fMaxVelocity = 95.0f;
	public float fAcceleration = 6.0f;
	public float fHandling = 3.0f;
	public float fMass = 5.0f;
	public float maxBoost=100.0f;
	public float fSlerpTime	= 0.333f;
	public Vector3 vCameraOffset;
	public int Polarity = 0;

	// Use this for initialization
	void Start () {
	
	}
}
