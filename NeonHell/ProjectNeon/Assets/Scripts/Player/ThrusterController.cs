using UnityEngine;
using System.Collections;

public class ThrusterController : MonoBehaviour {

	public float iThrustPercent = 2.0f;

	//Private variables
	private int iThrusterCount;
	public float fThrustStrength;
	private float fThrustDistance;
	private Transform[] thrusters;
	private Rigidbody rb;
	
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		iThrusterCount = transform.FindChild ("Thrusters").childCount;
		thrusters = new Transform[iThrusterCount];
		for (int i = 0; i < iThrusterCount; i++)
			thrusters [i] = transform.FindChild ("Thrusters").GetChild (i);
		fThrustStrength = (-Physics.gravity.y * rb.mass) / iThrusterCount;
		if (GetComponent<PlayerController> () != null)
			fThrustDistance = GetComponent<PlayerController> ().getAirborneDistance () - 1;
		else
			fThrustDistance = GetComponent<NPCController> ().getAirborneDistance () - 1;
	}

	void Awake(){
		rb = GetComponent<Rigidbody>();
	}
	
	//Fixed update is called every frame
	void FixedUpdate(){
		//RaycastHit to check if car is close to an object
		RaycastHit hit;

		//Iterate through each thruster
		foreach(Transform i in thrusters){
			//Variables needed for each thruster
			Vector3 downardForce;
			float distancePercentage;

			//If the raycast hit an object
			if(Physics.Raycast (i.position, -i.up, out hit, fThrustDistance)){
				//Check to make sure the object hit wasn't a trigger
				if(hit.collider.isTrigger)
					return;

				float x = rb.GetPointVelocity(i.position).y < 0? iThrustPercent + iThrustPercent * -rb.GetPointVelocity(i.position).y : iThrustPercent;
				//Do some math to calculate the thruster strength and apply it to the ships rigid body
				distancePercentage = -x / fThrustDistance * hit.distance + x;
				downardForce = transform.up * fThrustStrength * distancePercentage;
				rb.AddForceAtPosition(downardForce, i.position);
			}//End if(Physics.Raycast (i.position, -i.up, out hit, thrustDistance)
		}//End foreach(Transform i in thrusters)
	}//End void FixedUpdate()
}//End public class ThrusterController : MonoBehaviour
