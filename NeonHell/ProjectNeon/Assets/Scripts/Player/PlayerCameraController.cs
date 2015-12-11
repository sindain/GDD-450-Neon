using UnityEngine;
using System.Collections;

public class PlayerCameraController : MonoBehaviour {

	public float fSlerpTime	= 0.333f;
	public Camera playerCamera;
	public Vector3 vCameraOffset;
	
	void FixedUpdate(){
		playerCamera.transform.position = Vector3.Slerp(playerCamera.transform.position, transform.position + transform.rotation * vCameraOffset, fSlerpTime);
		playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, transform.rotation, fSlerpTime);
	}
}
