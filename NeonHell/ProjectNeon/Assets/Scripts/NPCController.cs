using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour {

	//Public variables
	public float fMaxVelocity = 95.0f;
	public float fAcceleration = 6.0f;
	public float fHandling = 3.0f;
	public float fMass = 5.0f;
	public bool canMove;
	public GameObject placeCounter; // The networked object holding the placing counter

	private int 		lap = 0;
	private float 		fAccelDir = 0;
	private float 		rotationVelocity;
	private float 		fAirborneDistance = 6.0f;
	private float 		fThrustCurrent;	
	private float 		fBoostTime = 2.0f;
	private float 		fBoostTargetTime;
    private float       degOffset;
	private bool  		bManuallyBoosting = false;
	private Rigidbody 	rb;
	private GameObject 	direction;
	private GameObject 	currentPoint;
	private GameObject 	trackWaypoints;
	private bool        finished;



	// Use this for initialization
	void Start () {
		trackWaypoints = GameObject.FindWithTag("WList");
		placeCounter = GameObject.FindWithTag("placeCounter");
		finished = false;
		rb = GetComponent<Rigidbody> ();
		rb.angularDrag = 3.0f;
		currentPoint = trackWaypoints.transform.GetChild (0).GetComponent<WaypointController> ().getPoint();
		direction = new GameObject();
		//target = trackWaypoints.transform.GetChild (0).GetComponent<WaypointController> ().getPoint ().transform;
	}

	// Update is called once per frame
	void Update () {
		if (lap >= 2&& !finished) {
			//Incrementing the server place counter variable
			placeCounter.GetComponent<PlaceCounter>().placeCounter = placeCounter.GetComponent<PlaceCounter>().placeCounter + 1;
			//finishing the player
			finished = true;
		}
	}

	void FixedUpdate(){
        if (PlayerPrefs.GetFloat("start") != 1)
            return;

		//Update the direction that the NPC needs to go
		direction.transform.position = transform.position;
		direction.transform.eulerAngles = transform.eulerAngles;
		direction.transform.LookAt(currentPoint.GetComponent<WaypointController>().getNextPoint().transform);

		//Rotation vector to help keep ships upright
		Vector3 newRotation;

		//Find the angle the ship is going and where it wants to go relative to eachother
		Vector3 tProj = direction.transform.forward - (Vector3.Dot(direction.transform.forward, gameObject.transform.up)/Mathf.Pow(Vector3.Magnitude(gameObject.transform.up),2)) * gameObject.transform.up;
        degOffset = Mathf.Acos(Vector3.Dot(tProj, gameObject.transform.forward) / (Vector3.Magnitude(tProj) * Vector3.Magnitude(gameObject.transform.forward)));
        print("degOff: " + degOffset);
        //Is the ship hovering close enough to the ground?
		if (Physics.Raycast (transform.position, - this.transform.up, 4.0f)) {
			rb.drag = 1;
			//If turn goal is within threshold, the speed up.
			//iAccelDir = Mathf.Abs (y2 - y1) <= 45 ? 1 : 0;
			float threshold = Mathf.PI/4.0f;
			if(degOffset <= threshold){
				//Add force if ship is within threshold
				float fThrustTarget = (1-(degOffset/threshold)) * (fAcceleration-0.5f);
				//print (fThrustTarget);
				float c = (Mathf.Exp(1) - 1) / (fAcceleration-0.5f);
				if(fThrustTarget <= fThrustCurrent)
					fThrustCurrent = fThrustTarget;
				else{
					if((Mathf.Exp (rb.velocity.magnitude/fMaxVelocity) - 1) / c > fThrustCurrent)
						fThrustCurrent = (Mathf.Exp (rb.velocity.magnitude/fMaxVelocity) - 1) / c;
					fThrustCurrent += Time.deltaTime;
				} //End Else
				fThrustCurrent = Mathf.Clamp(fThrustCurrent,0,(fAcceleration-0.5f));
				float fPercThrustPower = Mathf.Log(c * fThrustCurrent + 1);

				float flTotalThrust = fMaxVelocity;
				if(bManuallyBoosting || Time.time <= fBoostTargetTime){
					flTotalThrust = fMaxVelocity + 12.0f;
					fPercThrustPower = 1.0f;
				}

				//More force calculations
				Vector3 forwardForce = transform.forward * (fMaxVelocity+2.0f) * fPercThrustPower * rb.mass;
				rb.AddForce(forwardForce);

			}
		} 
		//Ship to far from ground, turn drag off and right the ship
		else {
			rb.drag = 0;
			newRotation = transform.eulerAngles;
			newRotation.x = Mathf.SmoothDampAngle(newRotation.x, 0.0f, ref rotationVelocity, 0.25f);
			newRotation.z = Mathf.SmoothDampAngle(newRotation.z, 0.0f, ref rotationVelocity, 0.25f);
			transform.eulerAngles = newRotation;
		}	

		//Turn towards target
		fAccelDir = Mathf.Acos(Vector3.Dot(tProj,gameObject.transform.right)/(Vector3.Magnitude(tProj)*Vector3.Magnitude(gameObject.transform.right))) < (Mathf.PI/2.0f) ? 1 : -1;
        fAccelDir *= Mathf.Clamp(degOffset / (Mathf.PI/6f), 0.0f, 1.0f);
		//Rotate character up to turnspeed
		rb.AddTorque (gameObject.transform.up * (fHandling+15.0f) * rb.angularDrag * fAccelDir * rb.mass);
	}

	public void nextPoint(){
		//If we hit the finish line, increment laps and such
		if (currentPoint.GetComponent<WaypointController> ().getNextPoint ().Equals (trackWaypoints.transform.GetChild (0).GetComponent<WaypointController> ().getPoint ())) {
			lap ++;
			if(lap >= 2)
				canMove = false;
		} // End if (currentPoint.GetComponent ...
		currentPoint = currentPoint.GetComponent<WaypointController> ().getNextPoint ();
	} //End public void nextPoint()
	
	public int getLap(){
		return lap;
	} //End public int getLap()
	
	public void setLap(int pLap){
		lap = pLap;
	} // End public void setLap(int pLap)
	
	public float getAirborneDistance(){
		return fAirborneDistance;
	}

	//-----------------------------------------------------------------------------------------------------------------
	//Name: 		OnTriggerEnter
	//Description:	Handles events that occure when entering a trigger
	//Parameters:   Collider other - What the object has collided with
	//-----------------------------------------------------------------------------------------------------------------
	void OnTriggerEnter(Collider other){
		switch(other.tag){
		case "Waypoint":
                if (other.gameObject.Equals(currentPoint.GetComponent<WaypointController>().getNextPoint()))
                {
                    nextPoint();
                    GetComponent<ThrusterController>().setbMagnetize(other.gameObject.GetComponent<WaypointController>().getbMagnetize());
                }
			break;
		case "KillPlane":
			transform.position= new Vector3(currentPoint.transform.position.x,currentPoint.transform.position.y,currentPoint.transform.position.z);
			transform.rotation= new Quaternion(currentPoint.transform.rotation.x,currentPoint.transform.rotation.y,currentPoint.transform.rotation.z,currentPoint.transform.rotation.w);
			gameObject.GetComponent<Rigidbody>().velocity=Vector3.zero;
			gameObject.GetComponent<Rigidbody>().angularVelocity=Vector3.zero;
			break;
		case "Booster":
			fBoostTargetTime = Time.time + fBoostTime; 
			break;
		}
		/*if (other.tag == "Waypoint" && other.gameObject.Equals(currentPoint.GetComponent<WaypointController> ().getNextPoint ()))
			nextPoint ();
		if (other.tag == "KillPlane") {
			transform.position= new Vector3(currentPoint.transform.position.x,currentPoint.transform.position.y,currentPoint.transform.position.z);
			transform.rotation= new Quaternion(currentPoint.transform.rotation.x,currentPoint.transform.rotation.y,currentPoint.transform.rotation.z,currentPoint.transform.rotation.w);
			gameObject.GetComponent<Rigidbody>().velocity=Vector3.zero;
			gameObject.GetComponent<Rigidbody>().angularVelocity=Vector3.zero;
		}*/
		
	} // End void OnTriggerEnter(Collider other)
}
