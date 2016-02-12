﻿//-----------------------------------------------------------------------------------------------------------------
//Name:			PlayerController
//Author:		Aaron Aumann
//Date:			11/6/2015
//Description:	This file handles and user input for controlling the player ship.  Also handles track waypoint handling
//
//-----------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

  //Public variables
  public Mesh[] healthM = new Mesh[4];
  public GameObject[] pieces = new GameObject[4];

  //Private variables
  private int lap = 0;
  private int BoostType = 0;
  private float fCurrentHealth;
  private float fCurrentEnergy;
  private float rotationVelocityX = 0.0f;
  private float rotationVelocityZ = 0.0f;
  private float fAirborneDistance = 3.0f;
  private float fRotationSeekSpeed = 0.6f;
  private float fBoostTime = 0.25f;
  private float fBoostTargetTime;
  private float fThrustCurrent;
  private float fLastUpdTime;
  private float fUpdTime = 333.33f;
  public bool bCameraControl = false;
  private bool bIsRacing = false;
  [SyncVar] public bool bCanMove = false;
  private bool bManuallyBoosting = false;
  private GameObject currentPoint;
  private GameObject direction;
  private Rigidbody rb;
  private NetPlayer _NetPlayer;
  private ShipStats _ShipStats;

  // Use this for initialization
  void Start (){
    _ShipStats = GetComponent<ShipStats> ();
    direction = new GameObject ();
    direction.transform.SetParent (transform);
    DontDestroyOnLoad (transform.gameObject);
    fLastUpdTime = Time.time;
    lap = 0; 
    fCurrentHealth = _ShipStats.fMaxHealth;
    fCurrentEnergy = _ShipStats.fMaxEnergy;
    fThrustCurrent = 0.0f;
    PlayerPrefs.SetInt ("laps", 0);
    rb = GetComponent<Rigidbody> ();
    rb.angularDrag = 3.0f;
    rb.mass += _ShipStats.fMass * 250.0f;
  }
  //End void Start()
	
  // Update is called once per frame
  void Update (){
    if (!hasAuthority)
      return;
    
    bManuallyBoosting = Input.GetKey (KeyCode.Space);

    //Player 1 needs to update places every 1/3 of a second
    if(bIsRacing && Time.time - fLastUpdTime < fUpdTime){
        fLastUpdTime = Time.time;
        CmdUpdPlaces ();
    }

    if (!bManuallyBoosting){
      fCurrentEnergy += 5.0f * Time.deltaTime;
      if (fCurrentEnergy > _ShipStats.fMaxEnergy)
        fCurrentEnergy = _ShipStats.fMaxEnergy;
    } //End if (!bManuallyBoosting)
  } //End void Update()

  //FixedUpdate is called every frame
  void FixedUpdate (){
    //Only continue if the player has authority of this ship or testing
    if (!hasAuthority || _NetPlayer.PlayerState == NetPlayer.PLAYER_STATE.Testing)
      return;
    
    if (bCameraControl) {
      Camera cam = Camera.main;
      cam.transform.position = Vector3.Slerp (cam.transform.position, transform.position + transform.rotation * _ShipStats.vCameraOffset, _ShipStats.fSlerpTime);
      cam.transform.rotation = Quaternion.Slerp (cam.transform.rotation, transform.rotation, _ShipStats.fSlerpTime);
    }

    if (_NetPlayer.PlayerState != NetPlayer.PLAYER_STATE.Racing)
      return;

    //Vector help keep the ship upright
    float fTorque = 0.0f;
    Vector3 newRotation;
    RaycastHit hit;		

    //Apply torque, e.g. turn the ship left and right
    if (_NetPlayer.bIsHuman)
      fTorque = Input.GetAxis ("Horizontal");
    else{
      float fDegOffset = 0.0f;
      direction.transform.position = transform.position;
      WaypointController _WaypointController = currentPoint.GetComponent<WaypointController> ();
      direction.transform.LookAt (_WaypointController.nextPoint [Random.Range (0, _WaypointController.nextPoint.Length - 1)].transform);

      //Find the angle the ship is going and where it wants to go relative to eachother
      Vector3 tProj = direction.transform.forward - (Vector3.Dot(direction.transform.forward, gameObject.transform.up)/Mathf.Pow(Vector3.Magnitude(gameObject.transform.up),2)) * gameObject.transform.up;
      fDegOffset = Mathf.Acos(Vector3.Dot(tProj, gameObject.transform.forward) / (Vector3.Magnitude(tProj) * Vector3.Magnitude(gameObject.transform.forward)));

      //Turn towards target
      fTorque = Mathf.Acos(Vector3.Dot(tProj,gameObject.transform.right)/(Vector3.Magnitude(tProj)*Vector3.Magnitude(gameObject.transform.right))) < (Mathf.PI/2.0f) ? 1 : -1;
      fTorque *= Mathf.Clamp(fDegOffset / (Mathf.PI/6f), 0.0f, 1.0f);
    }

    rb.AddTorque (transform.up * _ShipStats.fHandling * rb.angularDrag * fTorque * rb.mass);

    //If the player is close to the something, allow moving forward
    if (Physics.Raycast (transform.position, -this.transform.up, out hit, fAirborneDistance)) {
      rb.drag = 1;
      //Thrust Calculations
      float fThrustTarget = Mathf.Clamp (Input.GetAxis ("Vertical"), 0, 1) * _ShipStats.fAcceleration;
      float c = (Mathf.Exp (1) - 1) / _ShipStats.fAcceleration;
      if (fThrustTarget <= fThrustCurrent)
        fThrustCurrent = fThrustTarget;
      else {
        if ((Mathf.Exp (rb.velocity.magnitude / _ShipStats.fMaxVelocity) - 1) / c > fThrustCurrent)
          fThrustCurrent = (Mathf.Exp (rb.velocity.magnitude / _ShipStats.fMaxVelocity) - 1) / c;
        fThrustCurrent += Time.deltaTime;
      } //End Else
      fThrustCurrent = Mathf.Clamp (fThrustCurrent, 0, _ShipStats.fAcceleration);
      float fPercThrustPower = Mathf.Log (c * fThrustCurrent + 1);

      float lfTotalThrust = _ShipStats.fMaxVelocity;

      if ((bManuallyBoosting && fCurrentEnergy >= 0.0f)) {
        lfTotalThrust = _ShipStats.fMaxVelocity + 10.0f;
        fPercThrustPower = 1.0f;
        if (bManuallyBoosting) {
          fCurrentEnergy -= 20.0f * Time.deltaTime;
          if (fCurrentEnergy < 0)
            fCurrentEnergy = 0;
        }

      }
      if (Time.time <= fBoostTargetTime) {
        if (BoostType == 1) {
          lfTotalThrust = _ShipStats.fMaxVelocity + 100.0f;
          fPercThrustPower = 1.0f;
        }
        else if (BoostType == -1) {
          lfTotalThrust = _ShipStats.fMaxVelocity - 100.0f;
          //fPercThrustPower = .5f;
        }
      }

      //More force calculations
      Vector3 forwardForce = transform.forward * lfTotalThrust * fPercThrustPower * rb.mass;
      rb.AddForce (forwardForce);

      //Brake force
      if (Input.GetAxis ("Brake") < 0)
        rb.AddForce (transform.forward * _ShipStats.fMaxVelocity * 0.2f * Input.GetAxis ("Brake") * rb.mass);
    } //End if (Physics.Raycast(transform.position, -this.transform.up, 4.0f))

		//If the player isn't close to something
		else {
      rb.drag = 0.16f;
      //The following 4 lines help keep the ship upright while in midair
      newRotation = transform.eulerAngles;
      newRotation.x = Mathf.SmoothDampAngle (newRotation.x, 0.0f, ref rotationVelocityX, fRotationSeekSpeed);
      newRotation.z = Mathf.SmoothDampAngle (newRotation.z, 0.0f, ref rotationVelocityZ, fRotationSeekSpeed);
      transform.eulerAngles = newRotation;
    } //End else
  } // End void FixedUpdate()

  //-----------------------------------------------------------------------------------------------------------------
  //Name: 		OnTriggerEnter
  //Description:	Handles events that occure when entering a trigger
  //Parameters:   Collider other - What the object has collided with
  //-----------------------------------------------------------------------------------------------------------------
  void OnTriggerEnter (Collider other){
    switch (other.tag) {
    case "Waypoint":
      GameObject[] possiblePoints = currentPoint.GetComponent<WaypointController> ().nextPoint;
      for(int i = 0; i < possiblePoints.Length; i++){
        if(other.gameObject.Equals(possiblePoints[i])){
          //Reassign current point
          currentPoint = possiblePoints [i];
          GetComponent<ThrusterController> ().setbMagnetize (currentPoint.GetComponent<WaypointController> ().getbMagnetize ());
          //Increment waypoint count
          WaypointController _WaypointController = currentPoint.GetComponent<WaypointController> ();
          _NetPlayer.incNumWaypointsHit (_WaypointController.iWeight);
          //If just hit the finish line, increment the lap counter.
          if (_WaypointController.bFinishLine)
            _NetPlayer.incLap ();
        }
      }
      break;
    case "KillPlane":
      transform.position = new Vector3 (currentPoint.transform.position.x, currentPoint.transform.position.y, currentPoint.transform.position.z);
      transform.rotation = new Quaternion (currentPoint.transform.rotation.x, currentPoint.transform.rotation.y, currentPoint.transform.rotation.z, currentPoint.transform.rotation.w);
      gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
      gameObject.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
      break;
    case "+Booster":
      fBoostTargetTime = Time.time + fBoostTime;
      BoostType = _ShipStats.Polarity == 1 ? 1 : -1;
      break;
    case "-Booster":
      fBoostTargetTime = Time.time + fBoostTime;
      BoostType = _ShipStats.Polarity == -1 ? 1 : -1;
      break;
    case "SwitchGate":
      if (_ShipStats.Polarity == 0)
        return;
      _ShipStats.Polarity = _ShipStats.Polarity * -1;
      gameObject.GetComponent<ShipStats> ().Polarity = _ShipStats.Polarity;
      break;
    case "PosGate":
      _ShipStats.Polarity = 1;
      gameObject.GetComponent<ShipStats> ().Polarity = 1;
      break;
    case "NegGate":
      _ShipStats.Polarity = -1;
      gameObject.GetComponent<ShipStats> ().Polarity = -1;
      break;
    /*case "PosMine":
			if (Polarity == 0) {
				this.ShutDown ();
			} 
			else if (Polarity == 1) 
			{
				return;
			}
			else if (Polarity == -1) 
			{
				this.ShutDown ();
			}
			break;
		case "NegMine":
			if (Polarity == 0) {
				this.ShutDown ();
			} 
			else if (Polarity == -1) 
			{
				return;
			}
			else if (Polarity == 1) 
			{
				this.ShutDown ();
			}
			break;*/

    }
  }

  void OnCollisionEnter(Collision collision)
  {
//      if (health == 0)
//          return;
//      bool spawn = (int)health/25 > (int)(health-10)/25;
//      health -= 10;
//      if (spawn)
//      {
//          GameObject.Instantiate(pieces[(int)health / 25], gameObject.transform.position, gameObject.transform.rotation);
//      }
//      print(health);
//      gameObject.transform.FindChild("Model").GetComponent<MeshFilter>().mesh = healthM[(int)health/25];
//      
  }// End void OnTriggerEnter(Collider other)

  [ClientRpc]
  public void RpcSetup (){
    DontDestroyOnLoad (transform.gameObject);
  }

  [Command]
  public void CmdUpdPlaces(){
    GameObject.Find ("GameManager").GetComponent<GameManager> ().UpdatePlaces ();
  }

//-------------------------------------Getters and Setters--------------------------------------------------------------

  public int getLap (){return lap;}
  public void setLap (int pLap){lap = pLap;}

  public float getAirborneDistance (){return fAirborneDistance;}

  public float getDisplayEnergy(){return fCurrentEnergy / _ShipStats.fMaxEnergy * 100;}

  public bool getCameraControl (){return bCameraControl;}
  public void setCameraControl (bool pbCameraControl){bCameraControl = pbCameraControl;}

  public bool getCanMove (){return bCanMove;}
  public void setCanMove (bool pbCanMove){bCanMove = pbCanMove; RpcSetCanMove(pbCanMove);}
  [ClientRpc] public void RpcSetCanMove(bool pbCanMove){bCanMove = pbCanMove;}

  public bool getIsRacing(){return bIsRacing;}
  public void setIsRacing(bool pbIsRacing){bIsRacing = pbIsRacing;}

  public GameObject getCurrentPoint (){return currentPoint;}  
  public void setCurrentPoint(GameObject point){currentPoint = point;}

  public NetPlayer getNetPlayer(){return _NetPlayer;}
  public void setNetPlayer(NetPlayer pNetPlayer){_NetPlayer = pNetPlayer;}

  public ShipStats getShipStats(){return _ShipStats;}
 
}
