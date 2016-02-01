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
public class PlayerController : MonoBehaviour {

	//Public variables

	public bool canMove;
	public bool bMasterCanMove = false;
	public float DispBoost = 100.0f; 
	public Camera playerCamera;
	public float currentBoost;
	public int Polarity=0;




	//Private variables
	private float fMaxVelocity = 1.0f;
	private float fAcceleration = 1.0f;
	private float fHandling= 1.0f;
	private float fMass = 1.0f; 
	private float maxBoost = 1.0f;
	private float fSlerpTime = 0.0f;
	private Vector3 vCameraOffset;

	private int lap = 0;
	private int BoostType = 0;// 1 is good boost -1 is bad boost
	private float rotationVelocityX = 0.0f;
	private float rotationVelocityZ = 0.0f;
	private float fAirborneDistance = 3.0f;
	private float fRotationSeekSpeed = 0.6f;
	private float fBoostTime = 0.5f;
	private float fBoostTargetTime;
	private float fThrustCurrent;

	private bool  bManuallyBoosting = false;
	private GameObject currentPoint;
	private GameObject trackWaypoints;
	private Rigidbody rb;


	// Use this for initialization
	void Start () {
		Polarity = gameObject.GetComponent<ShipStats> ().Polarity;
		fMaxVelocity = gameObject.GetComponent<ShipStats>().fMaxVelocity;
		fAcceleration = gameObject.GetComponent<ShipStats>().fAcceleration;
		fHandling = gameObject.GetComponent<ShipStats>().fHandling;
		fMass = gameObject.GetComponent<ShipStats>().fMass;
		maxBoost = gameObject.GetComponent<ShipStats>().maxBoost;
		fSlerpTime = gameObject.GetComponent<ShipStats> ().fSlerpTime;
		vCameraOffset=gameObject.GetComponent<ShipStats>().vCameraOffset;
		playerCamera = Camera.main;
		lap = 0; 
		currentBoost = maxBoost;
        fThrustCurrent = 0.0f;
		PlayerPrefs.SetInt ("laps", 0);
        trackWaypoints = GameObject.FindWithTag("WList");
		if (trackWaypoints == null)
			print ("ERROR- PlayerController.cs.68:  No trackwaypoints found in scene");
		else
		currentPoint = trackWaypoints.transform.GetChild (0).GetComponent<WaypointController> ().getPoint();
		rb = GetComponent<Rigidbody> ();
		rb.angularDrag = 3.0f;
        canMove = false;
		rb.mass += fMass * 250.0f;
	} //End void Start()
	
	// Update is called once per frame
	void Update () {
		bManuallyBoosting = Input.GetKey(KeyCode.Space);
		if (Input.GetKeyDown (KeyCode.Alpha9)) 
		{
			if (Polarity == 0) {
				return;
			} 
			else if (Polarity == 1) 
			{
				Polarity = -1;
				gameObject.GetComponent<ShipStats> ().Polarity = -1;
			}
			else if (Polarity == -1) 
			{
				Polarity = 1;
				gameObject.GetComponent<ShipStats> ().Polarity = 1;
			}
		}
		if (Input.GetKeyDown (KeyCode.Alpha1)) 
		{
			Polarity = -1;
			gameObject.GetComponent<ShipStats> ().Polarity = -1;
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) 
		{
			Polarity = 1;
			gameObject.GetComponent<ShipStats> ().Polarity = 1;
		}

	} //End void Update()

	//FixedUpdate is called every frame
    void FixedUpdate()
    {
		playerCamera.transform.position = Vector3.Slerp(playerCamera.transform.position, transform.position + transform.rotation * vCameraOffset, fSlerpTime);
		playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, transform.rotation, fSlerpTime);
		//if ((!(PlayerPrefs.GetFloat ("start") == 1) || lap >=2) && !bMasterCanMove)
		//	return;
		if (currentBoost < 100f && !bManuallyBoosting) {
			currentBoost += 5.0f * Time.deltaTime;
		}
		//Polarity=gameObject.GetComponent<ShipStats>().Polarity;
		//Vector help keep the ship upright
        Vector3 newRotation;
		RaycastHit hit;		

		//Apply torque, e.g. turn the ship left and right
		rb.AddTorque(transform.up * fHandling * rb.angularDrag * Input.GetAxis("Horizontal") * rb.mass);

		//If the player is close to the something, allow moving forward
    if (Physics.Raycast(transform.position, -this.transform.up,out hit, fAirborneDistance)){
      rb.drag = 1;
      //Thrust Calculations
			float fThrustTarget = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1) * fAcceleration;
			float c = (Mathf.Exp(1) - 1) / fAcceleration;
			if(fThrustTarget <= fThrustCurrent)
				fThrustCurrent = fThrustTarget;
			else{
				if((Mathf.Exp (rb.velocity.magnitude/fMaxVelocity) - 1) / c > fThrustCurrent)
					fThrustCurrent = (Mathf.Exp (rb.velocity.magnitude/fMaxVelocity) - 1) / c;
				fThrustCurrent += Time.deltaTime;
			} //End Else
			fThrustCurrent = Mathf.Clamp(fThrustCurrent,0,fAcceleration);
			float fPercThrustPower = Mathf.Log(c * fThrustCurrent + 1);

			float flTotalThrust = fMaxVelocity;

			if((bManuallyBoosting && currentBoost >= 1f)){
				flTotalThrust = fMaxVelocity + 10.0f;
				fPercThrustPower = 1.0f;
				if(bManuallyBoosting)
				{
					currentBoost-=20.0f*Time.deltaTime;
				}

			}
			if(Time.time <= fBoostTargetTime){
				if(BoostType==1)
				{
					flTotalThrust = fMaxVelocity + 20.0f;
					fPercThrustPower = 3.0f;
				}
				else if(BoostType==-1)
				{
					flTotalThrust = fMaxVelocity-100.0f;
					fPercThrustPower = .10f;
				}
			}

			//More force calculations
			Vector3 forwardForce = transform.forward * flTotalThrust * fPercThrustPower * rb.mass;
			rb.AddForce(forwardForce);

			//Brake force
			if(Input.GetAxis("Brake") < 0)
				rb.AddForce(transform.forward * fMaxVelocity * 0.2f * Input.GetAxis("Brake") * rb.mass);
		} //End if (Physics.Raycast(transform.position, -this.transform.up, 4.0f))

		//If the player isn't close to something
		else{
            rb.drag = 0f;
			//The following 4 lines help keep the ship upright while in midair
			newRotation = transform.eulerAngles;
			newRotation.x = Mathf.SmoothDampAngle(newRotation.x, 0.0f, ref rotationVelocityX, fRotationSeekSpeed);
			newRotation.z = Mathf.SmoothDampAngle(newRotation.z, 0.0f, ref rotationVelocityZ, fRotationSeekSpeed);
            transform.eulerAngles = newRotation;
        } //End else


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
			if(other.gameObject.Equals(currentPoint.GetComponent<WaypointController> ().getNextPoint ()))
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
		case "+Booster":
			//fBoostTargetTime = Time.time + fBoostTime;
			if(Polarity==1)
			{
				fBoostTargetTime = Time.time + fBoostTime;
				BoostType = 1;
			}

			else if (Polarity==-1)
			{
				fBoostTargetTime = Time.time + fBoostTime;
				BoostType = -1;
			}

			break;
		case "-Booster":
			if (Polarity==-1)
			{
				fBoostTargetTime = Time.time + fBoostTime;
				BoostType = 1;
			}
			
			else if (Polarity==1)
			{
				fBoostTargetTime = Time.time + fBoostTime;
				BoostType = -1;
			}
			break;
		case "SwitchGate":
			if (Polarity == 0) {
				return;
			} 
			else if (Polarity == 1) 
			{
				Polarity = -1;
				gameObject.GetComponent<ShipStats> ().Polarity = -1;
			}
			else if (Polarity == -1) 
			{
				Polarity = 1;
				gameObject.GetComponent<ShipStats> ().Polarity = 1;
			}
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
