using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    
  public Image Fader;
  public Image CompName;
  public float fadeTime = 1f;
  public float WaitTime = 1f;

  private int iFadeCounter = 0;
  private float refA;
  private float refU;
  private bool fadeToScreen=true;
  private bool fadeToBlack;
  //private bool beingHandled = false;
  private bool bCameraControl = true;
  public Vector3 cameraTargetPos;
  public Quaternion cameraTargetRot;
  private Vector3 vCamPosRef;

    
  void Start (){
    cameraTargetPos = new Vector3 (13.0f, 11.0f, 44.0f);
    cameraTargetRot.eulerAngles = new Vector3 (0.0f, 270.0f, 0.0f);
  }

  void Update (){
    //Update Camera Transform
    Camera.main.transform.position = Vector3.SmoothDamp (Camera.main.transform.position, cameraTargetPos, ref vCamPosRef, 1.0f);
    Camera.main.transform.rotation = Quaternion.Slerp (Camera.main.transform.rotation, cameraTargetRot, Time.deltaTime * 1.0f);



    if (Input.anyKey) 
    {
			iFadeCounter=3;
    }
    if (fadeToScreen)
    {
    	Fader.color=new Color (0,0,0,Mathf.SmoothDamp(Fader.color.a,0,ref refA,fadeTime));
    	if (Fader.color.a<=0.01f)
    	{
    		//StartCoroutine(HandleIt());
    		fadeToScreen =false;
    		fadeToBlack=true;
			iFadeCounter+=1;

    	}

    }
		if(iFadeCounter==3)
    {
    	fadeToBlack=false;
    	fadeToScreen=false;
    	Fader.gameObject.SetActive(false);
    	CompName.gameObject.SetActive(false);
    }

    if (fadeToBlack)
    {
    	Fader.color=new Color (0,0,0,Mathf.SmoothDamp(Fader.color.a,1,ref refU,fadeTime));
    	if (Fader.color.a>=0.95f)
    	{
    		fadeToScreen =true;
    		fadeToBlack=false;
			iFadeCounter+=1;
    		CompName.gameObject.SetActive(false);
    	}
    }
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
