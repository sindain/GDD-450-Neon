﻿using UnityEngine;
using System.Collections;

public class ThrusterController : MonoBehaviour {


	//Private variables
	private int   iThrusterCount;
	private float fThrustStrength;
	private float fThrustDistance;
	private float fGMax = 12.0f;
	public bool  bMagnetize = false;
	private Transform[] thrusters;
	private Rigidbody rb;
	
	// Use this for initialization
	void Start () {
    Vector3 lCenterOfThrusters = Vector3.zero;
		rb = GetComponent<Rigidbody> ();

		//Initialize each thruster
		iThrusterCount = transform.FindChild ("Thrusters").childCount - 1;
		thrusters = new Transform[iThrusterCount];
		for (int i = 0; i < iThrusterCount; i++) {
      if (transform.FindChild ("Thrusters").GetChild (i).tag == "Thruster"){
        thrusters [i] = transform.FindChild ("Thrusters").GetChild (i);
        lCenterOfThrusters += thrusters [i].localPosition;
      } //End if (transform.FindChild ("Thrusters").GetChild (i).tag == "Thruster")
		}//End for (int i = 0; i < iThrusterCount; i++)
    lCenterOfThrusters /= iThrusterCount;
    rb.centerOfMass = new Vector3(lCenterOfThrusters.x, -1.0f, lCenterOfThrusters.z);

		//Strength of each thruster
		fThrustStrength = (-Physics.gravity.y * rb.mass) / iThrusterCount;

		//Determine thrust distanct e.g. the hover height of vehicle
		if (GetComponent<PlayerController> () != null)
			fThrustDistance = GetComponent<PlayerController> ().getAirborneDistance () / 2.0f;
		else
			print ("ThrusterController.cs: 38.  Controller not found on object " + this.transform.parent.name);
	
	}

	void Awake(){
		rb = GetComponent<Rigidbody>();
	}

	//Fixed update is called every frame
	void FixedUpdate(){

    //Determine thrust distanct e.g. the hover height of vehicle
    if (GetComponent<PlayerController> () != null)
      fThrustDistance = GetComponent<PlayerController> ().getAirborneDistance () / 2.0f;
    else
      print ("ThrusterController.cs: 38.  Controller not found on object " + this.transform.parent.name);
		//Iterate through each thruster
		for (int i = 0; i < iThrusterCount; i++) {
			//Local variables
			RaycastHit hit;
			float alpha = 3.0f;

			if(Physics.Raycast(thrusters[i].position, -thrusters[i].up, out hit, alpha * fThrustDistance)){
				//Vector3 vForce = thrusters[i].up;
				float fGForce = 0.0f;
				
				//Ensure object hit wasn't a trigger or wall
				//if(hit.collider.isTrigger || hit.transform.tag == "Wall")
					//return;
				if((hit.transform.tag == "NegLightBridge"&&gameObject.GetComponent<ShipStats>().Polarity== 1))
					return;
				if((hit.transform.tag == "PosLightBridge"&&gameObject.GetComponent<ShipStats>().Polarity== -1))
					return;
				if (hit.transform.tag == "KillPlane" || hit.transform.tag == "Wall")
					return;
        if (hit.transform.gameObject == gameObject)
          return;
          				
				//Calculate g force to apply on each thruster
				if(hit.distance <= fThrustDistance)
					fGForce = ((fGMax - 1)/Mathf.Pow(fThrustDistance, 2)) * Mathf.Pow(hit.distance - fThrustDistance,2) + 1;
				else if(hit.distance > fThrustDistance && !bMagnetize)
					fGForce = (1/Mathf.Pow(fThrustDistance - alpha * fThrustDistance, 2)) * Mathf.Pow(hit.distance - alpha*fThrustDistance, 2);
				else{
					fGMax *= 1.5f;
					fGForce = ((1+fGMax)/Mathf.Pow(fThrustDistance - alpha*fThrustDistance, 2))* Mathf.Pow(hit.distance-alpha*fThrustDistance,2) - fGMax;
					fGMax /= 1.5f;
				}
				if(hit.distance > fThrustDistance && !bMagnetize)
					fGForce = Mathf.Abs(fGForce);
				
				rb.AddForceAtPosition(thrusters[i].up *(fThrustStrength * fGForce), thrusters[i].position);

			}//End Raycast
		}//End for(int i = 0; i < iThrusterCount; i++)
	}
	
	//Fixed update is called every frame
//	void FixedUpdate(){
//
//		//Iterate through each thruster
//		foreach(Transform i in thrusters){
//			//Variables needed for each thruster
//			RaycastHit hit;
//			Vector3 downardForce;
//			float fGForce = 0.0f;
//			float fRaycastDistance = fThrustDistance * 2.0f;
//
//			if(Physics.Raycast (i.position, -i.up, out hit, fRaycastDistance)){
//				//fRaycastDistance /= 2.0f;
//				//Check to make sure the object hit wasn't a trigger
//				if(hit.collider.isTrigger)
//					return;
//
//				/*if (hit.distance < fThrustDistance){
//					fGForce = -(fMaxG - 1) / fThrustDistance * hit.distance + fMaxG;
//					if(rb.GetPointVelocity(i.position).y < 0)
//						fGForce += -rb.GetPointVelocity(i.position).y;
//				}
//				else{
//					fGForce = Mathf.Pow (hit.distance - fRaycastDistance, 2)/ Mathf.Pow(fThrustDistance,2);
//				} */
//
//
//				//fGForce = Mathf.Pow (hit.distance - fRaycastDistance, 2)/ Mathf.Pow(fThrustDistance,2);
//				fGForce = Mathf.Abs(fGForce);
//				print ("2h: " + fRaycastDistance + " h: " + fThrustDistance);
//				print ("GForce: " + fGForce + ": distance" + hit.distance);
//
//				downardForce = transform.up * fThrustStrength * fGForce;
//				rb.AddForceAtPosition(downardForce, i.position);
//			}//End if(Physics.Raycast (i.position, -i.up, out hit, thrustDistance)
//			else{
//			}
//		}//End foreach(Transform i in thrusters)
//	}//End void FixedUpdate()


	//Setters
	public void setbMagnetize(bool pbMagnetize){bMagnetize = pbMagnetize;}

	//Getters
	public bool getbMagnetize(){return bMagnetize;}	
}//End public class ThrusterController : MonoBehaviour
