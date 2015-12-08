using UnityEngine;
using System.Collections;

public class ThrusterController : MonoBehaviour {


	//Private variables
	private int   iThrusterCount;
	private float fThrustStrength;
	private float fThrustDistance;
	private float fMaxG = 8.0f;
	private bool  bMagnetize = false;
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
		if (GetComponent<PlayerController> () != null)
			fThrustDistance = GetComponent<PlayerController> ().getAirborneDistance () / 2.0f;
        else if (GetComponent<NetworkPlayerController> () != null)
			fThrustDistance = GetComponent<NetworkPlayerController> ().getAirborneDistance () / 2.0f;
		else if (GetComponent<NPCController> () != null)
			fThrustDistance = GetComponent<NPCController> ().getAirborneDistance () / 2.0f;
		else
			print ("ThrusterController.cs: 38.  Controller not found on object " + this.transform.parent.name);
	
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
			float fGForce = 0.0f;
			float fRaycastDistance = fThrustDistance * 2.0f;

			if(Physics.Raycast (i.position, -i.up, out hit, fRaycastDistance)){
				//fRaycastDistance /= 2.0f;
				//Check to make sure the object hit wasn't a trigger
				if(hit.collider.isTrigger)
					return;

				if (hit.distance < fThrustDistance){
					fGForce = -(fMaxG - 1) / fThrustDistance * hit.distance + fMaxG;
					if(rb.GetPointVelocity(i.position).y < 0)
						fGForce += -rb.GetPointVelocity(i.position).y;
				}
				else{
					fGForce = Mathf.Pow (hit.distance - fRaycastDistance, 2)/ Mathf.Pow(fThrustDistance,2);
				} 
				//fGForce = Mathf.Pow (hit.distance - fRaycastDistance, 2)/ Mathf.Pow(fThrustDistance,2);
				fGForce = Mathf.Abs(fGForce);
				print ("2h: " + fRaycastDistance + " h: " + fThrustDistance);
				print ("GForce: " + fGForce + ": distance" + hit.distance);

				downardForce = transform.up * fThrustStrength * fGForce;
				rb.AddForceAtPosition(downardForce, i.position);
			}//End if(Physics.Raycast (i.position, -i.up, out hit, thrustDistance)
		}//End foreach(Transform i in thrusters)
	}//End void FixedUpdate()


	//Setters
	public void setbMagnetize(bool pbMagnetize){bMagnetize = pbMagnetize;}

	//Getters
	public bool getbMagnetize(){return bMagnetize;}	
}//End public class ThrusterController : MonoBehaviour
