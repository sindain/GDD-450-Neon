using UnityEngine;
using System.Collections;

public class PlayerCameraController : MonoBehaviour {
	public Camera playerCamera;

	public float rotSpeed 		= 750.0f;
	private float rotVelocity 	= 0.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 newPos = transform.position + transform.rotation * new Vector3 (0.0f, 1.0f, -5f);
		Vector3 newRot = new Vector3(playerCamera.transform.eulerAngles.x, transform.eulerAngles.y, playerCamera.transform.eulerAngles.z);
		newRot.x = Mathf.SmoothDampAngle(newRot.x, transform.eulerAngles.x + 15.0f, ref rotVelocity, rotSpeed);
		playerCamera.transform.eulerAngles = newRot;
		playerCamera.transform.position = newPos;
	}
}
