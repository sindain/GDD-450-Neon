//-----------------------------------------------------------------------------------------------------------------
//Name:			PlayerController
//Author:		Aaron Aumann
//Date:			11/6/2015
//Description:	This file handles and user input for controlling the player ship.  Also handles track waypoint handling
//
//-----------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

using UnityEngine.Networking;
public class PlayerController : NetworkBehaviour {

	//Public variables
	public float fMaxVelocity;
	public float fAcceleration;
	public float fTurnAngle;
	public float fRotationRate;
	//public float fStrafeAcceleration;
	public float fRotationSeekSpeed;
	public bool canMove;
	public GameObject trackWaypoints;

	//Private variables
	private int lap = 0;
	private int iNumEngines;
	private float rotationVelocity;
	private float fPercThrustCurrent;
	private GameObject currentPoint;
	private Rigidbody rb;
	private Transform[] engines;

	// Use this for initialization
	void Start () {
        
        lap = 0;
        //Grab the Engines
        iNumEngines = transform.FindChild("Engines").childCount;
        engines = new Transform[iNumEngines];
  		for(int i = 0; i < iNumEngines; i++)
  			engines[i] = transform.FindChild("Engines").GetChild(i);
  			
        fPercThrustCurrent = 0.0f;
		PlayerPrefs.SetInt ("laps", 0);
        trackWaypoints = GameObject.FindWithTag("WList");
		currentPoint = trackWaypoints.transform.GetChild (0).GetComponent<WaypointController> ().getPoint();
		rb = GetComponent<Rigidbody> ();
        canMove = false;
	} //End void Start()
	
	// Update is called once per frame
	void Update () {
	
	} //End void Update()

	//FixedUpdate is called every frame
    void FixedUpdate()
    {
        
		//if (!(canMove && PlayerPrefs.GetFloat ("start") == 1) || lap >=2)
		//	return;

		//Vector help keep the ship upright
        Vector3 newRotation;
        
		float turnAngle = -Input.GetAxis("Horizontal") * fTurnAngle;
		print (turnAngle);
		foreach(Transform i in engines)
			i.localEulerAngles = new Vector3(i.localEulerAngles.x, turnAngle, i.localEulerAngles.z);
			
		//If the player is close to the something, allow moving forward
        if (Physics.Raycast(transform.position, -this.transform.up, 4.0f)){
            rb.drag = 1;
            //Calculate force to apply
			float fPercThrustTarget = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);
			fPercThrustCurrent = fAcceleration / fPercThrustTarget < fAcceleration / fPercThrustCurrent ? Time.deltaTime + fPercThrustCurrent: fAcceleration * fPercThrustTarget;
			if(fPercThrustCurrent > fAcceleration)
				fPercThrustCurrent = 4.0f;
			float c = (Mathf.Exp(1) - 1) / fAcceleration;
			float fPercThrustPower = Mathf.Log(c * fPercThrustCurrent + 1);
			
			//More force calculations
			float forwardForce = fMaxVelocity * 50.0f * fPercThrustPower * Time.deltaTime * rb.mass;
			forwardForce /= iNumEngines;
            foreach(Transform i in engines)
            	rb.AddForceAtPosition(i.forward * forwardForce, i.position);
            //rb.AddForce(forwardForce);
            //rb.AddRelativeForce (Input.GetAxis ("Strafe") * new Vector3 (strafeAcceleration, 0.0f, 0.0f) * Time.deltaTime * rb.mass);
            //if ((transform.rotation * rb.velocity).z < minVelocity)
            //rb.AddRelativeForce (new Vector3 (0.0f, 0.0f, minVelocity * rb.drag * 50 * Time.deltaTime * rb.mass));
		} //End if (Physics.Raycast(transform.position, -this.transform.up, 4.0f))

		//If the player isn't close to something
		else{
            rb.drag = 0;
			//The following 4 lines help keep the ship upright while in midair
            newRotation = transform.eulerAngles;
			newRotation.x = Mathf.SmoothDampAngle(newRotation.x, 0.0f, ref rotationVelocity, fRotationSeekSpeed);
			newRotation.z = Mathf.SmoothDampAngle(newRotation.z, 0.0f, ref rotationVelocity, fRotationSeekSpeed);
            transform.eulerAngles = newRotation;
        } //End else

		
		//Apply torque, e.g. turn the ship left and right
		//Vector3 turnTorque = transform.up * fRotationRate * Input.GetAxis("Horizontal");
        //turnTorque = turnTorque * Time.deltaTime * rb.mass;
        //rb.AddTorque(turnTorque);
	} // End void FixedUpdate()

	//-----------------------------------------------------------------------------------------------------------------
	//Name:			nextPoint
	//Description:	Sets the current point to the next point in the sequence.  Also checks for a lap
	//Parameters:	NA
	//Returns:		NA
	//-----------------------------------------------------------------------------------------------------------------
	public void nextPoint(){
		//If we hit the finish line, increment laps and such
		if (currentPoint.GetComponent<WaypointController> ().getNextPoint ().Equals (trackWaypoints.transform.GetChild (0).GetComponent<WaypointController> ().getPoint ())) {
			lap ++;
			if(lap >= 2)
				print ("You win");
		} // End if (currentPoint.GetComponent ...
		currentPoint = currentPoint.GetComponent<WaypointController> ().getNextPoint ();
	} //End public void nextPoint()

	//-----------------------------------------------------------------------------------------------------------------
	//Name:			getLap
	//Description:  Gets the current lap
	//Parameters:   NA
	//Returns:      Int lap
	//-----------------------------------------------------------------------------------------------------------------
	public int getLap(){
		return lap;
	} //End public int getLap()

	//-----------------------------------------------------------------------------------------------------------------
	//Name: 		SetLap
	//Description:	Sets the lap variable to given parameter
	//Parameters:	int pLap
	//Return:		NA
	//-----------------------------------------------------------------------------------------------------------------
	public void setLap(int pLap){
		lap = pLap;
	} // End public void setLap(int pLap)

	//-----------------------------------------------------------------------------------------------------------------
	//Name: 		OnTriggerEnter
	//Description:	Handles events that occure when entering a trigger
	//Parameters:   Collider other - What the object has collided with
	//-----------------------------------------------------------------------------------------------------------------
	void OnTriggerEnter(Collider other){
		if (other.tag == "Waypoint" && other.gameObject.Equals(currentPoint.GetComponent<WaypointController> ().getNextPoint ()))
			nextPoint ();
		if (other.tag == "KillPlane") {
			transform.position= new Vector3(currentPoint.transform.position.x,currentPoint.transform.position.y,currentPoint.transform.position.z);
			transform.rotation= new Quaternion(currentPoint.transform.rotation.x,currentPoint.transform.rotation.y,currentPoint.transform.rotation.z,currentPoint.transform.rotation.w);
			gameObject.GetComponent<Rigidbody>().velocity=Vector3.zero;
			gameObject.GetComponent<Rigidbody>().angularVelocity=Vector3.zero;
		}
	} // End void OnTriggerEnter(Collider other)

	//-----------------------------------------------------------------------------------------------------------------
	//Name: 		GetCurrentPoint
	//Description:  Getter for currentPoint variable
	//Parameters:   NA
	//Returns:      GameObject currentPoint - The current point that the player is assigned to.
	//                                        Note: The current points NextPoint variable is the object the player
 	//												should be moving towards.
	//-----------------------------------------------------------------------------------------------------------------
	public GameObject getCurrentPoint()
	{
		return currentPoint;
	}//End public GameObject getCurrentPoint
}
