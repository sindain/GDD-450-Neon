using UnityEngine;
using System.Collections;

public class ThrusterController : MonoBehaviour {
	public float thrustStrength;
	public float thrustDistance;
	public Transform[] thrusters;

	private Rigidbody rb;
	// Use this for initialization
	void Start () {
	
	}

	void Awake(){
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
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
			if(Physics.Raycast (i.position, -i.up, out hit, thrustDistance)){
				//Check to make sure the object hit wasn't a trigger
				if(hit.collider.isTrigger)
					return;
				//Do some math to calculate the thruster strength and apply it to the ships rigid body
				distancePercentage = 1-(hit.distance/thrustDistance);
				downardForce = transform.up * thrustStrength * distancePercentage;
				downardForce = downardForce * Time.deltaTime * rb.mass;
				rb.AddForceAtPosition(downardForce, i.position);
			}//End if(Physics.Raycast (i.position, -i.up, out hit, thrustDistance)
		}//End foreach(Transform i in thrusters)
	}//End void FixedUpdate()
}//End public class ThrusterController : MonoBehaviour
