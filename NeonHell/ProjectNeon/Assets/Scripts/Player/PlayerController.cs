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

public class PlayerController : NetworkBehaviour
{

  //Public variables
  public Mesh[] healthM = new Mesh[4];
  public GameObject[] pieces = new GameObject[4];
  public Vector3[] wingPos = new Vector3[4];
  public GameObject explosion;
  public Vector3[] exploPos = new Vector3[4];

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
  private float fTurnThreshold;
  private float fDamageTimer;
  private float fDamageCooldown = 1.5f;
  public bool bCameraControl = false;
  private bool bIsRacing = false;
  private bool bManuallyBoosting = false;
  public GameObject currentPoint;
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
    lap = 0; 
    fCurrentHealth = _ShipStats.fMaxHealth;
    fCurrentEnergy = _ShipStats.fMaxEnergy;
    fThrustCurrent = 0.0f;
    fTurnThreshold = Mathf.PI / 4.0f;
    PlayerPrefs.SetInt ("laps", 0);
    rb = GetComponent<Rigidbody> ();
    rb.angularDrag = 3.0f;
    rb.mass += _ShipStats.fMass * 250.0f;
    exploPos[0] = new Vector3(1, .5f, 0);
    exploPos[1] = new Vector3(-1, .5f, 0);
    exploPos[2] = new Vector3(1, -.5f, 0);
    exploPos[3] = new Vector3(-1, -.5f, 0);
  }
  //End void Start()
	
  // Update is called once per frame
  void Update (){
    if (!hasAuthority)
      return;

    if(fDamageTimer > 0)
      fDamageTimer = fDamageTimer - Time.deltaTime < 0 ? 0 : fDamageTimer - Time.deltaTime;

    bManuallyBoosting = Input.GetKey (KeyCode.Space);

    if (!bManuallyBoosting){
      fCurrentEnergy += 5.0f * Time.deltaTime;
      if (fCurrentEnergy > _ShipStats.fMaxEnergy)
        fCurrentEnergy = _ShipStats.fMaxEnergy;
    } //End if (!bManuallyBoosting)
  } //End void Update()

  //FixedUpdate is called every frame
  void FixedUpdate (){
    if (_NetPlayer == null)
      return;
    //Only continue if the player has authority of this ship, or is an NPC
    if ((_NetPlayer.isHuman() && !hasAuthority) &&
        (!_NetPlayer.isHuman() && !isServer))
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
    float fDegOffset = 0.0f;
    Vector3 newRotation;
    RaycastHit hit;		

    //Apply torque, e.g. turn the ship left and right
    if (_NetPlayer.bIsHuman)
      fTorque = Input.GetAxis ("Horizontal");
    else{
      direction.transform.position = transform.position;
      WaypointController _WaypointController = currentPoint.GetComponent<WaypointController> ();
      direction.transform.LookAt (_WaypointController.nextPoint [Random.Range (0, _WaypointController.nextPoint.Length - 1)].transform);

      //Find the angle the ship is going and where it wants to go relative to eachother
      Vector3 tProj = direction.transform.forward - (Vector3.Dot(direction.transform.forward, gameObject.transform.up)/Mathf.Pow(Vector3.Magnitude(gameObject.transform.up),2)) * gameObject.transform.up;
      fDegOffset = Mathf.Acos(Mathf.Clamp(Vector3.Dot(tProj, gameObject.transform.forward) / (Vector3.Magnitude(tProj) * Vector3.Magnitude(gameObject.transform.forward)), -1.0f, 1.0f));
      //Turn towards target
      fTorque = Mathf.Acos(Vector3.Dot(tProj,gameObject.transform.right)/(Vector3.Magnitude(tProj)*Vector3.Magnitude(gameObject.transform.right))) < (Mathf.PI/2.0f) ? 1 : -1;
      fTorque *= Mathf.Clamp(fDegOffset / (Mathf.PI/6f), 0.0f, 1.0f);
    }
    rb.AddTorque (transform.up * (_NetPlayer.isHuman() ? _ShipStats.fHandling *1.0f : _ShipStats.fHandling *1.5f) * rb.angularDrag * fTorque * rb.mass);

    //If the player is close to the something, allow moving forward
    if (Physics.Raycast (transform.position, -this.transform.up, out hit, fAirborneDistance)) {
      rb.drag = 1;
      //Thrust Calculations
      float fThrustTarget = 0.0f;
      if(_NetPlayer.bIsHuman)
        fThrustTarget = Mathf.Clamp (Input.GetAxis ("Vertical"), 0, 1) * _ShipStats.fAcceleration;
      else
        fThrustTarget = (1.0f - Mathf.Clamp(fDegOffset/ fTurnThreshold,0,1)) * _ShipStats.fAcceleration;
      
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
      Vector3 forwardForce = transform.forward * (_NetPlayer.isHuman()?lfTotalThrust*1.0f:lfTotalThrust*1.2f) * fPercThrustPower * rb.mass;
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
    //Only continue if the player has authority of this ship, or is an NPC
    if ((_NetPlayer.isHuman() && !hasAuthority) ||
        (!_NetPlayer.isHuman() && !isServer))
      return;
    switch (other.tag) {
    case "Waypoint":
      if (currentPoint == null)
        return;
      if (currentPoint.GetComponent<WaypointController> ().nextPoint == null)
        return;
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

  void OnCollisionEnter(Collision collision){

    if (fCurrentHealth == 0 || fDamageTimer > 0 || !hasAuthority)
        return;

    fCurrentHealth -= 10;
    fDamageTimer = Time.time + fDamageCooldown;

    if ((int)fCurrentHealth/25 > (int)(fCurrentHealth-10)/25){
      //fMaxVelocity -= 5;
      //for (int i = 0; i < 4; i++){
        //GameObject piece1 = (GameObject)GameObject.Instantiate(pieces[i], gameObject.transform.position, gameObject.transform.rotation);
        //piece1.transform.localScale = new Vector3(40, 40, 40);
        //GameObject explosion1 = (GameObject)GameObject.Instantiate(explosion, gameObject.transform.position + exploPos[i], gameObject.transform.rotation);
      //} //End for (int i = 0; i < 4; i++)
      CmdSpawnPieces();
    } //End if ((int)fCurrentHealth/25 > (int)(fCurrentHealth-10)/25)
    if (healthM[0] != null)
    {
        gameObject.transform.FindChild("Model").GetComponent<MeshFilter>().mesh = healthM[(int)fCurrentHealth / 25];
    }
  } //End void OnCollisionEnter(Collision collision)

  [ClientRpc]
  public void RpcSetup (){
    DontDestroyOnLoad (transform.gameObject);
  }

  [Command]
  public void CmdUpdPlaces(){
    GameObject.Find ("GameManager").GetComponent<GameManager> ().UpdatePlaces ();
  }

  [Command]
  public void CmdSpawnPieces()
  {
      for (int i = 0; i < 4; i++)
      {
          GameObject piece1 = (GameObject)GameObject.Instantiate(pieces[i], gameObject.transform.position, gameObject.transform.rotation);
          //piece1.transform.localScale = new Vector3(40, 40, 40);
          NetworkServer.Spawn(piece1);
          
          //GameObject explosion1 = (GameObject)GameObject.Instantiate(explosion, gameObject.transform.position + exploPos[i], gameObject.transform.rotation);
      } //End for (int i = 0; i < 4; i++)
  }

//-------------------------------------Getters and Setters--------------------------------------------------------------

  public int getLap (){return lap;}
  public void setLap (int pLap){lap = pLap;}

  public float getAirborneDistance (){return fAirborneDistance;}

  public float getDisplayEnergy(){return fCurrentEnergy / _ShipStats.fMaxEnergy * 100;}

  public bool getCameraControl (){return bCameraControl;}
  public void setCameraControl (bool pbCameraControl){bCameraControl = pbCameraControl;}

  public bool getIsRacing(){return bIsRacing;}
  public void setIsRacing(bool pbIsRacing){bIsRacing = pbIsRacing;}

  public GameObject getCurrentPoint (){return currentPoint;}  
  public void setCurrentPoint(GameObject point){currentPoint = point;}

  public NetPlayer getNetPlayer(){return _NetPlayer;}
  public void setNetPlayer(NetPlayer pNetPlayer){_NetPlayer = pNetPlayer;}

  public ShipStats getShipStats(){return _ShipStats;}
 
}
