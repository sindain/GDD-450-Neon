using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    
//  public Image Fader;
//  public Image CompName;
//  public float fadeTime = 1f;
//  public float WaitTime = 1f;

 // private int iFadeCounter = 0;
//  private float refU;
 // private bool fadeToScreen=true;
 // private bool fadeToBlack;
  //private bool beingHandled = false;

  public Vector3 cameraTargetPos;
  public Quaternion cameraTargetRot;
  public Image fader;

  private float fFadeTime;
  private float refA;
  private bool bCameraControl = true;
  private Vector3 vCamPosRef;

    
  void Start (){
    cameraTargetPos = new Vector3 (13.0f, 11.0f, 44.0f);
    cameraTargetRot.eulerAngles = new Vector3 (0.0f, 270.0f, 0.0f);
    fFadeTime = Time.time + 0.5f;
  }

  void Update (){
    //Update Camera Transform
    Camera.main.transform.position = Vector3.SmoothDamp (Camera.main.transform.position, cameraTargetPos, ref vCamPosRef, 1.0f);
    Camera.main.transform.rotation = Quaternion.Slerp (Camera.main.transform.rotation, cameraTargetRot, Time.deltaTime * 1.0f);

    if (fader.color.a < 0.1f){
      fader.color = new Color (0, 0, 0, 0);
      fader.gameObject.SetActive (false);
    }
    else if (fader.color.a != 0.0f)
      fader.color = new Color (0, 0, 0, Mathf.SmoothDamp (fader.color.a, 0, ref refA, fFadeTime));
    
    
  }

  public void setCameraTarget (Vector3 pos, Quaternion rot){
    cameraTargetPos = pos;
    cameraTargetRot = rot;
  }

  public void resetCameraTarget(){
    cameraTargetPos = new Vector3 (13.0f, 11.0f, 44.0f);
    cameraTargetRot.eulerAngles = new Vector3 (0.0f, 270.0f, 0.0f);
  }

  public bool getCameraControl (){
    return bCameraControl;
  }

  public void setCameraControl (bool pbCameraControl){
    bCameraControl = pbCameraControl;
  }
}
