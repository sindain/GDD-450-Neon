using UnityEngine;
using System.Collections;

public class ThrusterController : MonoBehaviour {


	//Private variables
	private int   iThrusterCount;
	private float fThrustStrength;
	private float fThrustDistance;
	private float fGMax = 12.0f;
	private float[] fDThrusters;
	public bool  bMagnetize = false;
	private Transform[] thrusters;
	private Rigidbody rb;
	
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();

		//Initialize each thruster
		iThrusterCount = transform.FindChild ("Thrusters").childCount - 1;
		thrusters = new Transform[iThrusterCount];
		fDThrusters = new float[iThrusterCount * 2];
		for (int i = 0; i < iThrusterCount; i++) {
			fDThrusters[2*i] = 0.0f;
			fDThrusters[2*i + 1] = 0.0f;
			if (transform.FindChild ("Thrusters").GetChild (i).tag == "Thruster")
				thrusters [i] = transform.FindChild ("Thrusters").GetChild (i);
		}//End for (int i = 0; i < iThrusterCount; i++)

		//Strength of each thruster
		fThrustStrength = (-Physics.gravity.y * rb.mass) / iThrusterCount;

		//Determine thrust distanct e.g. the hover height of vehicle
		if (GetComponent<PlayerController> () != null)
			fThrustDistance = GetComponent<PlayerController> ().getAirborneDistance () / 2.0f;
//        else if (GetComponent<NetworkPlayerController> () != null)
//			fThrustDistance = GetComponent<NetworkPlayerController> ().getAirborneDistance () / 2.0f;
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
		for (int i = 0; i < iThrusterCount; i++) {
			//Local variables
			RaycastHit hit;
			float alpha = 3.0f;

			if(Physics.Raycast(thrusters[i].position, -thrusters[i].up, out hit, alpha * fThrustDistance)){
				//Vector3 vForce = thrusters[i].up;
				float fGForce = 0.0f;
				float fAPoint = 0.0f;
				
				//Ensure object hit wasn't a trigger or wall
				//if(hit.collider.isTrigger || hit.transform.tag == "Wall")
					//return;
				if(hit.collider.isTrigger&&(hit.transform.tag == "NegLightBridge"&&gameObject.GetComponent<ShipStats>().Polarity== 1))
					return;
				else if(hit.collider.isTrigger&&(hit.transform.tag == "PosLightBridge"&&gameObject.GetComponent<ShipStats>().Polarity== -1))
					return;


				//Update velocity of point relative to what it's hitting
				fDThrusters[2*i] = fDThrusters[2*i + 1];
				fDThrusters[2*i + 1] = hit.distance;
				fAPoint = (hit.distance - fDThrusters[2*i + 1] - (fDThrusters[2*i + 1] - fDThrusters[2*i])) / Mathf.Pow(Time.deltaTime, 2);
				//Add force to keep player from bottoming out
//				if(hit.distance < fThrustDistance / 3.0f  && fAPoint > 100.0f)
//					rb.AddForceAtPosition(thrusters[i].up * rb.mass * fAPoint / iThrusterCount, thrusters[i].position);
//				//Add force to keep player from flying off of track if magnetized
//				else if(hit.distance > fThrustDistance + (alpha * fThrustDistance - fThrustDistance ) * 0.75f && bMagnetize && fAPoint < -100.0f)
//					rb.AddForceAtPosition(thrusters[i].up * rb.mass * fAPoint / iThrusterCount, thrusters[i].position);	
				
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
