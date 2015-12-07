using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MenuScript : MonoBehaviour {

	public GameObject titleMenu;
	public GameObject creditsMenu;
	public GameObject controlsMenu;
	public Image Fader;
	public Image CompName;
	public float fadeTime=1f;
	public float WaitTime=1f;
	//public GameObject Origin;
	//public GameObject Credits;
	//public GameObject Play;
	
	private bool fadeToScreen;
	private bool fadeToBlack;
	private float refA;
	private float refU;
	private bool beingHandled = false;
	private int counter=0;
	//private Vector3 camPos;
	//private Vector3 startPos;
	//private Vector3 endPos;
	//private Quaternion camRot;
	//private Quaternion startRot;
	//private Quaternion endRot;
	
	void Start()
	{
		fadeToScreen=true;
		fadeToBlack = false;
		//camPos=Origin.transform.position;
		//camRot=Origin.transform.rotation;
	}
	void Update ()
	{
		//not used in 2d menus this is a start for 3d menus if we add them
		/*
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast(ray,out hit,10000f))
		{	
			print (hit.collider.tag);
			if (Input.GetMouseButtonDown(0))
			{
				if(hit.collider.tag=="Tray")
				{
					hit.collider.gameObject.GetComponent<Animator>().SetBool("TrayOpen",!hit.collider.gameObject.GetComponent<Animator>().GetBool("TrayOpen"));
					if(camPos==Origin.transform.position)
					{
						camPos=Credits.transform.position;
						camRot=Credits.transform.rotation;
					}
					else
					{
						camPos=Origin.transform.position;
						camRot=Origin.transform.rotation;
					}
				}
				else if(hit.collider.tag=="Desk")
				{
					hit.collider.gameObject.GetComponent<Animator>().SetBool("DeskOpen",!hit.collider.gameObject.GetComponent<Animator>().GetBool("DeskOpen"));
					if(camPos==Origin.transform.position)
					{
						camPos=Play.transform.position;
						camRot=Play.transform.rotation;
					}
					else
					{
						camPos=Origin.transform.position;
						camRot=Origin.transform.rotation;
					}
				}
				else if(hit.collider.tag=="PlayBox")
				{
					Application.LoadLevel("LevelDesign1");			
				}
			}
		}
		Camera.main.transform.position = camPos;
		Camera.main.transform.rotation = camRot;
		*/
		if (Input.anyKey) 
		{
			counter=3;
		}
		if (fadeToScreen)
		{
			Fader.color=new Color (0,0,0,Mathf.SmoothDamp(Fader.color.a,0,ref refA,fadeTime));
			if (Fader.color.a<=0.01f)
			{
				//StartCoroutine(HandleIt());
				fadeToScreen =false;
				fadeToBlack=true;
				counter+=1;

			}

		}
		if(counter==3)
		{
			fadeToBlack=false;
			fadeToScreen=false;
			Fader.gameObject.SetActive(false);
			CompName.gameObject.SetActive(false);
		}

		print (fadeToBlack+": "+fadeToScreen);
		print (Fader.color.a);
		if (fadeToBlack)
		{
			Fader.color=new Color (0,0,0,Mathf.SmoothDamp(Fader.color.a,1,ref refU,fadeTime));
			if (Fader.color.a>=0.95f)
			{
				fadeToScreen =true;
				fadeToBlack=false;
				counter+=1;
				CompName.gameObject.SetActive(false);
			}
		}
	}
	
	public void onSinglePlayerClicked()
	{
		//Application.LoadLevel("MultiplayerLobby");
		Application.LoadLevel("LvlSelect");
	}
	public void onMultiPlayerClicked()
	{
		//Application.LoadLevel("MultiplayerLobby");
		//Application.LoadLevel("LvlSelect");
	}
	public void onControlsClicked()
	{
		titleMenu.SetActive (false);
		controlsMenu.SetActive (true);
	}
	public void onCreditsClicked()
	{
		titleMenu.SetActive (false);
		creditsMenu.SetActive (true);
	}
	public void onExitClicked()
	{
		Application.Quit ();
	}
	public void onBackClicked()
	{
		creditsMenu.SetActive (false);
		controlsMenu.SetActive (false);
		titleMenu.SetActive (true);
	}
	private IEnumerator HandleIt()
	{
		beingHandled = true;
		// process pre-yield
		yield return new WaitForSeconds( 1f );
		// process post-yield
		beingHandled = false;
	}
	void OnGUI()
	{
		/*if (GUI.Button(new Rect(15, 15, Screen.width / 10, Screen.height / 20), "Multiplayer Lobby"))
        {
            Application.LoadLevel("MultiplayerLobby");
        }*/
	}
}
