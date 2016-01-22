using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MenuScript : MonoBehaviour {
    
	//public Image Fader;
	//public Image CompName;
	public float fadeTime=1f;
	public float WaitTime=1f;

  private int iFadeCounter = 0;
  private float refA;
	private float refU;
  //private bool fadeToScreen;
  //private bool fadeToBlack;
  //private bool beingHandled = false;
  private Vector3 cameraTargetPos;
  private Quaternion cameraTargetRot;
  private Vector3 vCamPosRef;
  
    
    void Start()
	{
		//fadeToScreen=true;
		//fadeToBlack = false;
    cameraTargetPos = Vector3.zero;
    cameraTargetRot = Quaternion.identity;
	}
	void Update ()
	{
    //Update Camera Transform
    Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, cameraTargetPos, ref vCamPosRef,1.0f);
    Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, cameraTargetRot, Time.time * 1.0f);



		//if (Input.anyKey) 
		//{
		//	counter=3;
		//}
		//if (fadeToScreen)
		//{
		//	Fader.color=new Color (0,0,0,Mathf.SmoothDamp(Fader.color.a,0,ref refA,fadeTime));
		//	if (Fader.color.a<=0.01f)
		//	{
		//		//StartCoroutine(HandleIt());
		//		fadeToScreen =false;
		//		fadeToBlack=true;
		//		counter+=1;

		//	}

		//}
		//if(counter==3)
		//{
		//	fadeToBlack=false;
		//	fadeToScreen=false;
		//	Fader.gameObject.SetActive(false);
		//	CompName.gameObject.SetActive(false);
		//}

		//if (fadeToBlack)
		//{
		//	Fader.color=new Color (0,0,0,Mathf.SmoothDamp(Fader.color.a,1,ref refU,fadeTime));
		//	if (Fader.color.a>=0.95f)
		//	{
		//		fadeToScreen =true;
		//		fadeToBlack=false;
		//		counter+=1;
		//		CompName.gameObject.SetActive(false);
		//	}
		//}
	}

  public void setCameraTarget(Transform pCameraTarget)    {
    cameraTargetPos = pCameraTarget.position;
    cameraTargetRot = pCameraTarget.rotation;
  }
}
