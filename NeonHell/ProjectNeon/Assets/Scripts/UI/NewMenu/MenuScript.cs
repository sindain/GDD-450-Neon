using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MenuScript : MonoBehaviour {
    
	public Image Fader;
	public Image CompName;
	public float fadeTime=1f;
	public float WaitTime=1f;

    private GameObject titleMenu;
    private GameObject creditsMenu;
    private GameObject controlsMenu;
    private GameObject vehicleMenu;
    private int counter = 0;
    private float refA;
	private float refU;
    private bool fadeToScreen;
    private bool fadeToBlack;
    private bool beingHandled = false;
    
    void Start()
	{
		fadeToScreen=true;
		fadeToBlack = false;
        titleMenu = transform.FindChild("Title").gameObject;
        creditsMenu = transform.FindChild("Credits").gameObject;
        controlsMenu = transform.FindChild("Controls").gameObject;
        vehicleMenu = transform.FindChild("CarSelector").gameObject;
	}
	void Update ()
	{
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
        //Application.LoadLevel("LvlSelect");
        vehicleMenu.SetActive(true);
        titleMenu.SetActive(false);
        
	}
	public void onMultiPlayerClicked()
	{
		//Application.LoadLevel("lobbyScene");
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
	}
}
