using UnityEngine;
using System.Collections;

public class ThrusterController : MonoBehaviour {


	//Private variables
	private int iThrusterCount;
	private float fThrustStrength;
	private float fThrustDistance;
	private float fTiltStrength;
	private float fMaxG = 8.0f;
	private Transform[] thrusters;
	private Rigidbody rb;
	
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();

		//Initialize each thruster
		iThrusterCount = transform.FindChild ("Thrusters").childCount - 1;
		thrusters = new Transform[iThrusterCount];
		for (int i = 0; i < iThrusterCount; i++)
			if(transform.FindChild("Thrusters").GetChild(i).tag == "Thruster")
				thrusters [i] = transform.FindChild ("Thrusters").GetChild (i);

		//Strength of each thruster
		fThrustStrength = (-Physics.gravity.y * rb.mass) / iThrusterCount;

		//Determine thrust distanct e.g. the hover height of vehicle
		if (GetComponent<PlayerController> () != null) {
			fThrustDistance = GetComponent<PlayerController> ().getAirborneDistance () / 2.0f;
			fTiltStrength = GetComponent<PlayerController>().fHandling;
		} else {
			fThrustDistance = GetComponent<NPCController> ().getAirborneDistance () / 2.0f;
			fTiltStrength = 4.0f; //GetComponent<NPCController>().fHandling;
		}
	
	}

	void Awake(){
		rb = GetComponent<Rigidbody>();
	}
	
	//Fixed update is called every frame
	void FixedUpdate(){

		//Iterate through each thruster
		foreach(Transform i in thrusters){
			//Variables needed for each thruster
			RaycastHit hit;
			Vector3 downardForce;
			float distancePercentage = 0.0f;
			float fRaycastDistance = fThrustDistance * 2.0f;

			if(Physics.Raycast (i.position, -i.up, out hit, fRaycastDistance)){
				fRaycastDistance /= 2.0f;
				//Check to make sure the object hit wasn't a trigger
				if(hit.collider.isTrigger)
					return;

				if (hit.distance < fThrustDistance){
					distancePercentage = -(fMaxG - 1) / fRaycastDistance * hit.distance + fMaxG;
					if(rb.GetPointVelocity(i.position).y < 0)
						distancePercentage += -rb.GetPointVelocity(i.position).y;
				}
				else
					distancePercentage = -1 / fRaycastDistance * hit.distance + 2;
				distancePercentage = Mathf.Abs(distancePercentage);

				//Calculate additional thrust from tilting
				/*float fTiltThrust = 0.0f;
				if(Input.GetAxis("HorizontalTilt") * -i.localPosition.x > 0.0f)
					fTiltThrust += Input.GetAxis("HorizontalTilt") * fTiltStrength;
				if(Input.GetAxis("VerticalTilt") * -i.localPosition.z > 0)
					fTiltThrust += Input.GetAxis("VerticalTilt") * fTiltStrength;
				fTiltThrust = Mathf.Abs(fTiltThrust * rb.mass / 10.0f);*/

				downardForce = transform.up * fThrustStrength * distancePercentage;
				rb.AddForceAtPosition(downardForce, i.position);
			}//End if(Physics.Raycast (i.position, -i.up, out hit, thrustDistance)
		}//End foreach(Transform i in thrusters)
	}//End void FixedUpdate()
}//End public class ThrusterController : MonoBehaviour
