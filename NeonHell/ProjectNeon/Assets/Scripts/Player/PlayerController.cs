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
  public float fAirborneDistance = 3.0f;
  public  bool  bTesting = false;
  public GameObject[] pieces = new GameObject[4];

  //Private variables
  public AudioClip[] soundEffects = new AudioClip[5];
  public int   BoostType = 0;
  private Vector3 BoostDir;
  private float fCurrentHealth;
  private float fCurrentEnergy;
  private float rotationVelocityX = 0.0f;
  private float rotationVelocityZ = 0.0f;
  private float fRotationSeekSpeed = 0.6f;
  //private float fBoostTime = 0.25f;
  private float fBoostTargetTime;
  private float fThrustCurrent;
  private float fTurnThreshold;
  private float fDamageTimer;
  private float fDamageCooldown = 5.0f;
  private bool  bCameraControl = false;
  private bool  bManuallyBoosting = false;
	public bool bPaused = false;
  private GameObject currentPoint;
  private GameObject direction;
  private Rigidbody rb;
  private NetPlayer _NetPlayer;
  private ShipStats _ShipStats;
  private float SucTimer = 0.0f;
  private AudioSource engine;
  public int modelChild;
  // Use this for initialization
  void Start (){
    engine = gameObject.AddComponent<AudioSource>();
    engine.clip = soundEffects[1];
    engine.playOnAwake = true;
    engine.loop = true;
    engine.volume = .40f;
    engine.Play();
    engine.spatialBlend = 1.0f;
		engine.minDistance = 8;
		engine.maxDistance = 40;
    _ShipStats = GetComponent<ShipStats> ();
    direction = new GameObject ();
    direction.transform.SetParent (transform);
    DontDestroyOnLoad (transform.gameObject);
    fCurrentHealth = _ShipStats.fMaxHealth;
    fCurrentEnergy = _ShipStats.fMaxEnergy;
    fThrustCurrent = 0.0f;
    fTurnThreshold = Mathf.PI / 4.0f;
    PlayerPrefs.SetInt ("laps", 0);
    rb = GetComponent<Rigidbody> ();
    rb.angularDrag = 3.0f;
    rb.mass += _ShipStats.fMass * 250.0f;
  }
  //End void Start()
	
  // Update is called once per frame
  void Update (){
      engine.pitch = 1+ (rb.velocity.magnitude / gameObject.GetComponent<ShipStats>().fMaxVelocity);
    if (!hasAuthority && !bTesting)
      return;

    if(fDamageTimer >= 0)
      fDamageTimer = fDamageTimer - Time.deltaTime;

	if(Input.GetKey(KeyCode.R)) {
		SucTimer += 1.0f*Time.deltaTime;
	}
	if (SucTimer >= 1.0f) {
		transform.position = new Vector3 (currentPoint.transform.position.x, currentPoint.transform.position.y, currentPoint.transform.position.z);
		transform.rotation = new Quaternion (currentPoint.transform.rotation.x, currentPoint.transform.rotation.y, currentPoint.transform.rotation.z, currentPoint.transform.rotation.w);
		gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		gameObject.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		SucTimer = 0.0f;
	}
	if(Input.GetKeyUp (KeyCode.R)) {
		SucTimer = 0.0f;
	}
    bManuallyBoosting = Input.GetKey (KeyCode.Space);

    if (!bManuallyBoosting){
      if (fCurrentEnergy > _ShipStats.fMaxEnergy)
        fCurrentEnergy = _ShipStats.fMaxEnergy;
    } //End if (!bManuallyBoosting)
  } //End void Update()

  //FixedUpdate is called every frame
  void FixedUpdate (){
    
    if (_NetPlayer == null && !bTesting)
      return;
    //Only continue if the player has authority of this ship, or is an NPC, or testing
    if (!bTesting){
      if ((_NetPlayer.isHuman () && !hasAuthority) &&
        (!_NetPlayer.isHuman () && !isServer))
        return;
    }

    //Vector help keep the ship upright
    float fTorque = 0.0f;
    float fDegOffset = 0.0f;
    bool lbIsAirborne = true;
    Vector3 newRotation;
    RaycastHit hit;   

    //Consider the ship airborne if the raycast fails or hits a wall.
    if (Physics.Raycast (transform.position, -this.transform.up, out hit, fAirborneDistance))
      lbIsAirborne = hit.collider.tag == "Wall";

    //Right the ship if it is airborne
    if(lbIsAirborne){
      newRotation = transform.eulerAngles;
      newRotation.x = Mathf.SmoothDampAngle (newRotation.x, 0.0f, ref rotationVelocityX, fRotationSeekSpeed);
      newRotation.z = Mathf.SmoothDampAngle (newRotation.z, 0.0f, ref rotationVelocityZ, fRotationSeekSpeed);
      transform.eulerAngles = newRotation;
    } //End if(lbAirborne)
    
    if (bCameraControl || bTesting) {
      Camera cam = Camera.main;
      cam.transform.position = Vector3.Slerp (cam.transform.position, transform.position + transform.rotation * _ShipStats.vCameraOffset, _ShipStats.fSlerpTime);
      cam.transform.rotation = Quaternion.Slerp (cam.transform.rotation, transform.rotation, _ShipStats.fSlerpTime);
    }

    if(!bTesting)
		if (_NetPlayer.PlayerState != NetPlayer.PLAYER_STATE.Racing || bPaused)
        return;

    //Apply torque, e.g. turn the ship left and right
    if(bTesting)
      fTorque = Input.GetAxis ("Horizontal");
    else if (_NetPlayer.bIsHuman)
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
    if(bTesting)
      rb.AddTorque (transform.up * _ShipStats.fHandling * rb.angularDrag * fTorque, ForceMode.Acceleration);
    else
      rb.AddTorque (transform.up * (_NetPlayer.isHuman() ? _ShipStats.fHandling *1.0f : _ShipStats.fHandling *1.5f) * rb.angularDrag * fTorque, ForceMode.Acceleration);
    
    //If the player is close to the something, allow moving forward
    if (!lbIsAirborne) {
      rb.drag = 1;
      //Thrust Calculations
      float fThrustTarget = 0.0f;
      if(bTesting)
        fThrustTarget = Mathf.Clamp (Input.GetAxis ("Vertical"), 0, 1) * _ShipStats.fAcceleration;
      else if(_NetPlayer.bIsHuman)
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

      if ((bManuallyBoosting && fCurrentEnergy > 0.0f)) {
				rb.AddForce(rb.transform.forward * 35.0f * rb.mass);
        if (bManuallyBoosting) {
          fCurrentEnergy -= 20.0f * Time.deltaTime;
          if (fCurrentEnergy < 0)
            fCurrentEnergy = 0;
        }

      }
        if (BoostType == 1) {
		  rb.AddForce(BoostDir * 50.0f * rb.mass);
        }
        else if (BoostType == -1) {
          lfTotalThrust *=.75f;
        }
      //More force calculations
      Vector3 forwardForce;
      if(bTesting)
        forwardForce= transform.forward * lfTotalThrust * fPercThrustPower * rb.mass;
      else
        forwardForce= transform.forward * (_NetPlayer.isHuman()?lfTotalThrust*1.0f:lfTotalThrust*1.2f) * fPercThrustPower * rb.mass;
      rb.AddForce (forwardForce);

      //Brake force
      if (Input.GetAxis ("Brake") < 0)
        rb.AddForce (transform.forward * _ShipStats.fMaxVelocity * 0.2f * Input.GetAxis ("Brake") * rb.mass);
    } //End if (!lbIsAirborne)
		//If the player isn't close to something
		else
      rb.drag = 0.0f;
  } // End void FixedUpdate()

  //-----------------------------------------------------------------------------------------------------------------
  //Name: 		OnTriggerEnter
  //Description:	Handles events that occure when entering a trigger
  //Parameters:   Collider other - What the object has collided with
  //-----------------------------------------------------------------------------------------------------------------
  void OnTriggerEnter (Collider other){
    if (!bTesting){
      //Only continue if the player has authority of this ship, or is an NPC
      if ((_NetPlayer.isHuman () && !hasAuthority) ||
        (!_NetPlayer.isHuman () && !isServer))
        return;
    }
    switch (other.tag) {
    case "Waypoint":
      if (currentPoint == null || _NetPlayer.PlayerState != NetPlayer.PLAYER_STATE.Racing)
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
          if (_NetPlayer != null)
            _NetPlayer.incNumWaypointsHit (_WaypointController.iWeight);
          else
            Debug.Log ("_NetPlayer missing in PlayerController:OnTriggerEnter", gameObject);
          //If just hit the finish line, increment the lap counter.
          if (_WaypointController.bFinishLine){
            if (_NetPlayer != null){
              _NetPlayer.incLap ();
              fCurrentEnergy += _ShipStats.fMaxEnergy * 0.5f; //TODO:  Move this to repair station eventually.
            }
            else
              Debug.Log ("_NetPlayer missing in PlayerController:OnTriggerEnter", gameObject);
          } //End if (_WaypointController.bFinishLine)
        } //End if(other.gameObject.Equals(possiblePoints[i]))
      } //End for(int i = 0; i < possiblePoints.Length; i++)
      break;
    case "KillPlane":
      if (gameObject == null || currentPoint == null)
        return;
      transform.position = new Vector3 (currentPoint.transform.position.x, currentPoint.transform.position.y, currentPoint.transform.position.z);
      transform.rotation = new Quaternion (currentPoint.transform.rotation.x, currentPoint.transform.rotation.y, currentPoint.transform.rotation.z, currentPoint.transform.rotation.w);
      gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
      gameObject.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
      break;
//		case "+Booster":
//			fBoostTargetTime = Time.time + fBoostTime;
//			BoostType = _ShipStats.Polarity == 1 ? 1 : -1;
//			BoostDir = other.transform.right;
//      break;
//    case "-Booster":
//      fBoostTargetTime = Time.time + fBoostTime;
//      BoostType = _ShipStats.Polarity == -1 ? 1 : -1;
//      break;
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
	case "HealthGate":
		fCurrentHealth += 50;
	  if (fCurrentHealth > 100) {
		  fCurrentHealth = 100;

	}
			if(fCurrentHealth >=100){
				gameObject.transform.FindChild("Model").GetChild(0).gameObject.active = true;
				gameObject.transform.FindChild("Model").GetChild(1).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(2).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(3).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(4).gameObject.active = false;
				modelChild = 0;
			}
			else if (fCurrentHealth < 100 && fCurrentHealth >=80)
			{
				gameObject.transform.FindChild("Model").GetChild(0).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(1).gameObject.active = true;
				gameObject.transform.FindChild("Model").GetChild(2).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(3).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(4).gameObject.active = false;
				modelChild = 1;
			}
			else if (fCurrentHealth < 80 && fCurrentHealth >= 60)
			{
				gameObject.transform.FindChild("Model").GetChild(0).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(1).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(2).gameObject.active = true;
				gameObject.transform.FindChild("Model").GetChild(3).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(4).gameObject.active = false;
				modelChild = 2;
			}
			else if (fCurrentHealth < 60 && fCurrentHealth >=40)
			{
				gameObject.transform.FindChild("Model").GetChild(0).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(1).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(2).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(3).gameObject.active = true;
				gameObject.transform.FindChild("Model").GetChild(4).gameObject.active = false;
				modelChild = 3;
			}
			else{
				gameObject.transform.FindChild("Model").GetChild(0).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(1).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(2).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(3).gameObject.active = false;
				gameObject.transform.FindChild("Model").GetChild(4).gameObject.active = true;
				modelChild = 4;
			}
	break;

    }
  }
	void OnTriggerStay (Collider other){
		if (!bTesting){
			//Only continue if the player has authority of this ship, or is an NPC
			if ((_NetPlayer.isHuman () && !hasAuthority) ||
				(!_NetPlayer.isHuman () && !isServer))
				return;
		}
		switch (other.tag) {
		case "+Booster":
			BoostType = _ShipStats.Polarity == 1 ? 1 : -1;
			BoostDir = other.transform.right;
			break;
		case "-Booster":
			BoostType = _ShipStats.Polarity == -1 ? 1 : -1;
			BoostDir = other.transform.right;
			break;
		}
	}

	void OnTriggerExit (Collider other){
		if (!bTesting){
			//Only continue if the player has authority of this ship, or is an NPC
			if ((_NetPlayer.isHuman () && !hasAuthority) ||
				(!_NetPlayer.isHuman () && !isServer))
				return;
		}
		switch (other.tag) {
		case "+Booster":
			BoostType = 0;
			break;
		case "-Booster":
			BoostType = 0;
			break;
		}
	}
  void OnCollisionEnter(Collision collision){
      
    if(collision.gameObject.tag == "Wall"){
      
      //rb.angularVelocity = Vector3.zero;
      rb.AddForce (collision.impulse, ForceMode.Impulse);
      rb.angularVelocity = Vector3.zero;
      foreach (ContactPoint c in collision.contacts){
        Vector3 force = -collision.impulse / collision.contacts.Length;
        //rb.AddForceAtPosition (force, c.point, ForceMode.Impulse);
        //rb.AddForce (collision.impulse / collision.contacts.Length);

      } //End foreach (ContactPoint c in collision.contacts)
    } //End if(collision.gameObject.tag == "Wall")

    if (fCurrentHealth == 0 || fDamageTimer > 0 || !hasAuthority)
        return;
    
    fCurrentHealth -= 5;
    fDamageTimer += fDamageCooldown;
    gameObject.GetComponent<AudioSource>().PlayOneShot(soundEffects[0]);

    if(fCurrentHealth >=100){
        gameObject.transform.FindChild("Model").GetChild(0).gameObject.active = true;
        gameObject.transform.FindChild("Model").GetChild(1).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(2).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(3).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(4).gameObject.active = false;
        modelChild = 0;
    }
    else if (fCurrentHealth < 100 && fCurrentHealth >=80)
    {
        gameObject.transform.FindChild("Model").GetChild(0).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(1).gameObject.active = true;
        gameObject.transform.FindChild("Model").GetChild(2).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(3).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(4).gameObject.active = false;
        modelChild = 1;
    }
    else if (fCurrentHealth < 80 && fCurrentHealth >= 60)
    {
        gameObject.transform.FindChild("Model").GetChild(0).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(1).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(2).gameObject.active = true;
        gameObject.transform.FindChild("Model").GetChild(3).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(4).gameObject.active = false;
        modelChild = 2;
    }
    else if (fCurrentHealth < 60 && fCurrentHealth >=40)
    {
        gameObject.transform.FindChild("Model").GetChild(0).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(1).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(2).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(3).gameObject.active = true;
        gameObject.transform.FindChild("Model").GetChild(4).gameObject.active = false;
        modelChild = 3;
    }
    else{
        gameObject.transform.FindChild("Model").GetChild(0).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(1).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(2).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(3).gameObject.active = false;
        gameObject.transform.FindChild("Model").GetChild(4).gameObject.active = true;
        modelChild = 4;
    }
    

    if ((int)fCurrentHealth/25 > (int)(fCurrentHealth-10)/25){
      //fMaxVelocity -= 5;
      for (int i = 0; i < 4; i++){
        GameObject piece1 = (GameObject)GameObject.Instantiate(pieces[i], gameObject.transform.position, gameObject.transform.rotation);
        //piece1.transform.localScale = new Vector3(40, 40, 40);
      } //End for (int i = 0; i < 4; i++)
      CmdSpawnPieces();
    } //End if ((int)fCurrentHealth/25 > (int)(fCurrentHealth-10)/25)

    
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

  public float getAirborneDistance (){return fAirborneDistance;}

  public float getDisplayEnergy(){return fCurrentEnergy / _ShipStats.fMaxEnergy * 100;}
  public float getHealth(){return fCurrentHealth;}
  public void setHealth(float health) {fCurrentHealth = health;}

  public float getEnergy() { return fCurrentEnergy; }
  public void setEnergy(float energy) { fCurrentEnergy = energy; }

  public bool getCameraControl (){return bCameraControl;}
  public void setCameraControl (bool pbCameraControl){bCameraControl = pbCameraControl;}

  public GameObject getCurrentPoint (){return currentPoint;}  
  public void setCurrentPoint(GameObject point){currentPoint = point;}

  public NetPlayer getNetPlayer(){return _NetPlayer;}
  public void setNetPlayer(NetPlayer pNetPlayer){_NetPlayer = pNetPlayer;}

  public ShipStats getShipStats(){return _ShipStats;}
 
}
